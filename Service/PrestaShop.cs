using System;
using System.Data;
using MySql.Data.MySqlClient;
using DLG.ToolBox.Log;

namespace Service
{
    class PrestaShop
    {
        //The log object
        private static readonly Logger _log = Logger.getInstance();

        private readonly string _connectionString = "SERVER=" + Settings.SqlHost + ";" +
                                                    "DATABASE=" + Settings.SqlDb + ";" +
                                                    "UID=" + Settings.SqlUser + ";" +
                                                    "PASSWORD=" + Settings.SqlPass + ";";

        //DB connection
        //private MySqlConnection _db;

        public Boolean AddItem(Product item)
        {
            _log.AddDebug("Adding item: " + item.Description);

            const string sql = "INSERT INTO product" +
                                 "(id_product, id_supplier, id_tax, id_category_default, on_sale, ean13, quantity, price, wholesale_price, supplier_reference, location, weight, out_of_stock, active, date_add, date_upd, sale_price)" +
                                 "VALUES " +
                                 "(?product_id, ?supplier_id, 0, ?category_id, ?on_sale, ?product_upc, ?product_quantity, ?product_price, ?product_wholesale, ?product_number, ?product_location, ?product_weight, 1, 1, CURRENT_DATE, CURRENT_DATE, ?sale_price);" +
                               "INSERT INTO product_lang" +
                                 "(id_product, id_lang, description, link_rewrite , meta_description, meta_title, name)" +
                                 "VALUES " +
                                 "(?product_id, 1, ?product_description, ?product_link, ?meta_Description, ?product_name, ?product_name);" +
                               "INSERT INTO image" +
                                 "(id_image ,id_product, position, cover)" +
                                 "VALUES " +
                                 "(?product_id, ?product_id, 1, 1);" +
                               "INSERT INTO category_product" +
                                 "(id_category, id_product, position)" +
                                 "VALUES " +
                                 "(?category_id, ?product_id, 0);" +
                               "INSERT INTO image_lang" +
                                 "(id_image, id_lang, legend)" +
                                 "VALUES" +
                                 "(?product_id, 1, ?product_name);" +
                               "INSERT INTO product_related" +
                                 "(id_product, mixandmatch)" +
                                 "VALUES" +
                                 "(?product_id, ?MixAndMatch)";
            try
            {
                var command = new MySqlCommand(sql);
                
                //Fill in them infos
                var pPrice = (item.IsOnSale ? item.SalePrice : item.Price);
                command.Parameters.AddWithValue("?product_id", item.SKU);
                command.Parameters.AddWithValue("?supplier_id", item.SupplierNo);
                command.Parameters.AddWithValue("?on_sale", Convert.ToInt32(item.IsOnSale));
                command.Parameters.AddWithValue("?category_id", item.CategoryNo);
                command.Parameters.AddWithValue("?product_upc", item.UPC);
                command.Parameters.AddWithValue("?product_quantity", item.InStock);
                command.Parameters.AddWithValue("?product_price", item.Price);
                command.Parameters.AddWithValue("?product_wholesale", item.Wholesale);
                command.Parameters.AddWithValue("?product_number", item.ProductNo);
                command.Parameters.AddWithValue("?product_location", item.Bin);
                command.Parameters.AddWithValue("?product_weight", item.Weight);
                command.Parameters.AddWithValue("?product_description", Utils.MySqlEscape(item.LongDesc, false));
                command.Parameters.AddWithValue("?product_link", Utils.buildLink(item.Description));
                command.Parameters.AddWithValue("?product_name", Utils.MySqlEscape(item.Description, true));
                command.Parameters.AddWithValue("?products_image", item.Images[0]);
                command.Parameters.AddWithValue("?MixAndMatch", item.MixAndMatch);
                command.Parameters.AddWithValue("?sale_price", item.SalePrice);
                command.Parameters.AddWithValue("?meta_Description", Utils.MySqlEscape(Utils.Truncate(item.LongDesc, 255), true));

                ExecuteCommand(command);

                if(item.Files.Count > 0)
                {
                    const string sqlFile = "INSERT INTO mp3player (id_product, mp3_filename, mp3_label, product_list, date_add, date_upd)" +
                                           "VALUES (?product_id, ?file_name, ?file_desc, 1, NOW(), NOW())";
                    const string SQL_DEL_FILE = "DELETE FROM mp3player WHERE id_product = ?product_id";

                    try
                    {
                        //Delete old DB entry for MP3
                        var commandDel = new MySqlCommand(SQL_DEL_FILE);
                        commandDel.Parameters.AddWithValue("?product_id", item.SKU);

                        ExecuteCommand(commandDel);
                    }
                    catch (MySqlException ex)
                    {
                        _log.AddError(ex.ToString());
                        return false;
                    }

                    _log.AddDebug("Uploading the Files");
                    item.uploadFiles();

                    foreach(var file in item.Files)
                    {
                        //Skip non MP3's
                        var dExt = file.FileName.Substring(file.FileName.Length - 3);
                        _log.AddDebug("dExt = {0}", dExt);

                        if (dExt.Equals("mp3", StringComparison.OrdinalIgnoreCase) == false)
                        {
                            continue;
                        }

                        try
                        {
                            var commandFile = new MySqlCommand(sqlFile);

                            //Fill in them infos
                            commandFile.Parameters.AddWithValue("?product_id", item.SKU);
                            commandFile.Parameters.AddWithValue("?file_name", file.FileServerPath);
                            commandFile.Parameters.AddWithValue("?file_desc", MySqlHelper.EscapeString(file.FileDesc));

                            ExecuteCommand(commandFile);
                        }
                        catch (Exception e)
                        {
                            _log.AddError("Error adding MP3 to item: {0}", item.ProductNo);
                            _log.AddError(e.Message);
                            _log.AddError(e.StackTrace);
                        }     
                    }
                }

                _log.AddDebug("Uploading the image");
                item.uploadImage();

            }
            catch (MySqlException ex)
            {
                _log.AddError("sql - Error Adding item: " + item.Description + "(" + item.ProductNo + ")");
                _log.AddError("Error: " + ex.Message);
                _log.AddError(ex.Number.ToString());
                return false;
            }
            catch (Exception e)
            {

                _log.AddError("Error Adding item: " + item.Description + "(" + item.ProductNo + ")");
                _log.AddError("Error: " + e.Message);
                _log.AddError(e.StackTrace);
                return false;
            }
            finally
            {
                _log.AddInfo("Added item: " + item.Description + "(" + item.ProductNo + ")");
            }
            return true;
        }

