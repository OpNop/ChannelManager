using System;
using System.Collections.Generic;
using System.Timers;
using DLG.Log;

namespace Service
{
    class BusinessLogic
    {
        public bool IsWorking;
        //Initialize the timer
        readonly Timer _tmrUpload = new Timer();
        readonly Timer _tmrDownload = new Timer();

        //Initialize
        private readonly PrestaShop _cart;
        private readonly MailWare _mailWare;

        //The log object
        public static Log SrvLog;

        public BusinessLogic(Log mainLog)
        {
            SrvLog = mainLog;
            _cart = new PrestaShop(mainLog);
            _mailWare = new MailWare(mainLog);
        }

        //Start the timers and get things going
        public bool start()
        {
            try
            {
                //Init the timers
                _tmrUpload.Elapsed += onElapsedTimeUpload;
                _tmrUpload.Interval = Settings.ProdUploadInt * 0xEA60;
                _tmrDownload.Elapsed += onElapsedTimeDownload;
                _tmrDownload.Interval = Settings.OrderDownloadInt * 0xEA60;

                //Get DB's going
                if (!_mailWare.connect() || !_cart.connect())
                    return false;

                //Fire off an the upload timer
                onElapsedTimeUpload(null, null);

                //Start the timers and run
                _tmrDownload.Start();
                _tmrUpload.Start();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void shoppingCartDispatch(ICollection<Product> productList)
        {
            IsWorking = true;

            SrvLog.PutLog(LogLevel.D, "CartDispatch Started");
            SrvLog.PutLog(LogLevel.I, "Updating " + productList.Count + " Product(s).");

            try
            {
                foreach (var item in productList)
                {
                    //Are we deleating an item?
                    if (item.IsDelete || item.IsRemove)
                        _cart.deleteItem(item);

                    //Or are we Adding/Updating an item?
                    else if (item.IsActive)
                    {
                        //Go ahead and fill in all the items data
                        SrvLog.PutLog(LogLevel.D, "Getting Data for item " + item.SKU);
                        _mailWare.getExtData(item);

                        //find out if its a new add or update
                        if (item.LastStockUpdateTime == new DateTime(0))
                        {
                            if (_cart.addItem(item))
                            {
                                _mailWare.updateLastModDate(item.SKU);
                            }
                        }
                        else
                        {
                            if (_cart.updateItem(item))
                            {
                                _mailWare.updateLastModDate(item.SKU);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                SrvLog.PutLog(LogLevel.E, "{0}\r\n Stack Trace:\r\n{1}", e.Message, e.StackTrace);
            }
            IsWorking = false;
        }

        private void onElapsedTimeDownload(object source, ElapsedEventArgs e)
        {
            SrvLog.PutLog(LogLevel.N, "onElapsedTimeDownload Called @ {0:T}", DateTime.Now);
        }

        private void onElapsedTimeUpload(object source, ElapsedEventArgs e)
        {
            //skip if we are still running from before
            if(IsWorking)
                return;

            var productsToUpdate = _mailWare.getProductsToUpdate();

            if (productsToUpdate.Count == 0)
            {
                SrvLog.PutLog(LogLevel.I, "No items to update this time.");
                return;
            }
            
            shoppingCartDispatch(productsToUpdate);
        }
    }
}
