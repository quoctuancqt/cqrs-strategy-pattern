namespace RecruitmentStrategy.Core.Models;

/// <summary>
/// Represents the result of a recruitment process
/// </summary>
public class RecruitmentResult
{
    public Guid CandidateId { get; set; }
    public Guid PositionId { get; set; }
    public bool IsApproved { get; set; }
    public int Score { get; set; }
    public List<string> Feedback { get; set; } = new();
    public DateTime ProcessedDate { get; set; }
    public string ProcessType { get; set; } = string.Empty;
}
