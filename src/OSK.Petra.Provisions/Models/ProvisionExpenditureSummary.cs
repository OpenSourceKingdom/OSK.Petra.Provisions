using System.Collections.Generic;
using System.Linq;

namespace OSK.Petra.Provisions.Models;

/// <summary>
/// A summary of the calculated expenses between provided and required provisions
/// </summary>
/// <param name="details">The details this summary will utilize</param>
public class ProvisionExpenditureSummary(IEnumerable<ProvisionExpenseDetail> details)
{
    #region Static

    /// <summary>
    /// Represents an empty provision expenditure summary
    /// </summary>
    public static readonly ProvisionExpenditureSummary Empty = new([]);

    #endregion

    #region Variables

    /// <summary>
    /// Whether the overall expenditure was sufficiently allotted the required provision amounts
    /// </summary>
    public bool Sufficient { get; } = details.All(expenditure => expenditure.Sufficient);

    /// <summary>
    /// The individual summaries that make up the expenditure summary
    /// </summary>
    public IReadOnlyList<ProvisionExpenseDetail> Details { get; } = details?.ToArray() ?? [];

    #endregion
}
