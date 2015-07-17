﻿namespace Manager
{
    partial class MainWindow
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
            this.mainTabControl = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.CompetitionListBox = new System.Windows.Forms.ListBox();
            this.RefreshButton = new System.Windows.Forms.Button();
            this.LoadButton = new System.Windows.Forms.Button();
            this.teamGridView = new System.Windows.Forms.DataGridView();
            this.PriceButton = new System.Windows.Forms.Button();
            this.Team = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Strength = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Value = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.mainTabControl.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.teamGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // mainTabControl
            // 
            this.mainTabControl.Controls.Add(this.tabPage1);
            this.mainTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainTabControl.Location = new System.Drawing.Point(0, 0);
            this.mainTabControl.Name = "mainTabControl";
            this.mainTabControl.SelectedIndex = 0;
            this.mainTabControl.Size = new System.Drawing.Size(976, 330);
            this.mainTabControl.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.PriceButton);
            this.tabPage1.Controls.Add(this.teamGridView);
            this.tabPage1.Controls.Add(this.CompetitionListBox);
            this.tabPage1.Controls.Add(this.RefreshButton);
            this.tabPage1.Controls.Add(this.LoadButton);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(968, 304);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Competition";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // CompetitionListBox
            // 
            this.CompetitionListBox.FormattingEnabled = true;
            this.CompetitionListBox.Location = new System.Drawing.Point(8, 45);
            this.CompetitionListBox.Name = "CompetitionListBox";
            this.CompetitionListBox.Size = new System.Drawing.Size(129, 251);
            this.CompetitionListBox.TabIndex = 2;
            // 
            // RefreshButton
            // 
            this.RefreshButton.Location = new System.Drawing.Point(8, 6);
            this.RefreshButton.Name = "RefreshButton";
            this.RefreshButton.Size = new System.Drawing.Size(129, 23);
            this.RefreshButton.TabIndex = 1;
            this.RefreshButton.Text = "Refresh Competition";
            this.RefreshButton.UseVisualStyleBackColor = true;
            // 
            // LoadButton
            // 
            this.LoadButton.Location = new System.Drawing.Point(143, 6);
            this.LoadButton.Name = "LoadButton";
            this.LoadButton.Size = new System.Drawing.Size(133, 23);
            this.LoadButton.TabIndex = 0;
            this.LoadButton.Text = "Load Competition";
            this.LoadButton.UseVisualStyleBackColor = true;
            // 
            // teamGridView
            // 
            this.teamGridView.AllowUserToAddRows = false;
            this.teamGridView.AllowUserToDeleteRows = false;
            this.teamGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.teamGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Team,
            this.Strength,
            this.Value});
            this.teamGridView.Location = new System.Drawing.Point(143, 74);
            this.teamGridView.Name = "teamGridView";
            this.teamGridView.Size = new System.Drawing.Size(817, 228);
            this.teamGridView.TabIndex = 3;
            // 
            // PriceButton
            // 
            this.PriceButton.Location = new System.Drawing.Point(143, 45);
            this.PriceButton.Name = "PriceButton";
            this.PriceButton.Size = new System.Drawing.Size(133, 23);
            this.PriceButton.TabIndex = 4;
            this.PriceButton.Text = "Price";
            this.PriceButton.UseVisualStyleBackColor = true;
            // 
            // Team
            // 
            this.Team.HeaderText = "Team";
            this.Team.Name = "Team";
            // 
            // Strength
            // 
            this.Strength.HeaderText = "Strengh";
            this.Strength.Name = "Strength";
            // 
            // Value
            // 
            this.Value.HeaderText = "Value";
            this.Value.Name = "Value";
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(976, 330);
            this.Controls.Add(this.mainTabControl);
            this.Name = "MainWindow";
            this.Text = "Epic Bet Manager";
            this.mainTabControl.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.teamGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl mainTabControl;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Button LoadButton;
        private System.Windows.Forms.ListBox CompetitionListBox;
        private System.Windows.Forms.Button RefreshButton;
        private System.Windows.Forms.Button PriceButton;
        private System.Windows.Forms.DataGridView teamGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn Team;
        private System.Windows.Forms.DataGridViewTextBoxColumn Strength;
        private System.Windows.Forms.DataGridViewTextBoxColumn Value;
    }
}