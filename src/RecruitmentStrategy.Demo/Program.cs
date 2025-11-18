using MediatR;
using Microsoft.Extensions.DependencyInjection;
using RecruitmentStrategy.Application;
using RecruitmentStrategy.Application.Commands;
using RecruitmentStrategy.Core.Models;

namespace RecruitmentStrategy.Demo;

/// <summary>
/// Demo program showing how to use the CQRS + Strategy Pattern implementation
/// </summary>
public static class Program
{
    public static async Task Main(string[] args)
    {
        // Setup Dependency Injection
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddRecruitmentApplication();
        var serviceProvider = services.BuildServiceProvider();

        var mediator = serviceProvider.GetRequiredService<IMediator>();

        Console.WriteLine("=== Recruitment Strategy Demo - CQRS + Strategy Pattern ===\n");

        // Demo Process A
        await DemoProcessA(mediator);
        
        Console.WriteLine("\n" + new string('=', 60) + "\n");
        
        // Demo Process B
        await DemoProcessB(mediator);
    }

    private static async Task DemoProcessA(IMediator mediator)
    {
        Console.WriteLine(">>> Process A: Technical Focus <<<\n");

        var command = new RecruitmentProcessACommand
        {
            CandidateId = Guid.NewGuid(),
            PositionId = Guid.NewGuid(),
            
            // Common fields (90%)
            Candidate = new Candidate
            {
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                PhoneNumber = "+1-555-0123",
                YearsOfExperience = 6,
                Skills = new List<string> { "C#", ".NET", "Azure", "Docker", "Kubernetes" },
                ApplicationDate = DateTime.UtcNow
            },
            Position = new Position
            {
                Id = Guid.NewGuid(),
                Title = "Senior Software Engineer",
                Department = "Engineering",
                RequiredSkills = new List<string> { "C#", ".NET", "Azure" },
                MinimumExperience = 5,
                Location = "Remote"
            },
            RecruitmentChannel = "Referral",
            PriorityLevel = 1,
            SubmittedDate = DateTime.UtcNow,
            RecruiterName = "Alice Smith",
            
            // Process A specific fields (10%)
            RequiresTechnicalAssessment = true,
            PreferredInterviewTime = "Morning (9 AM - 12 PM)",
            CertificationRequirements = new List<string> { "Azure Developer Associate" },
            AdditionalMetadata = new Dictionary<string, string>
            {
                { "certifications", "Azure Developer Associate, AWS Solutions Architect" }
            }
        };

        var result = await mediator.Send(command);

        Console.WriteLine($"Candidate: {command.Candidate.FirstName} {command.Candidate.LastName}");
        Console.WriteLine($"Position: {command.Position.Title}");
        Console.WriteLine($"Process Type: {result.ProcessType}");
        Console.WriteLine($"Final Score: {result.Score}");
        Console.WriteLine($"Approved: {(result.IsApproved ? "✓ YES" : "✗ NO")}");
        Console.WriteLine("\nFeedback:");
        foreach (var feedback in result.Feedback)
        {
            Console.WriteLine($"  • {feedback}");
        }
    }

    private static async Task DemoProcessB(IMediator mediator)
    {
        Console.WriteLine(">>> Process B: Cultural & Team Fit <<<\n");

        var command = new RecruitmentProcessBCommand
        {
            CandidateId = Guid.NewGuid(),
            PositionId = Guid.NewGuid(),
            
            // Common fields (90%)
            Candidate = new Candidate
            {
                Id = Guid.NewGuid(),
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane.smith@example.com",
                PhoneNumber = "+1-555-0456",
                YearsOfExperience = 8,
                Skills = new List<string> { "Team Leadership", "Agile", "Scrum", "Communication" },
                ApplicationDate = DateTime.UtcNow
            },
            Position = new Position
            {
                Id = Guid.NewGuid(),
                Title = "Engineering Manager",
                Department = "Engineering",
                RequiredSkills = new List<string> { "Team Leadership", "Agile" },
                MinimumExperience = 6,
                Location = "Hybrid"
            },
            RecruitmentChannel = "LinkedIn",
            PriorityLevel = 2,
            SubmittedDate = DateTime.UtcNow,
            RecruiterName = "Bob Johnson",
            
            // Process B specific fields (10%)
            RequiresCulturalFitInterview = true,
            TeamSizePreference = 12,
            SoftSkillsEmphasis = new List<string> { "Communication", "Empathy", "Conflict Resolution" },
            AdditionalMetadata = new Dictionary<string, string>
            {
                { "soft_skills", "Excellent communicator with strong leadership qualities" }
            }
        };

        var result = await mediator.Send(command);

        Console.WriteLine($"Candidate: {command.Candidate.FirstName} {command.Candidate.LastName}");
        Console.WriteLine($"Position: {command.Position.Title}");
        Console.WriteLine($"Process Type: {result.ProcessType}");
        Console.WriteLine($"Final Score: {result.Score}");
        Console.WriteLine($"Approved: {(result.IsApproved ? "✓ YES" : "✗ NO")}");
        Console.WriteLine("\nFeedback:");
        foreach (var feedback in result.Feedback)
        {
            Console.WriteLine($"  • {feedback}");
        }
    }
}
