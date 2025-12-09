namespace ms43x_util.Forms;

public partial class NarrowbandErrorPercent : Form
{
    static void PopulateDGV(DataGridView dgv, double[,] data)
    {
        for (int i = 0; i < data.GetLength(0); i++)
        {
            for (int j = 0; j < data.GetLength(1); j++)
            {
                dgv.Rows[i].Cells[j].Value = data[i, j].ToString("F3");
            }
        }
    }

    public NarrowbandErrorPercent()
    {
        InitializeComponent();

        FormClosing += (s, e) => G.CloseApp();

        // #TODO: fill in the tables based on xdf data
        // columns: ldpm_ve_n
        // rows: ldpm_ve_map
        
        dgv1.DoSetup(
            [192, 448, 704, 992, 1504, 2016, 2496, 3008, 3488, 4000, 4512, 4992, 5500, 6016, 6200, 6496],
            [5.000, 10.000, 15.000, 20.001, 25.001, 30.001, 35.001, 40.001, 45.001, 50.002, 60.002, 70.002, 80.003, 90.003, 105.003, 125.004]);

        dgv2.DoSetup(
            [9999.0, 9999.0, 9999.0, 9999.0, 9999.0, 9999.0, 9999.0, 9999.0, 9999.0, 9999.0, 9999.0, 9999.0, 9999.0, 9999.0, 9999.0, 9999.0],
            [10, 15, 20.001, 25.001, 30.001, 35.001, 40.001, 45.001, 50.002, 60.002, 70.002, 80.003, 100.003, 159.997, 209.999, 300.002]);

        dgv3.DoSetup(
            [192, 448, 704, 992, 1504, 2016, 2496, 3008, 3488, 4000, 4512, 4992, 5500, 6016, 6200, 6496],
            [5.000, 10.000, 15.000, 20.001, 25.001, 30.001, 35.001, 40.001, 45.001, 50.002, 60.002, 70.002, 80.003, 90.003, 105.003, 125.004]);

        dgv1.CellFormatting += Extensions.FormatColours;
        dgv3.CellFormatting += Extensions.FormatColours;

        dgv1.KeyDown += Extensions.HandleCopyPaste;
        dgv2.KeyDown += Extensions.HandleCopyPaste;
        dgv3.KeyDown += Extensions.HandleCopyPaste;

        HandleBinClick(ip_map_ve_1__map__n, null);
    }

    void PopulateDGV3()
    {
        for (int i = 0; i < dgv1.Rows.Count; i++)
        {
            for (int j = 0; j < dgv1.Columns.Count; j++)
            {
                var a = double.Parse(dgv1.Rows[i].Cells[j].Value.ToString());
                if (dgv2.Rows[i].Cells[j].Value == null)
                {
                    continue;
                }

                if (double.TryParse(dgv2.Rows[i].Cells[j].Value.ToString(), out double b))
                {
                    double result = a + (a * (b / 100.0 / 2));

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
        var table = G.Xdf.Tables.FirstOrDefault(c => c.Title == ctrl.Text);
        var axis = table.Axes.FirstOrDefault(c => c.Id == "z");

        var zData = (double[,])G.MapFile.ReadAxisData(table.Axes[2]);
        var resultz = XdfMath.ApplyMath(zData, "0.00097656252 * X");

        PopulateDGV(dgv1, resultz);
        PopulateDGV3();
    }
}
