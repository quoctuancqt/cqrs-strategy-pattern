namespace RecruitmentStrategy.Application.Commands;

/// <summary>
/// Process A specific command - contains the 10% unique data for Process A
/// </summary>
public class RecruitmentProcessACommand : RecruitmentBaseCommand
{
    public override string ProcessType => "A";
    
    // Process A specific fields (10% unique logic)
    public bool RequiresTechnicalAssessment { get; set; }
    public string PreferredInterviewTime { get; set; } = string.Empty;
    public List<string> CertificationRequirements { get; set; } = new();
}
