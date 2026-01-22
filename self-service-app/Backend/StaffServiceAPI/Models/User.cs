namespace StaffServiceAPI.Models;

public class User
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string ImmuneStatus { get; set; } // "Immune", "Non-Immune", or "Unknown"
    public required string PhoneNumber { get; set; }
    public int YearOfBirth { get; set; }
    public required string VaccineDate { get; set; } // 'YYYY-MM-DD' or 'N/A'
}
