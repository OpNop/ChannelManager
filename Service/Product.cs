using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.IO;
using System.Net;
using DLG.ToolBox.Log;
using System.Net.FtpClient;

namespace Service
{
    public class File
    {
        public string FileName;
        public string FileDesc;
        public string FileType;
        public string FileServerPath { get; set; }

        public File(string file, string desc)
        {

            FileName = file;
            FileDesc = desc;
        }
    }

    public class Product
    {
        private static readonly Logger _log = Logger.getInstance();
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
        public bool IsUseCurrentStock;
        public int InStock;
        public DateTime LastInStockUptated;

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
        public string MixAndMatch;
        public List<string> Images = new List<string>();
        public List<File> Files = new List<File>();



        public Product(OdbcDataReader recordSet)
        {
            ProductNo               = recordSet.GetString(0);

            //Log what product we are getting (ProductNo should always be suscessful 
            _log.AddDebug("Getting Product info for {0}", ProductNo);

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
            MixAndMatch             = Convert.IsDBNull(recordSet.GetValue(20)) ? ProductNo : recordSet.GetString(20);
            IsUseCurrentStock       = Convert.IsDBNull(recordSet.GetValue(21)) ? false : recordSet.GetBoolean(21);
            InStock                 = Convert.IsDBNull(recordSet.GetValue(22)) ? 0 : Convert.ToInt32(recordSet.GetValue(22));
            LastInStockUptated      = Convert.IsDBNull(recordSet.GetValue(23)) ? new DateTime(0) : Utils.processMwTime(recordSet.GetString(23));

            //set Price to the price we want to use
            Price = IsUseCurrentRetailPrice ? RealPrice : CLPrice;

        }

        public void uploadImage()
        {
            //if (Images[0] == "no_picture.gif") return;

            var ftpclient = new Ftp();
            var imagePath = LocalFilePath + Images[0];
            var newImageName = SKU + "-" + SKU;
            if (System.IO.File.Exists(imagePath))
            {
                _log.AddDebug("Uploading image: " + LocalFilePath + Images[0]);
                _log.AddDebug("Destination: " + getServerImagePath(SKU) + "/" + newImageName);
                ftpclient.Upload(LocalFilePath + Images[0], newImageName, getServerImagePath(SKU), true);

            }
            else
                _log.AddError("File does not exist: {0}", imagePath);
        }

        private string getServerImagePath(int pId)
        {
            var base1000 = (Math.Floor(Convert.ToDecimal(pId / 1000)) * 1000);
            return SrvImagePath + base1000;
        }

        public void uploadFiles()
        {
            //no files to upload
            if (Files.Count == 0)
            {
                _log.AddNotice("No files attached!!");
                return;
            }
            DoUpload();
            /*
            foreach(var file in Files)
            {
                var filePath = LocalFilePath + file.FileName;

                if (System.IO.File.Exists(filePath))
                {
                    //Checking for mp3 for now, wav, ra, and pdf's will come later
                    if(new System.IO.FileInfo(filePath).Extension == ".mp3")
                    {
                        var ftpclient = new Ftp();
                        ftpclient.Upload(filePath, "", SrvMP3Path, false);
                    }
                    else
                        _log.AddError("Skipping {0} because, we dont talk about mailware features.", filePath);
                }
                else
                    _log.AddError("File does not exist: {0}", filePath);
            }
            */
        }

        private void DoUpload()
        {
            _log.AddInfo(string.Format("Uploading Files for Product {0}", ProductNo));

            var folder = string.Format("/samples/{0:#/#/#}/{1}/", int.Parse(SKU.ToString().Substring(0, 3)), SKU);
            var track = 1;

            foreach (var file in Files)
            {
                //Skip non MP3's
                var filePath = LocalFilePath + file.FileName;
                if (new System.IO.FileInfo(filePath).Extension != ".mp3") {
                    continue;
                }

                var uFileName = string.Format("{0}{1}-{2}.mp3", folder, SKU, track);
                Stream istream = null;
                var buf = new byte[8192];
                var currentByte = 0;
                var fileInf = new FileInfo(LocalFilePath + file.FileName);
                decimal dFileSize = fileInf.Length / 1024;
                var fileSize = Math.Ceiling(dFileSize);

                //_log.AddInfo(String.Format("{0}: {1} ({2})", Settings.FtpServer, Settings.FtpUser, Settings.FtpPass));
                using (var ftpConn = new FtpClient())
                {
                    ftpConn.Host = Settings.FtpServer;
                    ftpConn.Credentials = new NetworkCredential(Settings.FtpUser, Settings.FtpPass);
                    ftpConn.DataConnectionType = FtpDataConnectionType.AutoPassive;
                    ftpConn.Connect();

                    //Make the directory if not created
                    if (!ftpConn.DirectoryExists(folder))
                    {
                        _log.AddInfo(String.Format("--Creating folder: {0}", folder));
                        ftpConn.CreateDirectory(folder, true);
                    }
                    //Check if file exists already
                    if (ftpConn.FileExists(uFileName))
                    {
                        _log.AddInfo(String.Format("--File \"{0}\" was found, deleating.", uFileName));
                        ftpConn.DeleteFile(uFileName);
                    }

                    //Upload the file
                    using (var ostream = ftpConn.OpenWrite(uFileName))
                    {
                        try
                        {
                            istream = new FileStream(LocalFilePath + file.FileName, FileMode.Open, FileAccess.Read);
                            var read = 0;
                            while ((read = istream.Read(buf, 0, buf.Length)) > 0)
                            {
                                ostream.Write(buf, 0, read);
                                //_log.AddInfo("*");
                                currentByte += read;
                                decimal current = currentByte/1024;
                                drawTextProgressBar((int)Math.Floor(current), (int)fileSize);
                            }
                            file.FileServerPath = uFileName;
                            track++;
                            _log.AddInfo(" ");
                        }
                        catch (Exception ex)
                        {
                            _log.AddError(ex.ToString());
                            break;
                        }
                        finally
                        {
                            if (istream != null)
                                istream.Close();

                            if (ostream != null)
                                ostream.Close();
                            _log.AddInfo("--Finished uploading");
                        }
                    }
                }
            }
        }

        private static void drawTextProgressBar(int progress, int total)
        {
            //draw empty progress bar
            Console.CursorLeft = 0;
            Console.Write("["); //start
            Console.CursorLeft = 32;
            Console.Write("]"); //end
            Console.CursorLeft = 1;
            var onechunk = 30.0f / total;

            //draw filled part
            var position = 1;
            for (var i = 0; i < onechunk * progress; i++)
            {
                Console.BackgroundColor = ConsoleColor.Gray;
                Console.CursorLeft = position++;
                Console.Write(" ");
            }

            //draw unfilled part
            for (var i = position; i <= 31; i++)
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.CursorLeft = position++;
                Console.Write(" ");
            }

            //draw totals
            Console.CursorLeft = 35;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Write(progress + " of " + total + "    "); //blanks at the end remove any excess
        }
    }

}
