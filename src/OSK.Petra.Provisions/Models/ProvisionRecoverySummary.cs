using System.Collections.Generic;
using System.Linq;

namespace OSK.Petra.Provisions.Models;

/// <summary>
/// A summary of the calculated recovered provisions
/// </summary>
/// <param name="provisions">The provisions this summary will utilize</param>
public class ProvisionRecoverySummary(IEnumerable<Provision> provisions)
{
    #region Static

    /// <summary>
    /// Represents an empty provision recovery summary
    /// </summary>
    public static readonly ProvisionRecoverySummary Empty = new ProvisionRecoverySummary([]);

    #endregion

    #region Variables

    /// <summary>
    /// The specific provision related information for the recovered provisions
    /// </summary>
    public IReadOnlyCollection<Provision> RecoveredProvisions { get; } = provisions?.ToArray() ?? [];

    #endregion
}
