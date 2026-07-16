using OSK.Petra.Provisions.Models;
using System;
using System.Collections.Generic;

namespace OSK.Petra.Provisions;

/// <summary>
/// A provision set represents a collection of mutable provisions that can represent a provision requirement for some entity.
/// </summary>
public interface IProvisionSet : IEnumerable<Provision>
{
    /// <summary>
    /// Compares this provision set against a collection of provisions
    /// </summary>
    /// <param name="provisions">The collection of provisions to compare the set against</param>
    /// <param name="appraiser">An appraiser for the comparison</param>
    /// <returns>Whether the provision set is less than, greater than, or equal to the provision collection</returns>
    int Compare(IEnumerable<Provision> provisions, IAppraiser appraiser);

    /// <summary>
    /// Adds a given provision to the provision set
    /// </summary>
    /// <param name="provision">The provision to add</param>
    void Add(Provision provision);

    /// <summary>
    /// Subtracts a given provision from the provision set
    /// </summary>
    /// <param name="provision">The provision to subtract</param>
    void Subtract(Provision provision);

    /// <summary>
    /// Completely removes a provision from the provision set
    /// </summary>
    /// <param name="provisionId">The provision id to remove</param>
    /// <returns>Whether the item existed in the set to be removed</returns>
    bool Remove(Guid provisionId);

    /// <summary>
    /// Set the provision set to the given provision collection
    /// </summary>
    /// <param name="provisions">The provisions the set will contain</param>
    void SetProvisions(IEnumerable<Provision> provisions);

    /// <summary>
    /// Determines whether the provision id exists within the set
    /// </summary>
    /// <param name="provisionId">The id of the provision to check</param>
    /// <returns>Whether a provision with the given id exists in the set</returns>
    bool Contains(Guid provisionId);

    /// <summary>
    /// Clones the provision set into another separate, but identical, set
    /// </summary>
    /// <returns>A set with identical provisions</returns>
    IProvisionSet Clone();

    /// <summary>
    /// Handles a provision within the set by id
    /// </summary>
    /// <remarks>
    /// 💡Notes:
    /// <list type="bullet">
    /// <item>Attempting to get a provision from the provision set will return a provision of 0, even if it does not exist.</item>
    /// </list>
    /// </remarks>
    /// <param name="provisionId">The id for the provision</param>
    /// <returns>The provision</returns>
    Provision this[Guid provisionId] { get; set; }
}
