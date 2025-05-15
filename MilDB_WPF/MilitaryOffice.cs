using System.Collections.ObjectModel;
using System.Linq;

namespace MilDB_WPF;

public class MilitaryOffice
{
    public string Name { get; set; }
    public string Address { get; set; }
    public string ServiceArea { get; set; }
    public ObservableCollection<Conscript> Conscripts { get; set; }
    public ObservableCollection<Officer> Officers { get; set; }

    public MilitaryOffice()
    {
        Name = string.Empty;
        Address = string.Empty;
        ServiceArea = string.Empty;
        Conscripts = new ObservableCollection<Conscript>();
        Officers = new ObservableCollection<Officer>();
    }

    public MilitaryOffice(string name, string address, string serviceArea)
    {
        Name = name;
        Address = address;
        ServiceArea = serviceArea;
        Conscripts = new ObservableCollection<Conscript>();
        Officers = new ObservableCollection<Officer>();
    }

    public bool Valid()
    {
        return !string.IsNullOrWhiteSpace(Name)
            && !string.IsNullOrWhiteSpace(Address)
            && !string.IsNullOrWhiteSpace(ServiceArea);
    }

    // Пошук через LINQ
    public ObservableCollection<Conscript> FindConscriptsByFitnessCategory(string category) =>
        new(Conscripts.Where(c => c.FitnessCategory == category));

    public ObservableCollection<Conscript> FindConscriptsByStatus(string status) =>
        new(Conscripts.Where(c => c.Status == status));

    public ObservableCollection<Conscript> FindConscriptsByAddress(string addressPart) =>
        new(Conscripts.Where(c => !string.IsNullOrEmpty(c.Address) && c.Address.Contains(addressPart)));

    public ObservableCollection<Officer> FindOfficersByRank(string rank) =>
        new(Officers.Where(o => o.Rank == rank));

    public ObservableCollection<Officer> FindOfficersByYearsOfService(int minYears, int maxYears) =>
        new(Officers.Where(o => o.YearsOfService >= minYears && o.YearsOfService <= maxYears));
}
