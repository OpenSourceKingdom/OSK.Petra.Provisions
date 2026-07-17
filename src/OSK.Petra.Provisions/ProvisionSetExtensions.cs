using OSK.Petra.Provisions.Adjustments;
using OSK.Petra.Provisions.Appraisers;
using OSK.Petra.Provisions.Models;
using System;
using System.Collections.Generic;

namespace OSK.Petra.Provisions;

public static class ProvisionSetExtensions
{
    #region Remove

    /// <summary>
    /// Completely removes a collection of provisions from the provision set
    /// </summary>    
    /// <param name="set">The provisions being referenced as the provided resources</param>
    /// <param name="provisionIds">The provision ids to remove</param>
    public static void Remove(this IProvisionSet set, IEnumerable<Guid> provisionIds)
    {
        if (provisionIds is null) 
        {
            throw new ArgumentNullException(nameof(provisionIds));
        }

        foreach (var provisionId in provisionIds) 
        { 
            set.Remove(provisionId); 
        }
    }

    #endregion

    #region Addition

    /// <summary>
    /// Adds a collection of provisions to the provision set
    /// </summary>
    /// <param name="set">The provisions being referenced as the provided resources</param>
    /// <param name="provisions">The provision to add</param>
    public static void Add(this IProvisionSet set, IEnumerable<Provision> provisions)
    {
        if (provisions is null)
        {
            throw new ArgumentNullException(nameof(provisions));
        }

        foreach (var provision in provisions) 
        {
            set.Add(provision);
        }
    }

    #endregion

    #region Subtraction

    /// <summary>
    /// Subtracts a collection of provisions to the provision set
    /// </summary>
    /// <param name="set">The provisions being referenced as the provided resources</param>
    /// <param name="provisions">The provisions to subtract</param>
    public static void Subtract(this IProvisionSet set, IEnumerable<Provision> provisions)
    {
        if (provisions is null)
        {
            throw new ArgumentNullException(nameof(provisions));
        }

        foreach (var provision in provisions)
        {
            set.Subtract(provision);
        }
    }

    #endregion

    #region Comparisons

    /// <summary>
    /// Compares this provision set against a collection of provisions using a standard additive sum comparison
    /// </summary>
    /// <param name="set">The provisions being referenced as the provided resources</param>
    /// <param name="provisions">The collection of provisions to compare the set against</param>
    /// <param name="provisionDenominationConverter">An optional converter that can be used to convert all the provisions in the set into a common value for summation</param>
    /// <returns>Whether the provision set is less than, greater than, or equal to the provision collection</returns>
    public static int SumCompare(IProvisionSet set, IEnumerable<Provision> provisions, Func<Provision, float>? provisionDenominationConverter = null)
        => set.Compare(provisions, new ProvisionSumAppraiser(provisionDenominationConverter));

    #endregion

    #region Expenditure

