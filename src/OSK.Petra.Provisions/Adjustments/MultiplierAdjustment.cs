using OSK.Petra.Provisions.Models;
using System;
using System.Linq;

namespace OSK.Petra.Provisions.Adjustments;

/// <summary>
/// Create a multiplier adjustment for a expenditure or recovery calculation
/// </summary>
/// <remarks>
/// 💡Notes:
/// <list type="bullet">
/// <item>A null or empty filter will result in the adjustment being applied to ALL provisions</item>
/// </list>
/// </remarks>
/// <param name="multiplier">The multiplier adjustment to use</param>
/// <param name="provisionFilter">A filter that cna be applied to limit the total number of provisions adjusted by this flat adjustment</param>11
public readonly struct MultiplierAdjustment(float multiplier, Guid[]? provisionFilter = null) : IProvisionAdjustment
{
    #region IProvisionAdjustment

    /// <inheritdoc/>
    public Provision ApplyAdjustment(Provision provision)
        => provisionFilter is null || provisionFilter.Length is 0 || provisionFilter.Contains(provision.Id)
            ? provision.WithAmount(provision.Amount * multiplier)
            : provision;

    #endregion
}
