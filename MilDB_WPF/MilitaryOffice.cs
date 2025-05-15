using System.IO;
using Newtonsoft.Json;

namespace MilDB_WPF;

internal class MilitaryOffice
    {
        // Властивості
        private string _name;
        private string _address;
        private string _serviceArea;
        private ConscriptList _conscripts;
        private OfficerList _officers;

        private bool _nameValid = true;
        private bool _addressValid = true;
        private bool _serviceAreaValid = true;

        public string? _check = null;

        public string Name
        {
            get => _name;
            set
            {
                _nameValid = true;
                if (string.IsNullOrWhiteSpace(value))
                {
                    _check += "✕ Назва військкомату не може бути порожньою.";
                    _nameValid = false;
                }
                else _name = value;
            }
        }

        public string Address
        {
            get => _address;
            set
            {
                _addressValid = true;
                if (string.IsNullOrWhiteSpace(value))
                {
                    _check += "\n✕ Адреса не може бути порожньою.";
                    _addressValid = false;
                }
                else _address = value;
            }
        }

        public string ServiceArea
        {
            get => _serviceArea;
            set
            {
                _serviceAreaValid = true;
                if (string.IsNullOrWhiteSpace(value))
                {
                    _check += "\n✕ Район обслуговування не може бути порожнім.";
                    _serviceAreaValid = false;
                }
                else _serviceArea = value;
            }
        }

        public ConscriptList Conscripts
        {
            get => _conscripts;
            private set => _conscripts = value;
        }

        public OfficerList Officers
        {
            get => _officers;
            private set => _officers = value;
        }

        // Конструктори
        public MilitaryOffice(string name, string address, string serviceArea)
        {
            _check = null;
            Name = name;
            Address = address;
            ServiceArea = serviceArea;
            Conscripts = new ConscriptList();
            Officers = new OfficerList();
        }

        public MilitaryOffice()
        {
            Conscripts = new ConscriptList();
            Officers = new OfficerList();
        }

        // Методи
        public List<Conscript> GetConscriptsList() => Conscripts.Conscripts;

        public List<Officer> GetOfficersList() => Officers.Officers;

        public bool Valid()
        {
            return _nameValid && _addressValid && _serviceAreaValid;
        }

        public string GetCheck() => _check;

        // Методи пошуку
        public List<Conscript> FindConscriptsByFitnessCategory(string category)
        {
            return Conscripts.Conscripts.FindAll(c => c.FitnessCategory == category);
        }

        public List<Conscript> FindConscriptsByStatus(string status)
        {
            return Conscripts.Conscripts.FindAll(c => c.Status == status);
        }

        public List<Conscript> FindConscriptsByAddress(string addressPart)
        {
            return Conscripts.Conscripts.FindAll(c => c.Address.Contains(addressPart));
        }

        public List<Officer> FindOfficersByRank(string rank)
        {
            return Officers.Officers.FindAll(o => o.Rank == rank);
        }

        public List<Officer> FindOfficersByYearsOfService(int minYears, int maxYears)
        {
            return Officers.Officers.FindAll(o => o.YearsOfService >= minYears && o.YearsOfService <= maxYears);
        }

        // Методи для зв'язування призовників з офіцерами
        public void AssignOfficerToConscript(Officer officer, Conscript conscript)
        {
            if (officer != null && conscript != null)
            {
                officer.AssignConscript(conscript);
            }
        }
    }