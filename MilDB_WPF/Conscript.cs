using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace MilDB_WPF
{
    internal class Conscript
    {
        // Властивості
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
                if (_fullNameValid) _fullName = value;
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
                else _birthDate = value;
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
                if (_addressValid) _address = value;
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
                else _healthStatus = value;
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
                else _fitnessCategory = value;
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
                else _status = value;
            }
        }

        public string FormattedBirthDate => $"{BirthDate.Year}.{BirthDate.Month:D2}.{BirthDate.Day:D2}";

        // Конструктори
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

        // Методи перевірки
        public bool Valid()
        {
            return _fullNameValid && _birthDateValid && _addressValid && _healthStatusValid && _fitnessCategoryValid && _statusValid;
        }

        public string GetCheck() => _check;
    }

    internal class ConscriptList : IComparable<ConscriptList>, IEnumerable<Conscript>, IEnumerator<Conscript>
    {
        // Властивості
        private string _file = "conscripts.json";
        public List<Conscript> Conscripts { get; set; } = new List<Conscript>();
        public List<Conscript> Buffered { get; set; } = new List<Conscript>();
        private int _position = -1;

        // Конструктор
        public ConscriptList() => Deserialize();

        // Методи для роботи зі списком
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

        public Conscript? GetConscriptByIndex(int index)
        {
            if (index >= 0 && index < Conscripts.Count) return Conscripts[index];
            return null;
        }

        public int GetTableIndex(int index) => index;

        // Методи для роботи з файлами
        public void Serialize()
        {
            string json = JsonConvert.SerializeObject(Conscripts, Formatting.Indented);
            File.WriteAllText(_file, json);
        }

        public void Deserialize()
        {
            if (!File.Exists(_file)) File.WriteAllText(_file, "[]");

            string jsonRead = File.ReadAllText(_file);
            Conscripts.Clear();
            Conscripts = JsonConvert.DeserializeObject<List<Conscript>>(jsonRead) ?? new List<Conscript>();
        }

        // Інтерфейс IComparable
        public int CompareTo(ConscriptList? other)
        {
            if (other == null) return 1;
            return 0;
        }

        public int CompareTo(object? obj)
        {
            if (obj is ConscriptList other) 
            {
                return CompareTo(other);
            }
            throw new ArgumentException("Об'єкт не відноситься до ConscriptList!");
        }

        public void Buffer()
        {
            Buffered.Clear();
            foreach (var conscript in Conscripts)
            {
                Buffered.Add(conscript);
            }
        }

        public void Default() => Conscripts = Buffered;

        // Інтерфейс IEnumerable
        public IEnumerator<Conscript> GetEnumerator() => this;
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        // Інтерфейс IEnumerator
        public Conscript Current => Conscripts[_position];
        object IEnumerator.Current => Current;
        public bool MoveNext() => ++_position < Conscripts.Count;
        public void Reset() => _position = -1;
        public void Dispose() {}
    }
}