        public Boolean UpdateItem(Product item)
        {
            _log.AddDebug("Updating item: {0} ({1})", item.Description, item.ProductNo);

            var now = DateTime.Now.ToString("G");
            const string sql = "UPDATE product SET " +
                                 "id_supplier=?supplier_id, " +
                                 "id_category_default=?category_id, " +
                                 "on_sale=?is_on_sale, " +
                                 "ean13=?productUPC, " +
                                 "quantity=?productQuantity, " +
                                 "price=?Price, " +
                                 "wholesale_price=?productWholesale, " +
                                 "supplier_reference=?productNumber, " +
                                 "location=?productLocation, " +
                                 "weight=?productWeight, " +
                                 "date_upd=?now, " +
                                 "sale_price=?salePrice " +
                               "WHERE id_product=?sku; " +
                               //
                               "UPDATE product_lang SET " +
                                 "description=?ProductLongDesc, " +
                                 "link_rewrite=?productLink, " +
                                 "meta_description=?metaDesc, " +
                                 "meta_title=?ProductName, " +
                                 "name=?ProductName " +
                               "WHERE id_product=?sku; " +
                               //
                               "UPDATE category_product SET " +
                                 "id_category=?category_id " +
                               "WHERE id_product=?sku AND id_category <> 1; " +
                               //
                               "UPDATE product_related SET " +
                                 "mixandmatch=?mixandmatch " + 
                               "WHERE id_product=?sku;";

            try
            {
                //Create the command object
                var command = new MySqlCommand(sql);
                
                command.Parameters.AddWithValue("?sku", item.SKU);
                command.Parameters.AddWithValue("?supplier_id", item.SupplierNo);
                command.Parameters.AddWithValue("?category_id", item.CategoryNo);
                command.Parameters.AddWithValue("?is_on_sale", Convert.ToInt32(item.IsOnSale));
                command.Parameters.AddWithValue("?productUPC", item.UPC);
                command.Parameters.AddWithValue("?productQuantity", item.InStock);
                command.Parameters.AddWithValue("?Price", item.Price);
                command.Parameters.AddWithValue("?productWholesale", item.Wholesale);
                command.Parameters.AddWithValue("?productNumber", item.ProductNo);
                command.Parameters.AddWithValue("?productLocation", item.ProdType);
                command.Parameters.AddWithValue("?productWeight", item.Weight);
                command.Parameters.Add("?now", MySqlDbType.DateTime);
                command.Parameters["?now"].Value = now;
                command.Parameters.AddWithValue("?salePrice", item.SalePrice);
                command.Parameters.AddWithValue("?ProductLongDesc", Utils.MySqlEscape(item.LongDesc, false));
                command.Parameters.AddWithValue("?productLink", Utils.buildLink(item.Description));
                command.Parameters.AddWithValue("?metaDesc", Utils.MySqlEscape(Utils.Truncate(item.LongDesc, 255), true));
                command.Parameters.AddWithValue("?ProductName", Utils.MySqlEscape(item.Description, true));
                command.Parameters.AddWithValue("?mixandmatch", item.MixAndMatch);

                //execute the command
                ExecuteCommand(command);
            }
            catch (MySqlException ex)
            {
                _log.AddError("MySQL Error Updating item: {0}({1})", item.Description, item.ProductNo);
                _log.AddError("Error: {0}", ex.InnerException.Message);
                _log.AddDebug(ex.StackTrace);
                return false;
            }
            catch (Exception e)
            {
                _log.AddError("Error Updating item: {0}({1})", item.Description, item.ProductNo);
                _log.AddError("Error: {0}", e.InnerException.Message);
                _log.AddError("Line: {0}", e.Source);
                _log.AddDebug(e.StackTrace);
                return false;
            }

            //Upload files everytime
            if(hasMP3s(item.SKU))
            {
                DeleteAttachments(item);
            }

            _log.AddDebug("Uploading the Files");
            item.uploadFiles();

            if (item.Files.Count > 0)
            {
                const string sqlFile = "INSERT INTO mp3player (id_product, mp3_filename, mp3_label, product_list, date_add, date_upd)" +
                                       "VALUES (?product_id, ?file_name, ?file_desc, 1, NOW(), NOW())";
                
                foreach (var file in item.Files)
                {
                    //Skip non MP3's
                    var dExt = file.FileName.Substring(file.FileName.Length - 3);
                    _log.AddDebug("dExt = {0}", dExt);

                    if (dExt.Equals("mp3", StringComparison.OrdinalIgnoreCase) == false)
                    {
                        continue;
                    }

                    try
                    {
                        var commandFile = new MySqlCommand(sqlFile);

                        //Fill in them infos
                        commandFile.Parameters.AddWithValue("?product_id", item.SKU);
                        commandFile.Parameters.AddWithValue("?file_name", file.FileServerPath);
                        commandFile.Parameters.AddWithValue("?file_desc", MySqlHelper.EscapeString(file.FileDesc));

                        ExecuteCommand(commandFile);
                    }
                    catch (Exception e)
                    {
                        _log.AddError("Error adding MP3 to item: {0}", item.ProductNo);
                        _log.AddError(e.Message);
                        _log.AddError(e.StackTrace);
                        return false;
                    }
                }
            }
            
            _log.AddDebug("Uploading the image");
            item.uploadImage();
            return true;
        }

