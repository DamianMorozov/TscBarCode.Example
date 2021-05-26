// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com

//using System.Runtime.InteropServices;

//namespace TscBarcode
//{
//    public class TscLib
//    {
//        [DllImport("tsclibnet_x64.dll", EntryPoint = "about")]
//        public static extern int about();

//        [DllImport("tsclibnet_x64.dll", EntryPoint = "openport")]
//        public static extern int openport(string printername);

//        [DllImport("tsclibnet_x64.dll", EntryPoint = "barcode")]
//        public static extern int barcode(string x, string y, string type,
//                    string height, string readable, string rotation,
//                    string narrow, string wide, string code);

//        [DllImport("tsclibnet_x64.dll", EntryPoint = "clearbuffer")]
//        public static extern int clearbuffer();

//        [DllImport("tsclibnet_x64.dll", EntryPoint = "closeport")]
//        public static extern int closeport();

//        [DllImport("tsclibnet_x64.dll", EntryPoint = "downloadpcx")]
//        public static extern int downloadpcx(string filename, string image_name);

//        [DllImport("tsclibnet_x64.dll", EntryPoint = "formfeed")]
//        public static extern int formfeed();

//        [DllImport("tsclibnet_x64.dll", EntryPoint = "nobackfeed")]
//        public static extern int nobackfeed();

//        [DllImport("tsclibnet_x64.dll", EntryPoint = "printerfont")]
//        public static extern int printerfont(string x, string y, string fonttype,
//                        string rotation, string xmul, string ymul,
//                        string text);

//        [DllImport("tsclibnet_x64.dll", EntryPoint = "printlabel")]
//        public static extern int printlabel(string set, string copy);

//        [DllImport("tsclibnet_x64.dll", EntryPoint = "sendcommand")]
//        public static extern int sendcommand(string printercommand);

//        [DllImport("tsclibnet_x64.dll", EntryPoint = "setup")]
//        public static extern int setup(string width, string height,
//                  string speed, string density,
//                  string sensor, string vertical,
//                  string offset);

//        [DllImport("tsclibnet_x64.dll", EntryPoint = "windowsfont")]
//        public static extern int windowsfont(int x, int y, int fontheight,
//                        int rotation, int fontstyle, int fontunderline,
//                        string szFaceName, string content);
//        [DllImport("tsclibnet_x64.dll", EntryPoint = "windowsfontUnicode")]
//        public static extern int windowsfontUnicode(int x, int y, int fontheight,
//                         int rotation, int fontstyle, int fontunderline,
//                         string szFaceName, byte[] content);

//        [DllImport("tsclibnet_x64.dll", EntryPoint = "sendBinaryData")]
//        public static extern int sendBinaryData(byte[] content, int length);

//        [DllImport("tsclibnet_x64.dll", EntryPoint = "usbportqueryprinter")]
//        public static extern byte usbportqueryprinter();
//    }
//}