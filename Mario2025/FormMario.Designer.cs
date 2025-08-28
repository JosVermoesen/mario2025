﻿namespace MarioApp
{
    partial class FormMario
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.MenuStripFormMario = new System.Windows.Forms.MenuStrip();
            this.ActionsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItemPeppolActions = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItemUserSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItemCloseApp = new System.Windows.Forms.ToolStripMenuItem();
            this.VpeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItemAutoPageBreak = new System.Windows.Forms.ToolStripMenuItem();
            this.AdminMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItemPeppolTesting = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItemMdvToSql = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItemPeppolSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.InfoMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStripFormMario.SuspendLayout();
            this.SuspendLayout();
            // 
            // MenuStripFormMario
            // 
            this.MenuStripFormMario.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ActionsMenuItem,
            this.VpeMenuItem,
            this.AdminMenuItem,
            this.InfoMenuItem});
            this.MenuStripFormMario.Location = new System.Drawing.Point(0, 0);
            this.MenuStripFormMario.Name = "MenuStripFormMario";
            this.MenuStripFormMario.Size = new System.Drawing.Size(800, 24);
            this.MenuStripFormMario.TabIndex = 1;
            this.MenuStripFormMario.Text = "menuStrip1";
            // 
            // ActionsMenuItem
            // 
            this.ActionsMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuItemPeppolActions,
            this.MenuItemUserSettings,
            this.MenuItemCloseApp});
            this.ActionsMenuItem.Name = "ActionsMenuItem";
            this.ActionsMenuItem.Size = new System.Drawing.Size(51, 20);
            this.ActionsMenuItem.Text = "Acties";
            // 
            // MenuItemPeppolActions
            // 
            this.MenuItemPeppolActions.Name = "MenuItemPeppolActions";
            this.MenuItemPeppolActions.Size = new System.Drawing.Size(203, 22);
            this.MenuItemPeppolActions.Text = "Peppol Acties";
            this.MenuItemPeppolActions.Click += new System.EventHandler(this.MenuItemPeppolActions_Click);
            // 
            // MenuItemUserSettings
            // 
            this.MenuItemUserSettings.Name = "MenuItemUserSettings";
            this.MenuItemUserSettings.Size = new System.Drawing.Size(203, 22);
            this.MenuItemUserSettings.Text = "Actief Bedrijf Aanduiden";
            this.MenuItemUserSettings.Click += new System.EventHandler(this.MenuItemUserSettings_Click);
            // 
            // MenuItemCloseApp
            // 
            this.MenuItemCloseApp.Name = "MenuItemCloseApp";
            this.MenuItemCloseApp.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.MenuItemCloseApp.Size = new System.Drawing.Size(203, 22);
            this.MenuItemCloseApp.Text = "Afsluiten";
            this.MenuItemCloseApp.Click += new System.EventHandler(this.MenuItemCloseApp_Click);
            // 
            // VpeMenuItem
            // 
            this.VpeMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuItemAutoPageBreak});
            this.VpeMenuItem.Name = "VpeMenuItem";
            this.VpeMenuItem.Size = new System.Drawing.Size(39, 20);
            this.VpeMenuItem.Text = "VPE";
            this.VpeMenuItem.Visible = false;
            // 
            // MenuItemAutoPageBreak
            // 
            this.MenuItemAutoPageBreak.Name = "MenuItemAutoPageBreak";
            this.MenuItemAutoPageBreak.Size = new System.Drawing.Size(179, 22);
            this.MenuItemAutoPageBreak.Text = "AutoPagebreak Test";
            // 
            // AdminMenuItem
            // 
            this.AdminMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuItemPeppolTesting,
            this.MenuItemMdvToSql,
            this.MenuItemPeppolSettings});
            this.AdminMenuItem.Name = "AdminMenuItem";
            this.AdminMenuItem.Size = new System.Drawing.Size(55, 20);
            this.AdminMenuItem.Text = "Admin";
            // 
            // MenuItemPeppolTesting
            // 
            this.MenuItemPeppolTesting.Name = "MenuItemPeppolTesting";
            this.MenuItemPeppolTesting.Size = new System.Drawing.Size(180, 22);
            this.MenuItemPeppolTesting.Text = "Peppol Acties";
            this.MenuItemPeppolTesting.Click += new System.EventHandler(this.MenuItemPeppolTesting_Click);
            // 
            // MenuItemMdvToSql
            // 
            this.MenuItemMdvToSql.Name = "MenuItemMdvToSql";
            this.MenuItemMdvToSql.Size = new System.Drawing.Size(180, 22);
            this.MenuItemMdvToSql.Text = "MdvToSql Sync";
            this.MenuItemMdvToSql.Click += new System.EventHandler(this.MenuItemMdvToSql_Click);
            // 
            // MenuItemPeppolSettings
            // 
            this.MenuItemPeppolSettings.Name = "MenuItemPeppolSettings";
            this.MenuItemPeppolSettings.Size = new System.Drawing.Size(180, 22);
            this.MenuItemPeppolSettings.Text = "Peppol Instellingen";
            this.MenuItemPeppolSettings.Click += new System.EventHandler(this.MenuItemPeppolSettings_Click);
            // 
            // InfoMenuItem
            // 
            this.InfoMenuItem.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.InfoMenuItem.Name = "InfoMenuItem";
            this.InfoMenuItem.Size = new System.Drawing.Size(24, 20);
            this.InfoMenuItem.Text = "?";
            this.InfoMenuItem.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // FormMario
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.MenuStripFormMario);
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.MenuStripFormMario;
            this.Name = "FormMario";
            this.Text = "FormMario";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMario_FormClosing);
            this.Load += new System.EventHandler(this.FormMario_Load);
            this.Shown += new System.EventHandler(this.FormMario_Shown);
            this.MenuStripFormMario.ResumeLayout(false);
            this.MenuStripFormMario.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip MenuStripFormMario;
        private System.Windows.Forms.ToolStripMenuItem ActionsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem MenuItemPeppolActions;
        private System.Windows.Forms.ToolStripMenuItem MenuItemUserSettings;
        private System.Windows.Forms.ToolStripMenuItem MenuItemCloseApp;
        private System.Windows.Forms.ToolStripMenuItem VpeMenuItem;
        private System.Windows.Forms.ToolStripMenuItem AdminMenuItem;
        private System.Windows.Forms.ToolStripMenuItem MenuItemPeppolTesting;
        private System.Windows.Forms.ToolStripMenuItem MenuItemMdvToSql;
        private System.Windows.Forms.ToolStripMenuItem MenuItemPeppolSettings;
        private System.Windows.Forms.ToolStripMenuItem InfoMenuItem;
        private System.Windows.Forms.ToolStripMenuItem MenuItemAutoPageBreak;
    }
}