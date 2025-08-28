﻿using ADODB;
using AForge.Video;
using AForge.Video.DirectShow;
using Mono.Cecil;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Drawing;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using ZXing;
// using Squirrel;

namespace MarioApp
{
    public partial class FormMarioTools : Form
    {
        // private string dbTB2Locatie = "PROVIDER=Microsoft.Jet.OLEDB.4.0;Data Source=\\JOS-HP\\Users\\Jos\\Documents\telebib2.mdb;";

        // private string dbQualifierSQL = "SELECT * FROM A_DE_QUALIFIANT";
        // private string dbDocumentenSQL = "select * from Dokumenten where dnnSyncAlready = False";
        // private string dbUDocumentenSQL = "select * from Dokumenten where dnnSync = False";        
        // private string dbBriefwisselingSQL = "select * from Briefwisseling where dnnSyncAlready = False";
        // private string dbAllerleiSQL = "select * from Allerlei where dnnSyncAlready = False";
        // private string dbTB2SQL = "select * from TB2 where dnnSyncAlready = False";
        // private string dbProductenSQL = "select * from Produkten where dnnSyncAlready = False";

        public string dbJetProvider = "PROVIDER=Microsoft.Jet.OLEDB.4.0;Data Source=";
        public string dbmarLocatie = "";

        public string dbKlantenSQLUpdate = "select * from Klanten where dnnSyncAlready = True and dnnSync = False";
        public string dbKlantenSQLNieuw = "select * from Klanten where dnnSyncAlready = False";

        public string dbContractenSQLUpdate = "select * from Polissen where dnnSyncAlready = True and dnnSync = False";
        public string dbContractenSQLnieuw = "select * from Polissen where dnnSyncAlready = False";

        public string dbTelebibContractenSQLnieuw = "select * from TB2 where dnnSyncAlready = False AND (DocType = '01' OR DocType = '02')";
        public string dbTelebibContractenSQLUpdate = "select * from TB2 where (dnnSyncAlready = True AND dnnSync = False) AND (DocType = '01' OR DocType = '02')";

        public string dbCustomerInvoicesSQLUpdate = "select * from Dokumenten where dnnSyncAlready = True and dnnSync = False and v034 like 'K%'";
        public string dbCustomerInvoicesSQLnieuw = "select * from Dokumenten where dnnSyncAlready = False and v034 like 'K%'";

        public string dbRekeningenSQLUpdate = "select * from Rekeningen where dnnSyncAlready = True and dnnSync = False";
        public string dbRekeningenSQLNieuw = "select * from Rekeningen where dnnSyncAlready = False";

        public string dbJournalenSQLUpdate = "select * from Journalen where dnnSyncAlready = True and dnnSync = False";
        public string dbJournalenSQLNieuw = "select * from Journalen where dnnSyncAlready = False";

        public string dbLeveranciersSQLUpdate = "select * from Leveranciers where dnnSyncAlready = True and dnnSync = False";
        public string dbLeveranciersSQLNieuw = "select * from Leveranciers where dnnSyncAlready = False";

        public string dbSupplierInvoicesSQLUpdate = "select * from Dokumenten where dnnSyncAlready = True and dnnSync = False and v034 like 'L%'";
        public string dbSupplierInvoicesSQLnieuw = "select * from Dokumenten where dnnSyncAlready = False and v034 like 'L%'";

        public string dbQualifiersSQL = "select * from A_DE_QUALIFIANT";
        public string dbValeursSQL = "select * from VALEUR";

        public string cnnChosen = "";

        FilterInfoCollection filterInfoCollection;
        VideoCaptureDevice videoCaptureDevice;
        private HttpClient connectionCheck;

        public FormMarioTools()
        {
            InitializeComponent();
        }

