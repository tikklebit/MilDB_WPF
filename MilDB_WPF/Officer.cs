using System.Collections.ObjectModel;

namespace MilDB_WPF;

public class Officer
{
    public string FullName { get; set; }
    public string Rank { get; set; }
    public int YearsOfService { get; set; }
    public ObservableCollection<Conscript> AssignedConscripts { get; set; }

    public Officer()
    {
        FullName = string.Empty;
        Rank = string.Empty;
        YearsOfService = 0;
        AssignedConscripts = new ObservableCollection<Conscript>();
    }

    public Officer(string fullName, string rank, int yearsOfService)
    {
        FullName = fullName;
        Rank = rank;
        YearsOfService = yearsOfService;
        AssignedConscripts = new ObservableCollection<Conscript>();
    }

    public bool Valid()
    {
        return !string.IsNullOrWhiteSpace(FullName)
            && !string.IsNullOrWhiteSpace(Rank)
            && YearsOfService >= 0 && YearsOfService <= 40;
    }
}
