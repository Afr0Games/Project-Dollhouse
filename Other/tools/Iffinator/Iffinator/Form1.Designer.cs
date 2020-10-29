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
            this.BtnUpdateText = new MaterialSkin.Controls.MaterialButton();
            this.LblFloorName = new MaterialSkin.Controls.MaterialLabel();
            this.LblFloorPrice = new MaterialSkin.Controls.MaterialLabel();
            this.BtnZoomOut = new MaterialSkin.Controls.MaterialButton();
            this.BtnZoomIn = new MaterialSkin.Controls.MaterialButton();
            this.TxtFloorName = new MaterialSkin.Controls.MaterialMultiLineTextBox();
            this.TxtFloorPrice = new MaterialSkin.Controls.MaterialMultiLineTextBox();
            this.TxtFloorStrings = new MaterialSkin.Controls.MaterialMultiLineTextBox();
            this.LstFloorLanguages = new MaterialSkin.Controls.MaterialListView();
            this.Languages = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Strings = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.LblArchive = new MaterialSkin.Controls.MaterialLabel();
            this.MnuArchive = new MaterialSkin.Controls.MaterialContextMenuStrip();
            this.MnuItemUpdateArchive = new System.Windows.Forms.ToolStripMenuItem();
            this.RndFloors = new Iffinator.FloorRenderer();
            this.LstFloors = new MaterialSkin.Controls.MaterialListView();
            this.FilenameColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.FloorsTabSelector = new MaterialSkin.Controls.MaterialTabSelector();
            this.WallsTabPage = new System.Windows.Forms.TabPage();
            this.TxtWallStrings = new MaterialSkin.Controls.MaterialMultiLineTextBox();
            this.TxtWallName = new MaterialSkin.Controls.MaterialMultiLineTextBox();
            this.TxtWallPrice = new MaterialSkin.Controls.MaterialMultiLineTextBox();
            this.LblWallName = new MaterialSkin.Controls.MaterialLabel();
            this.LblWallPrice = new MaterialSkin.Controls.MaterialLabel();
            this.BtnZoomOutWall = new MaterialSkin.Controls.MaterialButton();
            this.BtnZoomInWall = new MaterialSkin.Controls.MaterialButton();
            this.RndWalls = new Iffinator.WallRenderer();
            this.LstWallLanguages = new MaterialSkin.Controls.MaterialListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.LstWalls = new MaterialSkin.Controls.MaterialListView();
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.WallsTabSelector = new MaterialSkin.Controls.MaterialTabSelector();
            this.IFFTabControl.SuspendLayout();
            this.FloorsTabPage.SuspendLayout();
            this.MnuArchive.SuspendLayout();
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
            this.FloorsTabPage.Controls.Add(this.BtnUpdateText);
            this.FloorsTabPage.Controls.Add(this.LblFloorName);
            this.FloorsTabPage.Controls.Add(this.LblFloorPrice);
            this.FloorsTabPage.Controls.Add(this.BtnZoomOut);
            this.FloorsTabPage.Controls.Add(this.BtnZoomIn);
            this.FloorsTabPage.Controls.Add(this.TxtFloorName);
            this.FloorsTabPage.Controls.Add(this.TxtFloorPrice);
            this.FloorsTabPage.Controls.Add(this.TxtFloorStrings);
            this.FloorsTabPage.Controls.Add(this.LstFloorLanguages);
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
            // BtnUpdateText
            // 
            this.BtnUpdateText.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BtnUpdateText.Depth = 0;
            this.BtnUpdateText.DrawShadows = true;
            this.BtnUpdateText.HighEmphasis = true;
            this.BtnUpdateText.Icon = null;
            this.BtnUpdateText.Location = new System.Drawing.Point(636, 189);
            this.BtnUpdateText.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.BtnUpdateText.MouseState = MaterialSkin.MouseState.HOVER;
            this.BtnUpdateText.Name = "BtnUpdateText";
            this.BtnUpdateText.Size = new System.Drawing.Size(113, 36);
            this.BtnUpdateText.TabIndex = 10;
            this.BtnUpdateText.Text = "Update text";
            this.BtnUpdateText.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.BtnUpdateText.UseAccentColor = false;
            this.BtnUpdateText.UseVisualStyleBackColor = true;
            this.BtnUpdateText.Visible = false;
            // 
            // LblFloorName
            // 
            this.LblFloorName.AutoSize = true;
            this.LblFloorName.Depth = 0;
            this.LblFloorName.Font = new System.Drawing.Font("Roboto", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.LblFloorName.Location = new System.Drawing.Point(486, 136);
            this.LblFloorName.MouseState = MaterialSkin.MouseState.HOVER;
            this.LblFloorName.Name = "LblFloorName";
            this.LblFloorName.Size = new System.Drawing.Size(43, 19);
            this.LblFloorName.TabIndex = 9;
            this.LblFloorName.Text = "Name";
            this.LblFloorName.Visible = false;
            // 
            // LblFloorPrice
            // 
            this.LblFloorPrice.AutoSize = true;
            this.LblFloorPrice.Depth = 0;
            this.LblFloorPrice.Font = new System.Drawing.Font("Roboto", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.LblFloorPrice.Location = new System.Drawing.Point(486, 190);
            this.LblFloorPrice.MouseState = MaterialSkin.MouseState.HOVER;
            this.LblFloorPrice.Name = "LblFloorPrice";
            this.LblFloorPrice.Size = new System.Drawing.Size(36, 19);
            this.LblFloorPrice.TabIndex = 9;
            this.LblFloorPrice.Text = "Price";
            this.LblFloorPrice.Visible = false;
            // 
            // BtnZoomOut
            // 
            this.BtnZoomOut.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BtnZoomOut.Depth = 0;
            this.BtnZoomOut.DrawShadows = true;
            this.BtnZoomOut.HighEmphasis = true;
            this.BtnZoomOut.Icon = null;
            this.BtnZoomOut.Location = new System.Drawing.Point(673, 113);
            this.BtnZoomOut.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.BtnZoomOut.MouseState = MaterialSkin.MouseState.HOVER;
            this.BtnZoomOut.Name = "BtnZoomOut";
            this.BtnZoomOut.Size = new System.Drawing.Size(25, 36);
            this.BtnZoomOut.TabIndex = 8;
            this.BtnZoomOut.Text = "-";
            this.BtnZoomOut.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.BtnZoomOut.UseAccentColor = false;
            this.BtnZoomOut.UseVisualStyleBackColor = true;
            this.BtnZoomOut.Visible = false;
            this.BtnZoomOut.Click += new System.EventHandler(this.BtnZoomOut_Click);
            // 
            // BtnZoomIn
            // 
            this.BtnZoomIn.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BtnZoomIn.Depth = 0;
            this.BtnZoomIn.DrawShadows = true;
            this.BtnZoomIn.HighEmphasis = true;
            this.BtnZoomIn.Icon = null;
            this.BtnZoomIn.Location = new System.Drawing.Point(635, 113);
            this.BtnZoomIn.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.BtnZoomIn.MouseState = MaterialSkin.MouseState.HOVER;
            this.BtnZoomIn.Name = "BtnZoomIn";
            this.BtnZoomIn.Size = new System.Drawing.Size(29, 36);
            this.BtnZoomIn.TabIndex = 8;
            this.BtnZoomIn.Text = "+";
            this.BtnZoomIn.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.BtnZoomIn.UseAccentColor = false;
            this.BtnZoomIn.UseVisualStyleBackColor = true;
            this.BtnZoomIn.Visible = false;
            this.BtnZoomIn.Click += new System.EventHandler(this.BtnZoomIn_Click);
            // 
            // TxtFloorName
            // 
            this.TxtFloorName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.TxtFloorName.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TxtFloorName.Depth = 0;
            this.TxtFloorName.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.TxtFloorName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.TxtFloorName.Hint = "";
            this.TxtFloorName.Location = new System.Drawing.Point(489, 158);
            this.TxtFloorName.MouseState = MaterialSkin.MouseState.HOVER;
            this.TxtFloorName.Name = "TxtFloorName";
            this.TxtFloorName.Size = new System.Drawing.Size(273, 22);
            this.TxtFloorName.TabIndex = 7;
            this.TxtFloorName.Text = "";
            this.TxtFloorName.Visible = false;
            // 
            // TxtFloorPrice
            // 
            this.TxtFloorPrice.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.TxtFloorPrice.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TxtFloorPrice.Depth = 0;
            this.TxtFloorPrice.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.TxtFloorPrice.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.TxtFloorPrice.Hint = "";
            this.TxtFloorPrice.Location = new System.Drawing.Point(489, 212);
            this.TxtFloorPrice.MouseState = MaterialSkin.MouseState.HOVER;
            this.TxtFloorPrice.Name = "TxtFloorPrice";
            this.TxtFloorPrice.Size = new System.Drawing.Size(100, 22);
            this.TxtFloorPrice.TabIndex = 6;
            this.TxtFloorPrice.Text = "";
            this.TxtFloorPrice.Visible = false;
            // 
            // TxtFloorStrings
            // 
            this.TxtFloorStrings.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.TxtFloorStrings.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TxtFloorStrings.Depth = 0;
            this.TxtFloorStrings.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.TxtFloorStrings.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.TxtFloorStrings.Hint = "";
            this.TxtFloorStrings.Location = new System.Drawing.Point(489, 240);
            this.TxtFloorStrings.MouseState = MaterialSkin.MouseState.HOVER;
            this.TxtFloorStrings.Name = "TxtFloorStrings";
            this.TxtFloorStrings.Size = new System.Drawing.Size(273, 140);
            this.TxtFloorStrings.TabIndex = 5;
            this.TxtFloorStrings.Text = "";
            this.TxtFloorStrings.Visible = false;
            // 
            // LstFloorLanguages
            // 
            this.LstFloorLanguages.AutoSizeTable = false;
            this.LstFloorLanguages.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.LstFloorLanguages.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.LstFloorLanguages.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Languages,
            this.Strings});
            this.LstFloorLanguages.Depth = 0;
            this.LstFloorLanguages.FullRowSelect = true;
            this.LstFloorLanguages.HideSelection = false;
            this.LstFloorLanguages.Location = new System.Drawing.Point(206, 49);
            this.LstFloorLanguages.MinimumSize = new System.Drawing.Size(200, 100);
            this.LstFloorLanguages.MouseLocation = new System.Drawing.Point(-1, -1);
            this.LstFloorLanguages.MouseState = MaterialSkin.MouseState.OUT;
            this.LstFloorLanguages.Name = "LstFloorLanguages";
            this.LstFloorLanguages.OwnerDraw = true;
            this.LstFloorLanguages.Size = new System.Drawing.Size(267, 331);
            this.LstFloorLanguages.TabIndex = 4;
            this.LstFloorLanguages.UseCompatibleStateImageBehavior = false;
            this.LstFloorLanguages.View = System.Windows.Forms.View.Details;
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
            this.LblArchive.ContextMenuStrip = this.MnuArchive;
            this.LblArchive.Depth = 0;
            this.LblArchive.Font = new System.Drawing.Font("Roboto", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.LblArchive.Location = new System.Drawing.Point(329, 6);
            this.LblArchive.MouseState = MaterialSkin.MouseState.HOVER;
            this.LblArchive.Name = "LblArchive";
            this.LblArchive.Size = new System.Drawing.Size(1, 0);
            this.LblArchive.TabIndex = 3;
            // 
            // MnuArchive
            // 
            this.MnuArchive.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(242)))), ((int)(((byte)(242)))));
            this.MnuArchive.Depth = 0;
            this.MnuArchive.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MnuItemUpdateArchive});
            this.MnuArchive.MouseState = MaterialSkin.MouseState.HOVER;
            this.MnuArchive.Name = "MnuArchive";
            this.MnuArchive.Size = new System.Drawing.Size(154, 26);
            // 
            // MnuItemUpdateArchive
            // 
            this.MnuItemUpdateArchive.Enabled = false;
            this.MnuItemUpdateArchive.Name = "MnuItemUpdateArchive";
            this.MnuItemUpdateArchive.Size = new System.Drawing.Size(153, 22);
            this.MnuItemUpdateArchive.Text = "Update archive";
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
            this.LstFloors.Size = new System.Drawing.Size(200, 331);
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
            this.FloorsTabSelector.Font = new System.Drawing.Font("Roboto", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.FloorsTabSelector.Location = new System.Drawing.Point(0, 0);
            this.FloorsTabSelector.MouseState = MaterialSkin.MouseState.HOVER;
            this.FloorsTabSelector.Name = "FloorsTabSelector";
            this.FloorsTabSelector.Size = new System.Drawing.Size(199, 53);
            this.FloorsTabSelector.TabIndex = 0;
            this.FloorsTabSelector.Text = "Floors";
            // 
            // WallsTabPage
            // 
            this.WallsTabPage.Controls.Add(this.TxtWallStrings);
            this.WallsTabPage.Controls.Add(this.TxtWallName);
            this.WallsTabPage.Controls.Add(this.TxtWallPrice);
            this.WallsTabPage.Controls.Add(this.LblWallName);
            this.WallsTabPage.Controls.Add(this.LblWallPrice);
            this.WallsTabPage.Controls.Add(this.BtnZoomOutWall);
            this.WallsTabPage.Controls.Add(this.BtnZoomInWall);
            this.WallsTabPage.Controls.Add(this.RndWalls);
            this.WallsTabPage.Controls.Add(this.LstWallLanguages);
            this.WallsTabPage.Controls.Add(this.LstWalls);
            this.WallsTabPage.Controls.Add(this.WallsTabSelector);
            this.WallsTabPage.Location = new System.Drawing.Point(4, 22);
            this.WallsTabPage.Name = "WallsTabPage";
            this.WallsTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.WallsTabPage.Size = new System.Drawing.Size(768, 362);
            this.WallsTabPage.TabIndex = 1;
            this.WallsTabPage.Text = "Walls";
            this.WallsTabPage.UseVisualStyleBackColor = true;
            // 
            // TxtWallStrings
            // 
            this.TxtWallStrings.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.TxtWallStrings.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TxtWallStrings.Depth = 0;
            this.TxtWallStrings.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.TxtWallStrings.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.TxtWallStrings.Hint = "";
            this.TxtWallStrings.Location = new System.Drawing.Point(477, 240);
            this.TxtWallStrings.MouseState = MaterialSkin.MouseState.HOVER;
            this.TxtWallStrings.Name = "TxtWallStrings";
            this.TxtWallStrings.Size = new System.Drawing.Size(273, 140);
            this.TxtWallStrings.TabIndex = 15;
            this.TxtWallStrings.Text = "";
            this.TxtWallStrings.Visible = false;
            // 
            // TxtWallName
            // 
            this.TxtWallName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.TxtWallName.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TxtWallName.Depth = 0;
            this.TxtWallName.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.TxtWallName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.TxtWallName.Hint = "";
            this.TxtWallName.Location = new System.Drawing.Point(477, 156);
            this.TxtWallName.MouseState = MaterialSkin.MouseState.HOVER;
            this.TxtWallName.Name = "TxtWallName";
            this.TxtWallName.Size = new System.Drawing.Size(273, 22);
            this.TxtWallName.TabIndex = 14;
            this.TxtWallName.Text = "";
            this.TxtWallName.Visible = false;
            // 
            // TxtWallPrice
            // 
            this.TxtWallPrice.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.TxtWallPrice.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TxtWallPrice.Depth = 0;
            this.TxtWallPrice.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.TxtWallPrice.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.TxtWallPrice.Hint = "";
            this.TxtWallPrice.Location = new System.Drawing.Point(477, 210);
            this.TxtWallPrice.MouseState = MaterialSkin.MouseState.HOVER;
            this.TxtWallPrice.Name = "TxtWallPrice";
            this.TxtWallPrice.Size = new System.Drawing.Size(100, 22);
            this.TxtWallPrice.TabIndex = 13;
            this.TxtWallPrice.Text = "";
            this.TxtWallPrice.Visible = false;
            // 
            // LblWallName
            // 
            this.LblWallName.AutoSize = true;
            this.LblWallName.Depth = 0;
            this.LblWallName.Font = new System.Drawing.Font("Roboto", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.LblWallName.Location = new System.Drawing.Point(478, 134);
            this.LblWallName.MouseState = MaterialSkin.MouseState.HOVER;
            this.LblWallName.Name = "LblWallName";
            this.LblWallName.Size = new System.Drawing.Size(43, 19);
            this.LblWallName.TabIndex = 11;
            this.LblWallName.Text = "Name";
            this.LblWallName.Visible = false;
            // 
            // LblWallPrice
            // 
            this.LblWallPrice.AutoSize = true;
            this.LblWallPrice.Depth = 0;
            this.LblWallPrice.Font = new System.Drawing.Font("Roboto", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.LblWallPrice.Location = new System.Drawing.Point(478, 188);
            this.LblWallPrice.MouseState = MaterialSkin.MouseState.HOVER;
            this.LblWallPrice.Name = "LblWallPrice";
            this.LblWallPrice.Size = new System.Drawing.Size(36, 19);
            this.LblWallPrice.TabIndex = 12;
            this.LblWallPrice.Text = "Price";
            this.LblWallPrice.Visible = false;
            // 
            // BtnZoomOutWall
            // 
            this.BtnZoomOutWall.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BtnZoomOutWall.Depth = 0;
            this.BtnZoomOutWall.DrawShadows = true;
            this.BtnZoomOutWall.HighEmphasis = true;
            this.BtnZoomOutWall.Icon = null;
            this.BtnZoomOutWall.Location = new System.Drawing.Point(667, 117);
            this.BtnZoomOutWall.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.BtnZoomOutWall.MouseState = MaterialSkin.MouseState.HOVER;
            this.BtnZoomOutWall.Name = "BtnZoomOutWall";
            this.BtnZoomOutWall.Size = new System.Drawing.Size(25, 36);
            this.BtnZoomOutWall.TabIndex = 9;
            this.BtnZoomOutWall.Text = "-";
            this.BtnZoomOutWall.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.BtnZoomOutWall.UseAccentColor = false;
            this.BtnZoomOutWall.UseVisualStyleBackColor = true;
            this.BtnZoomOutWall.Visible = false;
            this.BtnZoomOutWall.Click += new System.EventHandler(this.BtnZoomOutWall_Click);
            // 
            // BtnZoomInWall
            // 
            this.BtnZoomInWall.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BtnZoomInWall.Depth = 0;
            this.BtnZoomInWall.DrawShadows = true;
            this.BtnZoomInWall.HighEmphasis = true;
            this.BtnZoomInWall.Icon = null;
            this.BtnZoomInWall.Location = new System.Drawing.Point(629, 117);
            this.BtnZoomInWall.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.BtnZoomInWall.MouseState = MaterialSkin.MouseState.HOVER;
            this.BtnZoomInWall.Name = "BtnZoomInWall";
            this.BtnZoomInWall.Size = new System.Drawing.Size(29, 36);
            this.BtnZoomInWall.TabIndex = 10;
            this.BtnZoomInWall.Text = "+";
            this.BtnZoomInWall.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.BtnZoomInWall.UseAccentColor = false;
            this.BtnZoomInWall.UseVisualStyleBackColor = true;
            this.BtnZoomInWall.Visible = false;
            this.BtnZoomInWall.Click += new System.EventHandler(this.BtnZoomInWall_Click);
            // 
            // RndWalls
            // 
            this.RndWalls.Location = new System.Drawing.Point(629, 6);
            this.RndWalls.MouseHoverUpdatesOnly = false;
            this.RndWalls.Name = "RndWalls";
            this.RndWalls.Size = new System.Drawing.Size(121, 102);
            this.RndWalls.TabIndex = 7;
            // 
            // LstWallLanguages
            // 
            this.LstWallLanguages.AutoSizeTable = false;
            this.LstWallLanguages.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.LstWallLanguages.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.LstWallLanguages.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.LstWallLanguages.Depth = 0;
            this.LstWallLanguages.FullRowSelect = true;
            this.LstWallLanguages.HideSelection = false;
            this.LstWallLanguages.Location = new System.Drawing.Point(205, 49);
            this.LstWallLanguages.MinimumSize = new System.Drawing.Size(200, 100);
            this.LstWallLanguages.MouseLocation = new System.Drawing.Point(-1, -1);
            this.LstWallLanguages.MouseState = MaterialSkin.MouseState.OUT;
            this.LstWallLanguages.Name = "LstWallLanguages";
            this.LstWallLanguages.OwnerDraw = true;
            this.LstWallLanguages.Size = new System.Drawing.Size(267, 331);
            this.LstWallLanguages.TabIndex = 6;
            this.LstWallLanguages.UseCompatibleStateImageBehavior = false;
            this.LstWallLanguages.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Languages";
            this.columnHeader1.Width = 185;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Strings";
            this.columnHeader2.Width = 80;
            // 
            // LstWalls
            // 
            this.LstWalls.AutoSizeTable = false;
            this.LstWalls.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.LstWalls.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.LstWalls.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3});
            this.LstWalls.Depth = 0;
            this.LstWalls.FullRowSelect = true;
            this.LstWalls.HideSelection = false;
            this.LstWalls.Location = new System.Drawing.Point(-1, 49);
            this.LstWalls.MinimumSize = new System.Drawing.Size(200, 100);
            this.LstWalls.MouseLocation = new System.Drawing.Point(-1, -1);
            this.LstWalls.MouseState = MaterialSkin.MouseState.OUT;
            this.LstWalls.Name = "LstWalls";
            this.LstWalls.OwnerDraw = true;
            this.LstWalls.Size = new System.Drawing.Size(200, 331);
            this.LstWalls.TabIndex = 5;
            this.LstWalls.UseCompatibleStateImageBehavior = false;
            this.LstWalls.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Filename";
            this.columnHeader3.Width = 185;
            // 
            // WallsTabSelector
            // 
            this.WallsTabSelector.BaseTabControl = this.IFFTabControl;
            this.WallsTabSelector.Depth = 0;
            this.WallsTabSelector.Font = new System.Drawing.Font("Roboto", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.WallsTabSelector.Location = new System.Drawing.Point(0, 0);
            this.WallsTabSelector.MouseState = MaterialSkin.MouseState.HOVER;
            this.WallsTabSelector.Name = "WallsTabSelector";
            this.WallsTabSelector.Size = new System.Drawing.Size(199, 53);
            this.WallsTabSelector.TabIndex = 1;
            this.WallsTabSelector.Text = "Floors";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.ClientSize = new System.Drawing.Size(800, 479);
            this.Controls.Add(this.IFFTabControl);
            this.Name = "Form1";
            this.Text = "Iffinator";
            this.IFFTabControl.ResumeLayout(false);
            this.FloorsTabPage.ResumeLayout(false);
            this.FloorsTabPage.PerformLayout();
            this.MnuArchive.ResumeLayout(false);
            this.WallsTabPage.ResumeLayout(false);
            this.WallsTabPage.PerformLayout();
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
        private MaterialSkin.Controls.MaterialListView LstFloorLanguages;
        private System.Windows.Forms.ColumnHeader Languages;
        private System.Windows.Forms.ColumnHeader Strings;
        private MaterialSkin.Controls.MaterialMultiLineTextBox TxtFloorStrings;
        private MaterialSkin.Controls.MaterialMultiLineTextBox TxtFloorName;
        private MaterialSkin.Controls.MaterialMultiLineTextBox TxtFloorPrice;
        private MaterialSkin.Controls.MaterialButton BtnZoomIn;
        private MaterialSkin.Controls.MaterialButton BtnZoomOut;
        private MaterialSkin.Controls.MaterialLabel LblFloorPrice;
        private MaterialSkin.Controls.MaterialLabel LblFloorName;
        private MaterialSkin.Controls.MaterialContextMenuStrip MnuArchive;
        private System.Windows.Forms.ToolStripMenuItem MnuItemUpdateArchive;
        private MaterialSkin.Controls.MaterialButton BtnUpdateText;
        private MaterialSkin.Controls.MaterialListView LstWallLanguages;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private MaterialSkin.Controls.MaterialListView LstWalls;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private WallRenderer RndWalls;
        private MaterialSkin.Controls.MaterialButton BtnZoomOutWall;
        private MaterialSkin.Controls.MaterialButton BtnZoomInWall;
        private MaterialSkin.Controls.MaterialLabel LblWallName;
        private MaterialSkin.Controls.MaterialLabel LblWallPrice;
        private MaterialSkin.Controls.MaterialMultiLineTextBox TxtWallName;
        private MaterialSkin.Controls.MaterialMultiLineTextBox TxtWallPrice;
        private MaterialSkin.Controls.MaterialMultiLineTextBox TxtWallStrings;
    }
}

