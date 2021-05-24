using Hardware.Print.Tsc;
using Hardware.Utils;
using log4net;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Zebra.Sdk.Comm;
using Zebra.Sdk.Printer;

namespace Hardware.Print
{
    public class PrintEntity
    {
        #region Public and private fields and properties

        public string Peeler { get; private set; }
        public int UserLabelCount { get; private set; }
        public PrinterStatus CurrentStatus { get; private set; }
        public int CommandThreadTimeOut { get; private set; } = 100;

        public delegate void OnHandler(PrintEntity state);
        public event OnHandler Notify;

        private readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public Connection Con { get; private set; }
        public ConcurrentQueue<string> CmdQueue { get; } = new ConcurrentQueue<string>();
        private readonly object _locker = new object();
        private Thread _sessionSharingThread = null;
        private bool _isThreadWork = true;

        public PrintControlEntity PrintControl { get; set; }

        #endregion

        #region Constructor and destructor

        public PrintEntity(Connection connection, string ip, int port, int commandThreadTimeOut = 100)
        {
            CommandThreadTimeOut = commandThreadTimeOut;
            Con = connection;
            PrintControl = new PrintControlEntity(Interface.Ethernet, ip, port);
        }

        public PrintEntity(string ip, int port, int commandThreadTimeOut = 100)
        {
            //var zebraCurrentState = new StateEntity();
            CommandThreadTimeOut = commandThreadTimeOut;
            Con = new TcpConnection(ip, port);
            PrintControl = new PrintControlEntity(Interface.Ethernet, ip, port);
        }

        #endregion

        #region Public and private methods

        /// <summary>
        /// Отправить задание в очередь печати.
        /// </summary>
        /// <param name="printCmd"></param>
        public async void SendAsync(string printCmd)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(1)).ConfigureAwait(false);
            CmdQueue.Enqueue(printCmd);
        }

        public void Open(string printerType)
        {
            Con.Open();
            _isThreadWork = true;
            if (printerType.Contains("TSC "))
                OpenTsc();
            else
                OpenZebra();
        }

        public void OpenZebra()
        {
            var printerDevice = ZebraPrinterFactory.GetInstance(Con);
            _sessionSharingThread = new Thread(t =>
            {
                while (_isThreadWork)
                {
                    lock (_locker)
                    {
                        try
                        {
                            CurrentStatus = printerDevice.GetCurrentStatus();
                            UserLabelCount = int.Parse(SGD.GET("odometer.user_label_count", printerDevice.Connection));
                            Peeler = SGD.GET("sensor.peeler", printerDevice.Connection);

                            if (CurrentStatus.isReadyToPrint)
                            {
                                if (Peeler == "clear")
                                {
                                    if (CmdQueue.TryDequeue(out var request))
                                    {
                                        request = request.Replace("|", "\\&");
                                        printerDevice.SendCommand(request);
                                    }
                                }
                            }
                            Notify?.Invoke(this);
                        }
                        catch (ConnectionException e)
                        {
                            _log.Error(e.ToString());
                        }
                        catch (ZebraPrinterLanguageUnknownException e)
                        {
                            _log.Error(e.ToString());
                        }
                        catch (Exception e)
                        {
                            _log.Error(e.ToString());
                        }
                    }
                    Thread.Sleep(CommandThreadTimeOut);
                }
            })
            { IsBackground = true };
            _sessionSharingThread.Start();
            Thread.Sleep(CommandThreadTimeOut);
        }

        public void OpenTsc()
        {
            _sessionSharingThread = new Thread(t =>
            {
                UserLabelCount = 1;
                while (_isThreadWork)
                {
                    lock (_locker)
                    {
                        try
                        {
                            if (CmdQueue.TryDequeue(out var request))
                            {
                                request = request.Replace("|", "\\&");
                                if (!request.Equals("^XA~JA^XZ") && !request.Contains("odometer.user_label_count"))
                                {
                                    //CurrentStatus = printerDevice.GetCurrentStatus();
                                    //UserLabelCount = int.Parse(SGD.GET("odometer.user_label_count", printerDevice.Connection));
                                    //UserLabelCount = 1;
                                    //Peeler = SGD.GET("sensor.peeler", printerDevice.Connection);
                                    PrintControl.SendCmd(false, request, false);
                                }
                            }
                            Notify?.Invoke(this);
                        }
                        catch (ConnectionException e)
                        {
                            _log.Error(e.ToString());
                        }
                        catch (ZebraPrinterLanguageUnknownException e)
                        {
                            _log.Error(e.ToString());
                        }
                        catch (Exception e)
                        {
                            _log.Error(e.ToString());
                        }
                    }
                    Thread.Sleep(CommandThreadTimeOut);
                    System.Windows.Forms.Application.DoEvents();
                }
            })
            { IsBackground = true };
            _sessionSharingThread.Start();
            Thread.Sleep(CommandThreadTimeOut);
        }

        public void Close()
        {
            if (_sessionSharingThread != null && _sessionSharingThread.IsAlive)
            {
                _isThreadWork = false;
                Thread.Sleep(5 * CommandThreadTimeOut);
                _sessionSharingThread.Join(10);
                _sessionSharingThread.Abort();
                _sessionSharingThread = null;
            }
            Con.Close();
        }

        public void ClearPrintBuffer(string printerType)
        {
            while (!CmdQueue.IsEmpty)
            {
                CmdQueue.TryDequeue(out _);
            }
            if (printerType.Contains("TSC "))
            {
                PrintControl.ClearBuffer(false);
            }
            else
            {
                CmdQueue.Enqueue("^XA~JA^XZ");
            }
        }

        public void SetOdometorUserLabel(int value)
        {
            CmdQueue.Enqueue($"! U1 setvar \"odometer.user_label_count\" \"{value}\"\r\n");
        }

        #endregion
    }
}
