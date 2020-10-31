using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace Service
{
    public class MySqlWrapper
    {
        private static MySqlConnection _connection;
        public static string ConnectionString { get; set; }

        public static MySqlConnection Connection
        {
            get
            {
                if(_connection.State != ConnectionState.Open)
                    _connection.Open();
                return _connection;
            }
            set { _connection = value; }
        }

        /*public static bool InsertRecord(MySqlCommand sqlCommand)
        {
            bool success = false;
            using (var con = new MySqlConnection()) ;
        }
         */
    }
}
