using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ms43x_util;

public class Config
{
    public static string ConfigName = "config.cfg";

    public string LastXdfDir { get; set; } = "";
    public string LastXdf { get; set; } = "";

    public string LastBinDir { get; set; } = "";
    public string LastBin { get; set; } = "";

    public string MS43XdfDir { get; set; } = "";
    public string MS43Xdf { get; set; } = "";

    public string MS43BinDir { get; set; } = "";
    public string MS43Bin { get; set; } = "";

    public Config()
    {
    }

    PropertyInfo[] GetProperties()
    {
        return typeof(Config).GetProperties(BindingFlags.Public | BindingFlags.Instance);
    }

    public void Save()
    {
        using StreamWriter writer = new StreamWriter(ConfigName);

        var props = GetProperties();
        foreach (var field in props)
        {
            writer.WriteLine($"{field.Name} = {field.GetValue(this)}");
        }

        writer.Flush();
        writer.Close();
    }

    public void Read()
    {
        if (!File.Exists(ConfigName))
        {
            return;
        }

        var lines = File.ReadAllLines(ConfigName);
        var props = GetProperties();

        foreach (var line in lines)
        {
            var split = line.Split(" = ");
            var field = props.FirstOrDefault(f => f.Name == split[0]);

            field.SetValue(this, split[1]);
        }
    }
}
