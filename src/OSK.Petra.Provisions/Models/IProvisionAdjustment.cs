namespace OSK.Petra.Provisions.Models;

/// <summary>
/// An adustment that can be applied to a provision group before it is used in calculations
/// </summary>
public interface IProvisionAdjustment
{
    /// <summary>
    /// Applies the specific adjustment to the provision, returning the adjusted provision
    /// </summary>
    /// <param name="provision">The provision to adjust</param>
    /// <returns>An adjusted provision</returns>
    Provision ApplyAdjustment(Provision provision);
}
