namespace XNAContentCompiler
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
            this.label1 = new System.Windows.Forms.Label();
            this.cboTipoArquivo = new System.Windows.Forms.ComboBox();
            this.txtFileName = new System.Windows.Forms.TextBox();
            this.btAbrirArquivo = new System.Windows.Forms.Button();
            this.lstArquivos = new System.Windows.Forms.ListBox();
            this.btOutput = new System.Windows.Forms.Button();
            this.txtOutput = new System.Windows.Forms.TextBox();
            this.btRemove = new System.Windows.Forms.Button();
            this.btAdd = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btCompile = new System.Windows.Forms.Button();
            this.picCompiling = new System.Windows.Forms.PictureBox();
            this.label4 = new System.Windows.Forms.Label();
            this.pnPrincipal = new System.Windows.Forms.Panel();
            this.pnCompiling = new System.Windows.Forms.Panel();
            this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.btClear = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.picCompiling)).BeginInit();
            this.pnPrincipal.SuspendLayout();
            this.pnCompiling.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(227, 7);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "File Type:";
            // 
            // cboTipoArquivo
            // 
            this.cboTipoArquivo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTipoArquivo.FormattingEnabled = true;
            this.cboTipoArquivo.Location = new System.Drawing.Point(309, 4);
            this.cboTipoArquivo.Margin = new System.Windows.Forms.Padding(4);
            this.cboTipoArquivo.Name = "cboTipoArquivo";
            this.cboTipoArquivo.Size = new System.Drawing.Size(202, 24);
            this.cboTipoArquivo.TabIndex = 1;
            // 
            // txtFileName
            // 
            this.txtFileName.Location = new System.Drawing.Point(49, 35);
            this.txtFileName.Name = "txtFileName";
            this.txtFileName.Size = new System.Drawing.Size(419, 23);
            this.txtFileName.TabIndex = 2;
            // 
            // btAbrirArquivo
            // 
            this.btAbrirArquivo.Location = new System.Drawing.Point(474, 35);
            this.btAbrirArquivo.Name = "btAbrirArquivo";
            this.btAbrirArquivo.Size = new System.Drawing.Size(37, 23);
            this.btAbrirArquivo.TabIndex = 3;
            this.btAbrirArquivo.Text = "...";
            this.btAbrirArquivo.UseVisualStyleBackColor = true;
            this.btAbrirArquivo.Click += new System.EventHandler(this.btAbrirArquivo_Click);
            // 
            // lstArquivos
            // 
            this.lstArquivos.FormattingEnabled = true;
            this.lstArquivos.ItemHeight = 16;
            this.lstArquivos.Location = new System.Drawing.Point(9, 64);
            this.lstArquivos.Name = "lstArquivos";
            this.lstArquivos.Size = new System.Drawing.Size(350, 180);
            this.lstArquivos.TabIndex = 4;
            // 
            // btOutput
            // 
            this.btOutput.Location = new System.Drawing.Point(474, 250);
            this.btOutput.Name = "btOutput";
            this.btOutput.Size = new System.Drawing.Size(37, 23);
            this.btOutput.TabIndex = 6;
            this.btOutput.Text = "...";
            this.btOutput.UseVisualStyleBackColor = true;
            this.btOutput.Click += new System.EventHandler(this.btOutput_Click);
            // 
            // txtOutput
            // 
            this.txtOutput.Location = new System.Drawing.Point(66, 250);
            this.txtOutput.Name = "txtOutput";
            this.txtOutput.Size = new System.Drawing.Size(402, 23);
            this.txtOutput.TabIndex = 5;
            // 
            // btRemove
            // 
            this.btRemove.Location = new System.Drawing.Point(365, 93);
            this.btRemove.Name = "btRemove";
            this.btRemove.Size = new System.Drawing.Size(103, 24);
            this.btRemove.TabIndex = 7;
            this.btRemove.Text = "Remove";
            this.btRemove.UseVisualStyleBackColor = true;
            this.btRemove.Click += new System.EventHandler(this.btRemove_Click);
            // 
            // btAdd
            // 
            this.btAdd.Location = new System.Drawing.Point(365, 64);
            this.btAdd.Name = "btAdd";
            this.btAdd.Size = new System.Drawing.Size(103, 24);
            this.btAdd.TabIndex = 8;
            this.btAdd.Text = "Add";
            this.btAdd.UseVisualStyleBackColor = true;
            this.btAdd.Click += new System.EventHandler(this.btAdd_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 38);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(36, 16);
            this.label2.TabIndex = 9;
            this.label2.Text = "File:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 253);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 16);
            this.label3.TabIndex = 10;
            this.label3.Text = "Output:";
            // 
            // btCompile
            // 
            this.btCompile.Location = new System.Drawing.Point(365, 279);
            this.btCompile.Name = "btCompile";
            this.btCompile.Size = new System.Drawing.Size(103, 24);
            this.btCompile.TabIndex = 11;
            this.btCompile.Text = "Compile";
            this.btCompile.UseVisualStyleBackColor = true;
            this.btCompile.Click += new System.EventHandler(this.btCompile_Click);
            // 
            // picCompiling
            // 
            this.picCompiling.Image = global::XNAContentCompiler.Properties.Resources.validation_anim;
            this.picCompiling.Location = new System.Drawing.Point(4, 26);
            this.picCompiling.Name = "picCompiling";
            this.picCompiling.Size = new System.Drawing.Size(117, 10);
            this.picCompiling.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picCompiling.TabIndex = 12;
            this.picCompiling.TabStop = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(23, 7);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(84, 16);
            this.label4.TabIndex = 13;
            this.label4.Text = "Compiling...";
            // 
            // pnPrincipal
            // 
            this.pnPrincipal.Controls.Add(this.btClear);
            this.pnPrincipal.Controls.Add(this.cboTipoArquivo);
            this.pnPrincipal.Controls.Add(this.label1);
            this.pnPrincipal.Controls.Add(this.txtFileName);
            this.pnPrincipal.Controls.Add(this.btCompile);
            this.pnPrincipal.Controls.Add(this.btAbrirArquivo);
            this.pnPrincipal.Controls.Add(this.label3);
            this.pnPrincipal.Controls.Add(this.lstArquivos);
            this.pnPrincipal.Controls.Add(this.label2);
            this.pnPrincipal.Controls.Add(this.txtOutput);
            this.pnPrincipal.Controls.Add(this.btAdd);
            this.pnPrincipal.Controls.Add(this.btOutput);
            this.pnPrincipal.Controls.Add(this.btRemove);
            this.pnPrincipal.Location = new System.Drawing.Point(1, 3);
            this.pnPrincipal.Name = "pnPrincipal";
            this.pnPrincipal.Size = new System.Drawing.Size(516, 307);
            this.pnPrincipal.TabIndex = 14;
            // 
            // pnCompiling
            // 
            this.pnCompiling.Controls.Add(this.label4);
            this.pnCompiling.Controls.Add(this.picCompiling);
            this.pnCompiling.Location = new System.Drawing.Point(378, 202);
            this.pnCompiling.Name = "pnCompiling";
            this.pnCompiling.Size = new System.Drawing.Size(130, 41);
            this.pnCompiling.TabIndex = 15;
            this.pnCompiling.Visible = false;
            // 
            // backgroundWorker
            // 
            this.backgroundWorker.WorkerReportsProgress = true;
            this.backgroundWorker.WorkerSupportsCancellation = true;
            this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker_DoWork);
            // 
            // btClear
            // 
            this.btClear.Location = new System.Drawing.Point(365, 123);
            this.btClear.Name = "btClear";
            this.btClear.Size = new System.Drawing.Size(103, 24);
            this.btClear.TabIndex = 12;
            this.btClear.Text = "Clear";
            this.btClear.UseVisualStyleBackColor = true;
            this.btClear.Click += new System.EventHandler(this.btClear_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(520, 311);
            this.Controls.Add(this.pnCompiling);
            this.Controls.Add(this.pnPrincipal);
            this.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Deploy - XNA Content Compiler";
            ((System.ComponentModel.ISupportInitialize)(this.picCompiling)).EndInit();
            this.pnPrincipal.ResumeLayout(false);
            this.pnPrincipal.PerformLayout();
            this.pnCompiling.ResumeLayout(false);
            this.pnCompiling.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cboTipoArquivo;
        private System.Windows.Forms.TextBox txtFileName;
        private System.Windows.Forms.Button btAbrirArquivo;
        private System.Windows.Forms.ListBox lstArquivos;
        private System.Windows.Forms.Button btOutput;
        private System.Windows.Forms.TextBox txtOutput;
        private System.Windows.Forms.Button btRemove;
        private System.Windows.Forms.Button btAdd;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btCompile;
        private System.Windows.Forms.PictureBox picCompiling;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel pnPrincipal;
        private System.Windows.Forms.Panel pnCompiling;
        private System.ComponentModel.BackgroundWorker backgroundWorker;
        private System.Windows.Forms.Button btClear;
    }
}

