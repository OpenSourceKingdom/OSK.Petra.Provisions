using OSK.Petra.Provisions.Models;
using System;
using System.Linq;

namespace OSK.Petra.Provisions.Models;

/// <summary>
/// Create a flat adjustment for a expenditure or recovery calculation
/// </summary>
/// 💡Notes:
/// <list type="bullet">
/// <item>A null or empty filter will result in the adjustment being applied to ALL provisions</item>
/// </list>
/// </remarks>
/// <param name="adjustment">The flat adjustment to use</param>
/// <param name="provisionFilter">A filter that cna be applied to limit the total number of provisions adjusted by this flat adjustment</param>11
public readonly struct ProvisionFlatAdjustment(float adjustment, Guid[]? provisionFilter = null): IProvisionAdjustment
{
    #region IProvisionAdjustment

    /// <inheritdoc/>
    public Provision ApplyAdjustment(Provision provision)
        => provisionFilter is null || provisionFilter.Length is 0 || provisionFilter.Contains(provision.Id)
            ? provision.WithAmount(provision.Amount + adjustment)
            : provision;

    #endregion
}