        public bool hasMP3s (int productID)
        {
            var result = false;
            var sql = string.Format("SELECT COUNT(id_product) FROM mp3player WHERE id_product={0}", productID);
            
            try
            {
                var command = new MySqlCommand(sql);
                using (var con = new MySqlConnection(_connectionString))
                {
                    command.Connection = con;
                    con.Open();
                     var queryReqult = command.ExecuteReader();
                     if (queryReqult.Read())
                         result = queryReqult.GetInt32(0) != 0;
                     queryReqult.Dispose();
                }
            }
            catch (Exception ex)
            {
                _log.AddError("Error checking of there is MP3's already for productID({0})", productID);
                _log.AddError(ex.Message);
            }
            return result;
        }

        public void DeleteAttachments(Product product)
        {

            var sql = string.Format("DELETE FROM mp3player WHERE id_product={0}", product.SKU);
            
            try
            {
                var command = new MySqlCommand(sql);
                ExecuteCommand(command);
            }
            catch (Exception ex)
            {
                _log.AddError("Error deleting mp3 records for ({0})", product.ProductNo);
                _log.AddError(ex.Message);
            }
        }

        public Boolean DeleteItem(Product item)
        {
            return true;
            /*
            
            _log.AddDebug("Deleating Item: " + item.ProductNo);

            //reopen the connection just incase it closed
            checkConnection();

            var sql =
                "DELETE FROM product WHERE id_product=" + item.SKU + ";" +
                "DELETE FROM image WHERE id_product=" + item.SKU + ";" +
                "DELETE FROM image_lang WHERE id_image=" + item.SKU + ";" +
                "DELETE FROM mp3player WHERE id_product=" + item.SKU + ";" +
                "DELETE FROM product_lang WHERE id_product=" + item.SKU + ";" +
                "DELETE FROM product_sale WHERE id_product=" + item.SKU + ";" +
                "DELETE FROM product_tag WHERE id_product=" + item.SKU + ";" +
                "DELETE FROM product_related WHERE id_product=" + item.SKU + ";" +
                "DELETE FROM category_product WHERE id_product=" + item.SKU + ";";

            var command = _db.CreateCommand();

            try
            {
                command.CommandText = sql;
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {

                _log.AddError("Error Deleating item: " + item.ProductNo);
                _log.AddError("Error: \n" + e.Message);
                return false;
            }
            finally
            {
                command.Dispose();
            }
            return true;
            */
        }

