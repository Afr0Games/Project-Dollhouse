namespace Iffinator
{
    partial class Form1
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
            this.IFFTabControl = new MaterialSkin.Controls.MaterialTabControl();
            this.FloorsTabPage = new System.Windows.Forms.TabPage();
            this.TxtName = new MaterialSkin.Controls.MaterialMultiLineTextBox();
            this.TxtPrice = new MaterialSkin.Controls.MaterialMultiLineTextBox();
            this.TxtStrings = new MaterialSkin.Controls.MaterialMultiLineTextBox();
            this.LstLanguages = new MaterialSkin.Controls.MaterialListView();
            this.Languages = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Strings = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.LblArchive = new MaterialSkin.Controls.MaterialLabel();
            this.LstFloors = new MaterialSkin.Controls.MaterialListView();
            this.FilenameColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.FloorsTabSelector = new MaterialSkin.Controls.MaterialTabSelector();
            this.WallsTabPage = new System.Windows.Forms.TabPage();
            this.WallsTabSelector = new MaterialSkin.Controls.MaterialTabSelector();
            this.RndFloors = new Iffinator.FloorRenderer();
            this.IFFTabControl.SuspendLayout();
            this.FloorsTabPage.SuspendLayout();
            this.WallsTabPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // IFFTabControl
            // 
            this.IFFTabControl.Controls.Add(this.FloorsTabPage);
            this.IFFTabControl.Controls.Add(this.WallsTabPage);
            this.IFFTabControl.Depth = 0;
            this.IFFTabControl.Location = new System.Drawing.Point(12, 65);
            this.IFFTabControl.MouseState = MaterialSkin.MouseState.HOVER;
            this.IFFTabControl.Multiline = true;
            this.IFFTabControl.Name = "IFFTabControl";
            this.IFFTabControl.SelectedIndex = 0;
            this.IFFTabControl.Size = new System.Drawing.Size(776, 388);
            this.IFFTabControl.TabIndex = 0;
            // 
            // FloorsTabPage
            // 
            this.FloorsTabPage.Controls.Add(this.TxtName);
            this.FloorsTabPage.Controls.Add(this.TxtPrice);
            this.FloorsTabPage.Controls.Add(this.TxtStrings);
            this.FloorsTabPage.Controls.Add(this.LstLanguages);
            this.FloorsTabPage.Controls.Add(this.LblArchive);
            this.FloorsTabPage.Controls.Add(this.RndFloors);
            this.FloorsTabPage.Controls.Add(this.LstFloors);
            this.FloorsTabPage.Controls.Add(this.FloorsTabSelector);
            this.FloorsTabPage.ForeColor = System.Drawing.SystemColors.ControlText;
            this.FloorsTabPage.Location = new System.Drawing.Point(4, 22);
            this.FloorsTabPage.Name = "FloorsTabPage";
            this.FloorsTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.FloorsTabPage.Size = new System.Drawing.Size(768, 362);
            this.FloorsTabPage.TabIndex = 0;
            this.FloorsTabPage.Text = "Floors";
            this.FloorsTabPage.UseVisualStyleBackColor = true;
            // 
            // TxtName
            // 
            this.TxtName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.TxtName.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TxtName.Depth = 0;
            this.TxtName.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.TxtName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.TxtName.Hint = "";
            this.TxtName.Location = new System.Drawing.Point(479, 152);
            this.TxtName.MouseState = MaterialSkin.MouseState.HOVER;
            this.TxtName.Name = "TxtName";
            this.TxtName.Size = new System.Drawing.Size(273, 22);
            this.TxtName.TabIndex = 7;
            this.TxtName.Text = "";
            this.TxtName.Visible = false;
            // 
            // TxtPrice
            // 
            this.TxtPrice.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.TxtPrice.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TxtPrice.Depth = 0;
            this.TxtPrice.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.TxtPrice.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.TxtPrice.Hint = "";
            this.TxtPrice.Location = new System.Drawing.Point(479, 180);
            this.TxtPrice.MouseState = MaterialSkin.MouseState.HOVER;
            this.TxtPrice.Name = "TxtPrice";
            this.TxtPrice.Size = new System.Drawing.Size(100, 22);
            this.TxtPrice.TabIndex = 6;
            this.TxtPrice.Text = "";
            this.TxtPrice.Visible = false;
            // 
            // TxtStrings
            // 
            this.TxtStrings.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.TxtStrings.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TxtStrings.Depth = 0;
            this.TxtStrings.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.TxtStrings.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.TxtStrings.Hint = "";
            this.TxtStrings.Location = new System.Drawing.Point(479, 213);
            this.TxtStrings.MouseState = MaterialSkin.MouseState.HOVER;
            this.TxtStrings.Name = "TxtStrings";
            this.TxtStrings.Size = new System.Drawing.Size(273, 143);
            this.TxtStrings.TabIndex = 5;
            this.TxtStrings.Text = "";
            this.TxtStrings.Visible = false;
            // 
            // LstLanguages
            // 
            this.LstLanguages.AutoSizeTable = false;
            this.LstLanguages.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.LstLanguages.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.LstLanguages.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Languages,
            this.Strings});
            this.LstLanguages.Depth = 0;
            this.LstLanguages.FullRowSelect = true;
            this.LstLanguages.HideSelection = false;
            this.LstLanguages.Location = new System.Drawing.Point(206, 49);
            this.LstLanguages.MinimumSize = new System.Drawing.Size(200, 100);
            this.LstLanguages.MouseLocation = new System.Drawing.Point(-1, -1);
            this.LstLanguages.MouseState = MaterialSkin.MouseState.OUT;
            this.LstLanguages.Name = "LstLanguages";
            this.LstLanguages.OwnerDraw = true;
            this.LstLanguages.Size = new System.Drawing.Size(267, 307);
            this.LstLanguages.TabIndex = 4;
            this.LstLanguages.UseCompatibleStateImageBehavior = false;
            this.LstLanguages.View = System.Windows.Forms.View.Details;
            // 
            // Languages
            // 
            this.Languages.Text = "Languages";
            this.Languages.Width = 185;
            // 
            // Strings
            // 
            this.Strings.Text = "Strings";
            this.Strings.Width = 80;
            // 
            // LblArchive
            // 
            this.LblArchive.AutoSize = true;
            this.LblArchive.Depth = 0;
            this.LblArchive.Font = new System.Drawing.Font("Roboto", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.LblArchive.Location = new System.Drawing.Point(329, 6);
            this.LblArchive.MouseState = MaterialSkin.MouseState.HOVER;
            this.LblArchive.Name = "LblArchive";
            this.LblArchive.Size = new System.Drawing.Size(1, 0);
            this.LblArchive.TabIndex = 3;
            // 
            // LstFloors
            // 
            this.LstFloors.AutoSizeTable = false;
            this.LstFloors.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.LstFloors.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.LstFloors.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.FilenameColumn});
            this.LstFloors.Depth = 0;
            this.LstFloors.FullRowSelect = true;
            this.LstFloors.HideSelection = false;
            this.LstFloors.Location = new System.Drawing.Point(0, 49);
            this.LstFloors.MinimumSize = new System.Drawing.Size(200, 100);
            this.LstFloors.MouseLocation = new System.Drawing.Point(-1, -1);
            this.LstFloors.MouseState = MaterialSkin.MouseState.OUT;
            this.LstFloors.Name = "LstFloors";
            this.LstFloors.OwnerDraw = true;
            this.LstFloors.Size = new System.Drawing.Size(200, 307);
            this.LstFloors.TabIndex = 1;
            this.LstFloors.UseCompatibleStateImageBehavior = false;
            this.LstFloors.View = System.Windows.Forms.View.Details;
            // 
            // FilenameColumn
            // 
            this.FilenameColumn.Text = "Filename";
            this.FilenameColumn.Width = 185;
            // 
            // FloorsTabSelector
            // 
            this.FloorsTabSelector.BaseTabControl = this.IFFTabControl;
            this.FloorsTabSelector.Depth = 0;
            this.FloorsTabSelector.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.FloorsTabSelector.Location = new System.Drawing.Point(0, 0);
            this.FloorsTabSelector.MouseState = MaterialSkin.MouseState.HOVER;
            this.FloorsTabSelector.Name = "FloorsTabSelector";
            this.FloorsTabSelector.Size = new System.Drawing.Size(199, 53);
            this.FloorsTabSelector.TabIndex = 0;
            this.FloorsTabSelector.Text = "Floors";
            // 
            // WallsTabPage
            // 
            this.WallsTabPage.Controls.Add(this.WallsTabSelector);
            this.WallsTabPage.Location = new System.Drawing.Point(4, 22);
            this.WallsTabPage.Name = "WallsTabPage";
            this.WallsTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.WallsTabPage.Size = new System.Drawing.Size(768, 362);
            this.WallsTabPage.TabIndex = 1;
            this.WallsTabPage.Text = "Walls";
            this.WallsTabPage.UseVisualStyleBackColor = true;
            // 
            // WallsTabSelector
            // 
            this.WallsTabSelector.BaseTabControl = this.IFFTabControl;
            this.WallsTabSelector.Depth = 0;
            this.WallsTabSelector.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.WallsTabSelector.Location = new System.Drawing.Point(0, 0);
            this.WallsTabSelector.MouseState = MaterialSkin.MouseState.HOVER;
            this.WallsTabSelector.Name = "WallsTabSelector";
            this.WallsTabSelector.Size = new System.Drawing.Size(199, 53);
            this.WallsTabSelector.TabIndex = 1;
            this.WallsTabSelector.Text = "Floors";
            // 
            // RndFloors
            // 
            this.RndFloors.Location = new System.Drawing.Point(635, 6);
            this.RndFloors.MouseHoverUpdatesOnly = false;
            this.RndFloors.Name = "RndFloors";
            this.RndFloors.Size = new System.Drawing.Size(117, 98);
            this.RndFloors.TabIndex = 2;
            this.RndFloors.Text = "FloorRenderer";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.IFFTabControl);
            this.Name = "Form1";
            this.Text = "Iffinator";
            this.IFFTabControl.ResumeLayout(false);
            this.FloorsTabPage.ResumeLayout(false);
            this.FloorsTabPage.PerformLayout();
            this.WallsTabPage.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private MaterialSkin.Controls.MaterialTabControl IFFTabControl;
        private System.Windows.Forms.TabPage FloorsTabPage;
        private MaterialSkin.Controls.MaterialTabSelector FloorsTabSelector;
        private System.Windows.Forms.TabPage WallsTabPage;
        private MaterialSkin.Controls.MaterialTabSelector WallsTabSelector;
        private MaterialSkin.Controls.MaterialListView LstFloors;
        private System.Windows.Forms.ColumnHeader FilenameColumn;
        private FloorRenderer RndFloors;
        private MaterialSkin.Controls.MaterialLabel LblArchive;
        private MaterialSkin.Controls.MaterialListView LstLanguages;
        private System.Windows.Forms.ColumnHeader Languages;
        private System.Windows.Forms.ColumnHeader Strings;
        private MaterialSkin.Controls.MaterialMultiLineTextBox TxtStrings;
        private MaterialSkin.Controls.MaterialMultiLineTextBox TxtName;
        private MaterialSkin.Controls.MaterialMultiLineTextBox TxtPrice;
    }
}

