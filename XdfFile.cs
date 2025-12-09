using System.Text.RegularExpressions;

namespace ms43x_util;

public class XdfFile
{
    public uint BaseOffset { get; set; } = 0;
    public Dictionary<string, XdfTable> Tables { get; set; } = new();

    public XdfFile()
    {

    }

    public static XdfFile ParseXDF(string path, bool isMS43 = false)
    {
        var lines = new StreamReader(path);

        bool inTable = false;
        int tableCount = 0;

        var tableLines = new Dictionary<int, List<string>>();

        uint baseOffset = 0;

        string line;
        while ((line = lines.ReadLine()) != null)
        {
            var trimmed = line.Trim();

            if (trimmed.StartsWith("<BASEOFFSET"))
            {
                // <BASEOFFSET offset="458752" subtract="0" />
                baseOffset = uint.Parse(trimmed.Split("=\"")[1].Split("\" ")[0]);
            }

            if (trimmed.StartsWith("<XDFTABLE"))
            {
                inTable = true;
                tableLines.Add(tableCount, new List<string>());
            }

            if (inTable)
            {
                tableLines[tableCount].Add(line);
            }

            if (trimmed.StartsWith("</XDFTABLE>"))
            {
                inTable = false;
                tableCount++;
            }
        }

        // find our maps :)
        var indexes = new List<int>();

        foreach (var (k, v) in tableLines)
        {
            if (v.Any(a => a.Contains(isMS43 ? "ip_maf_vo_" : "ip_map_ve_")))
            {
                indexes.Add(k);
            }

            if (isMS43)
            {
                if (v.Any(a => a.Contains("ldpm_n_32_10")))
                {
                    indexes.Add(k);
                }
                if (v.Any(a => a.Contains("ldpm_map_1")))
                {
                    indexes.Add(k);
                }
            }
            else
            {
                if (v.Any(a => a.Contains("ldpm_ve_n")))
                {
                    indexes.Add(k);
                }
                if (v.Any(a => a.Contains("ldpm_ve_map")))
                {
                    indexes.Add(k);
                }
            }
        }

        // process the map stuff
        var tables = new Dictionary<string, XdfTable>();
        foreach (var idx in indexes)
        {
            var parsed = XdfTable.Parse(tableLines[idx]);
            tables.Add(parsed.Title, parsed);
        }

        return new XdfFile()
        {
            BaseOffset = baseOffset,
            Tables = tables
        };
    }
}

public class XdfTable
{
    public string Title { get; set; }
    public List<XdfAxis> Axes { get; set; } = new List<XdfAxis>();

    public static XdfTable Parse(List<string> lines)
    {
        var table = new XdfTable();
        XdfAxis currentAxis = null;

        foreach (var line in lines)
        {
            var trimmed = line.Trim();

            if (trimmed.StartsWith("<title>"))
            {
                table.Title = ExtractValue(trimmed, "title");
            }
            else if (trimmed.StartsWith("<XDFAXIS"))
            {
                currentAxis = new XdfAxis
                {
                    Id = ExtractAttribute(trimmed, "id"),
                };
                table.Axes.Add(currentAxis);
            }
            else if (currentAxis != null)
            {
                if (trimmed.StartsWith("<EMBEDDEDDATA"))
                {
                    currentAxis.EmbeddedData = new EmbeddedData
                    {
                        MmedTypeFlags = ExtractAttribute(trimmed, "mmedtypeflags"),
                        MmedAddress = ExtractAttribute(trimmed, "mmedaddress"),
                        MmedElementSizeBits = ExtractAttribute(trimmed, "mmedelementsizebits"),
                        MmedRowCount = ExtractAttribute(trimmed, "mmedrowcount"),
                        MmedColCount = ExtractAttribute(trimmed, "mmedcolcount"),
                        MmedMajorStrideBits = ExtractAttribute(trimmed, "mmedmajorstridebits"),
                        MmedMinorStrideBits = ExtractAttribute(trimmed, "mmedminorstridebits")
                    };
                }
                else if (trimmed.StartsWith("</XDFAXIS>"))
                {
                    currentAxis = null;
                }
            }
        }

        return table;
    }

    private static string ExtractAttribute(string line, string attributeName)
    {
        var pattern = attributeName + @"\s*=\s*""([^""]*)""";
        var match = Regex.Match(line, pattern, RegexOptions.IgnoreCase);
        return match.Success ? match.Groups[1].Value : null;
    }

    private static string ExtractValue(string line, string tagName)
    {
        var pattern = $@"<{tagName}>([^<]*)</{tagName}>";
        var match = Regex.Match(line, pattern, RegexOptions.IgnoreCase);
        return match.Success ? match.Groups[1].Value : null;
    }
}

public class XdfAxis
{
    public string Id { get; set; }
    public EmbeddedData EmbeddedData { get; set; }
}

