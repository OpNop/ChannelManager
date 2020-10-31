using System;
using System.Collections.Generic;
using System.Timers;
using DLG.ToolBox.Log;

namespace Service
{
    class BusinessLogic
    {
        public bool IsWorking;
        public bool timeToQuit;
        //Initialize the timer
        readonly Timer _tmrUpload = new Timer();
        readonly Timer _tmrDownload = new Timer();

        //Initialize
        private readonly PrestaShop _cart;
        private readonly MailWare _mailWare;

        //The log object
        private static readonly Logger _log = Logger.getInstance();

        public BusinessLogic()
        {
            _cart = new PrestaShop();
            _mailWare = new MailWare();
        }

        //Start the timers and get things going
        public bool start ()
        {
            try
            {
                //Init the timers
                _tmrUpload.Elapsed += onElapsedTimeUpload;
                _tmrUpload.Interval = Settings.ProdUploadInt * 0xEA60;
                _tmrDownload.Elapsed += onElapsedTimeDownload;
                _tmrDownload.Interval = Settings.OrderDownloadInt * 0xEA60;

                //Get DB's going
                if (!_mailWare.connect())
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

        private bool shoppingCartDispatch(ICollection<Product> productList)
        {
            IsWorking = true;
            var sucess = true;

            _log.AddDebug("CartDispatch Started");
            _log.AddInfo("Updating " + productList.Count + " Product(s).");

            try
            {
                foreach (var item in productList)
                {
                    //Are we deleating an item?
                    if (item.IsDelete || item.IsRemove)
                        _cart.DeleteItem(item);

                    //Or are we Adding/Updating an item?
                    else if (item.IsActive)
                    {
                        //Go ahead and fill in all the items data
                        _log.AddDebug("Getting Data for item " + item.SKU);
                        _mailWare.getExtData(item);

                        //find out if its a new add or update
                        if (item.LastStockUpdateTime == new DateTime(0))
                        {
                            if (_cart.AddItem(item))
                                _mailWare.updateLastModDate(item.SKU);
                        }
                        else
                        {
                            if (_cart.UpdateItem(item))
                                _mailWare.updateLastModDate(item.SKU);
                        }
                    }
                    //break the loop if the exit command was called
                    if (timeToQuit)
                    {  
                        Environment.Exit(0);
                    }
                }
            }
            catch (Exception e)
            {
                _log.AddError("{0}\r\n Stack Trace:\r\n{1}", e.Message, e.StackTrace);
                sucess = false;
            }
            IsWorking = false;
            _log.AddInfo("Finished");
            return sucess;
        }

        private void onElapsedTimeDownload(object source, ElapsedEventArgs e)
        {
            //_log.AddNotice("onElapsedTimeDownload Called");
        }

        private void onElapsedTimeUpload(object source, ElapsedEventArgs e)
        {
            //skip if we are still running from before
            if(IsWorking)
                return;
            _log.AddNotice("onElapsedTimeUpload Called");
            var productsToUpdate = _mailWare.getProductsToUpdate();

            if (productsToUpdate.Count == 0)
            {
                _log.AddDebug("No items to update this time.");
                return;
            }
            _log.AddDebug("calling Dispatch");
            var result = shoppingCartDispatch(productsToUpdate);
        }

        public string shutDown()
        {
            if (timeToQuit)
                return "Shutdown in progress, Program will exit after current product.";

            timeToQuit = true;
            return "Shut down started, Program will exit after the curtrent product is finished";
        }

        public void printStats()
        {
            throw new NotImplementedException();
        }
    }
}
