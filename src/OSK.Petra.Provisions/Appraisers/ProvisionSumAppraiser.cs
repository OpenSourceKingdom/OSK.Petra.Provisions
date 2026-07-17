using OSK.Petra.Provisions.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OSK.Petra.Provisions.Appraisers;

/// <summary>
/// Appraises provisions based entirely upon the collective sum of their values
/// </summary>
/// <param name="provisionDenominationConverter">An optional value converter that can be used to convert a provision to a raw value to compare against other provisions of the same denomination. Most useful when comparing values like currencies.</param>
public class ProvisionSumAppraiser(Func<Provision, float>? provisionDenominationConverter = null): IAppraiser
{
    #region IAppraiser

    /// <inheritdoc/>
    public int Compare(IEnumerable<Provision> first, IEnumerable<Provision> second)
        => first.Sum(GetProvisionValue).CompareTo(second.Sum(GetProvisionValue));

    #endregion

    #region Helpers

    private float GetProvisionValue(Provision provision)
        => provisionDenominationConverter?.Invoke(provision) ?? provision.Amount;

    #endregion
}
