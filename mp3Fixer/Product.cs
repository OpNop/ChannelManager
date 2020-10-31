using System;
using System.Collections.Generic;
using System.Data.Odbc;

namespace mp3Fixer
{

    public class MP3
    {
        public string LocalFilePath = @".\Data\Attachments\Products\";
        public string FileName { get; set; }
        public string FileDesc { get; set; }
        public string FileServerPath { get; set; }

        public MP3(string file, string desc)
        {
            FileName = file;
            FileDesc = desc;
        }
    }

    public class Product
    {
        public string ProductNo { get; set; }
        public int SKU { get; set; }
        public List<MP3> Files { get; set; }

        public Product(OdbcDataReader recordSet)
        {
            SKU = int.Parse(recordSet.GetString(0));
            ProductNo = recordSet.GetString(1);
            Files = new List<MP3>();
            getFiles();
        }

        private void getFiles()
        {
            var command = mwODBC.Instance.mwConn.CreateCommand();
            command.CommandText = "SELECT FileName, description FROM ProductAttachments WHERE ProductNo='" + ProductNo + "' AND Type='File'";
            var mwReader = command.ExecuteReader();

            if (mwReader.RecordsAffected > 0)
            {
                while (mwReader.Read())
                {
                    var fileDesc = (Convert.IsDBNull(mwReader.GetValue(1)) ? mwReader.GetString(0) : mwReader.GetString(1));
                    Files.Add(new MP3(mwReader.GetString(0), fileDesc));
                }
            }

            command.Dispose();
            mwReader.Dispose();
        }
    }
}
