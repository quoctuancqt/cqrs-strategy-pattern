using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using RecruitmentStrategy.Application;
using RecruitmentStrategy.Application.Commands;
using RecruitmentStrategy.Core.Models;

namespace RecruitmentStrategy.Tests.Integration;

/// <summary>
/// Integration tests demonstrating the full CQRS + Strategy Pattern flow with generic handler
/// </summary>
public class RecruitmentProcessIntegrationTests
{
    private readonly IServiceProvider _serviceProvider;

    public RecruitmentProcessIntegrationTests()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddRecruitmentApplication();
        _serviceProvider = services.BuildServiceProvider();
    }

    [Fact]
    public async Task ProcessA_EndToEnd_ShouldProcessSuccessfully()
    {
        // Arrange
        var mediator = _serviceProvider.GetRequiredService<IMediator>();
        
        var command = new RecruitmentProcessACommand
        {
            CandidateId = Guid.NewGuid(),
            PositionId = Guid.NewGuid(),
            Candidate = new Candidate
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                PhoneNumber = "+1234567890",
                YearsOfExperience = 5,
                Skills = new List<string> { "C#", ".NET", "Azure" }
            },
            Position = new Position
            {
                Title = "Senior Developer",
                Department = "Engineering",
                RequiredSkills = new List<string> { "C#", ".NET" },
                MinimumExperience = 3
            },
            RecruitmentChannel = "Referral",
            PriorityLevel = 1,
            RequiresTechnicalAssessment = true,
            CertificationRequirements = new List<string> { "Azure" },
            AdditionalMetadata = new Dictionary<string, string>
            {
                { "certifications", "Azure, AWS" }
            }
        };

        // Act - Send as base command to test generic handler
        RecruitmentBaseCommand baseCommand = command;
        var result = await mediator.Send(baseCommand);

        // Assert
        result.Should().NotBeNull();
        result.CandidateId.Should().Be(command.CandidateId);
        result.PositionId.Should().Be(command.PositionId);
        result.ProcessType.Should().Be("Process A - Technical Focus");
        result.Score.Should().BeGreaterThan(60);
        result.IsApproved.Should().BeTrue();
        result.Feedback.Should().NotBeEmpty();
    }

    [Fact]
    public async Task ProcessB_EndToEnd_ShouldProcessSuccessfully()
    {
        // Arrange
        var mediator = _serviceProvider.GetRequiredService<IMediator>();
        
        var command = new RecruitmentProcessBCommand
        {
            CandidateId = Guid.NewGuid(),
            PositionId = Guid.NewGuid(),
            Candidate = new Candidate
            {
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane.smith@example.com",
                PhoneNumber = "+1987654321",
                YearsOfExperience = 7,
                Skills = new List<string> { "Leadership", "Project Management", "Agile" }
            },
            Position = new Position
            {
                Title = "Team Lead",
                Department = "Product",
                RequiredSkills = new List<string> { "Leadership", "Agile" },
                MinimumExperience = 5
            },
            RecruitmentChannel = "LinkedIn",
            PriorityLevel = 2,
            RequiresCulturalFitInterview = true,
            TeamSizePreference = 8,
            SoftSkillsEmphasis = new List<string> { "Communication", "Collaboration" },
            AdditionalMetadata = new Dictionary<string, string>
            {
                { "soft_skills", "excellent" }
            }
        };

        // Act - Send as base command to test generic handler
        RecruitmentBaseCommand baseCommand = command;
        var result = await mediator.Send(baseCommand);

        // Assert
        result.Should().NotBeNull();
        result.CandidateId.Should().Be(command.CandidateId);
        result.PositionId.Should().Be(command.PositionId);
        result.ProcessType.Should().Be("Process B - Cultural & Team Fit");
        result.Score.Should().BeGreaterThan(60);
        result.IsApproved.Should().BeTrue();
        result.Feedback.Should().NotBeEmpty();
    }

    [Fact]
    public async Task ProcessA_InsufficientExperience_ShouldReject()
    {
        // Arrange
        var mediator = _serviceProvider.GetRequiredService<IMediator>();
        
        var command = new RecruitmentProcessACommand
        {
            CandidateId = Guid.NewGuid(),
            PositionId = Guid.NewGuid(),
            Candidate = new Candidate
            {
                FirstName = "Junior",
                LastName = "Dev",
                Email = "junior@example.com",
                PhoneNumber = "+1111111111",
                YearsOfExperience = 1,
                Skills = new List<string> { "C#" }
            },
            Position = new Position
            {
                Title = "Senior Developer",
                Department = "Engineering",
                RequiredSkills = new List<string> { "C#", ".NET", "Azure", "Microservices" },
                MinimumExperience = 5
            },
            RecruitmentChannel = "JobBoard",
            PriorityLevel = 3,
            RequiresTechnicalAssessment = false
        };

        // Act - Send as base command
        RecruitmentBaseCommand baseCommand = command;
        var result = await mediator.Send(baseCommand);

        // Assert
        result.Should().NotBeNull();
        result.Score.Should().BeLessThan(60);
        result.IsApproved.Should().BeFalse();
    }
    
    [Fact]
    public async Task CanHandle_ProcessAStrategy_ShouldReturnTrueForProcessA()
    {
        // Arrange
        var strategy = new Application.Strategies.ProcessAStrategy();
        var command = new RecruitmentProcessACommand();
        
        // Act
        var canHandle = strategy.CanHandle(command);
        
        // Assert
        canHandle.Should().BeTrue();
    }
    
    [Fact]
    public async Task CanHandle_ProcessBStrategy_ShouldReturnTrueForProcessB()
    {
        // Arrange
        var strategy = new Application.Strategies.ProcessBStrategy();
        var command = new RecruitmentProcessBCommand();
        
        // Act
        var canHandle = strategy.CanHandle(command);
        
        // Assert
        canHandle.Should().BeTrue();
    }
}
