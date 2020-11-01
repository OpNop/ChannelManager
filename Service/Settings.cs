using System;
using Microsoft.Win32;
using DLG.ToolBox.Log;

namespace Service
{
    public class Settings
    {
        private static readonly RegistryKey RegKey = Registry.LocalMachine.CreateSubKey(@"Software\DLG Networks\DLG MCM");

        private static readonly Logger _log = Logger.getInstance();
        
        public static readonly string SqlHost;
        public static readonly string SqlDb;
        public static readonly string SqlUser;
        public static readonly string SqlPass;
        public static readonly Boolean ProdUpload;
        public static readonly int ProdUploadInt;
        public static readonly Boolean ShipUpload;
        public static readonly Boolean OrderDownload;
        public static readonly int OrderDownloadInt;
        public static readonly int OperatorId;

        //FTP Settings
        public static readonly string FtpServer;
        public static readonly string FtpUser;
        public static readonly string FtpPass;
        public static readonly string FtpRoot = "/";

        //Log Settings
        public static readonly string LogDirectory;
        public static readonly Boolean ShowNotice;
        public static readonly Boolean ShowInformation;
        public static readonly Boolean ShowWarning;
        public static readonly Boolean ShowError;
        public static readonly Boolean ShowDebug;

        public static Boolean Error;
        public static string LastError;

        static Settings()
        {
            try
            {
                SqlHost          = RegKey.GetValue("sqlHost", "localhost").ToString();
                SqlDb            = RegKey.GetValue("sqlDb", "zencart").ToString();
                SqlUser          = RegKey.GetValue("sqlUser", "root").ToString();
                SqlPass          = RegKey.GetValue("sqlPass", "").ToString();
                ProdUpload       = Convert.ToBoolean(RegKey.GetValue("prodUpload", "true").ToString());
                ProdUploadInt    = Convert.ToInt32(RegKey.GetValue("prodUploadInt", "1").ToString());
                ShipUpload       = Convert.ToBoolean(RegKey.GetValue("shipUpload", "true").ToString());
                OrderDownload    = Convert.ToBoolean(RegKey.GetValue("orderDownload", "true").ToString());
                OrderDownloadInt = Convert.ToInt32(RegKey.GetValue("orderDownloadInt", "1").ToString());
                OperatorId       = Convert.ToInt32(RegKey.GetValue("operatorId", "1").ToString());
                //FTP Settings
                FtpServer        = RegKey.GetValue("ftpHost", "localhost").ToString();
                FtpUser          = RegKey.GetValue("ftpUser", "ftpUser").ToString();
                FtpPass          = RegKey.GetValue("ftpPass", "ftpPass").ToString();
                //Log Settings
                LogDirectory     = RegKey.GetValue("logDir", @".\logs").ToString();
                ShowNotice       = Convert.ToBoolean(RegKey.GetValue("showNotice", "true").ToString());
                ShowInformation  = Convert.ToBoolean(RegKey.GetValue("showInformation", "true").ToString());
                ShowWarning      = Convert.ToBoolean(RegKey.GetValue("showWarning", "true").ToString());
                ShowError        = Convert.ToBoolean(RegKey.GetValue("showError", "true").ToString());
                ShowDebug        = Convert.ToBoolean(RegKey.GetValue("showDebug", "true").ToString());

                
                Error = false;
            }
            catch (Exception err)
            {
                LastError = err.Message + "\n" + err.StackTrace;
                Error = true;
            }
        }

        public static void DumpSettings()
        {
            try
            {
                _log.AddDebug("**************");
                _log.AddDebug("*  Settings  *");
                _log.AddDebug("**************");
                _log.AddDebug("SqlHost...........{0}", SqlHost);
                _log.AddDebug("SqlDb.............{0}", SqlDb);
                _log.AddDebug("SqlUser...........{0}", SqlUser);
                _log.AddDebug("SqlPass...........{0}", SqlPass);
                _log.AddDebug("ProdUpload........{0}", ProdUpload);
                _log.AddDebug("ProdUploadInt.....{0}", ProdUploadInt);
                _log.AddDebug("ShipUpload........{0}", ShipUpload);
                _log.AddDebug("OrderDownload.....{0}", OrderDownload);
                _log.AddDebug("OrderDownloadIn...{0}", OrderDownloadInt);
                _log.AddDebug("OperatorId........{0}", OperatorId);
                _log.AddDebug("");
                _log.AddDebug("Log Settings");
                _log.AddDebug("Log Dir...........{0}", LogDirectory);
                _log.AddDebug("ShowNotices.......{0}", ShowNotice);
                _log.AddDebug("ShowInformation...{0}", ShowInformation);
                _log.AddDebug("ShowWarnings......{0}", ShowWarning);
                _log.AddDebug("ShowErrors........{0}", ShowError);
                _log.AddDebug("ShowDebug.........{0}", ShowDebug);
            }
            catch(Exception e)
            {
                Console.Write(e.Source + "\n" + e.StackTrace + "\n" + e.InnerException);
            }
        }
    }
}
