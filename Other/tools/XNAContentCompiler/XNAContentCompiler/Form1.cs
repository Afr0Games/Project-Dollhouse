using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Threading;
namespace XNAContentCompiler
{
    public partial class Form1 : Form
    {
        private ComboItemCollection FileTypes;
        private ComboItemCollection SelectedFiles;
        private ContentBuilder contentBuilder;

        private void LoadFileTypes()
        {
            this.FileTypes = new ComboItemCollection();
            this.FileTypes.Add(new ComboItem("Textures", "Image Files(*.bmp;*.jpg;*.png;*.tga;*.dds)|*.bmp;*.jpg;*.png;*.tga;*.dds"));
            this.FileTypes.Add(new ComboItem("Audio", "Audio Files(*.wav;*.mp3;*.wma)|*.wav;*.mp3;*.wma"));
            this.FileTypes.Add(new ComboItem("Fonts", "SpriteFont Files(*.spritefont)|*.spritefont"));
            cboTipoArquivo.DataSource = FileTypes;
            cboTipoArquivo.DisplayMember = "Name";
            cboTipoArquivo.ValueMember = "Value";
            cboTipoArquivo.Refresh();
        }

        private void LoadListOfFiles()
        {
            lstArquivos.DataSource = null;
            lstArquivos.Refresh();
            lstArquivos.DataSource = SelectedFiles;
            lstArquivos.DisplayMember = "Name";
            lstArquivos.ValueMember = "Value";
            lstArquivos.Refresh();
        }

        public Form1()
        {
            InitializeComponent();
            this.Load += new EventHandler(Form1_Load);
        }

        void Form1_Load(object sender, EventArgs e)
        {
            LoadFileTypes();
            SelectedFiles = new ComboItemCollection();
            this.contentBuilder = new ContentBuilder();
            this.backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker_RunWorkerCompleted);
        }

        private void btAbrirArquivo_Click(object sender, EventArgs e)
        {
            OpenFileDialog jnl = new OpenFileDialog();
            jnl.Filter = (string)cboTipoArquivo.SelectedValue;
            if (jnl.ShowDialog() == DialogResult.OK)
            {
                txtFileName.Text = jnl.FileName;
            }
        }

        private void btAdd_Click(object sender, EventArgs e)
        {
            if(!SelectedFiles.ContainsValue(txtFileName.Text))
            {
                SelectedFiles.Add(new ComboItem(Path.GetFileName(txtFileName.Text), txtFileName.Text));
                LoadListOfFiles();
            }
            else
            {
                MessageBox.Show("The file is already in the collection.");
            }
            txtFileName.Text = String.Empty;
        }

        private void btRemove_Click(object sender, EventArgs e)
        {
            if (lstArquivos.SelectedIndex >= 0)
            {
                if (MessageBox.Show("Do you really want to remove this item?", "Remove Item", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    SelectedFiles.RemoveAt(lstArquivos.SelectedIndex);
                    LoadListOfFiles();
                }
            }
        }

        private void EnablePanels(Boolean enable)
        {
            pnPrincipal.Enabled = enable;
            pnCompiling.Visible = !enable;
        }

        private void btCompile_Click(object sender, EventArgs e)
        {
            if (SelectedFiles.Count > 0 && txtOutput.Text.Trim().Length > 0)
            {
                backgroundWorker.RunWorkerAsync();
            }
        }

        private void btOutput_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog jnl = new FolderBrowserDialog();
            if (jnl.ShowDialog() == DialogResult.OK)
            {
                txtOutput.Text = jnl.SelectedPath;
            }

        }

        void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            EnablePanels(true);
        }

        delegate void CallEnablePanels(Boolean enable);
        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            CallEnablePanels cm = new CallEnablePanels(EnablePanels);
            this.Invoke(cm, new object[] { false });
            //Remove os arquivos anteriormente adicionados.
            this.contentBuilder.Clear();
            //Adiciona os itens da lista
            foreach (ComboItem item in SelectedFiles)
            {
                this.contentBuilder.Add(item);
            }
            //Aplica o Build
            string error = this.contentBuilder.Build();
            //Se houve algum erro, informa-o
            if (!String.IsNullOrEmpty(error))
            {
                MessageBox.Show(error);
                return;
            }
            //Recupera os arquivos criados
            string tempPath = this.contentBuilder.OutputDirectory;
            string[] files = Directory.GetFiles(tempPath, "*.xnb");
            //Copia os arquivos para a saída
            foreach (string file in files)
            {
                System.IO.File.Copy(file, Path.Combine(txtOutput.Text, Path.GetFileName(file)), true);
            }
            MessageBox.Show("Files compiled !!");
        }

        private void btClear_Click(object sender, EventArgs e)
        {
            if (lstArquivos.SelectedIndex >= 0)
            {
                if (MessageBox.Show("Do you really want to clear the list?", "Clear List", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    SelectedFiles.Clear();
                    LoadListOfFiles();
                }
            }
        }

    }
}
