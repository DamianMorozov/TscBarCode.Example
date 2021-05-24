using Hardware.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Unicode;
using System.Xml;
using System.Xml.Xsl;
using Hardware.Print.Native;

namespace Hardware.Zpl
{
    public static class ZplPipeUtils
    {
        #region Public and private fields and properties

        public static char[] DigitsCharacters = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

        public static char[] SpecialCharacters = { 
            ' ', ',', '.', '-', 
            '~', '!', '#', '$', '%', '^', '&', '*', '(', ')', '_', '+', '=',
            '"', '№', ';', ':', '?',
            '/', '|', '\\', '{', '}', '<', '>'
        };

        #endregion

        #region pipe's

        public static string FakePipe(string zplCommand) => zplCommand;

        public static string XsltTransformationPipe(string xslInput, string xmlInput)
        {
            string result;
            using (var stringReaderXslt = new StringReader(xslInput.Trim())) // xslInput is a string that contains xsl
            {
                using (var stringReaderXml = new StringReader(xmlInput.Trim())) // xmlInput is a string that contains xml
                {
                    using (var xmlReaderXslt = XmlReader.Create(stringReaderXslt))
                    {
                        using (var xmlReaderXml = XmlReader.Create(stringReaderXml))
                        {
                            var xslt = new XslCompiledTransform();
                            xslt.Load(xmlReaderXslt);
                            using (var stringWriter = new StringWriter())
                            using (var xmlWriter = XmlWriter.Create(stringWriter, xslt.OutputSettings)) // use OutputSettings of xsl, so it can be output as HTML
                            {
                                xslt.Transform(xmlReaderXml, xmlWriter);
                                result = stringWriter.ToString();
                                result = ToCodePoints(result);
                            }
                        }
                    }
                }
            }
            return result;
        }

        //[Obsolete(@"Deprecated method")]
        //public string ZplCommandPipe(string zplCommand)
        //{
        //    var outMsg = new StringBuilder();
        //    try
        //    {
        //        string _zpl = ToCodePoints(zplCommand);
        //        string _errorMessage = string.Empty;
        //        string info = InterplayToPrinter(IpAddress, Port, _zpl.Split('\n'), out _errorMessage);
        //        outMsg.AppendLine(info);
        //        outMsg.AppendLine(_errorMessage);
        //    }
        //    catch (Exception ex)
        //    {
        //        outMsg.AppendLine(zplCommand);
        //        outMsg.AppendLine(ex.Message);
        //    }
        //    return outMsg.ToString();
        //}

        public static string ZplCommandPipeByIp(string ip, int port, string zplCommand)
        {
            var outMsg = new StringBuilder();
            try
            {

                var zpl = ToCodePoints(zplCommand);
                var info = InterplayToPrinter(ip, port, zpl.Split('\n'), out var errorMessage);
                outMsg.AppendLine(info);
                outMsg.AppendLine(errorMessage);
            }
            catch (Exception ex)
            {
                outMsg.AppendLine(zplCommand);
                outMsg.AppendLine(ex.Message);
            }
            return outMsg.ToString();
        }

        public static string ZplCommandPipeByRaw(string lptName, string zplCommand)
        {
            var outMsg = new StringBuilder();
            try
            {
                var zpl = ToCodePoints(zplCommand);
                RawPrinterHelper.SendStringToPrinter(lptName, zpl);
            }
            catch (Exception ex)
            {
                outMsg.AppendLine(zplCommand);
                outMsg.AppendLine(ex.Message);
            }
            return outMsg.ToString();
        }
        
        #endregion

        #region Commands
        
        public static string ZplHostQuery(string prm = "ES") => $"~HQ{prm}";

        public static string ZplFontsClear() => @"^XA^IDE:*.TTF^FS^XZ";

        public static string ZplLogoClear() => @"^XA^IDE:*.GRF^FS^XZ";

        public static string ZplCalibration() => "! U1 setvar \"media.type\" \"label\"\r\n" + "! U1 setvar \"media.sense_mode\" \"gap\"\r\n" + "~JC^XA^JUS^XZ";

        public static string ZplFilesDelete(string mask = "E:*.*") => $"! U1 do \"file.delete\" \"{mask}\"\r\n";

