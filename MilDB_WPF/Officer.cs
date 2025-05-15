using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace MilDB_WPF
{
    internal class Officer
    {
        // Властивості
        private string _fullName;
        private string _rank;
        private int _yearsOfService;
        private List<Conscript> _assignedConscripts = new List<Conscript>();

        private bool _fullNameValid = true;
        private bool _rankValid = true;
        private bool _yearsOfServiceValid = true;

        public string? _check = null;

        public string FullName
        {
            get => _fullName;
            set
            {
                _fullNameValid = true;
                if (string.IsNullOrWhiteSpace(value))
                {
                    _check += "✕ Ім'я та прізвище не можуть бути порожніми.";
                    _fullNameValid = false;
                }
                else
                {
                    if (value.Length < 5)
                    {
                        _check += "\n✕ Ім'я та прізвище повинні містити не менше 5 символів.";
                        _fullNameValid = false;
                    }
                    if (!value.Contains(" "))
                    {
                        _check += "\n✕ Ім'я та прізвище повинні бути розділені пробілом.";
                        _fullNameValid = false;
                    }
                }
                if (_fullNameValid) _fullName = value;
            }
        }

        public string Rank
        {
            get => _rank;
            set
            {
                _rankValid = true;
                if (string.IsNullOrWhiteSpace(value))
                {
                    _check += "\n✕ Звання не може бути порожнім.";
                    _rankValid = false;
                }
                else _rank = value;
            }
        }

        public int YearsOfService
        {
            get => _yearsOfService;
            set
            {
                _yearsOfServiceValid = true;
                if (value < 0 || value > 40)
                {
                    _check += "\n✕ Стаж роботи повинен бути від 0 до 40 років.";
                    _yearsOfServiceValid = false;
                }
                else _yearsOfService = value;
            }
        }

        public List<Conscript> AssignedConscripts
        {
            get => _assignedConscripts;
            set => _assignedConscripts = value ?? new List<Conscript>();
        }

        // Конструктори
        public Officer(string fullName, string rank, int yearsOfService)
        {
            _check = null;
            FullName = fullName;
            Rank = rank;
            YearsOfService = yearsOfService;
            AssignedConscripts = new List<Conscript>();
        }

        public Officer() { }

        // Методи
        public void AssignConscript(Conscript conscript)
        {
            if (conscript != null && !AssignedConscripts.Contains(conscript))
            {
                AssignedConscripts.Add(conscript);
            }
        }

        public void RemoveConscript(Conscript conscript)
        {
            AssignedConscripts.Remove(conscript);
        }

        public bool Valid()
        {
            return _fullNameValid && _rankValid && _yearsOfServiceValid;
        }

        public string GetCheck() => _check;
    }

    internal class OfficerList
    {
        // Властивості
        private string _file = "officers.json";
        public List<Officer> Officers { get; set; } = new List<Officer>();
        public List<Officer> Buffered { get; set; } = new List<Officer>();

        // Конструктор
        public OfficerList() => Deserialize();

        // Методи для роботи зі списком
        public void AddOfficer(Officer officer)
        {
            if (officer.Valid())
            {
                Officers.Add(officer);
                Serialize();
            }
        }

        public void RemoveOfficer(Officer officer)
        {
            Officers.Remove(officer);
            Serialize();
        }

        public void EditOfficer(Officer officer, int index)
        {
            if (officer.Valid())
            {
                Officers[index] = officer;
                Serialize();
            }
        }

        public Officer? GetOfficerByIndex(int index)
        {
            if (index >= 0 && index < Officers.Count) return Officers[index];
            return null;
        }

        public int GetTableIndex(int index) => index;

        // Методи для роботи з файлами
        public void Serialize()
        {
            string json = JsonConvert.SerializeObject(Officers, Formatting.Indented);
            File.WriteAllText(_file, json);
        }

        public void Deserialize()
        {
            if (!File.Exists(_file)) File.WriteAllText(_file, "[]");

            string jsonRead = File.ReadAllText(_file);
            Officers.Clear();
            Officers = JsonConvert.DeserializeObject<List<Officer>>(jsonRead) ?? new List<Officer>();
        }
    }
}