using System;
using Microsoft.Win32;

namespace Client
{
    public class Settings
    {
        private static readonly RegistryKey RegKey = Registry.LocalMachine.CreateSubKey(@"Software\DLG Networks\DLG MCM");

        private static string _sqlHost;
        private static string _sqlDb;
        private static string _sqlUser;
        private static string _sqlPass;
        private static string _ftpHost;
        private static string _ftpUser;
        private static string _ftpPass;

        private static Boolean _prodUpload;
        private static int _prodUploadInt;
        private static Boolean _shipUpload;
        private static Boolean _orderDownload;
        private static int _orderDownloadInt;
        private static int _operatorId;

        //Log Settings
        private static string _logDir;
        private static bool _showNotice;
        private static bool _showInformation;
        private static bool _showWarning;
        private static bool _showError;
        private static bool _showDebug;

        private static string _lastError;
        public static Boolean Error;


        static Settings()
        {
            try
            {
                _sqlHost          = RegKey.GetValue("sqlHost", "localhost").ToString();
                _sqlDb            = RegKey.GetValue("sqlDb", "zencart").ToString();
                _sqlUser          = RegKey.GetValue("sqlUser", "root").ToString();
                _sqlPass          = RegKey.GetValue("sqlPass", "").ToString();
                _ftpHost          = RegKey.GetValue("ftpHost", "").ToString();
                _ftpUser          = RegKey.GetValue("ftpUser", "").ToString();
                _ftpPass          = RegKey.GetValue("ftpPass", "").ToString();
                _prodUpload       = Convert.ToBoolean(RegKey.GetValue("prodUpload", "false").ToString());
                _prodUploadInt    = Convert.ToInt32(RegKey.GetValue("prodUploadInt", "1").ToString());
                _shipUpload       = Convert.ToBoolean(RegKey.GetValue("shipUpload", "false").ToString());
                _orderDownload    = Convert.ToBoolean(RegKey.GetValue("orderDownload", "false").ToString());
                _orderDownloadInt = Convert.ToInt32(RegKey.GetValue("orderDownloadInt", "1").ToString());
                _operatorId       = Convert.ToInt32(RegKey.GetValue("operatorId", "1").ToString());

                //Log Settings
                _logDir           = RegKey.GetValue("logDir", "./logs").ToString();
                _showNotice       = Convert.ToBoolean(RegKey.GetValue("showNotice", "true").ToString());
                _showInformation  = Convert.ToBoolean(RegKey.GetValue("showInformation", "true").ToString());
                _showWarning      = Convert.ToBoolean(RegKey.GetValue("showWarning", "true").ToString());
                _showError        = Convert.ToBoolean(RegKey.GetValue("showError", "true").ToString());
                _showDebug        = Convert.ToBoolean(RegKey.GetValue("showDebug", "true").ToString());

                Error = false;
            }
            catch (Exception err)
            {
                _lastError = err.Message + "\n" + err.StackTrace;
                Error = true;
            }
        }

        public static Boolean SaveSettings()
        {
            try
            {
                RegKey.SetValue("sqlHost", _sqlHost, RegistryValueKind.String);
                RegKey.SetValue("sqlDb", _sqlDb, RegistryValueKind.String);
                RegKey.SetValue("sqlUser", _sqlUser, RegistryValueKind.String);
                RegKey.SetValue("sqlPass", _sqlPass, RegistryValueKind.String);

                RegKey.SetValue("ftpHost", _ftpHost, RegistryValueKind.String);
                RegKey.SetValue("ftpUser", _ftpUser, RegistryValueKind.String);
                RegKey.SetValue("ftpPass", _ftpPass, RegistryValueKind.String);

                RegKey.SetValue("prodUpload", _prodUpload, RegistryValueKind.String);
                RegKey.SetValue("prodUploadInt", _prodUploadInt, RegistryValueKind.DWord);
                RegKey.SetValue("shipUpload", _shipUpload, RegistryValueKind.String);
                RegKey.SetValue("orderDownload", _orderDownload, RegistryValueKind.String);
                RegKey.SetValue("orderDownloadInt", _orderDownloadInt, RegistryValueKind.DWord);
                RegKey.SetValue("operatorId", _operatorId, RegistryValueKind.DWord);
                
                //LogSettings
                RegKey.SetValue("logDir", _logDir, RegistryValueKind.String);
                RegKey.SetValue("showNotice", _showNotice, RegistryValueKind.String);
                RegKey.SetValue("showInformation", _showInformation, RegistryValueKind.String);
                RegKey.SetValue("showWarning", _showWarning, RegistryValueKind.String);
                RegKey.SetValue("showError", _showError, RegistryValueKind.String);
                RegKey.SetValue("showDebug", _showDebug, RegistryValueKind.String);


                return true;
            }
            catch (Exception err)
            {
                _lastError = err.Message + "\n" + err.StackTrace;
                return false;
            }
        }

        #region Getters and Setters

        public static string SqlHost
        {
            get { return _sqlHost; }
            set { _sqlHost = value; }
        }
        public static string SqlDb
        {
            get { return _sqlDb; }
            set { _sqlDb = value; }
        }
        public static string SqlUser
        {
            get { return _sqlUser; }
            set { _sqlUser = value; }
        }
        public static string SqlPass
        {
            get { return _sqlPass; }
            set { _sqlPass = value; }
        }
        public static string FtpHost
        {
            get { return _ftpHost; }
            set { _ftpHost = value; }
        }
        public static string FtpUser
        {
            get { return _ftpUser; }
            set { _ftpUser = value; }
        }

        public static string FtpPass
        {
            get { return _ftpPass; }
            set { _ftpPass = value; }
        }
        public static Boolean ProdUpload
        {
            get { return _prodUpload; }
            set { _prodUpload = value; }
        }
        public static int ProdUploadInt
        {
            get { return _prodUploadInt; }
            set { _prodUploadInt = value; }
        }
        public static Boolean ShipUpload
        {
            get { return _shipUpload; }
            set { _shipUpload = value; }
        }
        public static Boolean OrderDownload
        {
            get { return _orderDownload; }
            set { _orderDownload = value; }
        }
        public static int OrderDownloadInt
        {
            get { return _orderDownloadInt; }
            set { _orderDownloadInt = value; }
        }
        public static int OperatorId
        {
            get { return _operatorId; }
            set { _operatorId = value; }
        }
        public static string LastError
        {
            get { return _lastError; }
            set { _lastError = value; }
        }

        //Log Settings
        public static string LogDir
        {
            get { return _logDir; }
            set { _logDir = value; }
        }
        public static Boolean ShowNotice
        {
            get { return _showNotice; }
            set { _showNotice = value; }
        }
        public static Boolean ShowWarning
        {
            get { return _showWarning; }
            set { _showWarning = value; }
        }
        public static Boolean ShowInformation
        {
            get { return _showInformation; }
            set { _showInformation = value; }
        }
        public static Boolean ShowError
        {
            get { return _showError; }
            set { _showError = value; }
        }
        public static Boolean ShowDebug
        {
            get { return _showDebug; }
            set { _showDebug = value; }
        }

        #endregion
    }
}
