using System;
using System.Reflection;
using System.Text.RegularExpressions;
using DLG.ToolBox.Log;

namespace Service
{
    public static class Utils
    {
        private static readonly Logger _log = Logger.getInstance();

        public static DateTime processMwTime(string mwTime)
        {
            //Mailware Date Format
            //YYYY/MM/DD HH:MM:SS.NNN
            //20090916114135766
            //2009/09/16 11:41:35.766

            DateTime thisDate;
            if (!DateTime.TryParseExact(mwTime, "yyyyMMddHHmmssFFF", new System.Globalization.CultureInfo("en-US"), System.Globalization.DateTimeStyles.None, out thisDate))
            {
                _log.AddError("Error Parsing Date. setting to beginning of time :P (" + mwTime + ")");
                thisDate = new DateTime(0);
            }

            return thisDate;
        }

        public static string createMwTime(DateTime date)
        {
            //Mailware Date Format
            //YYYY/MM/DD HH:MM:SS.NNN
            //20090916114135766
            //2009/09/16 11:41:35.766

            return date.ToString("yyyyMMddHHmmssFFF");
        }

        public static string MySqlEscape(string usString, bool escapeBR)
        {
            if (usString == null)
            {
                return null;
            }
            usString = Regex.Replace(usString, "’", "'");
            usString = Regex.Replace(usString, "‘", "'");
            usString = Regex.Replace(usString, "—", "-");
            usString = Regex.Replace(usString, "”", "\"");
            usString = Regex.Replace(usString, "“", "\"");
            if (escapeBR)
            {
                usString = Regex.Replace(usString, "<br />", "");
                usString = Regex.Replace(usString, "<br>", "");
            }
            return usString;
            //return Regex.Replace(usString, @"[\r\n\x00\x1a\\'""]", @"\$0");
        }

        public static string buildDate()
        {
            var date = new DateTime(2000, 1, 1);
            var parts = Assembly.GetExecutingAssembly().FullName.Split(',');
            var versionParts = parts[1].Split('.');
            date = date.AddDays(Int32.Parse(versionParts[2]));
            date = date.AddSeconds(Int32.Parse(versionParts[3]) * 2);
            return date.ToString();
        }

        public static string buildLink(string itemName)
        {
            var clearPunc = new Regex(@"[^a-z0-9\s\'\:\/\[\]-]");
            var fixSpaces = new Regex(@"[\s\'\:\/\[\]-]+");
            var convertSpaces = new Regex(@"[ ]");

            var str = itemName.ToLower();
            str = clearPunc.Replace(str, "");
            str = fixSpaces.Replace(str, " ");
            str = convertSpaces.Replace(str, "-");

            return str;
        }

        public static string Truncate(string source, int length)
        {
            if (source.Length > length)
            {
                source = source.Substring(0, length);
            }
            return source;
        }
    }
}
