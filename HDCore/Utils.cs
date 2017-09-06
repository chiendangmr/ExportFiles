using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.VisualBasic.Devices;

namespace HDCore
{
    public static class Utils
    {
        /// <summary>
        /// Ghi dữ liệu của đối tượng XML vào file local có đường dẫn FileName.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="FileName"></param>
        public static void SaveObject<T>(this T obj, string FileName)
        {
            var x = new XmlSerializer(typeof(T));
            using (var Writer = new StreamWriter(FileName, false))
            {
                x.Serialize(Writer, obj);
                Writer.Close();
            }
        }

        public static T GetObject<T>(string FileName)
        {
            if (File.Exists(FileName))
            {
                using (FileStream stream = new FileStream(FileName, FileMode.Open))
                {
                    XmlTextReader reader = new XmlTextReader(stream);
                    var x = new XmlSerializer(typeof(T));
                    var obj = (T)x.Deserialize(reader);
                    stream.Close();
                    return obj;
                }
            }
            else
                return default(T);
        }

        public static string ObjectToXml<T>(this T obj)
        {
            string xmlStr = string.Empty;

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = false;
            settings.OmitXmlDeclaration = true;
            settings.NewLineChars = string.Empty;
            settings.NewLineHandling = NewLineHandling.None;

            using (StringWriter stringWriter = new StringWriter())
            {
                using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, settings))
                {
                    XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
                    namespaces.Add(string.Empty, string.Empty);

                    XmlSerializer serializer = new XmlSerializer(obj.GetType());
                    serializer.Serialize(xmlWriter, obj, namespaces);

                    xmlStr = stringWriter.ToString();
                    xmlWriter.Close();
                }

                stringWriter.Close();
            }

