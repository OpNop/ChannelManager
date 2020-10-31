using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Windows.Forms;

namespace m2z
{
    public partial class frmMain : Form
    {

        //Need these for global Vars
        private OdbcConnection _mwConn;
        private readonly List<Supplier> _suppsList = new List<Supplier>();

        public frmMain()
        {
            //Draw me a form
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            //test that we can connect to the DB
            if(!MwConnect())
                Application.Exit();
            //Cool we can, now put that crap into the dropdown
            PopulateDropdown();
        }

        private void PopulateDropdown()
        {
            OdbcCommand command = _mwConn.CreateCommand();
            OdbcDataReader mwReader = null;

            try
            {
                command.CommandText = "SELECT SupplierNo, Company FROM Supplier ORDER BY Company";
                mwReader = command.ExecuteReader();

                //Loop through them suppliers
                while (mwReader.Read())
                {
                    var supp = new Supplier {SuppId = mwReader.GetInt32(0), SuppName = mwReader.GetString(1)};
                    //add it to the list
                    _suppsList.Add(supp);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Danger Will Robinson", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                //release the query if it was used
                if (mwReader != null)
                    mwReader.Dispose();
            }

            foreach(Supplier tmpSupp in _suppsList)
            {
                SupplierList.Items.Add(tmpSupp.SuppName);
            }

        }

        private Boolean MwConnect()
        {
            //Mailware Connection
            _mwConn = new OdbcConnection
                          {
                              ConnectionString = "DRIVER={DBISAM 4 ODBC Driver};" +
                                                 "ConnectionType=Local;" +
                                                 "CatalogName=./data"
                          };

            try
            {
                _mwConn.Open();
                //MessageBox.Show("Connected to Mailware DB. Version: " + _mwConn.ServerVersion, "Connected!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }
            catch (Exception)
            {
                MessageBox.Show("Error connecting to the MailWare DB!\nIs this app being run from the main mailware folder?",
                                "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(SupplierList.Text == "")
            {
                MessageBox.Show("Sorry, I can not read minds, yet.\n\nSelect a publisher first, N00B!!");
                return;
            }

            //Ok we have a publisher. Time to work some magic

            //Build us up some SQL
            
            //Basically looks for all non discontinued
            //products that have an image attached
            //from the specific publisher
            const string sql = "SELECT Products.ProductNo, Products.Price, Products.ProdType " +
                               "FROM Products INNER JOIN ProductAttachments " +
                               "ON Products.ProductNo = ProductAttachments.ProductNo " +
                               //"WHERE Products.SupplierNo=" + _suppsList[SupplierList.SelectedIndex].SuppId + " " +
                               "WHERE Products.Price<>NULL " +
                               "AND LongDesc<>NULL " +
                               //"AND Products.MixAndMatchCode<>NULL" +
                               //"AND ProductAttachments.Type='Picture' " +
                               //"AND ProductAttachments.FileName<>'' " +
                               //"AND Products.IsDiscontinued <> True "
                               "GROUP BY Products.ProductNo";
            
            //Now we put that SQl to work
            var command = _mwConn.CreateCommand();
            OdbcDataReader mwReader = null;
            command.CommandText = sql;
            //God DBISAM sux
            //get last auto_inc value
            int currerntSKU;
            var getAutoInc = _mwConn.CreateCommand();
            getAutoInc.CommandText = "SELECT IDENT_CURRENT('ChannelListings') FROM ChannelListings TOP 1";
            var tmpGetAutoInc = getAutoInc.ExecuteReader();
            
            if (tmpGetAutoInc.HasRows)
            {
                tmpGetAutoInc.Read();
                currerntSKU = tmpGetAutoInc.GetInt32(0) + 1;
            }
            else
            {
                currerntSKU = 1;
            }

            tmpGetAutoInc.Dispose();
            getAutoInc.Dispose();
            //END of lame DBISAM work around

            try
            {
                mwReader = command.ExecuteReader();

                while (mwReader.Read())
                {
                    //create the INSERT command
                    var insertCommand = _mwConn.CreateCommand();
                    

                    //Check that there is a Product Number
                    if (Convert.IsDBNull(mwReader.GetValue(0)))
                    {
                        MessageBox.Show("Error adding product.\n\nThe product Number is missing");
                        continue;
                    }
                    string productNo = mwReader.GetString(0);
                    //insertCommand.Parameters["ProductNo"].Value = mwReader.GetString(0);

                    //Check that there is a Price
                    if (Convert.IsDBNull(mwReader.GetValue(1)))
                    {
                        MessageBox.Show("Error adding Product: " + productNo + "\n\nThe Price is missing");
                        continue;
                    }
                    var price = Convert.ToDecimal(mwReader.GetValue(1));
                    //insertCommand.Parameters["Price"].Value =  Convert.ToDecimal(mwReader.GetValue(1));

                    //Check that there is a Prod Type
                    if (Convert.IsDBNull(mwReader.GetValue(2)))
                    {
                        MessageBox.Show("Error adding Product: " + productNo + "\nThe Product Type is missing\nMake a note, because I'm not going to add it!");
                        continue;
                    }
                    var tmpProdType = mwReader.GetString(2);

                    if (!Enum.IsDefined(typeof (CatReff), tmpProdType))
                        continue;

                    var catNo = (int) Enum.Parse(typeof (CatReff), tmpProdType, true);
                    //insertCommand.Parameters["CatNo"].Value = catNo;


                    //Create the SQL because DBISAM sucks and cant do Paramaters
                    var sqlInsert = "INSERT INTO ChannelListings " +
                                        "(" +
                                            "ChannelNo, " +
                                            "ChannelAccountNo, " +
                                            "ProductNo, " +
                                            "SKU, " +
                                            "Price, " +
                                            "IsUseCurrentRetailPrice, " +
                                            "IsOnSale, " +
                                            "IsActive, " +
                                            "IsRemove, " +
                                            "IsDelete, " +
                                            "IsGiftwrapAvailable, " +
                                            "IsGiftMessageAvailable, " +
                                            "IsUseProductNoAsSKU, " +
                                            "CategoryNo" +
                                            ")" +
                                        "VALUES " +
                                        "(" +
                                            "5, " +
                                            "3, " +
                                            "'" + productNo + "', " +
                                            "'" + currerntSKU + "', " +
                                            price + ", " +
                                            "True, " +
                                            "False, " +
                                            "True, " +
                                            "False, " +
                                            "False, " +
                                            "False, " +
                                            "False, " +
                                            "False, " +
                                            catNo +
                                        ")";
                    
                    insertCommand.CommandText = sqlInsert;
                    //Well guess we can run it
                   MessageBox.Show(sqlInsert);
                    //MessageBox.Show(insertCommand.CommandText, "Ok, this time the SQl should be good!");
                    //insertCommand.Prepare();
                    //int test = 1 + 1;
                    insertCommand.ExecuteNonQuery();
                    currerntSKU++;
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("Errror at Product: " + mwReader.GetString(0) + "\n" + err.Message + "\n" + err.Source + "\n" + err.InnerException + "\n\n" + err.StackTrace, "Danger Will Robinson", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                //release the query if it was used
                if (mwReader != null)
                    mwReader.Dispose();
            }
            MessageBox.Show("Done!!!!!!!!");

        }
    }

    public class Supplier
    {
        public int SuppId { get; set; }

        public string SuppName { get; set; }
    }
}