using System;
using System.Collections.Generic;
using System.Data.Odbc;
using DLG.Log;

namespace Service
{
    public class File
    {
        public string FileName;
        public string FileDesc;
        public string FileType;

        public File(string file, string desc)
        {

            FileName = file;
            FileDesc = desc;
        }
    }

    public class Product
    {
        private static Log _srvLog;
        private const string LocalFilePath = @".\Data\Attachments\Products\";
        private const string SrvImagePath = "img/p/";
        private const string SrvMP3Path = "modules/mp3player/upload/";

        public string ProductNo;
        public int SKU;
        public decimal Price;
        public bool IsUseCurrentRetailPrice;
        public bool IsOnSale;
        public decimal SalePrice;
        public DateTime SaleStartDt;
        public DateTime SaleEndDt;
        public bool IsActive;
        public bool IsRemove;
        public bool IsDelete;
        public DateTime LaunchDt;
        public DateTime LastStockUpdateTime;
        public int CategoryNo;
        public DateTime LastUpdated;
        public decimal RealPrice;
        public decimal CLPrice;

        //Ext Data
        public string Description;
        public string LongDesc;
        public int SupplierNo;
        public string SupplierCo;
        public string ProdType;
        public bool IsDiscontinued;
        public string UPC;
        public string Bin;
        public decimal Wholesale;
        public decimal Weight;
        public List<string> Images = new List<string>();
        public List<File> Files = new List<File>();
        

        public Product(OdbcDataReader recordSet, Log mainlog)
        {
            _srvLog = mainlog;

            ProductNo               = recordSet.GetString(0);
            SKU                     = int.Parse(recordSet.GetString(1));
            CLPrice                 = Convert.ToDecimal(recordSet.GetValue(2));
            IsUseCurrentRetailPrice = recordSet.GetBoolean(3);
            IsOnSale                = recordSet.GetBoolean(4);
            SalePrice               = Convert.IsDBNull(recordSet.GetValue(5)) ? new decimal(0.0f) : Convert.ToDecimal(recordSet.GetValue(5));
            SaleStartDt             = Convert.IsDBNull(recordSet.GetValue(6)) ? new DateTime(0) : recordSet.GetDate(6);
            SaleEndDt               = Convert.IsDBNull(recordSet.GetValue(7)) ? new DateTime(0) : recordSet.GetDate(7);
            IsActive                = recordSet.GetBoolean(8);
            IsRemove                = recordSet.GetBoolean(9);
            IsDelete                = recordSet.GetBoolean(10);
            LaunchDt                = Convert.IsDBNull(recordSet.GetValue(11)) ? new DateTime(0) : recordSet.GetDate(11);
            LastStockUpdateTime     = Convert.IsDBNull(recordSet.GetValue(12)) ? new DateTime(0) : Utils.processMwTime(recordSet.GetString(12));
            CategoryNo              = Convert.IsDBNull(recordSet.GetValue(13)) ? 0 : recordSet.GetInt32(13);
            LastUpdated             = Utils.processMwTime(recordSet.GetString(14));
            RealPrice               = Convert.ToDecimal(recordSet.GetValue(15));
            UPC                     = Convert.IsDBNull(recordSet.GetValue(16)) ? "" : recordSet.GetString(16);
            Bin                     = Convert.IsDBNull(recordSet.GetValue(17)) ? "" : recordSet.GetString(17);
            Wholesale               = Convert.IsDBNull(recordSet.GetValue(18)) ? new decimal(0.0f) : Convert.ToDecimal(recordSet.GetValue(18));
            Weight                  = Convert.IsDBNull(recordSet.GetValue(19)) ? new decimal(0.0f) : Convert.ToDecimal(recordSet.GetValue(19));

            //set Price to the price we want to use
            Price = IsUseCurrentRetailPrice ? RealPrice : CLPrice;

        }

        public void uploadImage()
        {
            //if (Images[0] == "no_picture.gif") return;

            var ftpclient = new Ftp(_srvLog);
            var imagePath = LocalFilePath + Images[0];
            var newImageName = SKU + "-" + SKU;
            if (System.IO.File.Exists(imagePath))
            {
                _srvLog.PutLog(LogLevel.D, "Uploading image: " + LocalFilePath + Images[0]);
                ftpclient.Upload(LocalFilePath + Images[0], newImageName, SrvImagePath);
            }
            else
                _srvLog.PutLog(LogLevel.E, "File does not exist: {0}", imagePath);
        }

        public void uploadFiles()
        {
            //no files to upload
            if (Files.Count == 0)
            {
                _srvLog.PutLog(LogLevel.N, "No files attached!!");
                return;
            }

            foreach(var file in Files)
            {
                var filePath = LocalFilePath + file.FileName;

                if (System.IO.File.Exists(filePath))
                {
                    //Checking for mp3 for now, wav, ra, and pdf's will come later
                    if(new System.IO.FileInfo(filePath).Extension == ".mp3")
                    {
                        var ftpclient = new Ftp(_srvLog);
                        ftpclient.Upload(filePath, "", SrvMP3Path);
                    }
                    else
                        _srvLog.PutLog(LogLevel.E, "Skipping {0} because, we dont talk about mailware features.", filePath);
                }
                else
                    _srvLog.PutLog(LogLevel.E, "File does not exist: {0}", filePath);
            }
        }
    }
}
