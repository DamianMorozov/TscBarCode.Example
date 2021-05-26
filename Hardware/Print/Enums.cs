// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com

namespace Hardware.Print
{
    public enum PrintSpeed
    {
        Zero = 0,
        One = 1,
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5,
        Six = 6,
        Seven = 7,
        Eight = 8,
        Nine = 9,
        Ten = 10,
        Eleven = 11,
        Twelve = 12,
    }

    public enum PrintDensity
    {
        Zero = 0,
        One = 1,
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5,
        Six = 6,
        Seven = 7,
        Eight = 8,
        Nine = 9,
        Ten = 10,
        Eleven = 11,
        Twelve = 12,
        Thirteen = 13,
        Fourteen = 14,
        Fifteen = 15,
    }

    public enum PrintSensor
    {
        Zero = 0,
        One = 1,
    }

    // \\palych\VladimirStandardCorp\TSC BarCode Printer ML340P\SDKs\Pdf\TSC_TSPL_TSPL2_Programming.pdf
    public enum PrintStatus
    {
        Zero = 0,       // 00 Normal
        One = 1,        // 01 Head opened
        Two = 2,        // 02 Paper Jam
        Three = 3,      // 03 Paper Jam and head opened
        Four = 4,       // 04 Out of paper
        Five = 5,       // 05 Out of paper and head opened
        Eight = 8,      // 08 Out of ribbon
        Nine = 9,       // 09 Out of ribbon and head opened
        Ten = 10,       // 0A Out of ribbon and paper jam
        Eleven = 11,    // 0B Out of ribbon, paper jam and head opened
        Twelve = 12,    // 0C Out of ribbon and out of paper
        Thirteen = 13,  // 0D Out of ribbon, out of paper and head opened
        Sixteen = 16,   // 10 Pause
        ThirtyTwo = 32, // 20 Printing
        HundredTwentyEight = 128, // 80 Other error
    }

    public enum PrintInitialCrcValue
    {
        Zeros,
        NonZero1 = 0xffff,
        NonZero2 = 0x1D0F,
    }

    public enum PrintInterface
    {
        Usb,
        Ethernet,
    }

    public enum PrintLabelSize
    {
        Size40x60,
        Size60x150,
        Size60x90,
        Size60x100,
        Size80x100,
        Size100x100,
        Size100x110,
    }

    public enum PrintDpi
    {
        Dpi100,
        Dpi200,
        Dpi300,
        Dpi400,
        Dpi500,
        Dpi600,
        Dpi700,
        Dpi800,
        Dpi900,
        Dpi1000,
        Dpi1100,
        Dpi1200,
    }
}
