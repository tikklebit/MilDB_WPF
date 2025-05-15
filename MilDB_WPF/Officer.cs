namespace MilDB_WPF;

internal class Officer
{
    private string _fullName;
    private string _rank;
    private int _experience;
    
    private bool _fullNameValid = true;
    private bool _rankValid = true;
    private bool _experienceValid = true;

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
                _check = "\n✕ Оберіть звання!";
                _rankValid = false;
            }
            else _rank = value;
        }
    }
    
    public int Experience { get; set; }

    public Officer(string fullName, string rank, int experience)
    {
        _check = null;
        FullName = fullName;
        Rank = rank;
        Experience = experience;
    }
    
    public Officer() {}

    public bool Valid()
    {
        return _fullNameValid && _rankValid && _experienceValid;
    }
    
    public string GetCheck() => _check;
}