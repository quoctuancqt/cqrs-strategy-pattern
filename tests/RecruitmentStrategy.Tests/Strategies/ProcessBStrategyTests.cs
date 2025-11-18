using FluentAssertions;
using RecruitmentStrategy.Application.Commands;
using RecruitmentStrategy.Application.Strategies;
using RecruitmentStrategy.Core.Models;

namespace RecruitmentStrategy.Tests.Strategies;

public class ProcessBStrategyTests
{
    private readonly ProcessBStrategy _strategy;

    public ProcessBStrategyTests()
    {
        _strategy = new ProcessBStrategy();
    }

    [Fact]
    public async Task ApplySpecificRules_WithCulturalFitInterview_ShouldAddBonusPoints()
    {
        // Arrange
        var command = new RecruitmentProcessBCommand
        {
            CandidateId = Guid.NewGuid(),
            PositionId = Guid.NewGuid(),
            RequiresCulturalFitInterview = true,
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
        result.Score.Should().Be(58); // 50 + 8 bonus
        result.Feedback.Should().Contain("Cultural fit interview scheduled");
        result.ProcessType.Should().Be("Process B - Cultural & Team Fit");
    }

    [Fact]
    public async Task ApplySpecificRules_WithLargeTeamPreference_ShouldAddBonusPoints()
    {
        // Arrange
        var command = new RecruitmentProcessBCommand
        {
            CandidateId = Guid.NewGuid(),
            PositionId = Guid.NewGuid(),
            TeamSizePreference = 10,
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
        result.Score.Should().Be(55); // 50 + 5 bonus for large team
        result.Feedback.Should().Contain("Preferred team size: 10");
        result.Feedback.Should().Contain("Large team experience preferred");
    }

    [Fact]
    public async Task ApplySpecificRules_WithSoftSkills_ShouldAddBonusPoints()
    {
        // Arrange
        var command = new RecruitmentProcessBCommand
        {
            CandidateId = Guid.NewGuid(),
            PositionId = Guid.NewGuid(),
            SoftSkillsEmphasis = new List<string> { "Communication", "Leadership" },
            AdditionalMetadata = new Dictionary<string, string>
            {
                { "soft_skills", "excellent" }
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
        result.Score.Should().Be(62); // 50 + 12 bonus
        result.Feedback.Should().Contain("Emphasis on 2 soft skills");
        result.Feedback.Should().Contain("Candidate demonstrates strong soft skills");
    }
}
