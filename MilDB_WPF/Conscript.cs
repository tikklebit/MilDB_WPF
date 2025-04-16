using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace MilDB_WPF
{
    internal class Conscript
    {
        private string _fullName;
        private DateTime _birthDate;
        private string _address;
        private string _healthStatus;
        private string _fitnessCategory;
        private string _status;

        private bool _fullNameValid = true;
        private bool _birthDateValid = true;
        private bool _addressValid = true;
        private bool _healthStatusValid = true;
        private bool _fitnessCategoryValid = true;
        private bool _statusValid = true;

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
                if (_fullNameValid)
                    _fullName = value;
            }
        }

        public DateTime BirthDate
        {
            get => _birthDate;
            set
            {
                _birthDateValid = true;
                int age = DateTime.Now.Year - value.Year;
                if (DateTime.Now.DayOfYear < value.DayOfYear) age--;

                if (age < 18 || age > 27)
                {
                    _check += "\n✕ Дата народження повинна відповідати віку від 18 до 27 років.";
                    _birthDateValid = false;
                }
                else
                {
                    _birthDate = value;
                }
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
                if (_addressValid)
                    _address = value;
            }
        }

        public string HealthStatus
        {
            get => _healthStatus;
            set
            {
                _healthStatusValid = true;
                if (string.IsNullOrWhiteSpace(value))
                {
                    _check += "\n✕ Стан здоров'я не може бути порожнім.";
                    _healthStatusValid = false;
                }
                else
                {
                    _healthStatus = value;
                }
            }
        }

        public string FitnessCategory
        {
            get => _fitnessCategory;
            set
            {
                _fitnessCategoryValid = true;
                if (string.IsNullOrWhiteSpace(value))
                {
                    _check += "\n✕ Оберіть категорію придатності!";
                    _fitnessCategoryValid = false;
                }
                else
                {
                    _fitnessCategory = value;
                }
            }
        }

        public string Status
        {
            get => _status;
            set
            {
                _statusValid = true;
                if (string.IsNullOrWhiteSpace(value))
                {
                    _check += "\n✕ Оберіть статус!";
                    _statusValid = false;
                }
                else
                {
                    _status = value;
                }
            }
        }

        public Conscript(string fullName, DateTime birthDate, string address, string healthStatus, string fitnessCategory, string status)
        {
            _check = null;
            FullName = fullName;
            BirthDate = birthDate;
            Address = address;
            HealthStatus = healthStatus;
            FitnessCategory = fitnessCategory;
            Status = status;
        }

        public Conscript() { }

        public bool Valid()
        {
            return _fullNameValid && _birthDateValid && _addressValid && _healthStatusValid && _fitnessCategoryValid && _statusValid;
        }

        public string GetCheck() => _check;

        public void ShowConscript(out string? text, out string? text2, out string? text3)
        {
            string birthdate = $"{BirthDate.Year}.{BirthDate.Month}.{BirthDate.Day}";
            text = $"{FullName}    Дата народження: {birthdate}    Адреса проживання: {Address}";
            text2 = $"Стан здоров'я: {HealthStatus}";
            text3 = $"Категорія придатності: {FitnessCategory}    Статус: {Status}";
        }
    }

    internal class ConscriptList
    {
        private string _file = "conscripts.json";
        public List<Conscript> Conscripts { get; set; } = new List<Conscript>();

        public ConscriptList()
        {
            Deserialize();
        }

        public void AddConscript(Conscript conscript)
        {
            if (conscript.Valid())
            {
                Conscripts.Add(conscript);
                Serialize();
            }
        }

        public void RemoveConscript(Conscript conscript)
        {
            Conscripts.Remove(conscript);
            Serialize();
        }

        public void EditConscript(Conscript conscript, int index)
        {
            if (conscript.Valid())
            {
                Conscripts[index] = conscript;
                Serialize();
            }
        }

        public void Serialize()
        {
            string json = JsonConvert.SerializeObject(Conscripts, Formatting.Indented);
            File.WriteAllText(_file, json);
        }

        public void Deserialize()
        {
            if (File.Exists(_file))
            {
                string jsonRead = File.ReadAllText(_file);
                Conscripts.Clear();
                Conscripts = JsonConvert.DeserializeObject<List<Conscript>>(jsonRead);
            }
            else
            {
                File.WriteAllText(_file, "[]");
            }
        }

        public Conscript? GetConscriptByIndex(int index)
        {
            if (index >= 0 && index < Conscripts.Count)
            {
                return Conscripts[index];
            }
            return null;
        }

        public int GetTableIndex(int index)
        {
            return index;
        }
    }
}
