using MediatR;
using RecruitmentStrategy.Core.Models;

namespace RecruitmentStrategy.Application.Commands;

/// <summary>
/// Base command containing 90% of common recruitment data for both Process A and Process B
/// </summary>
public abstract class RecruitmentBaseCommand : IRequest<RecruitmentResult>
{
    public Guid CandidateId { get; set; }
    public Guid PositionId { get; set; }
    public Candidate Candidate { get; set; } = null!;
    public Position Position { get; set; } = null!;
    
    // Common fields (90% shared logic)
    public string RecruitmentChannel { get; set; } = string.Empty;
    public DateTime SubmittedDate { get; set; }
    public string RecruiterName { get; set; } = string.Empty;
    public int PriorityLevel { get; set; }
    public Dictionary<string, string> AdditionalMetadata { get; set; } = new();
    
    /// <summary>
    /// Process type identifier for strategy selection
    /// </summary>
    public abstract string ProcessType { get; }
}
