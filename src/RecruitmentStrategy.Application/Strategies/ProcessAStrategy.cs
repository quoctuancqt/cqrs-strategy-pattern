using RecruitmentStrategy.Application.Commands;
using RecruitmentStrategy.Core.Models;

namespace RecruitmentStrategy.Application.Strategies;

/// <summary>
/// Strategy for Process A specific rules (10% unique logic)
/// </summary>
public class ProcessAStrategy : IRecruitmentSpecificRulesStrategy
{
    public bool CanHandle(RecruitmentBaseCommand command)
    {
        return command.ProcessType == "A";
    }

    public async Task<RecruitmentResult> ApplySpecificRules(
        RecruitmentBaseCommand command, 
        RecruitmentResult baseResult)
    {
        if (command is not RecruitmentProcessACommand processACommand)
        {
            throw new InvalidOperationException($"Expected {nameof(RecruitmentProcessACommand)} but got {command.GetType().Name}");
        }

        // Process A specific logic (10%)
        var feedback = new List<string>(baseResult.Feedback);

        // Technical assessment requirement check
        if (processACommand.RequiresTechnicalAssessment)
        {
            feedback.Add("Technical assessment scheduled");
            baseResult.Score += 10; // Bonus for technical rigor
        }

        // Certification requirements validation
        if (processACommand.CertificationRequirements.Any())
        {
            feedback.Add($"Requires {processACommand.CertificationRequirements.Count} certifications");
            
            // Check if candidate has certifications in metadata
            if (command.AdditionalMetadata.ContainsKey("certifications"))
            {
                feedback.Add("Candidate has relevant certifications");
                baseResult.Score += 15;
            }
        }

        // Preferred interview time consideration
        if (!string.IsNullOrEmpty(processACommand.PreferredInterviewTime))
        {
            feedback.Add($"Preferred interview time: {processACommand.PreferredInterviewTime}");
        }

        baseResult.Feedback = feedback;
        baseResult.ProcessType = "Process A - Technical Focus";

        return await Task.FromResult(baseResult);
    }
}
