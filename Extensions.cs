using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace ms43x_util;

public static class Extensions
{
    private static (double min, double max) GetMinMaxValues(DataGridView dgv)
    {
        double minValue = double.MaxValue;
        double maxValue = double.MinValue;
        bool foundAnyValue = false;

        foreach (DataGridViewRow row in dgv.Rows)
        {
            foreach (DataGridViewCell cell in row.Cells)
            {
                if (cell.Value != null && double.TryParse(cell.Value.ToString(), out double value))
                {
                    minValue = Math.Min(minValue, value);
                    maxValue = Math.Max(maxValue, value);
                    foundAnyValue = true;
                }
            }
        }

        if (!foundAnyValue)
        {
            return (0, 1);
        }

        return (minValue, maxValue);
    }

    public static void FormatColours(object sender, DataGridViewCellFormattingEventArgs e)
    {
        // skip header row and non-numeric columns
        if (e.RowIndex < 0 || e.ColumnIndex < 0)
        {
            return;
        }

        DataGridView dgv = (DataGridView)sender;
        var (min, max) = GetMinMaxValues(dgv);


        if (e.Value != null && double.TryParse(e.Value.ToString(), out double value))
        {
            Color lowColor = Color.FromArgb(150, 255, 255);
            Color highColor = Color.FromArgb(255, 80, 80);

            double normalizedValue = (value - min) / (max - min);
            normalizedValue = Math.Max(0, Math.Min(1, normalizedValue));

            int r = (int)(lowColor.R + (highColor.R - lowColor.R) * normalizedValue);
            int g = (int)(lowColor.G + (highColor.G - lowColor.G) * normalizedValue);
            int b = (int)(lowColor.B + (highColor.B - lowColor.B) * normalizedValue);

            e.CellStyle.BackColor = Color.FromArgb(r, g, b);

            e.CellStyle.ForeColor = Color.Black;
        }
    }

    public static void HandleCopyPaste(object sender, KeyEventArgs e)
    {
        var s = (DataGridView)sender;

        if (e.Control && e.KeyCode == Keys.V)
        {
            try
            {
                string clipboardText = Clipboard.GetText();
                if (string.IsNullOrEmpty(clipboardText)) return;

                string[] lines = clipboardText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

                for (int i = 0; i < lines.Length && i < s.Rows.Count; i++)
                {
                    string[] values = lines[i].Split(new[] { '\t', ' ' });

                    for (int j = 0; j < values.Length && j < s.Columns.Count; j++)
                    {
                        s.Rows[i].Cells[j].Value = values[j];
                        //dgv2.Rows[startRow + i].Cells[startCol + j].Value = 
                    }
                }

                e.Handled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Paste error: {ex.Message}");
            }
        }

        if (e.Control && e.KeyCode == Keys.C)
        {
            e.Handled = true;
        }
    }

    // real extensions start, lol
    public static void DoSetup(this DataGridView ctrl, double[] columns, double[] rows)
    {
        ctrl.AllowUserToAddRows = false;
        ctrl.AllowUserToDeleteRows = false;
        ctrl.ReadOnly = true;
        ctrl.RowHeadersVisible = true;

        ctrl.RowHeadersWidth = 100;

        ctrl.RowTemplate.Height = 18;
        ctrl.ColumnHeadersHeight = 20;
        ctrl.ColumnHeadersHeight = 18;

        var font = new Font("Arial", 9);
        var padding = new Padding(2);
        var align = DataGridViewContentAlignment.MiddleCenter;

        ctrl.DefaultCellStyle.SelectionBackColor = Color.Blue;
        ctrl.DefaultCellStyle.SelectionForeColor = Color.White;

        ctrl.DefaultCellStyle.Font = font;
        ctrl.DefaultCellStyle.Padding = padding;
        ctrl.DefaultCellStyle.Alignment = align;

        ctrl.ColumnHeadersDefaultCellStyle.Font = font;
        ctrl.ColumnHeadersDefaultCellStyle.Padding = padding;
        ctrl.ColumnHeadersDefaultCellStyle.Alignment = align;

        ctrl.RowHeadersDefaultCellStyle.Font = font;
        ctrl.RowHeadersDefaultCellStyle.Padding = padding;
        ctrl.RowHeadersDefaultCellStyle.Alignment = align;

        ctrl.SelectionMode = DataGridViewSelectionMode.CellSelect;
        ctrl.RowHeadersVisible = true;
        ctrl.AllowUserToOrderColumns = false;

        ctrl.DefaultCellStyle.WrapMode = DataGridViewTriState.False;
        //ctrl.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

        ctrl.Columns.Clear();
        ctrl.Rows.Clear();

        ctrl.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
        ctrl.DefaultCellStyle.Padding = new Padding(0);
        ctrl.DefaultCellStyle.Format = "";

        foreach (double header in columns)
        {
            DataGridViewTextBoxColumn col = new DataGridViewTextBoxColumn();
            var hdr = header.ToString();
            if (header == 9999.0)
            {
                hdr = " ";
            }
            col.HeaderText = hdr;
            col.Name = $"col_{header}";
            col.Width = 70;
            col.SortMode = DataGridViewColumnSortMode.NotSortable;
            col.DefaultCellStyle.Format = "0.000";
            ctrl.Columns.Add(col);
        }

        for (int i = 0; i < rows.Length; i++)
        {
            int rowIndex = ctrl.Rows.Add();
            ctrl.Rows[rowIndex].HeaderCell.Value = rows[i].ToString("F3");
            ctrl.Rows[rowIndex].Height = 25;
        }

        ctrl.CellPainting += (s, e) =>
        {
            if (e.RowIndex == -1 || e.ColumnIndex == -1)
            {
                return;
            }

            if (ctrl.Rows[e.RowIndex].Cells[e.ColumnIndex].Selected)
            {
                e.CellStyle.SelectionForeColor = Color.White;
                e.CellStyle.SelectionBackColor = e.CellStyle.BackColor;
            }
            else
            {
                e.CellStyle.SelectionForeColor = Color.Black;
            }
        };
    }
}
