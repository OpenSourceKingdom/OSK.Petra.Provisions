using OSK.Petra.Provisions.Models;
using System;
using System.Collections.Generic;

namespace OSK.Petra.Provisions.Options;

/// <summary>
/// Options for a provision set to help drive a sets operation
/// </summary>
public class ProvisionSetOptions
{
    #region Static

    /// <summary>
    /// A default set of provision options
    /// </summary>
    public static readonly ProvisionSetOptions Default = new();

    #endregion

    /// <summary>
    /// Determines if a provision will be removed from the provision set once its value reaches the floor limit.
    /// </summary>
    /// <remarks>
    /// 💡Notes:
    /// <list type="bullet">
    /// <item>This value will not impact the set if a provision limit is not provided for the provision being adjusted</item>
    /// <item>Removing a provision from the set is not guaranteed to impact retrieving provisions using an indexer. This will only impact the provisions returned when enumerated or validating provisions in the set</item>
    /// </list>
    /// </remarks>
    public bool RemoveAtMinimum { get; set; }

    /// <summary>
    /// Sets a global default limit, applied to all provisions if a limit is not set in <see cref="ProvisionLimits"/>
    /// </summary>
    public ProvisionLimit? DefaultProvisionLimit { get; set; }

    /// <summary>
    /// Provides a set of limits on a per provision basis
    /// </summary>
    public Dictionary<Guid, ProvisionLimit> ProvisionLimits { get; set; } = [];
}
