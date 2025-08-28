using ADODB;
using MarioApp.MarioMenu.Admin;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Windows.Forms;

namespace MarioApp.MarioMenu.Actions
{
    public partial class FormPeppolClientActions : Form
    {
        private readonly HttpClient httpCheck;
        public Form FormDataGridJsonPopUp { get; set; }

        // Testing ADODB connection to marnt.mdv file
        public Recordset JournalRS { get; private set; }

        public FormPeppolClientActions()
        {
            InitializeComponent();
            Text = "Peppol Verrichtingen [ " + SharedGlobals.CompanyName + "]";
            httpCheck = new HttpClient();
            FormDataGridJsonPopUp = new FormDataGridJsonPopUp { };
            RadioButtonGetReceived.Checked = true;
            TextBoxLegalEntityId.Text = ""; // Default to empty to enable country/scheme/identifier fields

            // Ensure events are hooked up if you didn't use the Designer
            this.DragEnter += TabSendDocument_DragEnter;
            this.DragDrop += TabSendDocument_DragDrop;
            this.RadioButtonGetReceived.Checked = true;
            FillListPeppolToSend();
        }

        private void TabSendDocument_DragEnter(object sender, DragEventArgs e)
        {
            // Only allow file drops
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void FillListPeppolToSend()
        {
            string folderPath = SharedGlobals.MimDataLocation + "\\" + SharedGlobals.ActiveCompany + "\\peppol\\out";
            
            if (Directory.Exists(folderPath))
            {
                string[] xmlFiles = Directory.GetFiles(folderPath, "*.xml");
                ListBoxDocumentsToSend.Items.Clear();
                ListBoxDocumentsToSend.Items.AddRange(xmlFiles);
                ButtonCheckFile.Visible = true;
                LabelFile.Text = "";
            }
            else
            {
                MessageBox.Show("Er zijn geen te verzenden documenten voor bedrijf " + SharedGlobals.ActiveCompany);
                ListBoxDocumentsToSend.Visible = false;
            }
        }

        private void TabSendDocument_DragDrop(object sender, DragEventArgs e)
        {
            // Use pattern matching to simplify the type check and cast
            if (e.Data is DataObject dataObject && dataObject.GetData(DataFormats.FileDrop) is string[] files && files.Length > 0)
            {
                // Example 1: Open each file with its default program
                // foreach (var file in files)
                //     Process.Start(new ProcessStartInfo(file) { UseShellExecute = true });

                // Example 2: Load the first file’s text into a TextBox
                if (File.Exists(files[0]))
                {
                    // Assuming you have a TextBox named LabelFile and LabelInvoiceId, LabelIssueDate
                    LabelFile.Text = files[0];
                }
            }
        }

        private void LabelFile_TextChanged(object sender, EventArgs e)
        {
            ButtonCheckFile.Visible = !string.IsNullOrWhiteSpace(LabelFile.Text) && File.Exists(LabelFile.Text);
            if (SharedGlobals.PeppolOutFiles > 0)
                ButtonCheckFile.Visible = true;
        }

        private void ButtonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        async private void ButtonCheckConnectivity_Click(object sender, EventArgs e)
        {
            ToolStripStatusLabel.Text = "Bezig...";

            var response = await AdemicoClient.CheckConnection(new HttpClient());
            if (response != null)
            {
                ToolStripStatusLabel.Text = response;
            }
            else
            {
                MessageBox.Show(response, "Foutmelding", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RadioButtonGetReceived_CheckedChanged(object sender, EventArgs e)
        {
            if (RadioButtonGetReceived.Checked)
            {
                // Enable the fields for received documents                
                TextBoxReceiver.Enabled = true;
                TextBoxReceiver.Text = "0208:" + SharedGlobals.CompanyKBONumber; // Default receiver for received documents
                TextBoxSender.Enabled = false;
                TextBoxSender.Text = "";
            }
        }

        private void RadioButtonGetSent_CheckedChanged(object sender, EventArgs e)
        {
            if (RadioButtonGetSent.Checked)
            {
                // Enable the fields for sent documents                
                TextBoxSender.Enabled = true;
                TextBoxSender.Text = "9925:" + SharedGlobals.CompanyKBONumber; // Default sender for sent documents
                TextBoxReceiver.Enabled = false;
                TextBoxReceiver.Text = "";
            }
        }

        private void RadioButtonAll_CheckedChanged(object sender, EventArgs e)
        {
            if (RadioButtonAll.Checked)
            {
                // Enable both fields for all documents                
                TextBoxSender.Enabled = true;
                TextBoxSender.Text = "9925:" + SharedGlobals.CompanyKBONumber; // Default sender for sent documents
                TextBoxReceiver.Enabled = true;
                TextBoxReceiver.Text = "0208:" + SharedGlobals.CompanyKBONumber; // Default receiver for received documents                
            }

        }

        async private void ButtonNotifications_Click(object sender, EventArgs e)
        {
            ToolStripStatusLabel.Text = "Bezig...";

            string eventType = RadioButtonGetReceived.Checked ? "DOCUMENT_RECEIVED" : "DOCUMENT_SENT";
            var jsonResponse = await AdemicoClient.GetNotificationsAsync(
                transmissionId: "", // "f8a591c77b2211f0b1ed0af13d778bd4"
                documentId: "",
                eventType: eventType, // "DOCUMENT_RECEIVED" or "DOCUMENT_SENT"
                peppolDocumentType: "", // "INVOICE"
                sender: TextBoxSender.Text, // "9925:BE0440058217",
                receiver: TextBoxReceiver.Text, // "0208:0440058217",
                startDateTime: "", // "2023-07-25T11:03:26.688Z"
                endDateTime: "", // "2023-07-29T11:03:26.688Z"
                page: "",
                pageSize: ""
            );

            if (jsonResponse != null)
            {
                ToolStripStatusLabel.Text = "Notifications retrieved successfully.";
                var deserializedString = JsonConvert.DeserializeObject(jsonResponse);
                RichTextBoxResult.Text = JsonConvert.SerializeObject(deserializedString, Newtonsoft.Json.Formatting.Indented);
                DoPopUpDataGridJsonData(RichTextBoxResult.Text); // Show the result in a popup with JSON table view
            }
            else
            {
                ToolStripStatusLabel.Text = "Failed to retrieve notifications.";
                RichTextBoxResult.Text = "";
            }

        }

        private void DoPopUpDataGridJsonData(string messageAsJson)
        {
            FormDataGridJsonPopUp.Controls.Clear();
            FormDataGridJsonPopUp formJsonTable = new FormDataGridJsonPopUp
            {
                jsonData = messageAsJson, // Pass the JSON data to the popup form
                Dock = DockStyle.Fill // Fill the popup form with the JSON table view                
            };
            formJsonTable.LoadNotificationJsonData();
            formJsonTable.ShowDialog(this); // Show the popup form as a dialog, centered on the main form
        }

        async private void ButtonGetPeppolRegistrations_Click(object sender, EventArgs e)
        {
            ToolStripStatusLabel.Text = "Bezig...";
            string country = TextBoxCountryCode.Text; // Example country code
            string peppolRegistrationScheme = TextBoxRegScheme.Text; // Example scheme code, e.g., "0208"
            string peppolRegistrationIdentifier = TextBoxRegIdentifier.Text; // Example identifier, e.g., "0529835180"
            string peppolSupportedDocument = TextBoxSupportedDocument.Text; // Example supported document, e.g., "PEPPOL_BIS_BILLING_UBL_INVOICE_V3"
            string legalEntityId = TextBoxLegalEntityId.Text; // Example legal entity ID, if needed

            try
            {
                var respons = await AdemicoClient.GetPeppolRegistrationAsync(
                    country,
                    peppolRegistrationScheme,
                    peppolRegistrationIdentifier,
                    peppolSupportedDocument,
                    legalEntityId);

                if (respons.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    ToolStripStatusLabel.Text = "Registration(s) found (200).";
                }
                else if (respons.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    ToolStripStatusLabel.Text = "Unauthorized (401) — check credentials.";
                }
                else if (respons.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    ToolStripStatusLabel.Text = "Not found (404) — no matching registration.";
                }
                else
                {
                    ToolStripStatusLabel.Text = $"Status: {(int)respons.StatusCode} {respons.StatusCode}";
                }

                if (!string.IsNullOrEmpty(respons.ResponseBody))
                {
                    var deserializedString = JsonConvert.DeserializeObject(respons.ResponseBody);
                    RichTextBoxResult.Text = JsonConvert.SerializeObject(deserializedString, Newtonsoft.Json.Formatting.Indented);
                    DoPopUpEntitiesData(RichTextBoxResult.Text); // Show the result in a popup with JSON table view
                }
                else
                {
                    ToolStripStatusLabel.Text = "No response body.";
                }
            }
            catch (Exception ex)
            {
                ToolStripStatusLabel.Text = $"Request failed: {ex.Message}";
            }
        }

        private void DoPopUpEntitiesData(string messageAsJson)
        {
            FormDataGridJsonPopUp formJsonTable = new FormDataGridJsonPopUp
            {
                jsonData = messageAsJson, // Pass the JSON data to the popup form
                Dock = DockStyle.Fill // Fill the popup form with the JSON table view                
            };
            formJsonTable.LoadRegistrationJsonData();
            formJsonTable.ShowDialog(this); // Show the popup form as a dialog, centered on the main form
        }

        private void TextBoxLegalEntityId_TextChanged(object sender, EventArgs e)
        {
            if (TextBoxLegalEntityId.Text.Length > 0)
            {
                // If a Legal Entity ID is choosen, set specific values for Belgium
                TextBoxCountryCode.Text = "";
                TextBoxRegScheme.Text = "";
                TextBoxRegIdentifier.Text = "";
                TextBoxSupportedDocument.Text = "";
                TextBoxLegalEntityId.Enabled = true;
                TextBoxRegIdentifier.Enabled = false;
                TextBoxCountryCode.Enabled = false;
                TextBoxRegScheme.Enabled = false;
                TextBoxSupportedDocument.Enabled = false;
            }
            else
            {
                // If no Legal Entity ID is provided, set default values for Belgium
                TextBoxCountryCode.Text = "BE"; // Default to Belgium if no Legal Entity ID is provided                                
                TextBoxRegScheme.Text = "0208"; // Default to 0208 scheme for Belgium  
                TextBoxRegIdentifier.Text = "0440058217"; // Default to a common identifier for Belgium
                TextBoxSupportedDocument.Text = "PEPPOL_BIS_BILLING_UBL_INVOICE_V3"; // Default to UBL Invoice for Belgium            
                TextBoxLegalEntityId.Text = ""; // Default Legal Entity ID for Belgium

                TextBoxRegIdentifier.Enabled = true; // Enable the registration identifier field
                TextBoxCountryCode.Enabled = true; // Enable the country code field 
                TextBoxRegScheme.Enabled = true; // Enable the registration scheme field
                TextBoxSupportedDocument.Enabled = true; // Enable the supported document field
            }
        }

        private void ButtonSendUblDocument_Click(object sender, EventArgs e)
        {

            //string result = {
            //    "transmissionId": "803828d52d4911ed85f512ef9c5638d0",
            //        "documentId": "pbe000512-1",
            //        "documentTypeEnum": "INVOICE",
            //        "sender": "0208:0552912569",
            //        "receiver": "0208:0465105397",
            //        "sbdhInstanceIdentifier": "6b56b28947e511eda0b802bb4e4747f9",
            //        "c5SubmissionResult": {
            //    "status": "SCHEDULED",
            //            "details": []
            //        }
        }

        private void ButtonShowSharedGlobals_Click(object sender, EventArgs e)
        {
            MessageBox.Show(                
                $"Naam Bedrijf (KBO): {SharedGlobals.CompanyName}\n" +
                $"Adres: {SharedGlobals.CompanyAddress}\n" +
                $"Postcode en Plaats: {SharedGlobals.CompanyPostalCodeAndCity}\n" +
                $"Telefoon: {SharedGlobals.CompanyPhoneNumber}\n" +
                $"KBO Nummer: {SharedGlobals.CompanyKBONumber}\n" +
                $"BTW Nummer (zonder 'BE'): {SharedGlobals.CompanyVATNumber}\n" +
                $"IBAN Rekening Nummer: {SharedGlobals.CompanyIBANNumber}\n" +
                $"BIC Nummer: {SharedGlobals.CompanyBICNumber}\n" +
                $"Email Adres Bedrijf: {SharedGlobals.CompanyEmailAddress}\n" +
                $"Contactpersoon: {SharedGlobals.CompanyContactPerson}\n" +
                $"Email Adres Contactpersoon: {SharedGlobals.CompanyContactEmailAddress}\n\n" +
                // $"DbJet Provider: {SharedGlobals.DbJetProvider}\n" +
                $"Mapnummer Actief Bedrijf: {SharedGlobals.ActiveCompany}\n" +
                $"Inhoudsopgave Mar Mdv Bestand: {SharedGlobals.MarntMdvLocation}\n" +
                $"Inhoudsopgave Mar Data: {SharedGlobals.MimDataLocation}\n",                
                "Variabele gegevens van actief bedrijf",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

        }

        public void GetJournalRS()
        {
            Cursor.Current = Cursors.WaitCursor;
            string sSQL = "SELECT Journalen.v066, Journalen.v019, Rekeningen.v020, Journalen.v067, Journalen.v033, Journalen.dece068, Journalen.v069 FROM Journalen, Rekeningen WHERE  Journalen.v019=Rekeningen.v019 ORDER BY Journalen.rvID";  // 'Journalen.v066";
            // Open the connection and execute the insert command.
            // The connection is automatically closed when the
            // code exits the using block.
            string connectionString = SharedGlobals.DbJetProvider + SharedGlobals.MimDataLocation + SharedGlobals.MarntMdvLocation;
            JournalRS = new Recordset()
            {
                CursorLocation = CursorLocationEnum.adUseClient
            };
            JournalRS.Open(sSQL, connectionString, CursorTypeEnum.adOpenForwardOnly, LockTypeEnum.adLockReadOnly);
            if (JournalRS.EOF)
            {
                MessageBox.Show ("0");
            }
            else
            {
                MessageBox.Show (JournalRS.RecordCount.ToString());                
            }
            Cursor.Current = Cursors.Default;
        }
    }
}