    /// <summary>
    /// Attempts to expense the required provisions from the set.
    /// </summary>
    /// <param name="set">The provisions being referenced as the provided resources</param>
    /// <param name="requiredProvisions">The required provision amounts</param>
    /// <param name="force">Whether the expense should occur, even if the expenditure wouldn't be fully sufficient</param>
    /// <param name="summary">The summary of the expenditure</param>
    /// <returns>Whether the required provisions were expensed from the set</returns>
    public static bool TryExpend(this IProvisionSet set, IEnumerable<Provision> requiredProvisions, out ProvisionExpenditureSummary summary, bool force = false)
    {
        summary = ProvisionCalculator.CalculateExpenditure(requiredProvisions, set);
        if (summary.Sufficient || force)
        {
            set.Subtract(requiredProvisions);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Calculates the expenditure summary using the set with the required provisioning, including adjustments
    /// </summary>
    /// <param name="set">The provisions being referenced as the provided resources</param>
    /// <param name="providedProvisionAdjustments">The adjustments to apply to the provided amounts</param>
    /// <param name="requiredProvisions">The required provision amounts</param>
    /// <param name="requiredProvisionAdjustments">The adjustments to apply to the required amounts</param>
    /// <param name="force">Whether the expense should occur, even if the expenditure wouldn't be fully sufficient</param>
    /// <param name="summary">The summary of the expenditure</param>
    /// <returns>Whether the required provisions were expensed from the set</returns>
    public static bool TryExpend(this IProvisionSet set, IEnumerable<IProvisionAdjustment> providedProvisionAdjustments, IEnumerable<Provision> requiredProvisions,
        IEnumerable<IProvisionAdjustment> requiredProvisionAdjustments, out ProvisionExpenditureSummary summary, bool force = false)
    {
        summary = ProvisionCalculator.CalculateExpenditure(
            new ProvisionTerm()
            {
                Provisions = requiredProvisions,
                Adjustments = requiredProvisionAdjustments
            }, 
            new ProvisionTerm()
            {
                Provisions = set,
                Adjustments = providedProvisionAdjustments
            });

        if (summary.Sufficient || force)
        {
            set.Subtract(requiredProvisions);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Calculates the expenditure summary using the set with the required provisions.
    /// </summary>
    /// <param name="set">The provisions being referenced as the provided resources</param>
    /// <param name="requiredProvisions">The required provision amounts</param>
    /// <returns>An expidenture summary between the provided and required provisions</returns>
    public static ProvisionExpenditureSummary CalculateExpenditure(this IProvisionSet set, IEnumerable<Provision> requiredProvisions)
        => ProvisionCalculator.CalculateExpenditure(requiredProvisions, set);

    /// <summary>
    /// Calculates the expenditure summary using the set with the required provisioning, including adjustments
    /// </summary>
    /// <param name="set">The provisions being referenced as the provided resources</param>
    /// <param name="providedProvisionAdjustments">The adjustments to apply to the provided amounts</param>
    /// <param name="requiredProvisions">The required provision amounts</param>
    /// <param name="requiredProvisionAdjustments">The adjustments to apply to the required amounts</param>
    /// <returns>An expidenture summary between the provided and required provisions</returns>
    public static ProvisionExpenditureSummary CalculateExpenditure(IProvisionSet set, IEnumerable<IProvisionAdjustment> providedProvisionAdjustments, IEnumerable<Provision> requiredProvisions,
        IEnumerable<IProvisionAdjustment> requiredProvisionAdjustments)
        => ProvisionCalculator.CalculateExpenditure(
            new ProvisionTerm()
            {
                Provisions = requiredProvisions,
                Adjustments = requiredProvisionAdjustments
            }, new ProvisionTerm()
            {
                Provisions = set,
                Adjustments = providedProvisionAdjustments
            });

    #endregion

    #region Recovery

    /// <summary>
    /// Calculate a recovery of the current provisions using the provided adjustment amount as a global adjustment
    /// </summary>
    /// <param name="set">The provisions being referenced as the provided resources</param>
    /// <param name="adjustment">The amount to use an additive adjustment</param>
    /// <returns>A recovery summary of the current provisions</returns>
    public static ProvisionRecoverySummary CalculateAdditiveRecovery(this IProvisionSet set, float adjustment)
        => set.CalculateRecovery([new AdditiveAdjustment(adjustment)]);

    /// <summary>
    /// Calculate a recovery of the current provisions using the provided adjustment amount as a global adjustment
    /// </summary>
    /// <param name="set">The provisions being referenced as the provided resources</param>
    /// <param name="multiplier">The amount to use a multiplier adjustment</param>
    /// <returns>A recovery summary of the current provisions</returns>
    public static ProvisionRecoverySummary CalculateMultiplierRecovery(this IProvisionSet set, float multiplier)
        => set.CalculateRecovery([new MultiplierAdjustment(multiplier)]);

    /// <summary>
    /// Calculate a recovery of the current provisions using the provided adjustments
    /// </summary>
    /// <param name="set">The provisions being referenced as the provided resources</param>
    /// <param name="adjustments">The adjustments to apply to the recovery</param>
    /// <returns>A recovery summary of the current provisions</returns>
    public static ProvisionRecoverySummary CalculateRecovery(this IProvisionSet set, IEnumerable<IProvisionAdjustment> adjustments)
        => ProvisionCalculator.CalculateRecovery(set, adjustments);

    #endregion
}
