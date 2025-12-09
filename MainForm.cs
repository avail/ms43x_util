using ms43x_util.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ms43x_util
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            G.Config = new Config();
            G.Config.Read();
        }

        void LoadXDF(string path)
        {
            if (File.Exists(path))
            {
                G.Xdf = XdfFile.ParseXDF(path);

                loadMapBtn.Enabled = true;
            }
        }

        void LoadMAP(string path)
        {
            if (File.Exists(path))
            {
                G.MapFile = new XdfBinaryReader(path, G.Xdf.BaseOffset);

                narrowbandErrorBtn.Enabled = true;
                voVeConversionBtn.Enabled = true;
            }
        }

        private void loadXdfBtn_Click(object sender, EventArgs e)
        {
            var file = ChooseFile(FolderPickerOption.Xdf);

            LoadXDF(file);
        }

        private void loadMapBtn_Click(object sender, EventArgs e)
        {
            var file = ChooseFile(FolderPickerOption.Bin);

            LoadMAP(file);
        }

        public enum FolderPickerOption
        {
            Xdf,
            Bin,
        }

        public static string ChooseFile(FolderPickerOption option, bool isMS43 = false)
        {
            OpenFileDialog ofd = new();
            ofd.Multiselect = false;
            ofd.Filter = option == FolderPickerOption.Xdf ? "XDF files (*.xdf)|*.xdf" : "bin files (*.bin)|*.bin";
            if (!isMS43)
            {
                ofd.InitialDirectory = option == FolderPickerOption.Xdf ? G.Config.LastXdfDir : G.Config.LastBinDir;
            }
            else
            {
                ofd.InitialDirectory = option == FolderPickerOption.Xdf ? G.Config.MS43XdfDir : G.Config.MS43BinDir;
            }

            var result = ofd.ShowDialog();
            var response = "";

            if (result == DialogResult.OK)
            {
                response = ofd.FileName;
            }
            else if (result == DialogResult.Cancel)
            {
                response = "";
            }
            else
            {
                response = ChooseFile(option);
            }

            switch (option)
            {
                case FolderPickerOption.Xdf:
                    if (!isMS43)
                    {
                        G.Config.LastXdfDir = Path.GetDirectoryName(response);
                        G.Config.LastXdf = Path.GetFileName(response);
                    }
                    else
                    {
                        G.Config.MS43XdfDir = Path.GetDirectoryName(response);
                        G.Config.MS43Xdf = Path.GetFileName(response);
                    }
                    break;
                case FolderPickerOption.Bin:
                    if (!isMS43)
                    {
                        G.Config.LastBinDir = Path.GetDirectoryName(response);
                        G.Config.LastBin = Path.GetFileName(response);
                    }
                    else
                    {
                        G.Config.MS43BinDir = Path.GetDirectoryName(response);
                        G.Config.MS43Bin = Path.GetFileName(response);
                    }

                    break;
            }

            G.Config.Save();

            return response;
        }


        void HideMain()
        {
            G.MainForm.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var frm = new NarrowbandErrorPercent();
            frm.Show();
            HideMain();
        }

        private void voVeConversionBtn_Click(object sender, EventArgs e)
        {
            var frm = new VOVE();
            frm.Show();
            HideMain();
        }

        private void reloadLastCombo_Click(object sender, EventArgs e)
        {
            LoadXDF(Path.Combine(G.Config.LastXdfDir, G.Config.LastXdf));
            LoadMAP(Path.Combine(G.Config.LastBinDir, G.Config.LastBin));
        }
    }
}