        /*
        public Boolean connect()
        {
            //MySQL connection
            _db = new MySqlConnection(_connectionString);
            try
            {
                _log.AddInfo("Connecting to MySQL DB....");
                _db.Open();
                _log.AddNotice("Success! Server version: " + _db.ServerVersion);
                return true;
            }
            catch (Exception ex)
            {
                _log.AddError("Failed");
                _log.AddError(ex.Message);
                Console.WriteLine("Press any key to Exit!!");
                Console.ReadLine();
                return false;
            }
        }

        private void checkConnection()
        {
            if (_db.State != ConnectionState.Open)
                connect();
        }
        */

        private void ExecuteCommand(MySqlCommand sqlCommand)
        {
            try
            {
                using (var con = new MySqlConnection(_connectionString))
                {
                    sqlCommand.Connection = con;
                    con.Open();
                    sqlCommand.ExecuteNonQuery();
                }
            }
            catch(Exception e)
            {
                _log.AddError("Error - executeCommand()");
                _log.AddError(e.Message);
                _log.AddDebug(e.InnerException.Message);
            }
        }

        private MySqlDataReader ExecuteReader(MySqlCommand sqlCommand)
        {
            try
            {
                using (var con = new MySqlConnection(_connectionString))
                {
                    sqlCommand.Connection = con;
                    con.Open();
                    return sqlCommand.ExecuteReader();
                }
            }
            catch (Exception e)
            {
                _log.AddError("Error - executeCommand()");
                _log.AddError(e.Message);
                _log.AddDebug(e.InnerException.Message);
            }
            return null;
        }
    }
}
