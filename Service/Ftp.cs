using System;
using System.Net;
using System.IO;
using DLG.ToolBox.Log;

namespace Service
{
    public class Ftp
    {
        private static readonly Logger _log = Logger.getInstance();

        public void Upload(string filename, string newName, string location, bool isImage)
        {
            var fileInf = new FileInfo(filename);
            string uri;

            if (newName == "")
                uri = "ftp://" + Settings.FtpServer + "/" + location + "/" + fileInf.Name;
            else
                uri = "ftp://" + Settings.FtpServer + "/" + location + "/" + newName + fileInf.Extension;

            // Create FtpWebRequest object from the Uri provided
            var reqFtp = (FtpWebRequest) FtpWebRequest.Create(new Uri(uri));

            // Provide the WebPermission Credintials
            reqFtp.Credentials = new NetworkCredential(Settings.FtpUser, Settings.FtpPass);

            // By default KeepAlive is true, where the control connection
            // is not closed after a command is executed.
            reqFtp.KeepAlive = false;

            // Specify the command to be executed.
            reqFtp.Method = WebRequestMethods.Ftp.UploadFile;

            // Specify the data transfer type.
            reqFtp.UseBinary = true;

            // Notify the server about the size of the uploaded file
            reqFtp.ContentLength = fileInf.Length;

            // The buffer size is set to 2kb
            const int buffLength = 2048;
            var buff = new byte[buffLength];

            // Opens a file stream (System.IO.FileStream) to read the file
            // to be uploaded
            var fs = fileInf.OpenRead();

            try
            {
                // Stream to which the file to be upload is written
                var strm = reqFtp.GetRequestStream();

                // Read from the file stream 2kb at a time
                var contentLen = fs.Read(buff, 0, buffLength);

                var currentByte = 0;

                // Till Stream content ends
                while (contentLen != 0)
                {
                    // Write Content from the file stream to the FTP Upload
                    // Stream
                    strm.Write(buff, 0, contentLen);
                    contentLen = fs.Read(buff, 0, buffLength);
                    currentByte += 2048;
                    decimal current = currentByte/1024;
                    drawTextProgressBar((int)Math.Floor(current), (int)fileInf.Length / 1024);
                    
                }
                _log.AddDebug(" "); //add newline after progress bar

                // Close the file stream and the Request Stream
                strm.Close();
                fs.Close();


                if (isImage)
                {
                    var attempt = 1;
                    CreateThumbs:
                    //Call the manual thumbnail Creator
                    if (attempt <= 3)
                    {
                        HttpWebRequest objRequest = null;
                        try
                        {
                            ServicePointManager.SecurityProtocol = (SecurityProtocolType)48 | (SecurityProtocolType)192 | (SecurityProtocolType)768 | (SecurityProtocolType)3072;
                            //objRequest = (HttpWebRequest)WebRequest.Create("http://shop.shotsranch.com/proshopadmin/manualCreateThumbnails.php?imagename=" + newName + fileInf.Extension);
                            objRequest =(HttpWebRequest)WebRequest.Create("http://www.chandlermusic.com/admincm/manualCreateThumbnails.php?imagename=" + newName + fileInf.Extension);
                            objRequest.Method = "GET";
                            objRequest.Timeout = 4000;
                            objRequest.AllowAutoRedirect = true;
                            using (var objResponse = (HttpWebResponse)objRequest.GetResponse())
                            {
                                _log.AddWarning("StatusCode was: " + objResponse.StatusCode);
                            }
                        }
                        catch (WebException e)
                        {
                            attempt++;
                            _log.AddError(e.Status.ToString());
                            _log.AddError(e.Message);
                            _log.AddError("Source: " + e.Source);
                            _log.AddError("Inner: " + e.InnerException.Message);
                            _log.AddInfo("Retrying in 2 seconds");
                            if (objRequest != null)
                            {
                                _log.AddInfo("Sent: " + objRequest.RequestUri.ToString());
                                objRequest.Abort();
                            }
                            System.Threading.Thread.Sleep(2000);
                            goto CreateThumbs;
                        }
                        finally
                        {
                            if (objRequest != null)
                            {
                                objRequest = null;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _log.AddError("Error uploading file ({0})", fileInf.Name);
                _log.AddError("Remote file path: {0}", uri);
                _log.AddError(ex.Message);
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