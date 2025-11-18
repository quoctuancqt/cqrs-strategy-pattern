using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using RecruitmentStrategy.Application.Commands;
using RecruitmentStrategy.Application.Strategies;
using RecruitmentStrategy.Core.Models;

namespace RecruitmentStrategy.Tests.Strategies;

public class ProcessAStrategyTests
{
    private readonly ProcessAStrategy _strategy;

    public ProcessAStrategyTests()
    {
        _strategy = new ProcessAStrategy();
    }

    [Fact]
    public async Task ApplySpecificRules_WithTechnicalAssessment_ShouldAddBonusPoints()
    {
        // Arrange
        var command = new RecruitmentProcessACommand
        {
            CandidateId = Guid.NewGuid(),
            PositionId = Guid.NewGuid(),
            RequiresTechnicalAssessment = true,
            Candidate = new Candidate(),
            Position = new Position()
        };

        var baseResult = new RecruitmentResult
        {
            CandidateId = command.CandidateId,
            PositionId = command.PositionId,
            Score = 50,
            Feedback = new List<string>()
        };

        // Act
        var result = await _strategy.ApplySpecificRules(command, baseResult);

        // Assert
        result.Score.Should().Be(60); // 50 + 10 bonus
        result.Feedback.Should().Contain("Technical assessment scheduled");
        result.ProcessType.Should().Be("Process A - Technical Focus");
    }

    [Fact]
    public async Task ApplySpecificRules_WithCertifications_ShouldAddFeedbackAndPoints()
    {
        // Arrange
        var command = new RecruitmentProcessACommand
        {
            CandidateId = Guid.NewGuid(),
            PositionId = Guid.NewGuid(),
            RequiresTechnicalAssessment = false,
            CertificationRequirements = new List<string> { "AWS", "Azure" },
            AdditionalMetadata = new Dictionary<string, string>
            {
                { "certifications", "AWS, Azure, GCP" }
            },
            Candidate = new Candidate(),
            Position = new Position()
        };

        var baseResult = new RecruitmentResult
        {
            CandidateId = command.CandidateId,
            PositionId = command.PositionId,
            Score = 50,
            Feedback = new List<string>()
        };

        // Act
        var result = await _strategy.ApplySpecificRules(command, baseResult);

        // Assert
        result.Score.Should().Be(65); // 50 + 15 bonus
        result.Feedback.Should().Contain("Requires 2 certifications");
        result.Feedback.Should().Contain("Candidate has relevant certifications");
    }

    [Fact]
    public async Task ApplySpecificRules_WithPreferredInterviewTime_ShouldAddToFeedback()
    {
        // Arrange
        var command = new RecruitmentProcessACommand
        {
            CandidateId = Guid.NewGuid(),
            PositionId = Guid.NewGuid(),
            PreferredInterviewTime = "10:00 AM - 12:00 PM",
            Candidate = new Candidate(),
            Position = new Position()
        };

        var baseResult = new RecruitmentResult
        {
            CandidateId = command.CandidateId,
            PositionId = command.PositionId,
            Score = 50,
            Feedback = new List<string>()
        };

        // Act
        var result = await _strategy.ApplySpecificRules(command, baseResult);

        // Assert
        result.Feedback.Should().Contain("Preferred interview time: 10:00 AM - 12:00 PM");
    }
}
