namespace RecruitmentStrategy.Application.Commands;

/// <summary>
/// Process B specific command - contains the 10% unique data for Process B
/// </summary>
public class RecruitmentProcessBCommand : RecruitmentBaseCommand
{
    public override string ProcessType => "B";
    
    // Process B specific fields (10% unique logic)
    public bool RequiresCulturalFitInterview { get; set; }
    public int TeamSizePreference { get; set; }
    public List<string> SoftSkillsEmphasis { get; set; } = new();
}
