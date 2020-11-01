using System;
using System.Collections.Generic;
using System.Data.Odbc;
using DLG.ToolBox.Log;

namespace Service
{
    class MailWare
    {
        //The log object
        private readonly string _doNotUpdate = "12442 12443 12463 12465";
        private static readonly Logger _log = Logger.getInstance();
        private OdbcConnection _db;

        // Fix a couple of database issues that cause the update service to fail
        public Boolean CleanDatabase()
        {
            var command = _db.CreateCommand();
            var commandUpdate = _db.CreateCommand();

            try
            {
                command.CommandText = "DROP TABLE IF EXISTS tmpListingTable; ";
                command.CommandText += "CREATE TABLE tmpListingTable (tmpChannelNo Integer, tmpChannelAccountNo Integer, tmpListingNo Integer); ";
                command.CommandText += "INSERT INTO tmpListingTable(tmpChannelNo, tmpChannelAccountNo, tmpListingNo)";
                command.CommandText +=      "SELECT ChannelNo, ChannelAccountNo, ListingNo FROM ChannelListings WHERE SKU <> CAST(ListingNo AS CHAR(100));";
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                _log.AddError("Error creating tmpListingTable");
                _log.AddError(e.Message);
                _log.AddError(e.StackTrace);
            }

            command.CommandText = "SELECT * FROM ChannelListings";
            commandUpdate.CommandText = "SELECT * FROM tmpListingTable";
            try
            {
                var mwUpdateReader = commandUpdate.ExecuteReader();
                while (mwUpdateReader.Read())
                {
                    command.CommandText = "UPDATE ChannelListings SET SKU='" + mwUpdateReader.GetInt32(2) + "' WHERE ChannelNo=" + mwUpdateReader.GetInt32(0) + " AND ChannelAccountNo=" + mwUpdateReader.GetInt32(1) + " AND ListingNo=" + mwUpdateReader.GetInt32(2);
                    command.ExecuteNonQuery();
                }
                mwUpdateReader.Close();
            }
            catch (Exception e)
            {
                _log.AddError("Error updating ChannelListings");
                _log.AddError(e.Message);
                _log.AddError(e.StackTrace);
            }

            command.CommandText = "UPDATE ChannelListings SET IsUseProductNoAsSKU=False";
            try
            {
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                _log.AddError(e.Message);
                _log.AddError(e.StackTrace);
            }
            command.Dispose();
            commandUpdate.Dispose();
            return true;
        }

        public List<Product> getProductsToUpdate()
        {
            var productsToUpdate = new List<Product>();
            var command = _db.CreateCommand();

            command.CommandText = "SELECT cl.ProductNo, cl.SKU, cl.Price, cl.IsUseCurrentRetailPrice, cl.IsOnSale, cl.SalePrice, cl.SaleStartDt, cl.SaleEndDt, cl.IsActive, cl.IsRemove, cl.IsDelete, cl.LaunchDt, cl.LastStockUpdateTime, cl.CategoryNo, p.LastUpdated, p.Price, p.UPC, p.Bin, p.StockNo, p.Weight, p.MixAndMatchCode, cl.IsUseCurrentInStock, p.InStock, p.LastInStockUpdated FROM ChannelListings cl INNER JOIN Products p ON cl.ProductNo=p.ProductNo WHERE (cl.isActive=true AND p.ProductNo=cl.ProductNo) AND ((p.LastUpdated > cl.LastStockUpdateTime) OR (cl.LastStockUpdateTime = null) OR (cl.LastStockUpdateTime < p.LastInStockUpdated))";

            try
            {
                var mwReader = command.ExecuteReader();
                while (mwReader.Read())
                {
                    var lastUpdate = Utils.processMwTime(mwReader.GetString(14));
                    var lastwebUpdate = Convert.IsDBNull(mwReader.GetValue(12)) ? new DateTime(0) : Utils.processMwTime(mwReader.GetString(12));
                    var lastQtyUpdate = Convert.IsDBNull(mwReader.GetValue(23)) ? new DateTime(0) : Utils.processMwTime(mwReader.GetString(23));
                    var psSKU = Convert.IsDBNull(mwReader.GetValue(1)) ? "" : mwReader.GetString(1);

                    //skip items that might be better formatted on the website
                    if(_doNotUpdate.Contains(psSKU)) {continue;}

                    if ((lastUpdate > lastwebUpdate) || (lastQtyUpdate > lastwebUpdate))
                    {
                        productsToUpdate.Add(new Product(mwReader));
                    }
                }

            }
            catch (Exception e)
            {
                _log.AddError(e.Message);
                _log.AddError(e.StackTrace);
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

        public void updateLastModDate(int productSku)
        {
            //if all went well, lets update the DB so we know it
            var command = _db.CreateCommand();
            command.CommandText = "UPDATE ChannelListings SET LastStockUpdateTime='" + Utils.createMwTime(DateTime.Now) + "' WHERE SKU='" + productSku + "'";
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
                command.CommandText = "SELECT p.Description, p.LongDesc, p.SupplierNo, s.Company, p.ProdType, p.IsDiscontinued, p.MixAndMatchCode FROM Products p INNER JOIN Supplier s ON p.SupplierNo = s.SupplierNo WHERE p.ProductNo='" + item.ProductNo + "'";
                var mwReader = command.ExecuteReader();

                mwReader.Read();

                item.Description = Convert.IsDBNull(mwReader.GetValue(0)) ? "" : mwReader.GetString(0);
                item.LongDesc = Convert.IsDBNull(mwReader.GetValue(1)) ? "" : mwReader.GetString(1).Replace("\n", "<br />");
                item.SupplierNo = Convert.IsDBNull(mwReader.GetValue(2)) ? 0 : Convert.ToInt32(mwReader.GetValue(2));
                item.SupplierCo = Convert.IsDBNull(mwReader.GetString(3)) ? "" : mwReader.GetString(3);
                item.ProdType = Convert.IsDBNull(mwReader.GetValue(4)) ? "" : mwReader.GetString(4);
                item.IsDiscontinued = Convert.IsDBNull(mwReader.GetValue(5)) ? false : mwReader.GetBoolean(5);
                item.MixAndMatch = Convert.IsDBNull(mwReader.GetValue(6)) ? "" : mwReader.GetString(6);

                item.Images = getImageData(item);
                item.Files = getFileData(item);

                command.Dispose();
                mwReader.Dispose();
            }
            catch (Exception e)
            {
                _log.AddError("Error Getting \"getExtData\"");
                _log.AddError(e.Message);
                _log.AddError(e.StackTrace);
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
                _log.AddInfo("Connecting to MW DB.......");
                _db.Open();
                _log.AddNotice("Success! DB Server version: " + _db.ServerVersion);
                return true;
            }
            catch (Exception ex)
            {
                _log.AddError("Failed\n");
                _log.AddError("**" + ex.Message);
                Console.WriteLine("Press any key to Exit!!");
                Console.ReadLine();
                return false;
            }
        }
    }
}
