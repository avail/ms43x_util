

using System.Diagnostics;

namespace ms43x_util.Forms;

public partial class VOVE : Form
{
    XdfFile m_Ms43Xdf;
    XdfBinaryReader m_Ms43Bin;

    int m_CurrentMap = 1;

    double m_CurrentIAT = 30.0f;
    double m_CurrentDisplacement = 2.793;

    public VOVE()
    {
        InitializeComponent();

        FormClosing += (s, e) => G.CloseApp();

        voTable.DoSetup(
            [544, 704, 992, 1248, 1504, 2016, 2496, 3008, 4000, 4992, 6016, 7111],
            [5, 10, 20, 30, 60, 90, 105, 111]);

        veTable.DoSetup(
            [192, 448, 704, 992, 1504, 2016, 2496, 3008, 3488, 4000, 4512, 4992, 5500, 6016, 6200, 6226],
            [5.000, 10.000, 15.000, 20.001, 25.001, 30.001, 35.001, 40.001, 45.001, 50.002, 60.002, 70.002, 80.003, 90.003, 105.003, 123.004]);

        iatTextBox.KeyPress += HandleKeyPress;
        displacementTextBox.KeyPress += HandleKeyPress;

        voTable.CellFormatting += Extensions.FormatColours;
        veTable.CellFormatting += Extensions.FormatColours;
    }

    void LoadXdf(string path)
    {
        if (File.Exists(path))
        {
            m_Ms43Xdf = XdfFile.ParseXDF(path, true);
        }
    }

    void LoadBin(string path)
    {
        if (File.Exists(path))
        {
            m_Ms43Bin = new XdfBinaryReader(path, m_Ms43Xdf.BaseOffset);
        }

        buttonPrevMap.Enabled = true;
        buttonNextMap.Enabled = true;
        iatTextBox.Enabled = true;
        displacementTextBox.Enabled = true;
    }

    void HandleKeyPress(object sender, KeyPressEventArgs e)
    {
        if (sender is TextBox tb)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != '\b' && e.KeyChar != '.')
            {
                e.Handled = true;
            }
            else if (e.KeyChar == '.' && tb.Text.Contains("."))
            {
                e.Handled = true; // Prevent multiple decimal points
            }
        }
    }

    private void ms43XdfBtn_Click(object sender, EventArgs e)
    {
        var file = MainForm.ChooseFile(MainForm.FolderPickerOption.Xdf, true);
        LoadXdf(file);

        ms43MapBtn.Enabled = true;
    }

    private void ms43MapBtn_Click(object sender, EventArgs e)
    {
        var file = MainForm.ChooseFile(MainForm.FolderPickerOption.Bin, true);
        LoadBin(file);

        CalcVE();
    }

    private void lastBtn_Click(object sender, EventArgs e)
    {
        LoadXdf(Path.Combine(G.Config.MS43XdfDir, G.Config.MS43Xdf));
        LoadBin(Path.Combine(G.Config.MS43BinDir, G.Config.MS43Bin));

        CalcVE();
    }

    void DisplayVO(double[,] data)
    {
        for (int i = 0; i < data.GetLength(0); i++)
        {
            for (int j = 0; j < data.GetLength(1); j++)
            {
                voTable.Rows[i].Cells[j].Value = data[i, j].ToString("F3");
            }
        }
    }

    void CalcVE()
    {
        var rpm = m_Ms43Bin.GetData(m_Ms43Xdf.Tables["ldpm_n_32_10"].Axes[2], "32.0 * X").Take(12).ToArray();
        var load = m_Ms43Bin.GetData(m_Ms43Xdf.Tables["ldpm_map_1"].Axes[2], "0.082921489 * X").Take(8).ToArray();

        voTable.DoSetup(rpm, load);

        var axis = m_Ms43Xdf.Tables[currentMapLabel.Text].Axes[2];
        var data = (double[,])m_Ms43Bin.ReadAxisData(axis);
        var result = XdfMath.ApplyMath(data, "0.021194781 * X");

        DisplayVO(result);

        var rpmx = G.MapFile.GetData(G.Xdf.Tables["ldpm_ve_n"].Axes[2], "1.0 * X").ToArray();
        var loadx = G.MapFile.GetData(G.Xdf.Tables["ldpm_ve_map"].Axes[2], "0.0082921489 * X").ToArray();

        veTable.DoSetup(rpmx, loadx);

        var ve = new VOtoVEConverter(result, m_CurrentIAT, m_CurrentDisplacement);
        var vet = ve.CalculateVETable(rpmx, loadx, m_CurrentIAT, m_CurrentDisplacement);

        for (int i = 0; i < vet.GetLength(0); i++)
        {
            for (int j = 0; j < vet.GetLength(1); j++)
            {
                veTable.Rows[i].Cells[j].Value = vet[i, j].ToString("F3");
            }
        }
    }

    private void buttonNextMap_Click(object sender, EventArgs e)
    {
        m_CurrentMap += 1;
        if (m_CurrentMap > 8)
        {
            m_CurrentMap = 1;
        }

        currentMapLabel.Text = $"ip_maf_vo_{m_CurrentMap}__map__n";

        CalcVE();
    }

    private void buttonPrevMap_Click(object sender, EventArgs e)
    {
        m_CurrentMap -= 1;
        if (m_CurrentMap < 1)
        {
            m_CurrentMap = 8;
        }

        currentMapLabel.Text = $"ip_maf_vo_{m_CurrentMap}__map__n";

        CalcVE();
    }

    private void iatTextBox_TextChanged(object sender, EventArgs e)
    {
        m_CurrentIAT = double.Parse(iatTextBox.Text);
        CalcVE();
    }

    private void displacementTextBox_TextChanged(object sender, EventArgs e)
    {
        m_CurrentDisplacement = double.Parse(displacementTextBox.Text);
        CalcVE();
    }
}

