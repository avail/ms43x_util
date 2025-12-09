namespace ms43x_util.Forms;

public partial class NarrowbandErrorPercent : Form
{
    static void PopulateDGV(DataGridView dgv, double[,] data)
    {
        for (var i = 0; i < data.GetLength(0); i++)
        {
            for (var j = 0; j < data.GetLength(1); j++)
            {
                dgv.Rows[i].Cells[j].Value = data[i, j].ToString("F3");
            }
        }
    }

    public NarrowbandErrorPercent()
    {
        InitializeComponent();

        FormClosing += (s, e) => G.CloseApp();

        var columns = G.MapFile.GetData(G.Xdf.Tables["ldpm_ve_n"].Axes[2], "1.0 * X").ToArray();
        var rows = G.MapFile.GetData(G.Xdf.Tables["ldpm_ve_map"].Axes[2], "0.0082921489 * X").ToArray();
        
        dgv1.DoSetup(
            columns,
            rows);

        dgv2.DoSetup(
            [9999.0, 9999.0, 9999.0, 9999.0, 9999.0, 9999.0, 9999.0, 9999.0, 9999.0, 9999.0, 9999.0, 9999.0, 9999.0, 9999.0, 9999.0, 9999.0],
            [10, 15, 20.001, 25.001, 30.001, 35.001, 40.001, 45.001, 50.002, 60.002, 70.002, 80.003, 100.003, 159.997, 209.999, 300.002]);

        dgv3.DoSetup(
            columns,
            rows);

        dgv1.CellFormatting += Extensions.FormatColours;
        dgv3.CellFormatting += Extensions.FormatColours;

        dgv1.KeyDown += Extensions.HandleCopyPaste;
        dgv2.KeyDown += Extensions.HandleCopyPaste;
        dgv3.KeyDown += Extensions.HandleCopyPaste;

        HandleBinClick(ip_map_ve_1__map__n, null);
    }

    void PopulateDGV3()
    {
        for (var i = 0; i < dgv1.Rows.Count; i++)
        {
            for (var j = 0; j < dgv1.Columns.Count; j++)
            {
                var a = double.Parse(dgv1.Rows[i].Cells[j].Value.ToString());
                if (dgv2.Rows[i].Cells[j].Value == null)
                {
                    continue;
                }

                if (double.TryParse(dgv2.Rows[i].Cells[j].Value.ToString(), out double b))
                {
                    var result = a + (a * (b / 100.0 / 2));

                    dgv3.Rows[i].Cells[j].Value = result.ToString("F3");
                }
                else
                {
                    dgv3.Rows[i].Cells[j].Value = a.ToString("F3");
                }
            }
        }
    }

    private void HandleBinClick(object sender, EventArgs e)
    {
        var ctrl = (Control)sender;
        var axis = G.Xdf.Tables[ctrl.Text].Axes[2];

        var zData = (double[,])G.MapFile.ReadAxisData(axis);
        var resultz = XdfMath.ApplyMath(zData, "0.00097656252 * X");

        PopulateDGV(dgv1, resultz);
        PopulateDGV3();
    }
}
