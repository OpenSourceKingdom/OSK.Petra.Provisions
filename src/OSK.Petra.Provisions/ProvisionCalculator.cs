using System.Collections.Generic;
using System.Linq;
using OSK.Petra.Provisions.Models;

namespace OSK.Petra.Provisions;

public static class ProvisionCalculator
{
    #region Expenditure

    /// <summary>
    /// Calculates a provision expenditure summary, by utilizing only required and provided provisions
    /// </summary>
    /// <param name="requiredProvisions">The required <see cref="Provision"/> collection</param>
    /// <param name="providedProvisions">The provided <see cref="Provision"/> collection</param>
    /// <returns>The summary for the expenditure</returns>
    public static ProvisionExpenditureSummary CalculateExpenditure(IEnumerable<Provision> requiredProvisions, IEnumerable<Provision> providedProvisions)
        => CalculateExpenditure(new ProvisionTerm() { Provisions = requiredProvisions }, new ProvisionTerm() { Provisions = providedProvisions } );

    /// <summary>
    /// Calculates the expendiutre summary, given the required and provided provision <see cref="ProvisionTerm"/>.
    /// </summary>
    /// <param name="requiredProvisions">The required provision term</param>
    /// <param name="providedProvisions">The provided provision term</param>
    /// <returns></returns>
    public static ProvisionExpenditureSummary CalculateExpenditure(ProvisionTerm requiredProvisions, ProvisionTerm providedProvisions)
    { 
        var requiredProvisionLookup = requiredProvisions.Provisions.GroupBy(provision => provision.Id)
                                                        .ToDictionary(provisionGroup => provisionGroup.Key,
                                                                      provisionGroup => provisionGroup.Aggregate(new Provision(provisionGroup.Key), (Provision aggregate, Provision provision) => aggregate + provision.Amount));
        var requiredProvisionAdjustments = requiredProvisions.Adjustments ??= [];

        var allocatedProvisions = providedProvisions.Provisions.GroupBy(provision => provision.Id)
                                               .Select(provisionGroup => provisionGroup.Aggregate(new Provision(provisionGroup.Key), (Provision aggregate, Provision provision) => aggregate + provision.Amount));
        var allocatedProvisionAdjustments = providedProvisions.Adjustments ??= [];

        var expenseDetails = new List<ProvisionExpenseDetail>();
        foreach (var providedProvision in allocatedProvisions)
        {
            if (requiredProvisionLookup.TryGetValue(providedProvision.Id, out var requiredProvision))
            {
                requiredProvisionLookup.Remove(providedProvision.Id);
            }

            foreach (var requiredAdjustment in requiredProvisionAdjustments)
            {
                requiredProvision = requiredAdjustment.ApplyAdjustment(requiredProvision);
            }

            var allocatedProvision = providedProvision;
            foreach (var allocatedAdjustment in allocatedProvisionAdjustments)
            {
                allocatedProvision = allocatedAdjustment.ApplyAdjustment(allocatedProvision);
            }

            expenseDetails.Add(new ProvisionExpenseDetail(allocatedProvision, requiredProvision.Amount));
        }

        foreach (var unusedProvision in requiredProvisionLookup.Values)
        {
            expenseDetails.Add(new ProvisionExpenseDetail(0, unusedProvision));
        }

        return new ProvisionExpenditureSummary(expenseDetails);
    }

    #endregion

    #region Recovery

    /// <summary>
    /// Calculates a provision recovery, utilizing a global flat adjustment 
    /// </summary>1
    /// <param name="provisions">The collection of provisions used to determine a recovery from</param>
    /// <param name="adjustment">The flat adjustment</param>
    /// <returns>The recovery information</returns>
    public static ProvisionRecoverySummary CalculateFlatAdjustmentRecovery(IEnumerable<Provision> provisions, float adjustment)
        => CalculateRecovery(provisions, [new ProvisionFlatAdjustment(adjustment)]);

    /// <summary>
    /// Calculates a provision recovery, utilizing a global percentage adjustment 
    /// </summary>1
    /// <param name="provisions">The collection of provisions used to determine a recovery from</param>
    /// <param name="adjustment">The percentage adjustment</param>
    /// <returns>The recovery information</returns>
    public static ProvisionRecoverySummary CalculatePercentageAdjustmentRecovery(IEnumerable<Provision> provisions, float adjustment)
        => CalculateRecovery(provisions, [new ProvisionPercentageAdjustment(adjustment)]);

    /// <summary>
    /// Calculates a provision recovery, utilizing the provided provision colelction as the basis for the recovery calculation 
    /// </summary>1
    /// <param name="provisions">The collection of provisions used to determine a recovery from</param>
    /// <param name="adjustments">The collection of adjustments that are applied to the recovery</param>
    /// <returns>The recovery information</returns>
    public static ProvisionRecoverySummary CalculateRecovery(IEnumerable<Provision> provisions, IEnumerable<IProvisionAdjustment>? adjustments = null)
    {
        if (provisions is null)
        {
            return ProvisionRecoverySummary.Empty;
        }
        var providedProvisions = provisions.GroupBy(provision => provision.Id)
                                           .Select(provisionGroup => provisionGroup.Aggregate(new Provision(provisionGroup.Key), (Provision aggregate, Provision provision) => aggregate + provision.Amount));
        adjustments ??= [];

        var recoveredProvisions = new List<Provision>();
        foreach(var providedProvision in providedProvisions)
        {
            var provision = providedProvision;
            foreach (var adjustment in adjustments)
            {
                provision = adjustment.ApplyAdjustment(provision);
            }

            recoveredProvisions.Add(provision);
        }

        return new ProvisionRecoverySummary(recoveredProvisions);
    }

    #endregion
}
