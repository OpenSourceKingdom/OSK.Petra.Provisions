using System;

namespace OSK.Petra.Provisions.Models;

/// <summary>
/// A provision data structure that provides details between a provided and required provision expense
/// </summary>
public readonly struct ProvisionExpenseDetail
{
    #region Variables

    /// <summary>
    /// The unique provision id this expense detail is associated to
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// Whether the provision expesne had sufficient provisions available
    /// </summary>
    public bool Sufficient { get; }

    /// <summary>
    /// The required provision amount
    /// </summary>
    public float RequiredAmount { get; }

    /// <summary>
    /// The provided provision amount
    /// </summary>
    public float ProvidedAmount { get; }

    /// <summary>
    /// The remaining provisions that are still outstanding and required before the expense will be sufficiently met
    /// </summary>
    public float OutstandingAmount { get; }

    #endregion

    #region Constructors

    /// <summary>
    /// Create an expense detail using a provided constant amount and data for a required provision
    /// </summary>
    /// <param name="providedAmount">The provided amount</param>
    /// <param name="requiredProvision">The <see cref="Provision"/> required for the expense</param>
    public ProvisionExpenseDetail(float providedAmount, Provision requiredProvision)
        : this(requiredProvision.Id, requiredProvision.Amount, providedAmount)
    {
    }

    /// <summary>
    /// Create an expense detail using a required constant amount and data for a provided provision
    /// </summary>
    /// <param name="providedProvision">The <see cref="Provision"/> provided for the expense</param>
    /// <param name="requiredAmount">The required amount</param>
    public ProvisionExpenseDetail(Provision providedProvision, float requiredAmount)
        : this(providedProvision.Id, requiredAmount, providedProvision.Amount)
    {
    }

    /// <summary>
    /// Create an expense detail using the specified provision id and the provided/required provision amounts
    /// </summary>
    /// <param name="id">The unique provision id the expense is associated with</param>
    /// <param name="requiredAmount">The total amount required to suffice the expense</param>
    /// <param name="providedAmount">The total amount provided, or allotted, to paying the expense</param>
    public ProvisionExpenseDetail(Guid id, float requiredAmount, float providedAmount)
    {
        Id = id;
        Sufficient = providedAmount >= requiredAmount;
        RequiredAmount = requiredAmount;
        ProvidedAmount = providedAmount;
        OutstandingAmount = Math.Max(0, requiredAmount - providedAmount);
    }

    #endregion
}

