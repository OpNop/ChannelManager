using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DLG.ToolBox.Log;
using Service;

namespace JustCreateThumbnail
{
    class Program
    {
        static void Main(string[] args)
        {
            string fileName, newName, location;
            bool isImage;
            var ftp = new Service.Ftp();
            fileName = @"D:\ChandlerMusic\ChannelManager\testImage.jpg";
            newName = "63000-63000";
            location = "/img/p/63000";
            isImage = true;
            ftp.Upload(fileName, newName, location, isImage);
        }
    }
}
