namespace RecruitmentStrategy.Core.Models;

/// <summary>
/// Represents a job position
/// </summary>
public class Position
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public List<string> RequiredSkills { get; set; } = new();
    public int MinimumExperience { get; set; }
    public string Location { get; set; } = string.Empty;
}
