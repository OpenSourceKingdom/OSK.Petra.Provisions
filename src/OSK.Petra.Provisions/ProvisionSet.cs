using OSK.Petra.Provisions.Models;
using OSK.Petra.Provisions.Options;
using System;
using System.Collections;
using System.Collections.Generic;

namespace OSK.Petra.Provisions;

/// <inheritdoc/>
public class ProvisionSet: IProvisionSet
{
    #region Variables

    private readonly Dictionary<Guid, Provision> _provisions = [];
    private readonly ProvisionSetOptions _options;

    #endregion

    #region Constructors

    /// <summary>
    /// Creates an empty provision set, using default <see cref="ProvisionSetOptions"/>
    /// </summary>
    public ProvisionSet()
        : this([], ProvisionSetOptions.Default)
    {
    }

    /// <summary>
    /// Creates a provision set from a parameter input collection, using default <see cref="ProvisionSetOptions"/>
    /// </summary>
    /// <param name="provisions">The provisions to create the collection with</param>
    public ProvisionSet(params Provision[] provisions)
        : this(provisions, ProvisionSetOptions.Default)
    {

    }

    /// <summary>
    /// Creates a provision set from a generic provision enumerable, using default <see cref="ProvisionSetOptions"/>
    /// </summary>
    /// <param name="provisions">The provisions to create the collection with</param>
    public ProvisionSet(IEnumerable<Provision> provisions)
        : this(provisions, ProvisionSetOptions.Default)
    {
    }

    /// <summary>
    /// Creates an empty provision set using a custom <see cref="ProvisionSetOptions"/>
    /// </summary>
    /// <param name="options"></param>
    public ProvisionSet(ProvisionSetOptions options)
        : this([], options)
    {

    }

    /// <summary>
    /// Creates a provision set from a generic provision enumerable, using custom <see cref="ProvisionSetOptions"/>
    /// </summary>
    /// <param name="provisions">The provisions to create the set with</param>
    /// <param name="options">The options to create the set with</param>
    public ProvisionSet(IEnumerable<Provision> provisions, ProvisionSetOptions options)
    {
        if (provisions is null)
        {
            throw new ArgumentNullException(nameof(provisions));
        }
        if (options is null)
        {
            throw new ArgumentNullException(nameof(options));            
        }

        SetProvisions(provisions);

        _options = options;
    }

    #endregion

    #region IProvisionSet

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    /// <inheritdoc/>
    public IEnumerator<Provision> GetEnumerator()
        => _provisions.Values.GetEnumerator();

    /// <inheritdoc/>
    public Provision this[Guid provisionId] 
    { 
        get => _provisions.TryGetValue(provisionId, out var provision) ? provision : new Provision(provisionId, 0); 
        set => _provisions[provisionId] = value;
    }

    /// <inheritdoc/>
    public int Compare(IEnumerable<Provision> provisions, IAppraiser appraiser)
    {
        if (provisions is null)
        {
            throw new ArgumentNullException(nameof(provisions));
        }
        if (appraiser is null)
        {
            throw new ArgumentNullException(nameof(appraiser));
        }
        return appraiser.Compare(this, provisions);
    }

    /// <inheritdoc/>
    public void Add(Provision provision)
    {
        if (!_provisions.TryGetValue(provision.Id, out var current))
        {
            current = new Provision(provision.Id);
        }

        var limit = GetLimit(provision.Id);
        var totalProvision = limit.HasValue && limit.Value.Maximum.HasValue
            ? Math.Min(provision.Amount + current.Amount, limit.Value.Maximum.Value)
            : provision.Amount + current.Amount;

        _provisions[provision.Id] = provision.WithAmount(totalProvision);
    }

    /// <inheritdoc/>
    public void Subtract(Provision provision)
    {
        if (!_provisions.TryGetValue(provision.Id, out var current))
        {
            current = new Provision(provision.Id);
        }

        var limit = GetLimit(provision.Id);
        var totalProvision = limit.HasValue && limit.Value.Minimum.HasValue
            ? Math.Max(current.Amount - provision.Amount, limit.Value.Minimum.Value)
            : current.Amount - provision.Amount;

        if (limit?.Minimum is not null && _options.RemoveAtMinimum && totalProvision == limit.Value.Minimum.Value)
        {
            _provisions.Remove(provision.Id);
        }
        else
        {
            _provisions[provision.Id] = provision.WithAmount(totalProvision);
        }
    }

    /// <inheritdoc/>
    public IProvisionSet Clone()
        => new ProvisionSet(this);

    /// <inheritdoc/>
    public bool Remove(Guid provisionId)
        => _provisions.Remove(provisionId);

    /// <inheritdoc/>
    public void SetProvisions(IEnumerable<Provision> provisions)
    {
        if (provisions is null)
        {
            throw new ArgumentNullException(nameof(provisions));
        }

        _provisions.Clear();
        foreach (var provision in provisions)
        {
            if (!_provisions.TryGetValue(provision.Id, out var aggregatedProvision))
            {
                aggregatedProvision = new Provision(provision.Id);
            }

            _provisions[provision.Id] = aggregatedProvision + provision.Amount;
        }
    }

    /// <inheritdoc/>
    public bool Contains(Guid provisionId)
        => _provisions.TryGetValue(provisionId, out _);

    #endregion

    #region Helpers

    private ProvisionLimit? GetLimit(Guid provisionId)
        => _options.ProvisionLimits.TryGetValue(provisionId, out var customLimit)
                ? customLimit
                : _options.DefaultProvisionLimit;

    #endregion
}