            return xmlStr;
        }

        public static T ObjectFromXml<T>(string xml)
        {
            var stringReader = new System.IO.StringReader(xml);
            var serializer = new XmlSerializer(typeof(T));
            var obj = (T)serializer.Deserialize(stringReader);
            return obj;
        }

        public static string ObjectToString(this Object obj)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        }

        public static string ConvertToVietnameseNonSign(string s)
        {
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = s.Normalize(NormalizationForm.FormD);
            return regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
        }

        public static string DayOfWeekToVietNam(DayOfWeek day)
        {
            switch (day)
            {
                case DayOfWeek.Monday:
                    return "THỨ HAI";
                case DayOfWeek.Tuesday:
                    return "THỨ BA";
                case DayOfWeek.Wednesday:
                    return "THỨ TƯ";
                case DayOfWeek.Thursday:
                    return "THỨ NĂM";
                case DayOfWeek.Friday:
                    return "THỨ SÁU";
                case DayOfWeek.Saturday:
                    return "THỨ BẢY";
                default:
                    return "CHỦ NHẬT";
            }
        }

        public static Type GetListType<T>(List<T> _)
        {
            return typeof(T);
        }

        public static T Copy<T>(this T obj)
        {
            T newObj = Activator.CreateInstance<T>();
            Type objType = typeof(T);

            var props = objType.GetProperties().Where(p => p.CanRead && p.CanWrite).OrderBy(o => o.Name).ToList();

            foreach (PropertyInfo prop in props)
            {
                if (prop.CanRead && prop.CanWrite)
                    prop.SetValue(newObj, prop.GetValue(obj, null), null);
            }

            return newObj;
        }

        public static int FindIndex<T>(this IEnumerable<T> items, Func<T, bool> predicate)
        {
            var itemsWithIndices = items.Select((item, index) => new { Item = item, Index = index });
            var matchingIndices =
                from itemWithIndex in itemsWithIndices
                where predicate(itemWithIndex.Item)
                select (int?)itemWithIndex.Index;

            return matchingIndices.FirstOrDefault() ?? -1;
        }

        public static string FileSizeToString(long fileSize)
        {
            double size = fileSize;
            string currentUnit = "B";
            while (size > 1024)
            {
                size /= 1024;
                if (currentUnit == "B")
                    currentUnit = "kB";
                else if (currentUnit == "kB")
                    currentUnit = "MB";
                else if (currentUnit == "MB")
                    currentUnit = "GB";
                else if (currentUnit == "GB")
                {
                    currentUnit = "TB";
                    break;
                }
            }
            return size.ToString("0.##") + currentUnit;
        }

        public static string GetHex(long x, int length)
        {
            return x.ToString("x").PadLeft(length, '0').Substring(0, length);
        }

        public static long GetLongFromHex(string hex)
        {
            long result;
            if (long.TryParse(hex, System.Globalization.NumberStyles.HexNumber, null, out result))
                return result;
            else
                return 0;
        }

        private static char[] _mBase32Alphabet = new char[]{'A','B','C','D','E','F','G','H','I','J',
            'K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z','2','3','4','5','6','7'};
        public static string Base32Encode(long num, int length)
        {
            string base32 = "";
            for (int i = length; i > 0; i--)
            {
                base32 = _mBase32Alphabet[(num % 32)].ToString() + base32;
                num /= 32;
            }
            return base32;
        }

        public static long Base32Decode(string base32)
        {
            long num = 0;
            base32 = base32.ToUpper();
            foreach (char c in base32)
            {
                for (int i = 0; i < 32; i++)
                    if (c == _mBase32Alphabet[i])
                    {
                        num = num * 32 + i;
                        break;
                    }
            }
            return num;
        }

        public static string GetCurrentIP()
        {
            string myIP = "";

            IPHostEntry host;
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily.ToString() == "InterNetwork")
                {
                    if (myIP != "") myIP += ";";
                    myIP += ip.ToString();
                }
            }

            return myIP;
        }

        public static ulong GetRamSpace()
        {
            try
            {
                var computerInfo = new ComputerInfo();
                return computerInfo.AvailablePhysicalMemory;
            }
            catch { }
            return 0;
        }

        public const string RegExIP = @"(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)";

        /// <summary>
        /// Compresses the string.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static string CompressString(string text)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(text);
            var memoryStream = new MemoryStream();
            using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
            {
                gZipStream.Write(buffer, 0, buffer.Length);
            }

            memoryStream.Position = 0;

            var compressedData = new byte[memoryStream.Length];
            memoryStream.Read(compressedData, 0, compressedData.Length);

            var gZipBuffer = new byte[compressedData.Length + 4];
            Buffer.BlockCopy(compressedData, 0, gZipBuffer, 4, compressedData.Length);
            Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gZipBuffer, 0, 4);
            return Convert.ToBase64String(gZipBuffer);
        }

        /// <summary>
        /// Decompresses the string.
        /// </summary>
        /// <param name="compressedText">The compressed text.</param>
        /// <returns></returns>
        public static string DecompressString(string compressedText)
        {
            byte[] gZipBuffer = Convert.FromBase64String(compressedText);
            using (var memoryStream = new MemoryStream())
            {
                int dataLength = BitConverter.ToInt32(gZipBuffer, 0);
                memoryStream.Write(gZipBuffer, 4, gZipBuffer.Length - 4);

                var buffer = new byte[dataLength];

                memoryStream.Position = 0;
                using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                {
                    gZipStream.Read(buffer, 0, buffer.Length);
                }

                return Encoding.UTF8.GetString(buffer);
            }
        }

        public static string ToJSon<T>(this T obj)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        }

        public static T FromJSon<T>(this string str)
        {
            try
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(str);
            }
            catch { }

            return default(T);
        }

        public static string ByteArrayToString(byte[] ba)
        {
            string hex = BitConverter.ToString(ba);
            return hex.Replace("-", "");
        }

        public static byte[] StringToByteArray(String hex)
        {
            byte[] bytes = new byte[hex.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
                bytes[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            return bytes;
        }

        static List<string> UnicodeChar = new List<string>(){
            // Chu thuong
            "à", "á","ả","ã","ạ",
            "ă","ằ","ắ","ẳ","ẵ","ặ",
            "â","ầ","ấ","ẩ","ẫ","ậ",
            "đ",
            "è","é","ẻ","ẽ","ẹ",
            "ê","ề","ế","ể","ễ","ệ",
            "ì","í","ỉ","ĩ","ị",
            "ò","ó","ỏ","õ","ọ",
            "ô","ồ","ố","ổ","ỗ","ộ",
            "ơ","ờ","ớ","ở","ỡ","ợ",
            "ù","ú","ủ","ũ","ụ",
            "ư","ừ","ứ","ử","ữ","ự",
            "ỳ","ý","ỷ","ỹ","ỵ",
            // Chu hoa
            "À", "Á","Ả","Ã","Ạ",
            "Ă","Ằ","Ắ","Ẳ","Ẵ","Ặ",
            "Â","Ầ","Ấ","Ẩ","Ẫ","Ậ",
            "Đ",
            "È","É","Ẻ","Ẽ","Ẹ",
            "Ê","Ề","Ế","Ể","Ễ","Ệ",
            "Ì","Í","Ỉ","Ĩ","Ị",
            "Ò","Ó","Ỏ","Õ","Ọ",
            "Ô","Ồ","Ố","Ổ","Ỗ","Ộ",
            "Ơ","Ờ","Ớ","Ở","Ỡ","Ợ",
            "Ù","Ú","Ủ","Ũ","Ụ",
            "Ư","Ừ","Ứ","Ử","Ữ","Ự",
            "Ỳ","Ý","Ỷ","Ỹ","Ỵ"
        };

        static List<string> BkHCM2Char = new List<string>(){
            // Chu thuong
            "aâ", "aá","aã","aä","aå",
            "ù","ùç","ùæ","ùè","ùé","ùå",
            "ê","êì","êë","êí","êî","êå",
            "à",
            "eâ","eá","eã","eä","eå",
            "ï","ïì","ïë","ïí","ïî","ïå",
            "ò","ñ","ó","ô","õ",
            "oâ","oá","oã","oä","oå",
            "ö","öì","öë","öí","öî","öå",
            "ú","úâ","úá","úã","úä","úå",
            "uâ","uá","uã","uä","uå",
            "û","ûâ","ûá","ûã","ûä","ûå",
            "yâ","yá","yã","yä","yå",
            // Chu hoa
            "AÂ", "AÁ","AÃ","AÄ","AÅ",
            "Ù","ÙÇ","ÙÆ","ÙÈ","ÙÉ","ÙÅ",
            "Ê","ÊÌ","ÊË","ÊÍ","ÊÎ","ÊÅ",
            "À",
            "EÂ","EÁ","EÃ","EÄ","EÅ",
            "Ï","ÏÌ","ÏË","ÏÍ","ÏÎ","Ïå",
            "Ò","Ñ","Ó","Ô","Õ",
            "OÂ","OÁ","OÃ","OÄ","OÅ",
            "Ö","ÖÌ","ÖË","ÖÍ","ÖÎ","ÖÅ",
            "Ú","ÚÂ","ÚÁ","ÚÃ","ÚÄ","ÚÅ",
            "UÂ","UÁ","UÃ","UÄ","UÅ",
            "Û","ÛÂ","ÛÁ","ÛÃ","ÛÄ","ÛÅ",
            "YÂ","YÁ","YÃ","YÄ","YÅ"
        };

        public static string UnicodeToBkHCM2(string unicodeStr)
        {
            string result = "";
            for (int index = 0; index < unicodeStr.Length; index++)
            {
                var bkChar = unicodeStr[index].ToString();
                var charIndex = UnicodeChar.IndexOf(bkChar);
                if (charIndex >= 0)
                    bkChar = BkHCM2Char[charIndex];
                result += bkChar;
            }
            return result;
        }

        public static string BkHCM2ToUnicode(string bkStr)
        {
            string result = "";
            for (int index = 0; index < bkStr.Length; index++)
            {
                var unicodeChar = bkStr[index].ToString();
                var unicodeChar2 = unicodeChar;
                if (index < bkStr.Length - 1)
                    unicodeChar2 += bkStr[index + 1].ToString();
                
                int charIndex = BkHCM2Char.IndexOf(unicodeChar2);
                if (charIndex >= 0)
                {
                    unicodeChar = UnicodeChar[charIndex];
                    index++;
                }
                else if (unicodeChar != unicodeChar2)
                {
                    charIndex = BkHCM2Char.IndexOf(unicodeChar);
                    if (charIndex >= 0)
                        unicodeChar = UnicodeChar[charIndex];
                }
                result += unicodeChar;
            }

            return result;
        }

        public static List<string> LocalIP()
        {
            List<string> result = new List<string>();

            IPHostEntry host;
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily.ToString() == "InterNetwork")
                {
                    result.Add(ip.ToString());
                }
            }

            return result;
        }

        public static string SqlSelectColumnParameter(Type type, string header)
        {
            string result = "";
            foreach (var property in type.GetProperties())
            {
                if (property.CanWrite)
                {
                    if (result != "") result += ",";
                    if (header != "") result += header + ".";
                    result += "[" + property.Name + "]";
                }
            }

            return result;
        }
    }
}
