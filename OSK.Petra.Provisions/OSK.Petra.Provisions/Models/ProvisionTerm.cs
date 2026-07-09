using System;
using System.Collections.Generic;
using System.Text;

namespace OSK.Petra.Provisions.Models;

/// <summary>
/// Represents a part of a larger expense or recovery or similar provision data operation
/// </summary>
public class ProvisionTerm
{
    /// <summary>
    /// The provisions for this term
    /// </summary>
    public IEnumerable<Provision> Provisions { get; set; } = [];

    /// <summary>
    /// The adjustments this provision term will utilize
    /// </summary>
    public IEnumerable<IProvisionAdjustment> Adjustments { get; set; } = [];
}
