using OSK.Petra.Provisions.Models;
using System.Collections.Generic;

namespace OSK.Petra.Provisions;

/// <summary>
/// A decider in the value of a collection of provisions
/// </summary>
public interface IAppraiser
{
    /// <summary>
    /// Compares the provision groups to one another to determine which has more value
    /// </summary>
    /// <param name="first">The provisions to comape with</param>
    /// <param name="second">The provisions to compare against</param>
    /// <returns>Wheather the first provisions is less than, greater than, or equal to the second provisions</returns>
    int Compare(IEnumerable<Provision> first, IEnumerable<Provision> second);
}
