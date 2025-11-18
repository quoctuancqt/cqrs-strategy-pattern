namespace RecruitmentStrategy.Core.Models;

/// <summary>
/// Represents a candidate in the recruitment process
/// </summary>
public class Candidate
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public int YearsOfExperience { get; set; }
    public List<string> Skills { get; set; } = new();
    public DateTime ApplicationDate { get; set; }
}
