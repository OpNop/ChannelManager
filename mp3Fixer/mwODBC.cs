using System;
using System.Diagnostics;
using System.Data.Odbc;

namespace mp3Fixer
{
    public class mwODBC
    {
        private static mwODBC _instance;

        public OdbcConnection mwConn { get; private set; }

        private mwODBC()
        {
            Connect();
        }

        public static mwODBC Instance
        {
            get { return _instance ?? (_instance = new mwODBC()); }
        }

        public bool Connect()
        {
            mwConn = new OdbcConnection
            {
                ConnectionString = "DRIVER={DBISAM 4 ODBC Driver};" +
                                   "ConnectionType=Local;" +
                                   "CatalogName=./data"
            };
            try
            {
                Debug.WriteLine("Connecting to MW DB.......");
                mwConn.Open();
                Debug.WriteLine("Success! Server version: " + mwConn.ServerVersion);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed\n");
                Debug.WriteLine("**" + ex.Message);
                return false;
            }
        }
    }
}
