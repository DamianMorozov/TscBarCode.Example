// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com

using Zebra.Sdk.Printer;

namespace Hardware.Print.Zebra
{
    /// <summary>
    /// Сбор данных.
    /// </summary>
    public class StatusDataCollector
    {
        #region Public and private fields and properties

        public string Ip { get; private set; }
        public int Port { get; private set; }
        public bool IsHeadCold { get; private set; }
        public bool IsHeadOpen { get; private set; }
        public bool IsHeadTooHot { get; private set; }
        public bool IsPaperOut { get; private set; }
        public bool IsPartialFormatInProgress { get; private set; }
        public bool IsPaused { get; private set; }
        public bool IsReadyToPrint { get; private set; }
        public bool IsReceiveBufferFull { get; private set; }
        public bool IsRibbonOut { get; private set; }
        public int LabelLengthInDots { get; private set; }
        public int LabelsRemainingInBatch { get; private set; }
        public int NumberOfFormatsInReceiveBuffer { get; private set; }
        public string PrintMode { get; private set; }

        #endregion

        #region Constructor and destructor

        public StatusDataCollector()
        {
            SetDefault();
        }

        #endregion

        #region Public and private methods

        public void SetDefault()
        {
            Ip = default;
            Port = default;
            IsHeadCold = default;
            IsHeadOpen = default;
            IsHeadTooHot = default;
            IsPaperOut = default;
            IsPartialFormatInProgress = default;
            IsPaused = default;
            IsReadyToPrint = default;
            IsReceiveBufferFull = default;
            IsRibbonOut = default;
            LabelLengthInDots = default;
            LabelsRemainingInBatch = default;
            NumberOfFormatsInReceiveBuffer = default;
            PrintMode = default;
        }

        public void Setup(PrinterStatus status)
        {
            IsHeadCold = status.isHeadCold;
            IsHeadOpen = status.isHeadOpen;
            IsHeadTooHot = status.isHeadTooHot;
            IsPaperOut = status.isPaperOut;
            IsPartialFormatInProgress = status.isPartialFormatInProgress;
            IsPaused = status.isPaused;
            IsReadyToPrint = status.isReadyToPrint;
            IsReceiveBufferFull = status.isReceiveBufferFull;
            IsRibbonOut = status.isRibbonOut;
            LabelLengthInDots = status.labelLengthInDots;
            LabelsRemainingInBatch = status.labelsRemainingInBatch;
            NumberOfFormatsInReceiveBuffer = status.numberOfFormatsInReceiveBuffer;
            PrintMode = status.printMode.ToString();
        }

        public void SetIpPort(string ip, int port)
        {
            Ip = ip;
            Port = port;
        }

        #endregion
    }
}
