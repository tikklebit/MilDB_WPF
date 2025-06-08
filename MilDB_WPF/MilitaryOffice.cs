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

    public ObservableCollection<Conscript> FilterConscripts(string criteria, string value)
    {
        if (string.IsNullOrEmpty(value))
            return new ObservableCollection<Conscript>(Conscripts);

        switch (criteria)
        {
            case "FullName":
                return new ObservableCollection<Conscript>(
                    Conscripts.Where(c => c.FullName.ToLower().Contains(value.ToLower())));
            case "Address":
                return new ObservableCollection<Conscript>(
                    Conscripts.Where(c => !string.IsNullOrEmpty(c.Address) && c.Address.ToLower().Contains(value.ToLower())));
            case "FitnessCategory":
                return new ObservableCollection<Conscript>(
                    Conscripts.Where(c => c.FitnessCategory.ToLower().Contains(value.ToLower())));
            case "Status":
                return new ObservableCollection<Conscript>(
                    Conscripts.Where(c => c.Status.ToLower().Contains(value.ToLower())));
            default:
                return new ObservableCollection<Conscript>(Conscripts);
        }
    }

    public ObservableCollection<Officer> FilterOfficers(string criteria, string value)
    {
        if (string.IsNullOrEmpty(value))
            return new ObservableCollection<Officer>(Officers);

        switch (criteria)
        {
            case "FullName":
                return new ObservableCollection<Officer>(
                    Officers.Where(o => o.FullName.ToLower().Contains(value.ToLower())));
            case "Rank":
                return new ObservableCollection<Officer>(
                    Officers.Where(o => o.Rank.ToLower().Contains(value.ToLower())));
            case "YearsOfService":
                if (int.TryParse(value, out int years))
                    return new ObservableCollection<Officer>(
                        Officers.Where(o => o.YearsOfService == years));
                return new ObservableCollection<Officer>(Officers);
            default:
                return new ObservableCollection<Officer>(Officers);
        }
     }
}