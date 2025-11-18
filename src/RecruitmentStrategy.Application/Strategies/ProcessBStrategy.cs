using RecruitmentStrategy.Application.Commands;
using RecruitmentStrategy.Core.Models;

namespace RecruitmentStrategy.Application.Strategies;

/// <summary>
/// Strategy for Process B specific rules (10% unique logic)
/// </summary>
public class ProcessBStrategy : IRecruitmentSpecificRulesStrategy
{
    public bool CanHandle(RecruitmentBaseCommand command)
    {
        return command.ProcessType == "B";
    }

    public async Task<RecruitmentResult> ApplySpecificRules(
        RecruitmentBaseCommand command, 
        RecruitmentResult baseResult)
    {
        if (command is not RecruitmentProcessBCommand processBCommand)
        {
            throw new InvalidOperationException($"Expected {nameof(RecruitmentProcessBCommand)} but got {command.GetType().Name}");
        }

        // Process B specific logic (10%)
        var feedback = new List<string>(baseResult.Feedback);

        // Cultural fit interview requirement
        if (processBCommand.RequiresCulturalFitInterview)
        {
            feedback.Add("Cultural fit interview scheduled");
            baseResult.Score += 8; // Bonus for cultural alignment
        }

        // Team size preference consideration
        if (processBCommand.TeamSizePreference > 0)
        {
            feedback.Add($"Preferred team size: {processBCommand.TeamSizePreference}");
            
            // Adjust score based on team collaboration preference
            if (processBCommand.TeamSizePreference > 5)
            {
                feedback.Add("Large team experience preferred");
                baseResult.Score += 5;
            }
        }

        // Soft skills emphasis
        if (processBCommand.SoftSkillsEmphasis.Any())
        {
            feedback.Add($"Emphasis on {processBCommand.SoftSkillsEmphasis.Count} soft skills");
            
            // Check candidate metadata for soft skills
            if (command.AdditionalMetadata.ContainsKey("soft_skills"))
            {
                feedback.Add("Candidate demonstrates strong soft skills");
                baseResult.Score += 12;
            }
        }

        baseResult.Feedback = feedback;
        baseResult.ProcessType = "Process B - Cultural & Team Fit";

        return await Task.FromResult(baseResult);
    }
}
