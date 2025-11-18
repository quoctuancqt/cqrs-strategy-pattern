using RecruitmentStrategy.Application.Commands;
using RecruitmentStrategy.Core.Models;

namespace RecruitmentStrategy.Application.Strategies;

/// <summary>
/// Strategy interface for process-specific recruitment rules (10% unique logic)
/// Non-generic version for runtime strategy selection
/// </summary>
public interface IRecruitmentSpecificRulesStrategy
{
    /// <summary>
    /// Determines if this strategy can handle the given command
    /// </summary>
    bool CanHandle(RecruitmentBaseCommand command);
    
    /// <summary>
    /// Apply process-specific rules (10% unique business logic)
    /// </summary>
    Task<RecruitmentResult> ApplySpecificRules(RecruitmentBaseCommand command, RecruitmentResult baseResult);
}
