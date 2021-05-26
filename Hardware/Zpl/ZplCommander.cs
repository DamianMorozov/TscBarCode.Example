// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com

using System;
using System.Threading;
using System.Threading.Tasks;
using Hardware.Print.Zebra;
using log4net;
using Zebra.Sdk.Comm;
using Zebra.Sdk.Printer;
using ZebraConnectionBuilder = Zebra.Sdk.Comm.ConnectionBuilder;
using ZebraPrinterStatus = Zebra.Sdk.Printer.PrinterStatus;

namespace Hardware.Zpl
{
    public class ZplCommander
    {
        #region Private fields and properties

        private static readonly int CommandThreadTimeOut = 10_000;
        private static readonly int CommandCountPackage = 1;
        private static readonly object locker = new object();
        private readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private Thread _commandThread;
        private bool _commandThreadExit;
        private Task _task;
        private bool _taskExit;
        private Connection _connection;

        #endregion

        #region Public methods

        public ZplCommander(string address, DeviceEntity mkDeviceEntity, string cmd)
        {
            //StartThread(mkDeviceEntity, cmd);
            StartTask(address, mkDeviceEntity, cmd);
        }

        public void StartThread(DeviceEntity mkDeviceEntity, string cmd)
        {
            if (_commandThread != null)
                return;

            _commandThreadExit = false;
            _commandThread = new Thread(t =>
            {
                while (!_commandThreadExit)
                {
                    lock (locker)
                    {
                        for (int i = 0; i < CommandCountPackage; i++)
                        {
                            mkDeviceEntity.SendAsync(cmd);
                        }
                    }
                    Thread.Sleep(CommandThreadTimeOut);
                }
            })
            { IsBackground = true };
            _commandThread.Start();
        }

        public void StopThread()
        {
            _commandThreadExit = true;
        }

        private string GetDescription(ZebraPrinterStatus status)
        {
            if (status.isReadyToPrint)
                return @"Готов к печати";
            if (status.isHeadCold)
                return @"Голова холодна";
            if (status.isHeadOpen)
                return @"Голова открыта";
            if (status.isHeadTooHot)
                return @"Голова слишком горячая";
            if (status.isPaperOut)
                return @"Нет бумаги";
            if (status.isPartialFormatInProgress)
                return @"Частичное форматирование в процессе";
            if (status.isPaused)
                return @"Приостановлено";
            if (status.isReceiveBufferFull)
                return @"Буфер приема заполнен";
            if (status.isRibbonOut)
                return @"Лента закончилась";
            return "Ошибка";

        }

        public void StartTask(string address, DeviceEntity mkDeviceEntity, string cmd)
        {
            if (_task != null)
                return;

            _taskExit = false;
            _task = Task.Run(async () =>
            {
                var isFirst = true;
                while (!_taskExit)
                {
                    try
                    {
                        ConnnectionOpen(ref address);
                        if (_connection != null)
                        {
                            if (_connection.GetType().Name.Contains("Status"))
                            {
                                var printer = ZebraPrinterFactory.GetLinkOsPrinter(_connection);
                                var status = printer?.GetCurrentStatus();
                                if (status != null)
                                {
                                    // Готов к печати
                                    if (status.isReadyToPrint)
                                    {
                                    }
                                    else
                                    {
                                        mkDeviceEntity.DataCollector.Setup(status);
                                    }
                                }
                                else
                                {
                                    throw new Exception("GetLinkOsPrinter error!");
                                }
                            }
                        }
                    }
                    catch (ConnectionException)
                    {
                        _log.Error("Zebra. Connection could not be opened!");
                    }
                    catch (ZebraPrinterLanguageUnknownException)
                    {
                        _log.Error("Zebra. Could not create printer!");
                    }
                    // Первый опрос.
                    if (isFirst)
                    {
                        isFirst = false;
                        await Task.Delay(TimeSpan.FromSeconds(1));
                    }
                    else
                        await Task.Delay(TimeSpan.FromMilliseconds(CommandThreadTimeOut));
                }
                ConnnectionClose();
            });
        }

        public void StopTask()
        {
            _taskExit = true;
            ConnnectionClose();
        }

        private void ConnnectionOpen(ref string address)
        {
            if (_connection == null)
            {
                _connection = ZebraConnectionBuilder.Build($"TCP_STATUS:{address}");
            }
            if (_connection != null)
                if (!_connection.Connected)
                    _connection.Open();
        }

        private void ConnnectionClose()
        {
            if (_connection != null)
            {
                try
                {
                    _connection.Close();
                }
                catch (ConnectionException ex)
                {
                    throw ex;
                }
                finally
                {
                    _connection = null;
                }
            }
        }

        public void Close()
        {
            StopThread();
            if (_commandThread != null)
            {
                _commandThread.Join(2500);
                _commandThread.Abort();
                _commandThread = null;
            }

            StopTask();
            if (_task != null)
            {
                _task.Wait(TimeSpan.FromMilliseconds(500));
                _task = null;
            }
        }

        ~ZplCommander()
        {
            Close();
        }

        #endregion
    }
}