public class EmbeddedData
{
    public string MmedTypeFlags { get; set; }
    public string MmedAddress { get; set; }
    public string MmedElementSizeBits { get; set; }
    public string MmedRowCount { get; set; }
    public string MmedColCount { get; set; }
    public string MmedMajorStrideBits { get; set; }
    public string MmedMinorStrideBits { get; set; }
}


public class XdfBinaryReader
{
    private byte[] m_FileData;
    private uint m_BaseOffset;

    public XdfBinaryReader(string filePath, uint baseOffset)
    {
        m_FileData = File.ReadAllBytes(filePath);
        m_BaseOffset = baseOffset;
    }

    public XdfBinaryReader(byte[] data)
    {
        m_FileData = data;
    }

    public Dictionary<string, object> ReadAllAxisData(XdfTable table)
    {
        var result = new Dictionary<string, object>();

        foreach (var axis in table.Axes)
        {
            result[axis.Id] = ReadAxisData(axis);
        }

        return result;
    }

    public double[] GetData(XdfAxis axis, string calc)
    {
        var data = Read1DData(axis, 16);

        return XdfMath.ApplyMath(data, calc);
    }

    public object ReadAxisData(XdfAxis axis)
    {
        if (axis.EmbeddedData == null)
        {
            throw new InvalidOperationException($"Axis {axis.Id} has no embedded data");
        }

        var embeddedData = axis.EmbeddedData;
        uint address = ParseHex(embeddedData.MmedAddress);
        int elementSizeBits = int.Parse(embeddedData.MmedElementSizeBits);
        int elementSizeBytes = elementSizeBits / 8;

        if (!string.IsNullOrEmpty(embeddedData.MmedRowCount) && !string.IsNullOrEmpty(embeddedData.MmedColCount))
        {
            int rowCount = int.Parse(embeddedData.MmedRowCount);
            int colCount = int.Parse(embeddedData.MmedColCount);
            return Read2DData(address, elementSizeBytes, rowCount, colCount);
        }
        else
        {
            throw new InvalidOperationException("Use Read1DData(XdfAxis, int count) for 1D axes");
        }
    }

    public double[] Read1DData(XdfAxis axis, int count)
    {
        if (axis.EmbeddedData == null)
        {
            throw new InvalidOperationException($"Axis {axis.Id} has no embedded data");
        }

        var embeddedData = axis.EmbeddedData;
        uint address = ParseHex(embeddedData.MmedAddress);
        int elementSizeBits = int.Parse(embeddedData.MmedElementSizeBits);
        int elementSizeBytes = elementSizeBits / 8;

        return Read1DData(address, elementSizeBytes, count);
    }

    private double[] Read1DData(uint address, int elementSizeBytes, int count)
    {
        double[] result = new double[count];

        for (int i = 0; i < count; i++)
        {
            uint offset = address + (uint)(i * elementSizeBytes);
            result[i] = ReadValue(offset, elementSizeBytes);
        }

        return result;
    }

    private double[,] Read2DData(uint address, int elementSizeBytes, int rowCount, int colCount)
    {
        double[,] result = new double[rowCount, colCount];

        for (int row = 0; row < rowCount; row++)
        {
            for (int col = 0; col < colCount; col++)
            {
                uint offset = address + (uint)((row * colCount + col) * elementSizeBytes);
                result[row, col] = ReadValue(offset, elementSizeBytes);
            }
        }

        return result;
    }

    private float ReadValue(uint offset, int sizeBytes)
    {
        if (m_BaseOffset + offset + sizeBytes > m_FileData.Length)
        {
            throw new IndexOutOfRangeException($"Offset 0x{offset:X} exceeds file size");
        }

        return sizeBytes switch
        {
            1 => m_FileData[m_BaseOffset + offset],
            2 => BitConverter.ToUInt16(m_FileData, (int)(m_BaseOffset + offset)),
            4 => BitConverter.ToUInt32(m_FileData, (int)(m_BaseOffset + offset)),
            _ => throw new NotSupportedException($"Element size {sizeBytes} bytes not supported")
        };
    }

    private static uint ParseHex(string hexString)
    {
        return uint.Parse(hexString.StartsWith("0x") ? hexString.Substring(2) : hexString, System.Globalization.NumberStyles.HexNumber);
    }
}

public static class XdfMath
{
    public static double[] ApplyMath(double[] data, string equation)
    {
        double[] result = new double[data.Length];
        for (int i = 0; i < data.Length; i++)
        {
            result[i] = EvaluateEquation(equation, data[i]);
        }
        return result;
    }

    public static double[,] ApplyMath(double[,] data, string equation)
    {
        int rows = data.GetLength(0);
        int cols = data.GetLength(1);
        double[,] result = new double[rows, cols];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                result[i, j] = EvaluateEquation(equation, data[i, j]);
            }
        }

        return result;
    }

    private static double EvaluateEquation(string equation, double x)
    {
        string expr = equation.Replace("X", $"({x})");
        var dt = new System.Data.DataTable();

        return Convert.ToSingle(dt.Compute(expr, null));
    }
}