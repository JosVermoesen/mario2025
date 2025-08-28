﻿using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace MarioApp.MarioMenu.Actions
{
    public partial class FormChooseCompany : Form
    {
        public FormChooseCompany()
        {
            InitializeComponent();
            LabelMimDataLocation.Text = SharedGlobals.MimDataLocation;
            Text = "Mario2025 - Kies een bedrijf";
            if (LabelMimDataLocation.Text.Length > 0)
            {
                FillCompanyList();
            }
        }

        private void CheckBoxIsAdmin_CheckedChanged(object sender, System.EventArgs e)
        {
            TextBoxIsAdminPassword.Visible = CheckBoxIsAdmin.Checked;
            ButtonValidate.Visible = CheckBoxIsAdmin.Checked;

        }
        private void ButtonValidate_Click(object sender, System.EventArgs e)
        {
            if (TextBoxIsAdminPassword.Text == "Thequickbrownfoxjumpsoverthelazydog123!")
            {
                SharedGlobals.IsAdmin = true;
                Properties.Settings.Default.IsAdmin = true;
                Properties.Settings.Default.Save();
                MessageBox.Show("Admin rechten verkregen.\nHerstart het programma om de admin rechten beschikbaar te maken.");
                TextBoxIsAdminPassword.Visible = false;
                ButtonValidate.Visible = false;
                CheckBoxIsAdmin.Checked = false;
            }
            else
            {
                Properties.Settings.Default.IsAdmin = false;
                Properties.Settings.Default.Save();
                SharedGlobals.IsAdmin = false;
                MessageBox.Show("Foutief paswoord");
            }
            TextBoxIsAdminPassword.Text = "";

        }
        private void ButtonClose_Click(object sender, System.EventArgs e)
        {
            Close();
        }
        private void ListBoxCompanies_Click(object sender, System.EventArgs e)
        {
            if (ListBoxCompanies.SelectedItem != null)
            {
                string selectedItem = ListBoxCompanies.SelectedItem.ToString();
                int separatorPos = selectedItem.IndexOf(" - ");
                if (separatorPos > 0)
                {
                    // Replace range operator with Substring method to fix CS8370, CS0518 errors
                    string selectedCompany = selectedItem.Substring(0, separatorPos);
                    MarHelpers.SetCompanyGlobals(selectedCompany);

                    try
                    {
                        string peppolOutDirectoryPath = SharedGlobals.MimDataLocation + "\\" + selectedCompany + "\\peppol\\out";
                        SharedGlobals.PeppolOutFiles = Directory.GetFiles(peppolOutDirectoryPath, "*.xml").Length;
                        Close();
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Het lijkt alsof dit bedrijf belangrijke inhoudsopgaves ontbreekt\nOpen dit bedrijf eerst met marIntegraal 11.3.026 (of hoger)");
                        MarHelpers.ResetCompanyGlobals();
                    }
                }
            }
        }

        private void FillCompanyList()
        {
            ListBoxCompanies.Items.Clear();

            string MyPath = LabelMimDataLocation.Text;
            ResultExploringFileSystem(MyPath);
        }
        private bool ResultExploringFileSystem(string path)
        {
            if (path == "" || path == null)
            {
                return false;
            }

            if (Directory.Exists(path))
            {
                ProcessDirectory(path, "companyFolder");
                return true;
            }
            else
            {
                MessageBox.Show(path + " is not a valid directory");
                return false;
            }
        }
        private void ProcessDirectory(string targetDirectory, string listType )
        {
            string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
            foreach (string subdirectory in subdirectoryEntries)
            {
                if (listType == "companyFolder")
                {
                    int subDirLength = subdirectory.Length;
                    string subDirMap = subdirectory.Substring(subDirLength - 3);
                    ProcessMarntTxtFile(subdirectory, subDirMap);
                } else
                if (listType == "peppolOut")
                {

                    
                }


            }
        }
        private void ProcessMarntTxtFile(string companyPath, string mapName)
        {
            string marntTxtPath = companyPath + @"\marnt.txt";
            if (File.Exists(marntTxtPath))
            {
                AddToMimList(marntTxtPath, mapName);
            }
            else
            {
                MessageBox.Show(marntTxtPath + " is not a valid file");
            }
        }
        private void AddToMimList(string marntTxtPath, string mapName)
        {
            // TODO add content to list view
            try
            {
                var stream = new StreamReader(marntTxtPath);
                if (stream.Peek() > -1)    // not EOF
                {
                    string line = stream.ReadLine();
                    ListBoxCompanies.Items.Add(mapName + " - " + line);
                }
                stream.Close();
            }
            catch (IOException e)
            {
                MessageBox.Show(marntTxtPath + " could not be read");
                MessageBox.Show(e.Message);
            }
        }
    }
}
