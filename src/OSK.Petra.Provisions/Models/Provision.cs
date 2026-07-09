using System;

namespace OSK.Petra.Provisions.Models;

/// <summary>
/// Represents a value of some provision
/// </summary>
public readonly struct Provision
{
    #region Variables

    /// <summary>
    /// The unique provision id
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// The amount value for this provision
    /// </summary>
    public float Amount { get; }

    #endregion

    #region Constructors

    /// <summary>
    /// Represents a value of some provision
    /// </summary>
    /// <param name="id">The provision id the value is set to</param>
    public Provision(Guid id)
        : this(id, 0)
    {
    }

    /// <summary>
    /// Represents a value of some provision
    /// </summary>
    /// <param name="id">The provision id the value is set to</param>
    /// <param name="amount">The value of the provision</param>
    public Provision(Guid id, float amount)
    {
        Id = id;
        Amount = amount;
    }

    #endregion

    #region Api

    /// <summary>
    /// Creates a provision with the same id, but with the specified amount
    /// </summary>
    /// <param name="amount">The amount the provision will have</param>
    /// <returns>The provision with the specified amount</returns>
    public Provision WithAmount(float amount)
        => new(Id, amount);

    #endregion

    #region Operators

    public static Provision operator +(Provision provision, float amount)
        => provision.WithAmount(provision.Amount + amount);

    public static Provision operator +(float amount, Provision provision)
        => provision.WithAmount(provision.Amount + amount);

    public static Provision operator -(Provision provision, float amount)
        => provision.WithAmount(provision.Amount - amount);

    public static Provision operator -(float amount, Provision provision)
        => provision.WithAmount(provision.Amount - amount);

    public static Provision operator *(Provision provision, float amount)
        => provision.WithAmount(provision.Amount * amount);

    public static Provision operator *(float amount, Provision provision)
        => provision.WithAmount(provision.Amount * amount);

    public static Provision operator /(Provision provision, float amount)
        => provision.WithAmount(provision.Amount / amount);

    public static Provision operator /(float amount, Provision provision)
        => provision.WithAmount(amount / provision.Amount);

    #endregion
}
