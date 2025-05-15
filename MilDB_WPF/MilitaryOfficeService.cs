using System.Collections.ObjectModel;
using System.IO;
using Newtonsoft.Json;

namespace MilDB_WPF;

public static class MilitaryOfficeService
{
    public static ObservableCollection<MilitaryOffice> LoadAll(string filePath)
    {
        if (!File.Exists(filePath))
            return new ObservableCollection<MilitaryOffice>();

        var json = File.ReadAllText(filePath);
        var list = JsonConvert.DeserializeObject<List<MilitaryOffice>>(json) ?? new List<MilitaryOffice>();
        return new ObservableCollection<MilitaryOffice>(list);
    }

    public static void SaveAll(string filePath, ObservableCollection<MilitaryOffice> offices)
    {
        var json = JsonConvert.SerializeObject(offices, Formatting.Indented);
        File.WriteAllText(filePath, json);
    }
}