        public static string ZplFilesList(string mask = "E:*.*") => $"! U1 do \"file.dir\" \"{mask}\"\r\n";

        public static string ZplSetOdometerUserLabel(int value = 1) => $"! U1 setvar \"odometer.user_label_count\" \"{value}\"\r\n";

        public static string ZplGetOdometerUserLabel() => $"! U1 getvar \"odometer.user_label_count\"\r\n";

        public static string ZplPrintDirectory(string mask = "E:*.*") => $"^XA^WD{mask}^XZ";

        public static string ZplPowerOnReset() => $"~JR";

        public static string ZplPeelerState() => $"! U1 getvar \"sensor.peeler\"\r\n";

        public static string ZplPrintConfigurationLabel() => $"~WC";

        public static string ZplHostStatusReturn() => $"~HS";

        public static string ZplClearPrintBuffer() => $"^XA~JA^XZ";

        public static string ZplLogoDownloadCommand(string imageName, string image, bool addHeaderFooter = true)
        {
            var b = Convert.FromBase64String(FixBase64ForImage(image));
            using (var bmpStream = new MemoryStream(b))
            {
                var img = System.Drawing.Image.FromStream(bmpStream);
                var bitmapData = new System.Drawing.Bitmap(img);
                return ZplLogoDownloadCommand(imageName, bitmapData, addHeaderFooter);
            }
        }

        public static string ZplLogoDownloadCommand(string imageName, System.Drawing.Bitmap image, bool addHeaderFooter = true)
        {

            System.Drawing.Imaging.BitmapData imgData = null;
            byte[] pixels;
            int x, y, width;
            StringBuilder sb;
            IntPtr ptr;

            try
            {
                imgData = image.LockBits(
                    new System.Drawing.Rectangle(0, 0, image.Width, image.Height),
                    System.Drawing.Imaging.ImageLockMode.ReadOnly,
                    System.Drawing.Imaging.PixelFormat.Format1bppIndexed);

                width = (image.Width + 7) / 8;
                pixels = new byte[width];
                sb = new StringBuilder(width * image.Height * 2);
                ptr = imgData.Scan0;
                for (y = 0; y < image.Height; y++)
                {
                    Marshal.Copy(ptr, pixels, 0, width);
                    for (x = 0; x < width; x++)
                        sb.AppendFormat("{0:X2}", (byte)~pixels[x]);
                    ptr = (IntPtr)(ptr.ToInt64() + imgData.Stride);
                }
            }
            finally
            {
                if (image != null)
                {
                    if (imgData != null) image.UnlockBits(imgData);
                    image.Dispose();
                }
            }

            var zplCode = $"~DGE:{imageName.ToUpper().Replace(".BMP", "")}.GRF,{width * y},{width}," + sb;
            if (addHeaderFooter)
            {
                zplCode = "^XA " + zplCode + "^XZ";
            }
            return zplCode;
        }

        private static string FixBase64ForImage(string image)
        {
            var sbText = new StringBuilder(image, image.Length);
            sbText.Replace("\r\n", string.Empty);
            sbText.Replace(" ", string.Empty);
            return sbText.ToString();
        }

        #endregion

        #region Other methods

        public static string InterplayToPrinter(string ipAddress, int port, string[] zplCommand, 
            out string errorMessage, int receiveTimeout = 1000, int sendTimeout = 100)
        {
            errorMessage = @"";
            var response = new StringBuilder();

            using (var client = new TcpClient())
            {
                //client.NoDelay = true;
                client.ReceiveTimeout = receiveTimeout;
                client.SendTimeout = sendTimeout;

                client.Connect(ipAddress, port);
                using (var stream = client.GetStream())
                {
                    using (var writer = new StreamWriter(stream))
                    {
                        writer.AutoFlush = true;
                        using (var reader = new StreamReader(stream))
                        {
                            try
                            {
                                foreach (var commandLine in zplCommand)
                                {
                                    writer.Write(Encoding.ASCII.GetChars(Encoding.ASCII.GetBytes(commandLine)), 0, commandLine.Length);
                                    writer.Flush();
                                }
                                var data = new byte[256];
                                var bytes = 0;
                                do
                                {
                                    bytes = stream.Read(data, 0, data.Length);

                                    if (bytes > 0)
                                    {
                                        response.Append(Encoding.UTF8.GetString(data, 0, bytes));
                                    }
                                }
                                while (stream.DataAvailable);
                            }
                            catch (Exception ex)
                            {
                                if (ex.InnerException is SocketException sockEx)
                                {
                                    errorMessage = @"(" + sockEx.NativeErrorCode + ") Exception = " + ex.Message;
                                }
                                throw;
                            }
                        }
                    }
                }
            }
            return response.ToString();
        }