        private void FrmMarioApp_Load(object sender, EventArgs e)
        {
            try
            {
                filterInfoCollection = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                foreach (FilterInfo device in filterInfoCollection)
                    cboCamera.Items.Add(device.Name);
                cboCamera.SelectedIndex = 0;
            }
            catch (Exception)
            {
                // throw;
                btnStartScan.Enabled = false;
                btnStopScan.Enabled = false;
                cboCamera.Items.Add("No webcam detected");
                cboCamera.SelectedIndex = 0;
            }

            Text = "Mario2025";

            tbMarLocatie.Text = Properties.Settings.Default.MdvSetting2025;
            tbLocalConnectionstring.Text = Properties.Settings.Default.LocalSQLSetting2025;
            tbHostedConnectionstring.Text = Properties.Settings.Default.HostedSQLSetting2025;
            // rbLocal.Checked = true;
        }

        private void BtnChangeMarNTLocation_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog MarntOpenFileDialog = new OpenFileDialog())
            {
                MarntOpenFileDialog.InitialDirectory = "c:\\";
                MarntOpenFileDialog.Filter = "mdv files (*.mdv)|*.mdv|All files (*.*)|*.*";
                MarntOpenFileDialog.FilterIndex = 1;
                MarntOpenFileDialog.RestoreDirectory = true;

                if (MarntOpenFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // string fileContent; // = string.Empty;
                    string filePath; // = string.Empty;

                    //Get the path of specified file
                    filePath = MarntOpenFileDialog.FileName;
                    tbMarLocatie.Text = dbJetProvider + MarntOpenFileDialog.FileName;
                    btnChangeMarNTLocation.Text = "OK";
                    tbMarLocatie.Enabled = false;
                    _ = MessageBox.Show("File at path: " + filePath);

                    Properties.Settings.Default.MdvSetting2025 = tbMarLocatie.Text;
                    Properties.Settings.Default.Save();
                }
            }
        }

        private void BtnUpdateMainFiles_Click(object sender, EventArgs e)
        {
            DataCustomers dbCustomers = new DataCustomers();
            DataSuppliers dbSuppliers = new DataSuppliers();
            DataLedgerAccounts dbAccounts = new DataLedgerAccounts();

            MessageBox.Show("Start updating tables customers, suppliers and ledgeraccounts. For every 2000 records moving to hosting account it will take one or more minutes");
            Cursor.Current = Cursors.WaitCursor;

            // Customers
            ToolStripStatusLabel.Text = "Customers new and update...";
            StatusStrip.Refresh();
            dbCustomers.CustomersInsertAndUpdate(tbMarLocatie.Text, dbKlantenSQLNieuw, dbKlantenSQLUpdate, cnnChosen);

            // Suppliers
            ToolStripStatusLabel.Text = "Suppliers new and update...";
            StatusStrip.Refresh();
            dbSuppliers.SuppliersInsertAndUpdate(tbMarLocatie.Text, dbLeveranciersSQLNieuw, dbLeveranciersSQLUpdate, cnnChosen);

            // Accounts
            ToolStripStatusLabel.Text = "Ledger accounts new and update...";
            StatusStrip.Refresh();
            dbAccounts.LedgerAccountsInsertAndUpdate(tbMarLocatie.Text, dbRekeningenSQLNieuw, dbRekeningenSQLUpdate, cnnChosen);

            Cursor.Current = Cursors.Default;
            MessageBox.Show("Updating tables customers, suppliers and ledgeraccounts finished");
            ToolStripStatusLabel.Text = "Ready";
            StatusStrip.Refresh();
        }

        private void BtnResetMarntCCI_Click(object sender, EventArgs e)
        {
            const string message = "marIntegraal ALLE tabellen resetten voor export. Bent U zeker?";
            const string caption = "MarNT Data Export";
            DialogResult result = MessageBox.Show(message, caption,
                                         MessageBoxButtons.YesNo,
                                         MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

            if (result == DialogResult.Yes)
            {
                try
                {
                    MessageBox.Show("Start reset sync");
                    OleDbConnection cnnMarJet = new OleDbConnection
                    {
                        ConnectionString = tbMarLocatie.Text
                    };
                    Cursor.Current = Cursors.WaitCursor;
                    cnnMarJet.Open();
                    OleDbCommand commandK = new OleDbCommand("UPDATE Klanten SET dnnSyncAlready = False, dnnSync = False", cnnMarJet);
                    _ = commandK.ExecuteNonQuery();

                    OleDbCommand commandP = new OleDbCommand("UPDATE Polissen SET dnnSyncAlready = False, dnnSync = False", cnnMarJet);
                    _ = commandP.ExecuteNonQuery();

                    OleDbCommand commandT = new OleDbCommand("UPDATE TB2 SET dnnSync = False, dnnSyncAlready = False", cnnMarJet);
                    _ = commandT.ExecuteNonQuery();

                    OleDbCommand commandDK = new OleDbCommand("UPDATE Dokumenten SET dnnSync = False, dnnSyncAlready = False where v034 like 'K%'", cnnMarJet);
                    _ = commandDK.ExecuteNonQuery();

                    OleDbCommand commandR = new OleDbCommand("UPDATE Rekeningen SET dnnSyncAlready = False, dnnSync = False", cnnMarJet);
                    _ = commandR.ExecuteNonQuery();

                    OleDbCommand commandJ = new OleDbCommand("UPDATE Journalen SET dnnSyncAlready = False, dnnSync = False", cnnMarJet);
                    _ = commandJ.ExecuteNonQuery();

                    OleDbCommand commandL = new OleDbCommand("UPDATE Leveranciers SET dnnSyncAlready = False, dnnSync = False", cnnMarJet);
                    _ = commandL.ExecuteNonQuery();

                    OleDbCommand commandDL = new OleDbCommand("UPDATE Dokumenten SET dnnSync = False, dnnSyncAlready = False where v034 like 'L%'", cnnMarJet);
                    _ = commandDL.ExecuteNonQuery();


                    cnnMarJet.Close();
                    Cursor.Current = Cursors.Default;
                    MessageBox.Show("End reset sync");
                }
                catch (Exception ex)
                {
                    Cursor.Current = Cursors.Default;
                    _ = MessageBox.Show(ex.Message);
                }
            }
        }

        private void BtnCheckCustomers_Click(object sender, EventArgs e)
        {
            Connection cn = new Connection();
            Recordset rs = new Recordset();

            string cnStr = tbMarLocatie.Text;
            string result = "";

            try
            {
                Cursor.Current = Cursors.WaitCursor;
                cn.Open(cnStr, null, null, 0);
                string countRs = "";
                string query = "";

                // New clients
                query = dbKlantenSQLNieuw;
                rs.Open(query, cn, CursorTypeEnum.adOpenStatic, LockTypeEnum.adLockReadOnly, -1);
                countRs = rs.RecordCount.ToString();
                result = result + "Nieuwe klanten: " + countRs + Environment.NewLine;
                rs.Close();

                // Update clients
                query = dbKlantenSQLUpdate;
                rs.Open(query, cn, CursorTypeEnum.adOpenStatic, LockTypeEnum.adLockReadOnly, -1);
                countRs = rs.RecordCount.ToString();
                result = result + "Update klanten: " + countRs + Environment.NewLine + Environment.NewLine;
                rs.Close();
                cn.Close();
                Cursor.Current = Cursors.Default;

                MessageBox.Show(result);
            }
            catch (Exception ex)
            {
                Cursor.Current = Cursors.Default;
                MessageBox.Show(ex.Message);
            }
        }

        private void BtnCheckLedgerAccounts_Click(object sender, EventArgs e)
        {
            Connection cn = new Connection();
            Recordset rs = new Recordset();

            string cnStr = tbMarLocatie.Text;
            string result = "";

            try
            {
                Cursor.Current = Cursors.WaitCursor;
                cn.Open(cnStr, null, null, 0);
                string countRs = "";

                // New accounts
                string query = dbRekeningenSQLNieuw;
                rs.Open(query, cn, CursorTypeEnum.adOpenStatic, LockTypeEnum.adLockReadOnly, -1);
                countRs = rs.RecordCount.ToString();
                result = result + "Nieuwe rekeningen: " + countRs + Environment.NewLine;
                rs.Close();

                // Update accounts
                query = dbRekeningenSQLUpdate;
                rs.Open(query, cn, CursorTypeEnum.adOpenStatic, LockTypeEnum.adLockReadOnly, -1);
                countRs = rs.RecordCount.ToString();
                result = result + "Update rekeningen: " + countRs + Environment.NewLine + Environment.NewLine;
                rs.Close();

                // New daybooks
                query = dbJournalenSQLNieuw;
                rs.Open(query, cn, CursorTypeEnum.adOpenStatic, LockTypeEnum.adLockReadOnly, -1);
                countRs = rs.RecordCount.ToString();
                result = result + "Nieuwe journalen: " + countRs + Environment.NewLine;
                rs.Close();

                // Update daybooks
                query = dbJournalenSQLUpdate;
                rs.Open(query, cn, CursorTypeEnum.adOpenStatic, LockTypeEnum.adLockReadOnly, -1);
                countRs = rs.RecordCount.ToString();
                result = result + "Update journalen: " + countRs + Environment.NewLine + Environment.NewLine;
                rs.Close();
                cn.Close();
                Cursor.Current = Cursors.Default;

                MessageBox.Show(result);
            }
            catch (Exception ex)
            {
                Cursor.Current = Cursors.Default;
                MessageBox.Show(ex.Message);
            }
        }

        private void BtnUpdateInvoicesAndLedgers_Click(object sender, EventArgs e)
        {
            DataCustomerInvoices dbCustomerInvoices = new DataCustomerInvoices();
            DataSupplierInvoices dbSupplierInvoices = new DataSupplierInvoices();
            DataLedgers dbDayBooks = new DataLedgers();

            MessageBox.Show("Start update invoices of customers, suppliers including ledgers. For every 2000 records moving to hosting account it will take one or more minutes");
            Cursor.Current = Cursors.WaitCursor;

            // Customer Invoices
            ToolStripStatusLabel.Text = "Customer Invoices new and update...";
            StatusStrip.Refresh();
            dbCustomerInvoices.CustomerInvoicesInsertAndUpdate(tbMarLocatie.Text, dbCustomerInvoicesSQLnieuw, dbCustomerInvoicesSQLUpdate, cnnChosen);

            // Supplier Invoices
            ToolStripStatusLabel.Text = "Supplier Invoices new and update...";
            StatusStrip.Refresh();
            dbSupplierInvoices.SupplierInvoicesInsertAndUpdate(tbMarLocatie.Text, dbSupplierInvoicesSQLnieuw, dbSupplierInvoicesSQLUpdate, cnnChosen);

            // DayBooks
            ToolStripStatusLabel.Text = "Ledgers adding new...";
            StatusStrip.Refresh();
            dbDayBooks.LedgersInsert(tbMarLocatie.Text, dbJournalenSQLNieuw, cnnChosen);

            Cursor.Current = Cursors.Default;
            MessageBox.Show("Update customer and supplier invoices, including ledger finished");
            ToolStripStatusLabel.Text = "Ready";
            StatusStrip.Refresh();
        }

        private void BtnCheckLocalSqlServer_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.LocalSQLSetting2025 = tbLocalConnectionstring.Text;
            Properties.Settings.Default.Save();
        }

        private void BtnCheckHostedSqlServer_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.HostedSQLSetting2025 = tbHostedConnectionstring.Text;
            Properties.Settings.Default.Save();
        }

        private void RbHosted_CheckedChanged(object sender, EventArgs e)
        {
            cnnChosen = tbHostedConnectionstring.Text;
            BtnUpdateInvoicesAndLedgers.Enabled = true;
            BtnUpdateMainFiles.Enabled = true;
            BtnUpdateContractsAndTB2Blocks.Enabled = true;
            BtnSPCreation.Enabled = true;

            BtnCheckCustomers.Enabled = true;
            BtnCheckSuppliers.Enabled = true;
            BtnCheckLedgerAccounts.Enabled = true;
            BtnCheckContractsInvoices.Enabled = true;
            BtnResyncAll.Enabled = true;
        }

        private void RbLocal_CheckedChanged(object sender, EventArgs e)
        {
            cnnChosen = tbLocalConnectionstring.Text;
            BtnUpdateInvoicesAndLedgers.Enabled = true;
            BtnUpdateMainFiles.Enabled = true;
            BtnUpdateContractsAndTB2Blocks.Enabled = true;
            BtnSPCreation.Enabled = true;

            BtnCheckCustomers.Enabled = true;
            BtnCheckSuppliers.Enabled = true;
            BtnCheckLedgerAccounts.Enabled = true;
            BtnCheckContractsInvoices.Enabled = true;
            BtnResyncAll.Enabled = true;
        }

        private void BtnCheckSuppliers_Click(object sender, EventArgs e)
        {
            Connection cn = new Connection();
            Recordset rs = new Recordset();

            string cnStr = tbMarLocatie.Text;
            string result = "";

            try
            {
                Cursor.Current = Cursors.WaitCursor;
                cn.Open(cnStr, null, null, 0);
                string countRs = "";

                // New suppliers
                string query = dbLeveranciersSQLNieuw;
                rs.Open(query, cn, CursorTypeEnum.adOpenStatic, LockTypeEnum.adLockReadOnly, -1);
                countRs = rs.RecordCount.ToString();
                result = result + "Nieuwe leveranciers: " + countRs + Environment.NewLine;
                rs.Close();

                // Update leveranciers
                query = dbLeveranciersSQLUpdate;
                rs.Open(query, cn, CursorTypeEnum.adOpenStatic, LockTypeEnum.adLockReadOnly, -1);
                countRs = rs.RecordCount.ToString();
                result = result + "Update leveranciers " + countRs + Environment.NewLine + Environment.NewLine;
                rs.Close();

                // New suppliersinvoices
                query = dbSupplierInvoicesSQLnieuw;
                rs.Open(query, cn, CursorTypeEnum.adOpenStatic, LockTypeEnum.adLockReadOnly, -1);
                countRs = rs.RecordCount.ToString();
                result = result + "Nieuwe leveranciersfacturen: " + countRs + Environment.NewLine;
                rs.Close();

                // Update suppliersinvoices
                query = dbSupplierInvoicesSQLUpdate;
                rs.Open(query, cn, CursorTypeEnum.adOpenStatic, LockTypeEnum.adLockReadOnly, -1);
                countRs = rs.RecordCount.ToString();
                result = result + "Update leveranciersfacturen: " + countRs + Environment.NewLine;
                rs.Close();
                cn.Close();
                Cursor.Current = Cursors.Default;

                MessageBox.Show(result);
            }
            catch (Exception ex)
            {
                Cursor.Current = Cursors.Default;
                MessageBox.Show(ex.Message);
            }
        }

        private void BtnUpdateContractsAndTB2Blocks_Click(object sender, EventArgs e)
        {
            DataContracts dbContracts = new DataContracts();
            // DataTelebibContracts dbTelebibContracts = new DataTelebibContracts();

            MessageBox.Show("TODO: Start update contracts and telebib. For every 2000 records moving to hosting account it will take one or more minutes");
            Cursor.Current = Cursors.WaitCursor;

            // Contracts
            ToolStripStatusLabel.Text = "Contracts new and update...";
            StatusStrip.Refresh();
            dbContracts.ContractsInsertAndUpdate(tbMarLocatie.Text, dbContractenSQLnieuw, dbContractenSQLUpdate, cnnChosen);

            // Customers sync
            ToolStripStatusLabel.Text = "Syncing contracts...";
            StatusStrip.Refresh();
            dbContracts.ContractsSync(tbMarLocatie.Text, cnnChosen);

            // Telebib Contracts
            ToolStripStatusLabel.Text = "Telebib for contracts new...";
            StatusStrip.Refresh();
            // dbTelebibContracts.TelebibContractsInsertAndUpdate(tbMarLocatie.Text, dbTelebibContractenSQLnieuw, dbTelebibContractenSQLUpdate, cnnChosen);

            Cursor.Current = Cursors.Default;
            MessageBox.Show("Update contracts and telebib finished");
            ToolStripStatusLabel.Text = "Ready";
            StatusStrip.Refresh();
        }



        private void BtnSPCreation_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog SPPathFileDialog = new OpenFileDialog())
            {
                SPPathFileDialog.InitialDirectory = "c:\\";
                SPPathFileDialog.Filter = "sql files (*.sql)|*.sql|All files (*.*)|*.*";
                SPPathFileDialog.Multiselect = true;
                SPPathFileDialog.FilterIndex = 1;
                SPPathFileDialog.RestoreDirectory = true;

                if (SPPathFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // string fileContent = string.Empty;
                    string filePath; // = string.Empty;

                    //Get the path of specified file
                    filePath = SPPathFileDialog.FileName;
                    var arrayFileNames = SPPathFileDialog.FileNames;

                    _ = MessageBox.Show("File at path: " + filePath);

                    // Stored procedure creation loop files
                    int myCounter = 0;
                    while (arrayFileNames.Length > myCounter)
                    {
                        MessageBox.Show("Start creating stored procedure: " + arrayFileNames[myCounter]);
                        Cursor.Current = Cursors.WaitCursor;

                        AddstoredProcedures createSP = new AddstoredProcedures();
                        createSP.AddSP(arrayFileNames[myCounter], cnnChosen);

                        Cursor.Current = Cursors.Default;
                        MessageBox.Show("Creation stored procedure finished");

                        myCounter++;
                    }
                }
            }
        }

        private void BtnGenerate_Click(object sender, EventArgs e)
        {
            QRCodeGenerator qr = new QRCodeGenerator();
            QRCodeData data = qr.CreateQrCode(txtQRCode.Text, QRCodeGenerator.ECCLevel.Q);
            QRCode code = new QRCode(data);
            pic.Image = code.GetGraphic(5);
        }

        private void BtnDemoContent_Click(object sender, EventArgs e)
        {
            string serviceTagValue = "BCD\n";
            string versionValue = "001\n";
            string charactersetValue = "1\n";
            string identificationValue = "SCT\n";
            string bicValue = "VDSPBE91\n";
            string nameValue = "Roelandt en Vermoesen bv\n";
            string ibanValue = "BE83891854037015\n";
            string amountValue = "EUR394.99\n";
            string purposeValue = "GDDS\n";
            string referenceValue = "107/0404/08059\n";
            string remittanceValue = "\n";
            string informationValue = "\n";

            string qrTMP = serviceTagValue + versionValue + charactersetValue + identificationValue + bicValue + nameValue + ibanValue + amountValue + purposeValue + referenceValue + remittanceValue + informationValue;
            txtQRCode.Text = qrTMP;
        }

        private void BtnStart_Click(object sender, EventArgs e)
        {
            videoCaptureDevice = new VideoCaptureDevice(filterInfoCollection[cboCamera.SelectedIndex].MonikerString);
            videoCaptureDevice.NewFrame += VideoCaptureDevice_NewFrame;
            videoCaptureDevice.Start();
        }

        private void VideoCaptureDevice_NewFrame(object sender, AForge.Video.NewFrameEventArgs eventArgs)
        {
            Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone();
            BarcodeReader reader = new BarcodeReader();
            try
            {
                var result = reader.Decode(bitmap);
                if (result != null)
                {
                    txtBarcode.Invoke(new MethodInvoker(delegate ()
                    {
                        txtBarcode.Text = result.ToString();
                    }));
                }
                pictureBox.Image = bitmap;
            }
            catch (Exception ex)
            {
                // throw; 
                txtBarcode.Text = ex.Message;
            }
        }

        private void FrmMarioApp_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (videoCaptureDevice != null)
            {
                if (videoCaptureDevice.IsRunning)
                    videoCaptureDevice.Stop();
            }
        }

        private void BtnStopScan_Click(object sender, EventArgs e)
        {
            if (videoCaptureDevice != null)
            {
                if (videoCaptureDevice.IsRunning)
                    videoCaptureDevice.Stop();
            }
        }

        private void BtnSetMarntDONE_Click(object sender, EventArgs e)
        {
            const string message = "marIntegraal ALLE tabellen setten as export DONE. Bent U zeker?";
            const string caption = "MarNT Data Export DONE";
            DialogResult result = MessageBox.Show(message, caption,
                                         MessageBoxButtons.YesNo,
                                         MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

            if (result == DialogResult.Yes)
            {
                try
                {
                    MessageBox.Show("Start setting sync done");
                    OleDbConnection cnnMarJet = new OleDbConnection
                    {
                        ConnectionString = tbMarLocatie.Text
                    };
                    Cursor.Current = Cursors.WaitCursor;
                    cnnMarJet.Open();
                    OleDbCommand commandK = new OleDbCommand("UPDATE Klanten SET dnnSyncAlready = True, dnnSync = True", cnnMarJet);
                    _ = commandK.ExecuteNonQuery();

                    OleDbCommand commandP = new OleDbCommand("UPDATE Polissen SET dnnSyncAlready = True, dnnSync = True", cnnMarJet);
                    _ = commandP.ExecuteNonQuery();

                    OleDbCommand commandT = new OleDbCommand("UPDATE TB2 SET dnnSync = True, dnnSyncAlready = True", cnnMarJet);
                    _ = commandT.ExecuteNonQuery();

                    OleDbCommand commandDK = new OleDbCommand("UPDATE Dokumenten SET dnnSync = TRUE, dnnSyncAlready = TRUE where v034 like 'K%'", cnnMarJet);
                    _ = commandDK.ExecuteNonQuery();

                    OleDbCommand commandR = new OleDbCommand("UPDATE Rekeningen SET dnnSyncAlready = True, dnnSync = True", cnnMarJet);
                    _ = commandR.ExecuteNonQuery();

                    OleDbCommand commandJ = new OleDbCommand("UPDATE Journalen SET dnnSyncAlready = True, dnnSync = True", cnnMarJet);
                    _ = commandJ.ExecuteNonQuery();

                    OleDbCommand commandL = new OleDbCommand("UPDATE Leveranciers SET dnnSyncAlready = True, dnnSync = True", cnnMarJet);
                    _ = commandL.ExecuteNonQuery();

                    OleDbCommand commandDL = new OleDbCommand("UPDATE Dokumenten SET dnnSync = True, dnnSyncAlready = True where v034 like 'L%'", cnnMarJet);
                    _ = commandDL.ExecuteNonQuery();


                    cnnMarJet.Close();
                    Cursor.Current = Cursors.Default;
                    MessageBox.Show("End seting sync DONE");
                }
                catch (Exception ex)
                {
                    Cursor.Current = Cursors.Default;
                    _ = MessageBox.Show(ex.Message);
                }
            }
        }

        private void BtnCheckContractsInvoices_Click(object sender, EventArgs e)
        {
            Connection cn = new Connection();
            Recordset rs = new Recordset();

            string cnStr = tbMarLocatie.Text;
            string result = "";

            try
            {
                Cursor.Current = Cursors.WaitCursor;
                cn.Open(cnStr, null, null, 0);
                string countRs = "";
                string query = "";

                // New contracts
                query = dbContractenSQLnieuw;
                rs.Open(query, cn, CursorTypeEnum.adOpenStatic, LockTypeEnum.adLockReadOnly, -1);
                countRs = rs.RecordCount.ToString();
                result = result + "Nieuwe contracten: " + countRs + Environment.NewLine;
                rs.Close();

                // Update contracts
                query = dbContractenSQLUpdate;
                rs.Open(query, cn, CursorTypeEnum.adOpenStatic, LockTypeEnum.adLockReadOnly, -1);
                countRs = rs.RecordCount.ToString();
                result = result + "Update contracten: " + countRs + Environment.NewLine + Environment.NewLine;
                rs.Close();

                // New customerinvoices - needs to be checked because of to long time to execute
                query = dbCustomerInvoicesSQLnieuw;
                rs.Open(query, cn, CursorTypeEnum.adOpenStatic, LockTypeEnum.adLockReadOnly, -1);
                countRs = rs.RecordCount.ToString();
                result = result + "Nieuwe klantfacturen: " + countRs + Environment.NewLine;
                rs.Close();

                // New customerinvoices
                query = dbCustomerInvoicesSQLUpdate;
                rs.Open(query, cn, CursorTypeEnum.adOpenStatic, LockTypeEnum.adLockReadOnly, -1);
                countRs = rs.RecordCount.ToString();
                result = result + "Update klantfacturen: " + countRs + Environment.NewLine;
                rs.Close();

                // New telebibcontracts
                //query = dbTelebibContractenSQLnieuw;
                //rs.Open(query, cn, CursorTypeEnum.adOpenKeyset, LockTypeEnum.adLockOptimistic, -1);
                //countRs = rs.RecordCount.ToString();
                //result = result + "Nieuwe telebibcontracten: " + countRs + Environment.NewLine;
                //rs.Close();

                // Update telebibcontracts
                //query = dbTelebibContractenSQLUpdate;
                //rs.Open(query, cn, CursorTypeEnum.adOpenKeyset, LockTypeEnum.adLockOptimistic, -1);
                //countRs = rs.RecordCount.ToString();
                //result = result + "Update telebibcontracten: " + countRs + Environment.NewLine + Environment.NewLine;
                //rs.Close();
                cn.Close();
                Cursor.Current = Cursors.Default;

                MessageBox.Show(result);
            }
            catch (Exception ex)
            {
                Cursor.Current = Cursors.Default;
                MessageBox.Show(ex.Message);
            }
        }

        private void BtnResyncAll_Click(object sender, EventArgs e)
        {
            DataCustomers dbCustomers = new DataCustomers();
            DataSuppliers dbSuppliers = new DataSuppliers();
            DataContracts dbContracts = new DataContracts();
            DataCustomerInvoices dbCustomerInvoices = new DataCustomerInvoices();
            DataSupplierInvoices dbSupplierInvoices = new DataSupplierInvoices();
            DataLedgerAccounts dbAccounts = new DataLedgerAccounts();

            Cursor.Current = Cursors.WaitCursor;
            MessageBox.Show("Start resyncing alls. This can take a while depending on your computer and server location");

            ToolStripStatusLabel.Text = "Resyncing customers...";
            dbCustomers.CustomersAllSync(tbMarLocatie.Text, cnnChosen);
            ToolStripStatusLabel.Text = "Resyncing customer invoices...";
            StatusStrip.Refresh();
            dbCustomerInvoices.CustomerInvoicesSyncID(tbMarLocatie.Text, cnnChosen);
            ToolStripStatusLabel.Text = "Resyncing suppliers...";
            StatusStrip.Refresh();
            dbSuppliers.SuppliersAllSync(tbMarLocatie.Text, cnnChosen);
            ToolStripStatusLabel.Text = "Resyncing supplier invoices...";
            StatusStrip.Refresh();
            dbSupplierInvoices.SupplierInvoicesSyncID(tbMarLocatie.Text, cnnChosen);
            ToolStripStatusLabel.Text = "Resyncing contracts...";
            StatusStrip.Refresh();
            dbContracts.ContractsSync(tbMarLocatie.Text, cnnChosen);
            ToolStripStatusLabel.Text = "Resyncing ledgeraccounts...";
            StatusStrip.Refresh();
            dbAccounts.LedgerAccountsAllSync(tbMarLocatie.Text, cnnChosen);

            MessageBox.Show("Resyncing finished");
            Cursor.Current = Cursors.Default;
            ToolStripStatusLabel.Text = "Ready";
            StatusStrip.Refresh();
        }

        async private void ButtonCheckVat_Click(object sender, EventArgs e)
        {
            string vatNumber = TextBoxVatNumber.Text;
            string countryCode = vatNumber.Substring(0, 2);
            string vat = vatNumber.Substring(2);
            LabelResponse.Text = "Bezig...";
            LabelResponseContent.Text = "Bezig...";

            string url = "https://ec.europa.eu/taxation_customs/vies/rest-api/ms/" + countryCode + "/vat/" + vat;

            connectionCheck = new HttpClient();

            HttpResponseMessage response = await connectionCheck.GetAsync(url);
            string responseContent = await response.Content.ReadAsStringAsync();
            LabelResponse.Text = response.ToString();
            LabelResponseContent.Text = responseContent;
        }

        private void ButtonClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}