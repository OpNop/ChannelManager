using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using MySql.Data.MySqlClient;
using System.Net;
using System.Net.FtpClient;

namespace mp3Fixer
{
    public class mp3FixThread
    {
        //private readonly FtpClient _ftpConn = new FtpClient();
        private readonly List<Product> _products = new List<Product>();
        private const string SQL_ADD_FILE = "INSERT INTO mp3player (id_product, mp3_filename, mp3_label, product_list, date_add, date_upd)" +
                                            "VALUES (?product_id, ?file_name, ?file_desc, 1, NOW(), NOW())";
        private const string SQL_DEL_FILE = "DELETE FROM mp3player WHERE id_product = ?product_id";
        private const string CONNECTION_STRING = "SERVER=cmusicwww.db.4417023.hostedresource.com;" +
                                                 "DATABASE=cmusicwww;" +
                                                 "UID=cmusicwww;" +
                                                 "PASSWORD=ka@9H,6Sta;";

        public void DoMp3Fix()
        {
            GetProducts();

            ///////////////////
            //  DEBUG LIMIT  //
            // var limit = 0;
            ///////////////////

            foreach (var product in _products)
            {
                if (DoUpload(product))
                {
                    if (AddToPrestashop(product))
                    {
                        if (UpdateMailWare(product))
                        {
                            Debug.WriteLine("---FINISHED---");
                        }
                    }
                }

                // DEBUG LIMIT
                // limit++;
                // if (limit >= 1)
                //    break;
            }
        }

        private void GetProducts()
        {
            var command = mwODBC.Instance.mwConn.CreateCommand();
            command.CommandText = "SELECT cl.SKU, cl.ProductNo FROM ChannelListings cl INNER JOIN ProductAttachments pa ON cl.ProductNo=pa.ProductNo WHERE (pa.Type='File' AND cl.isActive=true AND cl.LastStockUpdateTime <> NULL AND cl.ProductID=NULL AND cl.mp3Fixed<>True) group by cl.ProductNo";
            var mwReader = command.ExecuteReader();

            try
            {
                while (mwReader.Read())
                {
                    _products.Add(new Product(mwReader));
                }
            }

            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.StackTrace);
            }
            Debug.WriteLine("Total Products: {0}", _products.Count);

        }

        private bool DoUpload(Product product)
        {
            Debug.WriteLine(string.Format("Uploading Files for Product {0}", product.ProductNo));

            var folder = string.Format("/samples/{0:#/#/#}/{1}/", int.Parse(product.SKU.ToString().Substring(0, 3)), product.SKU);
            var track = 1;
            var ret = true;

            foreach (var file in product.Files)
            {
                var uFileName = string.Format("{0}{1}-{2}.mp3", folder, product.SKU, track);
                Stream istream = null;
                var buf = new byte[8192];

                using (var _ftpConn = new FtpClient())
                {
                    _ftpConn.Host = "chandlermusic.com";
                    _ftpConn.Credentials = new NetworkCredential("cmusicftp", "y94UneSt");
                    _ftpConn.Connect();

                    //Make the directory if not created
                    try
                    {
                        if (!_ftpConn.DirectoryExists(folder))
                        {
                            Debug.WriteLine(String.Format("--Creating folder: {0}", folder));
                            _ftpConn.CreateDirectory(folder, true);
                        }
                    }
                    catch (FtpCommandException fex)
                    {
                        Debug.WriteLine("--Hack: Duplicate Folder Creation");
                    }

                    //Check if file exists already
                    if (_ftpConn.FileExists(uFileName))
                    {
                        Debug.WriteLine(String.Format("--File \"{0}\" was found, deleating.", uFileName));
                        _ftpConn.DeleteFile(uFileName);
                    }

                    //Upload the file
                    using (var ostream = _ftpConn.OpenWrite(uFileName))
                    {
                        try
                        {
                            istream = new FileStream(file.LocalFilePath + file.FileName, FileMode.Open, FileAccess.Read);
                            var read = 0;
                            while ((read = istream.Read(buf, 0, buf.Length)) > 0)
                            {
                                ostream.Write(buf, 0, read);
                                Debug.Write("*");
                            }
                            file.FileServerPath = uFileName;
                            track++;
                            Debug.WriteLine("*");
                        }
                        catch (Exception ex)
                        {
                            ret = false;
                            Debug.WriteLine(ex.ToString());
                            break;
                        }
                        finally
                        {
                            if (istream != null)
                                istream.Close();

                            if (ostream != null)
                                ostream.Close();
                            Debug.WriteLine("--Finished uploading.");
                        }
                    }
                }
            }
            return ret;
        }

        private bool AddToPrestashop(Product product)
        {
            Debug.WriteLine(String.Format("Adding player data to Product {0}", product.ProductNo));

            //Clean up old enties if any
            try
            {
                var commandDel = new MySqlCommand(SQL_DEL_FILE);
                commandDel.Parameters.AddWithValue("?product_id", product.SKU);

                ExecuteCommand(commandDel);
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine(ex.ToString());
                return false;
            }

            //Try and add the new entries
            try
            {
                foreach (var file in product.Files)
                {
                    Debug.WriteLine(string.Format("--Adding File: {0} as {1}", file.FileDesc, file.FileServerPath));
                    var commandAdd = new MySqlCommand(SQL_ADD_FILE);

                    commandAdd.Parameters.AddWithValue("?product_id", product.SKU);
                    commandAdd.Parameters.AddWithValue("?file_name", file.FileServerPath);
                    commandAdd.Parameters.AddWithValue("?file_desc", MySqlHelper.EscapeString(file.FileDesc));

                    ExecuteCommand(commandAdd);
                }
            }
            catch(MySqlException ex)
            {
                Debug.WriteLine(ex.ToString());
                return false;
            }
            return true;
        }

        private bool UpdateMailWare(Product product)
        {
            try
            {
                Debug.WriteLine("Updating MailWare Database");
                var command = mwODBC.Instance.mwConn.CreateCommand();
                command.CommandText = String.Format("UPDATE ChannelListings SET mp3Fixed=True WHERE SKU='{0}'", product.SKU);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("--FAILED UPDATING--");
                Debug.WriteLine(ex.ToString());
                return false;
            }
            Debug.WriteLine("--Added");
            return true;
        }

        private static void ExecuteCommand(MySqlCommand sqlCommand)
        {
            try
            {
                using (var con = new MySqlConnection(CONNECTION_STRING))
                {
                    sqlCommand.Connection = con;
                    con.Open();
                    sqlCommand.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error - executeCommand()");
                Debug.WriteLine(e.Message);
            }
        }
    }
}