        public static bool IsCyrillic(char value)
        {
            var cyrillic = Enumerable
                .Range(UnicodeRanges.Cyrillic.FirstCodePoint, UnicodeRanges.Cyrillic.Length)
                .Select(ch => (char)ch)
                .ToArray();
            return Array.BinarySearch(cyrillic, value) >= 0;
        }

        public static bool IsDigit(char value)
        {
            return DigitsCharacters.Contains(value);
        }

        public static bool IsSpecial(char value, bool isExcludeTop = true)
        {
            if (isExcludeTop && value == '^')
                return false;
            return SpecialCharacters.Contains(value);
        }

        [Obsolete(@"Use ToCodePoints")]
        public static string ToCodePointsOld(string zplInput)
        {
            var result = new StringBuilder();
            var unicodeCharacterList = new Dictionary<char, string>();
            foreach (var ch in zplInput)
            {
                if (!unicodeCharacterList.ContainsKey(ch))
                {
                    var bytes = Encoding.UTF8.GetBytes(ch.ToString());
                    if (bytes.Length > 1)
                    {
                        var hexCode = string.Empty;
                        foreach (var b in bytes)
                        {
                            hexCode += $"_{BitConverter.ToString(new byte[] { b }).ToLower()}";
                        }
                        unicodeCharacterList[ch] = hexCode;
                    }
                    else
                        unicodeCharacterList[ch] = ch.ToString();
                    result.Append(unicodeCharacterList[ch]);
                }
                else
                // English characters.
                {
                    result.Append(unicodeCharacterList[ch]);
                }
            }
            return result.ToString();
        }

        public static string ToCodePoints(string zplInput)
        {
            var result = new StringBuilder();
            var unicodeCharacterList = new Dictionary<char, string>();
            // Поиск подстроки [^FH^FD].
            var isFieldData = 0;
            var isDataStart = false;
            var isDataEnd = false;
            foreach (var ch in zplInput)
            {
                if (isFieldData == 6)
                {
                    var bytes = Encoding.UTF8.GetBytes(ch.ToString());
                    var hexCode = string.Empty;
                    foreach (var b in bytes)
                    {
                        hexCode += $"_{BitConverter.ToString(new byte[] {b}).ToUpper()}";
                    }

                    unicodeCharacterList[ch] = hexCode;
                }
                else
                {
                    unicodeCharacterList[ch] = ch.ToString();
                }

                // Calc isFieldData.
                if (isFieldData == 0 && ch == '^')
                    isFieldData = 1;
                if (isFieldData == 1 && ch == 'F')
                    isFieldData = 2;
                if (isFieldData == 2 && ch == 'H')
                    isFieldData = 3;
                if (isFieldData == 3 && ch == '^')
                    isFieldData = 4;
                if (isFieldData == 4 && ch == 'F')
                    isFieldData = 5;
                if (isFieldData == 5 && ch == 'D')
                    isFieldData = 6;

                // Reset isFieldData.
                if (isFieldData == 6 && ch == '^')
                    isFieldData = 7;
                if (isFieldData == 7 && ch == 'F')
                    isFieldData = 8;
                if (isFieldData == 8 && ch == 'S')
                {
                    isFieldData = 0;
                    isDataStart = false;
                    isDataEnd = false;
                }
                if (isFieldData < 7)
                {
                    result.Append(unicodeCharacterList[ch]);
                    if (isFieldData == 6 && !isDataStart)
                    {
                        isDataStart = true;
                        result.Append(Environment.NewLine);
                    }
                }
                else
                {
                    if (isFieldData == 7 && !isDataEnd)
                    {
                        isDataEnd = true;
                        result.Append(Environment.NewLine);
                        result.Append("^");
                    }
                    else
                        result.Append(unicodeCharacterList[ch]);
                }
            }
            return result.ToString();
        }

        #endregion
    }
}