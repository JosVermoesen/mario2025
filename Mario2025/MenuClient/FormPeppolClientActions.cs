using ADODB;
using MarioApp.MarioMenu.Admin;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Windows.Forms;
using System.Xml;

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

            FillListPeppolToSend();
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
        private void ButtonCheckFile_Click(object sender, EventArgs e)
        {



            string filePath = LabelFile.Text.Trim();
            string extension = Path.GetExtension(filePath).ToLowerInvariant();
            if (extension != ".xml") // && extension != ".ubl")
            {
                // Show a message box if the file is not a valid UBL XML file
                MessageBox.Show("Dit is geen geldig UBL XML bestand");
                ButtonSendUblDocument.Enabled = false; // Disable the button if the file is not valid
                return;
            }
            else
            {
                // HandleUblDocument(filePath);
                ReadUBLInvoice(filePath);


                // If the file is valid, enable the button
                ButtonSendUblDocument.Enabled = true;
                ToolStripStatusLabel.Text = "Bestand is een geldig UBL XML bestand.";
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
                TextBoxRegIdentifier.Text = SharedGlobals.CompanyKBONumber; // Default to a common identifier for Belgium
                TextBoxSupportedDocument.Text = "UBL_BE_INVOICE_3_0"; // Default to UBL Invoice for Belgium            
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

        private static void ReadUBLInvoice(string filePath)
        {
            var xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.Load(filePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading XML: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var ns = new XmlNamespaceManager(xmlDoc.NameTable);
            ns.AddNamespace("cbc", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2");
            ns.AddNamespace("cac", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2");

            var sb = new StringBuilder();

            // UBL Version
            var ublVersionNode = xmlDoc.SelectSingleNode("//cbc:UBLVersionID", ns);
            sb.AppendLine("UBL VersionID: " + (ublVersionNode?.InnerText ?? "not found"));

            // Document ID
            var invoiceIdNode = xmlDoc.SelectSingleNode("//cbc:ID", ns);
            sb.AppendLine("Document ID: " + (invoiceIdNode?.InnerText ?? "not found"));

            // IssueDate
            var issueDateNode = xmlDoc.SelectSingleNode("//cbc:IssueDate", ns);
            sb.AppendLine("IssueDate: " + (issueDateNode?.InnerText ?? "not found"));

            // DueDate
            var dueDateNode = xmlDoc.SelectSingleNode("//cbc:DueDate", ns);
            sb.AppendLine("DueDate: " + (dueDateNode?.InnerText ?? "not found"));

            // InvoiceTypeCode
            var invTypeNode = xmlDoc.SelectSingleNode("//cbc:InvoiceTypeCode", ns);
            if (invTypeNode != null)
            {
                var invoiceTypeCode = invTypeNode.InnerText;
                var listID = invTypeNode.Attributes?["listID"]?.Value ?? "";
                sb.AppendLine("invoiceTypeCode: " + invoiceTypeCode);
                sb.AppendLine("invoice listID: " + listID);
            }
            else
            {
                MessageBox.Show("InvoiceTypeCode element not found.");
            }

            // OrderReference
            var orderList = xmlDoc.SelectNodes("//cac:OrderReference", ns);
            if (orderList != null)
            {
                foreach (XmlNode ordNode in orderList)
                {
                    var orderId = ordNode.SelectSingleNode("cbc:ID", ns)?.InnerText ?? "Order ID: not available";
                    sb.AppendLine("Order ID: " + orderId);
                }
            }
            MessageBox.Show(sb.ToString(), "Testing UBL DATA versie 0.01", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Supplier info
            var supplierNode = xmlDoc.SelectSingleNode("//cac:AccountingSupplierParty/cac:Party", ns);
            if (supplierNode != null)
            {
                var msg = new StringBuilder();
                msg.AppendLine("Supplier info");
                msg.AppendLine("-------------");
                msg.AppendLine("endpointOndernemingsnummer " + NodeText(supplierNode, "cbc:EndpointID", ns));
                msg.AppendLine("supplierID: " + NodeText(supplierNode, "cac:PartyIdentification/cbc:ID", ns));
                msg.AppendLine("tradingName: " + NodeText(supplierNode, "cac:PartyName/cbc:Name", ns));
                msg.AppendLine("street: " + NodeText(supplierNode, "cac:PostalAddress/cbc:StreetName", ns));
                msg.AppendLine("city: " + NodeText(supplierNode, "cac:PostalAddress/cbc:CityName", ns));
                msg.AppendLine("postalZone: " + NodeText(supplierNode, "cac:PostalAddress/cbc:PostalZone", ns));
                msg.AppendLine("countryCode: " + NodeText(supplierNode, "cac:PostalAddress/cac:Country/cbc:IdentificationCode", ns));
                msg.AppendLine("vatNumber: " + NodeText(supplierNode, "cac:PartyTaxScheme/cbc:CompanyID", ns));
                MessageBox.Show(msg.ToString(), "Testing UBL DATA versie 0.01", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("No AccountingSupplierParty element found.");
            }

            // Customer info
            var custNode = xmlDoc.SelectSingleNode("//cac:AccountingCustomerParty", ns);
            if (custNode != null)
            {
                var msg = new StringBuilder();
                msg.AppendLine("Customer info");
                msg.AppendLine("-------------");
                msg.AppendLine("custAssignedAccountID: " + NodeText(custNode, "cbc:CustomerAssignedAccountID", ns));
                msg.AppendLine("custEndpointID: " + NodeText(custNode, "cac:Party/cbc:EndpointID", ns));
                msg.AppendLine("custName: " + NodeText(custNode, "cac:Party/cac:PartyName/cbc:Name", ns));
                msg.AppendLine("custStreet: " + NodeText(custNode, "cac:Party/cac:PostalAddress/cbc:StreetName", ns));
                msg.AppendLine("custCity: " + NodeText(custNode, "cac:Party/cac:PostalAddress/cbc:CityName", ns));
                msg.AppendLine("custPostalZone: " + NodeText(custNode, "cac:Party/cac:PostalAddress/cbc:PostalZone", ns));
                msg.AppendLine("custCountryCode: " + NodeText(custNode, "cac:Party/cac:PostalAddress/cac:Country/cbc:IdentificationCode", ns));
                msg.AppendLine("custTaxID: " + NodeText(custNode, "cac:Party/cac:PartyTaxScheme/cbc:CompanyID", ns));
                msg.AppendLine("custTaxScheme: " + NodeText(custNode, "cac:Party/cac:PartyTaxScheme/cac:TaxScheme/cbc:ID", ns));
                MessageBox.Show(msg.ToString(), "Testing UBL DATA versie 0.01", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("No AccountingCustomerParty element found.");
            }

            // PaymentMeans
            var pmNodes = xmlDoc.SelectNodes("//cac:PaymentMeans", ns);
            if (pmNodes != null && pmNodes.Count > 0)
            {
                var msg = new StringBuilder();
                msg.AppendLine("PaymentMeans");
                msg.AppendLine("------------");
                foreach (XmlNode pmNode in pmNodes)
                {
                    msg.AppendLine("PaymentMeansCode: " + NodeText(pmNode, "cbc:PaymentMeansCode", ns));
                    msg.AppendLine("PaymentID: " + NodeText(pmNode, "cbc:PaymentID", ns));
                    msg.AppendLine("Payee IBAN: " + NodeText(pmNode, "cac:PayeeFinancialAccount/cbc:ID", ns));
                    msg.AppendLine("Account Name: " + NodeText(pmNode, "cac:PayeeFinancialAccount/cbc:Name", ns));
                    msg.AppendLine("BIC/Branch ID: " + NodeText(pmNode, "cac:PayeeFinancialAccount/cac:FinancialInstitutionBranch/cbc:ID", ns));
                    msg.AppendLine();
                    msg.AppendLine("Card account (if present)");
                    msg.AppendLine("Card Account ID: " + NodeText(pmNode, "cac:CardAccount/cbc:ID", ns));
                    msg.AppendLine("Card Account Name: " + NodeText(pmNode, "cac:CardAccount/cbc:Name", ns));
                    msg.AppendLine();
                    msg.AppendLine("Direct debit mandate (if present)");
                    msg.AppendLine("Mandate ID: " + NodeText(pmNode, "cac:PaymentMandate/cbc:ID", ns));
                    msg.AppendLine("Mandate Date: " + NodeText(pmNode, "cac:PaymentMandate/cbc:PaymentMandateDate", ns));
                }
                MessageBox.Show(msg.ToString(), "Testing UBL DATA versie 0.01", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("No PaymentMeans element found.");
            }

            // TaxTotal
            var msgTax = new StringBuilder();
            msgTax.AppendLine("TaxTotal");
            msgTax.AppendLine("--------");
            var taxAmountEl = xmlDoc.SelectSingleNode("//cac:TaxTotal/cbc:TaxAmount", ns);
            var currencyID = taxAmountEl?.Attributes?["currencyID"]?.Value ?? "";
            if (taxAmountEl != null && string.IsNullOrEmpty(currencyID))
            {
                MessageBox.Show("Attribute currencyID is missing on <cbc:TaxAmount>");
            }

            var taxTotals = xmlDoc.SelectNodes("//cac:TaxTotal", ns);
            if (taxTotals != null)
            {
                foreach (XmlNode taxTotalElem in taxTotals)
                {
                    var ttAmount = taxTotalElem.SelectSingleNode("cbc:TaxAmount", ns)?.InnerText ?? "";
                    msgTax.AppendLine($"TaxTotal: {ttAmount} {currencyID}");

                    var subtotals = taxTotalElem.SelectNodes("cac:TaxSubtotal", ns);
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                    foreach (XmlNode subElem in subtotals)
                    {
                        msgTax.AppendLine();
                        msgTax.AppendLine("SubDetail");
                        msgTax.AppendLine("TaxableAmount: " + NodeText(subElem, "cbc:TaxableAmount", ns));
                        msgTax.AppendLine("TaxAmount: " + NodeText(subElem, "cbc:TaxAmount", ns));
                        msgTax.AppendLine("Percent: " + NodeText(subElem, "cac:TaxCategory/cbc:Percent", ns) + "%");
                        msgTax.AppendLine();
                    }
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                }
                MessageBox.Show(msgTax.ToString(), "Testing UBL DATA versie 0.01", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            // LegalMonetaryTotal
            var msgMoney = new StringBuilder();
            msgMoney.AppendLine("LegalMonetaryTotal");
            msgMoney.AppendLine("------------------");
            var moneyTotalEl = xmlDoc.SelectSingleNode("//cac:LegalMonetaryTotal", ns);
            if (moneyTotalEl != null)
            {
                msgMoney.AppendLine("LineExtensionAmount: " + NodeText(moneyTotalEl, "cbc:LineExtensionAmount", ns));
                msgMoney.AppendLine("TaxExclusiveAmount: " + NodeText(moneyTotalEl, "cbc:TaxExclusiveAmount", ns));
                msgMoney.AppendLine("TaxInclusiveAmount: " + NodeText(moneyTotalEl, "cbc:TaxInclusiveAmount", ns));
                msgMoney.AppendLine("PayableAmount: " + NodeText(moneyTotalEl, "cbc:PayableAmount", ns) + $" ({currencyID})");
                MessageBox.Show(msgMoney.ToString(), "Testing UBL DATA versie 0.01", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            // InvoiceLines
            var msgLines = new StringBuilder();
            var invoiceLines = xmlDoc.SelectNodes("//cac:InvoiceLine", ns);
            if (invoiceLines != null)
            {
                foreach (XmlNode lineNode in invoiceLines)
                {
                    var desc = NodeText(lineNode, ".//cbc:Description", ns);
                    var qty = NodeText(lineNode, ".//cbc:InvoicedQuantity", ns);
                    var price = NodeText(lineNode, ".//cbc:PriceAmount", ns);
                    msgLines.AppendLine($"Item: {desc}, Quantity: {qty}, Price: {price}");
                }
                MessageBox.Show(msgLines.ToString(), "Testing UBL DATA versie 0.01", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // Helper for safe node text extraction
        private static string NodeText(XmlNode parentNode, string xpath, XmlNamespaceManager ns)
        {
            var node = parentNode.SelectSingleNode(xpath, ns);
            return node?.InnerText.Trim() ?? "";
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
                MessageBox.Show("0");
            }
            else
            {
                MessageBox.Show(JournalRS.RecordCount.ToString());
            }
            Cursor.Current = Cursors.Default;
        }

        private void ListBoxDocumentsToSend_Click(object sender, EventArgs e)
        {
            ButtonSendUblDocument.Enabled = false; // Disable the button when selecting a new file
            LabelFile.Text = "";

            if (ListBoxDocumentsToSend.SelectedItem != null)
            {
                LabelFile.Text  = ListBoxDocumentsToSend.SelectedItem.ToString();
            }
        }
    }
}
