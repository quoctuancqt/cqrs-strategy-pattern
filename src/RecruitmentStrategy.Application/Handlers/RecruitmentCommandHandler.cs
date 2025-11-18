using MediatR;
using Microsoft.Extensions.Logging;
using RecruitmentStrategy.Application.Commands;
using RecruitmentStrategy.Application.Strategies;
using RecruitmentStrategy.Core.Models;

namespace RecruitmentStrategy.Application.Handlers;

/// <summary>
/// Generic Mediator handler that routes to the appropriate strategy based on command type
/// This is the single entry point for all recruitment commands
/// </summary>
public class RecruitmentCommandHandler : IRequestHandler<RecruitmentBaseCommand, RecruitmentResult>
{
    private readonly IEnumerable<IRecruitmentSpecificRulesStrategy> _strategies;
    private readonly ILogger<RecruitmentCommandHandler> _logger;

    public RecruitmentCommandHandler(
        IEnumerable<IRecruitmentSpecificRulesStrategy> strategies,
        ILogger<RecruitmentCommandHandler> logger)
    {
        _strategies = strategies;
        _logger = logger;
    }

    public async Task<RecruitmentResult> Handle(
        RecruitmentBaseCommand request, 
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing recruitment for candidate {CandidateId} using Process {ProcessType}", 
            request.CandidateId, request.ProcessType);

        // Step 1: Apply 90% common logic
        var baseResult = await ApplyCommonRecruitmentLogic(request);

        // Step 2: Find and apply the appropriate strategy (10% specific logic)
        var strategy = _strategies.FirstOrDefault(s => s.CanHandle(request));
        
        if (strategy == null)
        {
            throw new InvalidOperationException($"No handler found for process type: {request.ProcessType}");
        }

        var finalResult = await strategy.ApplySpecificRules(request, baseResult);

        _logger.LogInformation("Process {ProcessType} completed. Final score: {Score}, Approved: {IsApproved}", 
            request.ProcessType, finalResult.Score, finalResult.IsApproved);

        return finalResult;
    }

    /// <summary>
    /// Apply common recruitment logic (90% shared business logic)
    /// </summary>
    private async Task<RecruitmentResult> ApplyCommonRecruitmentLogic(RecruitmentBaseCommand command)
    {
        _logger.LogInformation("Starting common recruitment logic for candidate {CandidateId}", 
            command.CandidateId);

        var result = new RecruitmentResult
        {
            CandidateId = command.CandidateId,
            PositionId = command.PositionId,
            ProcessedDate = DateTime.UtcNow,
            Score = 0,
            Feedback = new List<string>()
        };

        // 1. Validate basic candidate information
        ValidateCandidateBasicInfo(command, result);

        // 2. Check experience requirements
        await CheckExperienceRequirements(command, result);

        // 3. Evaluate skills match
        await EvaluateSkillsMatch(command, result);

        // 4. Apply priority level adjustments
        ApplyPriorityAdjustments(command, result);

        // 5. Validate recruitment channel
        ValidateRecruitmentChannel(command, result);

        // 6. Calculate preliminary approval
        result.IsApproved = result.Score >= 60; // 60% threshold for approval

        _logger.LogInformation("Common recruitment logic completed. Score: {Score}, Approved: {IsApproved}", 
            result.Score, result.IsApproved);

        return result;
    }

    private static void ValidateCandidateBasicInfo(RecruitmentBaseCommand command, RecruitmentResult result)
    {
        var candidate = command.Candidate;
        
        if (string.IsNullOrEmpty(candidate.Email) || string.IsNullOrEmpty(candidate.PhoneNumber))
        {
            result.Feedback.Add("Warning: Incomplete contact information");
            result.Score -= 5;
        }
        else
        {
            result.Feedback.Add("Contact information complete");
            result.Score += 5;
        }
    }

    private static Task CheckExperienceRequirements(RecruitmentBaseCommand command, RecruitmentResult result)
    {
        var candidate = command.Candidate;
        var position = command.Position;

        if (candidate.YearsOfExperience >= position.MinimumExperience)
        {
            var bonus = Math.Min((candidate.YearsOfExperience - position.MinimumExperience) * 2, 20);
            result.Score += 20 + bonus;
            result.Feedback.Add($"Experience requirement met (+{20 + bonus} points)");
        }
        else
        {
            result.Score += 5; // Partial credit
            result.Feedback.Add($"Experience below requirement (-15 points)");
        }

        return Task.CompletedTask;
    }

    private static Task EvaluateSkillsMatch(RecruitmentBaseCommand command, RecruitmentResult result)
    {
        var candidateSkills = command.Candidate.Skills;
        var requiredSkills = command.Position.RequiredSkills;

        if (!requiredSkills.Any())
        {
            result.Feedback.Add("No specific skills required");
            return Task.CompletedTask;
        }

        var matchedSkills = candidateSkills.Intersect(requiredSkills, StringComparer.OrdinalIgnoreCase).ToList();
        var matchPercentage = (double)matchedSkills.Count / requiredSkills.Count * 100;

        var skillScore = (int)(matchPercentage * 0.3); // Max 30 points for skills
        result.Score += skillScore;
        
        result.Feedback.Add($"Skills match: {matchPercentage:F1}% ({matchedSkills.Count}/{requiredSkills.Count}) (+{skillScore} points)");

        return Task.CompletedTask;
    }

    private static void ApplyPriorityAdjustments(RecruitmentBaseCommand command, RecruitmentResult result)
    {
        if (command.PriorityLevel >= 1 && command.PriorityLevel <= 5)
        {
            var priorityBonus = (6 - command.PriorityLevel) * 2; // Higher priority = more points
            result.Score += priorityBonus;
            result.Feedback.Add($"Priority level {command.PriorityLevel} (+{priorityBonus} points)");
        }
    }

    private static void ValidateRecruitmentChannel(RecruitmentBaseCommand command, RecruitmentResult result)
    {
        var validChannels = new[] { "LinkedIn", "Referral", "Direct", "Agency", "JobBoard" };
        
        if (validChannels.Contains(command.RecruitmentChannel, StringComparer.OrdinalIgnoreCase))
        {
            result.Feedback.Add($"Valid recruitment channel: {command.RecruitmentChannel}");
            
            // Referrals get bonus points
            if (command.RecruitmentChannel.Equals("Referral", StringComparison.OrdinalIgnoreCase))
            {
                result.Score += 10;
                result.Feedback.Add("Referral bonus (+10 points)");
            }
        }
        else
        {
            result.Feedback.Add($"Warning: Unrecognized recruitment channel: {command.RecruitmentChannel}");
        }
    }
}
