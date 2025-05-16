using System.Collections.ObjectModel;
using System.IO;
using Newtonsoft.Json;

namespace MilDB_WPF;

public static class MilitaryOfficeService
{
    private static readonly string DefaultFilePath = Path.Combine(
        System.AppDomain.CurrentDomain.BaseDirectory, "offices.json");

    public static ObservableCollection<MilitaryOffice> LoadAll(string filePath)
    {
        if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            return new ObservableCollection<MilitaryOffice>();

        var json = File.ReadAllText(filePath);
        var list = JsonConvert.DeserializeObject<List<MilitaryOffice>>(json) ?? new List<MilitaryOffice>();
        return new ObservableCollection<MilitaryOffice>(list);
    }

    public static void SaveAll(ObservableCollection<MilitaryOffice> offices)
    {
        var json = JsonConvert.SerializeObject(offices, Formatting.Indented);
        File.WriteAllText(DefaultFilePath, json);
    }
}
