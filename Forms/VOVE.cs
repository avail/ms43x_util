

using System.Diagnostics;

namespace ms43x_util.Forms;

public partial class VOVE : Form
{
    XdfFile m_Ms43Xdf;
    XdfBinaryReader m_Ms43Bin;

    int m_CurrentMap = 1;

    double m_CurrentIAT = 30.0f;
    double m_CurrentDisplacement = 2.793;

    double[] m_RpmValues = [192, 448, 704, 992, 1504, 2016, 2496, 3008, 3488, 4000, 4512, 4992, 5500, 6016, 6200, 6496];
    double[] m_LoadValues = [5.000, 10.000, 15.000, 20.001, 25.001, 30.001, 35.001, 40.001, 45.001, 50.002, 60.002, 70.002, 80.003, 90.003, 105.003, 125.004];

    public VOVE()
    {
        InitializeComponent();

        FormClosing += (s, e) => G.CloseApp();

        // #TODO: fill in the tables based on xdf data
        // columns: ldpm_n_32_10
        // rows: ldpm_map_1

        voTable.DoSetup(
            [544, 704, 992, 1248, 1504, 2016, 2496, 3008, 4000, 4992, 6016, 7008],
            [5, 10, 20, 30, 60, 90, 105, 125]);

        veTable.DoSetup(
            m_RpmValues,
            m_LoadValues);

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
        var table = m_Ms43Xdf.Tables.FirstOrDefault(c => c.Title == currentMapLabel.Text);
        var axis = table.Axes.FirstOrDefault(c => c.Id == "z");

        var data = (double[,])m_Ms43Bin.ReadAxisData(table.Axes[2]);
        var result = XdfMath.ApplyMath(data, "0.021194781 * X");

        DisplayVO(result);

        var ve = new VOtoVEConverter(result, m_CurrentIAT, m_CurrentDisplacement);
        var vet = ve.CalculateVETable(m_RpmValues, m_LoadValues, m_CurrentIAT, m_CurrentDisplacement);

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
        this.m_ExpandedVOTable = ExpandVOTable(inputVOTable, 16, 16);
        LogVOTable();
    }

    void LogVOTable()
    {
        for (int row = 0; row < 16; row++)
        {
            var final = "";
            for (int col = 0; col < 16; col++)
            {
                double voValue = m_ExpandedVOTable[row, col];
                final += voValue.ToString("F3");
                final += "\t";
            }

            Trace.WriteLine(final);
        }
    }

    double[,] ExpandVOTable(double[,] src, int newH, int newW)
    {
        int srcH = src.GetLength(0);
        int srcW = src.GetLength(1);

        double[,] dst = new double[newH, newW];

        for (int y = 0; y < newH; y++)
        {
            double gy = y * (srcH - 1.0) / (newH - 1.0);
            int y0 = (int)Math.Floor(gy);
            int y1 = Math.Min(y0 + 1, srcH - 1);
            double ty = gy - y0;

            for (int x = 0; x < newW; x++)
            {
                double gx = x * (srcW - 1.0) / (newW - 1.0);
                int x0 = (int)Math.Floor(gx);
                int x1 = Math.Min(x0 + 1, srcW - 1);
                double tx = gx - x0;

                double v00 = src[y0, x0];
                double v01 = src[y0, x1];
                double v10 = src[y1, x0];
                double v11 = src[y1, x1];

                // Bilinear interpolation
                double a = v00 * (1 - tx) + v01 * tx;
                double b = v10 * (1 - tx) + v11 * tx;
                dst[y, x] = a * (1 - ty) + b * ty;
            }
        }

        return dst;
    }

    public double[,] CalculateVETable(double[] rpmValues, double[] loadValues, double iat, double displ)
    {
        int rows = loadValues.Length;
        int cols = rpmValues.Length;
        m_VeTable = new double[rows, cols];

        double absoluteTemp = iat + 273.15; // convert to Kelvin
        double fixedRpm = rpmValues[0]; // always use first RPM value

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                double voValue = m_ExpandedVOTable[row, col];
                double loadValue = loadValues[row];

                double ve = (voValue * fixedRpm / AIRFLOW_FACTOR * R * absoluteTemp * MULTIPLIER)
                    / (loadValue * displ * fixedRpm * MOL_WEIGHT_AIR * TIME_FACTOR);

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
