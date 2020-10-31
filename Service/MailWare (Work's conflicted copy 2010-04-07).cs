using System;
using System.Collections.Generic;
using System.Data.Odbc;
using DLG.Log;

namespace Service
{
    class MailWare
    {
        //The log object
        public static Log SrvLog;
        private OdbcConnection _db;

        public MailWare(Log mainLog)
        {
            SrvLog = mainLog;
        }

        public List<Product> getProductsToUpdate()
        {
            var productsToUpdate = new List<Product>();
            var command = _db.CreateCommand();

            command.CommandText = "SELECT cl.ProductNo, cl.SKU, cl.Price, cl.IsUseCurrentRetailPrice, cl.IsOnSale, cl.SalePrice, cl.SaleStartDt, cl.SaleEndDt, cl.IsActive, cl.IsRemove, cl.IsDelete, cl.LaunchDt, cl.LastStockUpdateTime, cl.CategoryNo, p.LastUpdated, p.Price, p.UPC, p.Bin, p.StockNo, p.Weight FROM ChannelListings cl INNER JOIN Products p ON cl.ProductNo=p.ProductNo WHERE p.ProductNo=cl.ProductNo";
            var mwReader = command.ExecuteReader();

            while (mwReader.Read())
            {
                var lastUpdate = Utils.processMwTime(mwReader.GetString(14));
                var lastwebUpdate = Convert.IsDBNull(mwReader.GetValue(12)) ? new DateTime(0) : Utils.processMwTime(mwReader.GetString(12));

                if (lastUpdate > lastwebUpdate)
                {
                    productsToUpdate.Add(new Product(mwReader, SrvLog));
                }
            }
            return productsToUpdate;
        }

        public List<string> getImageData(Product item)
        {
            var images = new List<string>();

            var command = _db.CreateCommand();
            command.CommandText = "SELECT FileName FROM ProductAttachments WHERE ProductNo='" + item.ProductNo + "' AND Type='Picture'";
            var mwReader = command.ExecuteReader();

            if (mwReader.RecordsAffected == 0)
                images.Add("ImageNotAvailable.jpg");
            else
                while (mwReader.Read()) { images.Add(mwReader.GetString(0)); }

            command.Dispose();
            mwReader.Dispose();

            return images;
        }

        public void updateLastModDate(int productSKU)
        {
            //if all went well, lets update the DB so we know it
            var command = _db.CreateCommand();
            command.CommandText = "UPDATE ChannelListings SET LastStockUpdateTime='" + Utils.createMwTime(DateTime.Now) + "' WHERE SKU=" + productSKU;
            command.ExecuteNonQuery();
        }

        public List<File> getFileData(Product item)
        {
            var files = new List<File>();

            var command = _db.CreateCommand();
            command.CommandText = "SELECT FileName, description FROM ProductAttachments WHERE ProductNo='" + item.ProductNo + "' AND Type='File'";
            var mwReader = command.ExecuteReader();

            if (mwReader.RecordsAffected > 0)
            {
                while (mwReader.Read())
                {
                    var fileDesc = (Convert.IsDBNull(mwReader.GetValue(1)) ? mwReader.GetString(0) : mwReader.GetString(1));
                    files.Add(new File(mwReader.GetString(0), fileDesc));
                }
            }

            command.Dispose();
            mwReader.Dispose();

            return files;
        }

        public void getExtData(Product item)
        {
            try
            {
                var command = _db.CreateCommand();
                command.CommandText = "SELECT p.Description, p.LongDesc, p.SupplierNo, s.Company, p.ProdType, p.IsDiscontinued FROM Products p INNER JOIN Supplier s ON p.SupplierNo = s.SupplierNo WHERE p.ProductNo='" + item.ProductNo + "'";
                var mwReader = command.ExecuteReader();

                mwReader.Read();

                item.Description = Convert.IsDBNull(mwReader.GetValue(0)) ? "" : mwReader.GetString(0);
                item.LongDesc = Convert.IsDBNull(mwReader.GetValue(1)) ? "" : mwReader.GetString(1).Replace("\n", "<br />");
                item.SupplierNo = Convert.IsDBNull(mwReader.GetValue(2)) ? 0 : Convert.ToInt32(mwReader.GetValue(2));
                item.SupplierCo = Convert.IsDBNull(mwReader.GetString(3)) ? "" : mwReader.GetString(3);
                item.ProdType = Convert.IsDBNull(mwReader.GetValue(4)) ? "" : mwReader.GetString(4);
                item.IsDiscontinued = Convert.IsDBNull(mwReader.GetValue(5)) ? false : mwReader.GetBoolean(5);

                item.Images = getImageData(item);
                item.Files = getFileData(item);

                command.Dispose();
                mwReader.Dispose();
            }
            catch (Exception e)
            {
                SrvLog.PutLog(LogLevel.E, "Error Getting \"getExtData\"");
                SrvLog.PutLog(LogLevel.E, e.Message);
                SrvLog.PutLog(LogLevel.E, e.StackTrace);
            }
        }

        public Boolean connect()
        {
            //Mailware Connection
            _db = new OdbcConnection
                  {
                      ConnectionString = "DRIVER={DBISAM 4 ODBC Driver};" +
                                         "ConnectionType=Local;" +
                                         "CatalogName=./data"
                  };

            try
            {
                SrvLog.PutLog(LogLevel.I, "Connecting to MW DB.......");
                _db.Open();
                SrvLog.PutLog(LogLevel.N, "Success! Server version: " + _db.ServerVersion);
                return true;
            }
            catch (Exception ex)
            {
                SrvLog.PutLog(LogLevel.E, "Failed\n");
                SrvLog.PutLog(LogLevel.E, "**" + ex.Message);
                SrvLog.Pause();
                return false;
            }
        }
    }
}