public class VOtoVEConverter
{
    private double[,] m_ExpandedVOTable;
    private double[,] m_VeTable;

    private const double R = 8.314;
    private const double AIRFLOW_FACTOR = 5555;
    private const double MULTIPLIER = 120;
    private const double MOL_WEIGHT_AIR = 28.9;
    private const double TIME_FACTOR = 3.6;

    public VOtoVEConverter(double[,] inputVOTable, double iat, double engineDisplacement)
    {
        m_ExpandedVOTable = ExpandVOTable(inputVOTable, 16, 16);
    }

    double[,] ExpandVOTable(double[,] src, int newH, int newW)
    {
        var srcH = src.GetLength(0);
        var srcW = src.GetLength(1);

        var dst = new double[newH, newW];

        for (int y = 0; y < newH; y++)
        {
            double gy = y * (srcH - 1.0) / (newH - 1.0);
            int y0 = (int)Math.Floor(gy);
            int y1 = Math.Min(y0 + 1, srcH - 1);
            double ty = gy - y0;

            for (var x = 0; x < newW; x++)
            {
                var gx = x * (srcW - 1.0) / (newW - 1.0);
                var x0 = (int)Math.Floor(gx);
                var x1 = Math.Min(x0 + 1, srcW - 1);
                var tx = gx - x0;

                var v00 = src[y0, x0];
                var v01 = src[y0, x1];
                var v10 = src[y1, x0];
                var v11 = src[y1, x1];

                // bilinear interpolation
                var a = v00 * (1 - tx) + v01 * tx;
                var b = v10 * (1 - tx) + v11 * tx;
                dst[y, x] = a * (1 - ty) + b * ty;
            }
        }

        return dst;
    }

    public double[,] CalculateVETable(double[] rpmValues, double[] loadValues, double iat, double displ)
    {
        m_VeTable = new double[loadValues.Length, rpmValues.Length];

        var absoluteTemp = iat + 273.15; // convert to Kelvin

        for (var row = 0; row < loadValues.Length; row++)
        {
            for (var col = 0; col < rpmValues.Length; col++)
            {
                var voValue = m_ExpandedVOTable[row, col];
                var loadValue = loadValues[row];
                var rpm = rpmValues[row];

                var ve = (voValue * rpm / AIRFLOW_FACTOR * R * absoluteTemp * MULTIPLIER)
                    / (loadValue * displ * rpm * MOL_WEIGHT_AIR * TIME_FACTOR);

                // round to 3 decimal places
                m_VeTable[row, col] = Math.Round(ve, 3);
            }
        }

        return m_VeTable;
    }

    public double GetVEValue(int row, int col)
    {
        return m_VeTable[row, col];
    }
}
