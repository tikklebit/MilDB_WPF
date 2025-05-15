using System;

namespace MilDB_WPF;

public class Conscript
{
    public string FullName { get; set; }
    public DateTime BirthDate { get; set; }
    public string Address { get; set; }
    public string HealthStatus { get; set; }
    public string FitnessCategory { get; set; }
    public string Status { get; set; }

    public string FormattedBirthDate => BirthDate.ToString("yyyy.MM.dd");

    public Conscript()
    {
        FullName = string.Empty;
        BirthDate = DateTime.Now;
        Address = string.Empty;
        HealthStatus = string.Empty;
        FitnessCategory = string.Empty;
        Status = string.Empty;
    }

    public Conscript(string fullName, DateTime birthDate, string address, string healthStatus, string fitnessCategory, string status)
    {
        FullName = fullName;
        BirthDate = birthDate;
        Address = address;
        HealthStatus = healthStatus;
        FitnessCategory = fitnessCategory;
        Status = status;
    }

    public bool Valid()
    {
        int age = DateTime.Now.Year - BirthDate.Year;
        if (DateTime.Now.DayOfYear < BirthDate.DayOfYear) age--;

        return !string.IsNullOrWhiteSpace(FullName)
            && age >= 18 && age <= 27
            && !string.IsNullOrWhiteSpace(Address)
            && !string.IsNullOrWhiteSpace(HealthStatus)
            && !string.IsNullOrWhiteSpace(FitnessCategory)
            && !string.IsNullOrWhiteSpace(Status);
    }
}
