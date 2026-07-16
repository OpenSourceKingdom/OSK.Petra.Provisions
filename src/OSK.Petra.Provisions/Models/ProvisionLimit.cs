namespace OSK.Petra.Provisions.Models;

/// <summary>
/// Represents a limit for a particular provision
/// </summary>
/// <param name="min">The minimum value the provision may have</param>
/// <param name="max">The maximum value the provision may have</param>
public readonly struct ProvisionLimit(float? min, float? max)
{
    #region Variables

    /// <summary>
    /// The minimum value the provision may have. Null means inifinity.
    /// </summary>
    public float? Minimum { get; } = min;

    /// <summary>
    /// The maximum value the provision may have. Null means inifinity.
    /// </summary>
    public float? Maximum { get; } = max;

    #endregion

    #region Api

    /// <summary>
    /// Checks if a given value is within the provision limit
    /// </summary>
    /// <param name="value">The value to check</param>
    /// <returns>Whether the value is in the limit or not</returns>
    public bool WithinLimit(float value)
        => (!Minimum.HasValue || Minimum.Value <= value) && (!Maximum.HasValue || Maximum.Value >= value);

    #endregion
}
