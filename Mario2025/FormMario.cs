using MarioApp.MarioMenu.Actions;
using MarioApp.MarioMenu.Admin;
using Microsoft.VisualBasic;
using System;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace MarioApp
{
    public partial class FormMario : Form
    {
        public FormMario()
        {
            InitializeComponent();
            Text = "Mario2025";
        }

        private void MenuItemCloseApp_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void MenuItemMdvToSql_Click(object sender, EventArgs e)
        {
            var form = new FormMarioTools();
            form.ShowDialog(this);
        }

        private void FormMario_Load(object sender, EventArgs e)
        {
            if (!Properties.Settings.Default.IsUpgraded)
            {
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.IsUpgraded = true;
                Properties.Settings.Default.Save();
            }

            if (Properties.Settings.Default.MainTop <= 0)
            {
                this.Width = 816;
                this.Height = 489;
                this.StartPosition = FormStartPosition.CenterScreen;
            }
            else
            {
                this.Top = Properties.Settings.Default.MainTop;
                this.Left = Properties.Settings.Default.MainLeft;
                this.Width = Properties.Settings.Default.MainWidth;
                this.Height = Properties.Settings.Default.MainHeight;
            }
        }
        private void FormMario_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.MainTop = this.Top;
            Properties.Settings.Default.MainLeft = this.Left;
            Properties.Settings.Default.MainWidth = this.Width;
            Properties.Settings.Default.MainHeight = this.Height;
            Properties.Settings.Default.Save();
        }

        private void MenuItemUserSettings_Click(object sender, EventArgs e)
        {
            var form = new FormChooseCompany();
            form.ShowDialog(this);
            if (SharedGlobals.ActiveCompany != "")
            {
                Text = "Mario2025 - [" + SharedGlobals.ActiveCompany + "] " + SharedGlobals.CompanyName;
            }
            else
            {
                Text = "Mario2025";
            }
        }

        private void FormMario_Shown(object sender, EventArgs e)
        {
            bool isAdmin = Properties.Settings.Default.IsAdmin;
            AdminMenuItem.Visible = isAdmin; // Show or hide the Admin menu based on the IsAdmin setting             

            // Load MimDataLocation from marIntegraal settings
            // Value must contains "\marnt\data"
            string value = Interaction.GetSetting(
                "marINTEGRAAL",       // AppName
                "marIntegraal",     // Section
                "Bedrijfsinhoudsopgave2025",
                "" // Default if not found
                ) ?? ""; // Ensure null-coalescing operator to handle possible null value.
            
            bool containsPath = value.ToLower().Contains(@"\marnt\data".ToLower());
            if (!containsPath)
            {
                MessageBox.Show("De locatie van de bedrijfsinhoudsopgave is niet correct ingesteld.\n\nDuidt in marIntegraal een correcte locatie aan a.u.b.", "Fout in locatie", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Close();
            }
            SharedGlobals.SetMimDataLocation(value);
                        
            // Load MarNT CloudLocation from settings
            value = Interaction.GetSetting(
                "marINTEGRAAL",       // AppName
                "dnnInstellingen",     // Section
                "Cloud",
                "" // Default if not found
                ) ?? ""; // Ensure null-coalescing operator to handle possible null value.

            // A marNT Clouddrive Location must ends with  "\marnt"                        
            containsPath = value.ToLower().EndsWith(@"\marnt".ToLower());
            if (!containsPath)
            {
                MessageBox.Show("De locatie van de bedrijfsinhoudsopgave is niet correct ingesteld.\n\nDuidt in marIntegraal een correcte locatie aan a.u.b.", "Fout in locatie", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Close();
            }
            SharedGlobals.MarntCloudLocation = value;

            // Load MarNT Archive CloudLocation from settings
            value = Interaction.GetSetting(
                "marINTEGRAAL",       // AppName
                "dnnInstellingen",     // Section
                "Archief",
                "" // Default if not found
                ) ?? ""; // Ensure null-coalescing operator to handle possible null value.

            // A Clouddrive marNT archive Location must end with "\marnt\archief"                        
            containsPath = value.ToLower().EndsWith(@"\marnt\archief".ToLower());
            if (!containsPath)
            {
                MessageBox.Show("De locatie van de bedrijfsinhoudsopgave is niet correct ingesteld.\n\nDuidt in marIntegraal een correcte locatie aan a.u.b.", "Fout in locatie", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Close();
            }
            SharedGlobals.MarntCLoudArchiveLocation = value;

            // A Clouddrive MarNT Mario Location must end with "\marnt\mario"                        
            value = Interaction.GetSetting(
                "marINTEGRAAL",       // AppName
                "dnnInstellingen",     // Section
                "Mario",
                "" // Default if not found
                ) ?? ""; // Ensure null-coalescing operator to handle possible null value.

            // A Clouddrive marNT Location must end with "\marnt"                        
            containsPath = value.ToLower().EndsWith(@"\marnt\mario".ToLower());
            if (!containsPath)
            {
                MessageBox.Show("De locatie van de bedrijfsinhoudsopgave is niet correct ingesteld.\n\nDuidt in marIntegraal een correcte locatie aan a.u.b.", "Fout in locatie", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Close();
            }
            SharedGlobals.MarntCloudMarioLocation = value;
        }

        private void MenuItemPeppolActions_Click(object sender, EventArgs e)
        {
            if (SharedGlobals.CompanyKBONumber == "")
            {
                MessageBox.Show("Eerst een bedrijf activeren a.u.b.", "Kies eerst een bedrijf", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var form = new FormPeppolClientActions();
            form.ShowDialog(this);
        }

        private void MenuItemPeppolTesting_Click(object sender, EventArgs e)
        {
            var form = new FormPeppolActions();
            form.ShowDialog(this);
        }

        private void MenuItemPeppolSettings_Click(object sender, EventArgs e)
        {
            var form = new FormAdminSettings();
            form.ShowDialog(this);

        }
    }
}
