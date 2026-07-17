using Moq;
using OSK.Petra.Provisions.Models;
using OSK.Petra.Provisions.Options;

namespace OSK.Petra.Provisions.UnitTests;

public class ProvisionSetTests
{
    #region Variables

    private readonly Guid _id1 = new("11111111-1111-1111-1111-111111111111");
    private readonly Guid _id2 = new("22222222-2222-2222-2222-222222222222");

    #endregion

    #region Constructors

    [Fact]
    public void Constructor_Default_CreatesEmptySetWithDefaultOptions()
    {
        // Arrange & Act
        var set = new ProvisionSet();

        // Assert
        Assert.Empty(set);
    }

    [Fact]
    public void Constructor_WithProvisions_PopulatesSet()
    {
        // Arrange & Act
        var provisions = new[] { new Provision(_id1, 100f), new Provision(_id2, 200f) };
        var set = new ProvisionSet(provisions);

        // Assert
        Assert.Equal(2, set.Count());
        Assert.Contains(set, p => p.Id == _id1 && p.Amount == 100f);
        Assert.Contains(set, p => p.Id == _id2 && p.Amount == 200f);
    }

    [Fact]
    public void Constructor_WithEnumerable_PopulatesSet()
    {
        // Arrange & Act
        var provisions = new List<Provision> { new Provision(_id1, 50f) };
        var set = new ProvisionSet(provisions);

        // Assert
        Assert.Single(set);
    }

    [Fact]
    public void Constructor_WithNullProvisions_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new ProvisionSet((IEnumerable<Provision>)null!));
    }

    [Fact]
    public void Constructor_WithNullOptions_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new ProvisionSet((ProvisionSetOptions)null!));
    }

    #endregion

    #region Add

    [Fact]
    public void Add_NewProvision_AddsToSet()
    {
        // Arrange
        var set = new ProvisionSet();
        var provision = new Provision(_id1, 100f);

        // Act
        set.Add(provision);

        // Assert
        Assert.Single(set, p => p.Id == provision.Id && p.Amount == 100f);
    }

    [Fact]
    public void Add_ExistingProvisionId_SumsAmount()
    {
        // Arrange
        var set = new ProvisionSet([new Provision(_id1, 50f)]);

        // Act
        set.Add(new Provision(_id1, 30f));

        // Assert
        Assert.Single(set, p => p.Id == _id1 && p.Amount == 80f);
    }

    [Fact]
    public void Add_WithMaxLimit_CapsAtMaximum()
    {
        // Arrange
        var options = new ProvisionSetOptions
        {
            DefaultProvisionLimit = new ProvisionLimit(null, 100f)
        };
        var set = new ProvisionSet([new Provision(_id1, 80f)], options);

        // Act
        set.Add(new Provision(_id1, 50f));

        // Assert
        Assert.Equal(100f, set[_id1].Amount);
    }

    [Fact]
    public void Add_ProvisionLimitPerProvision_UsesCustomLimit()
    {
        // Arrange
        var limits = new Dictionary<Guid, ProvisionLimit> { [_id1] = new ProvisionLimit(null, 60f) };
        var options = new ProvisionSetOptions { ProvisionLimits = limits };
        var set = new ProvisionSet([new Provision(_id1, 50f)], options);

        // Act
        set.Add(new Provision(_id1, 30f));
        set.Add(new Provision(_id1, 30f));
        set.Add(new Provision(_id2, 130f));

        // Assert
        Assert.Equal(60f, set[_id1].Amount);
        Assert.Equal(130, set[_id2].Amount);
    }

    #endregion

    #region Expend

    [Fact]
    public void Expend_NewProvisionId_SetsNegativeAmount()
    {
        // Arrange
        var set = new ProvisionSet();

        // Act
        set.Expend(new Provision(_id1, 50f));

        // Assert
        Assert.Equal(-50f, set[_id1].Amount);
    }

    [Fact]
    public void Expend_ExistingProvisionId_ReducesAmount()
    {
        // Arrange
        var set = new ProvisionSet([new Provision(_id1, 100f)]);

        // Act
        set.Expend(new Provision(_id1, 30f));

        // Assert
        Assert.Equal(70f, set[_id1].Amount);
    }

    [Fact]
    public void Expend_ReachesMinimum_RemoveAtMinimumTrue_RemovesFromSet()
    {
        // Arrange
        var options = new ProvisionSetOptions
        {
            RemoveAtMinimum = true,
            DefaultProvisionLimit = new ProvisionLimit(50f, null)
        };
        var set = new ProvisionSet([new Provision(_id1, 100f)], options);

        // Act
        set.Expend(new Provision(_id1, 50f));

        // Assert
        Assert.False(set.Contains(_id1));
    }

    [Fact]
    public void Expend_ReachesMinimum_RemoveAtMinimumFalse_SetsToMinimum()
    {
        // Arrange
        var options = new ProvisionSetOptions
        {
            RemoveAtMinimum = false,
            DefaultProvisionLimit = new ProvisionLimit(50f, null)
        };
        var set = new ProvisionSet([new Provision(_id1, 100f)], options);

        // Act
        set.Expend(new Provision(_id1, 50f));

        // Assert
        Assert.True(set.Contains(_id1));
        Assert.Equal(50f, set[_id1].Amount);
    }

    [Fact]
    public void Expend_BelowMinimum_FloorsAtMinimum()
    {
        // Arrange
        var options = new ProvisionSetOptions
        {
            DefaultProvisionLimit = new ProvisionLimit(30f, null)
        };
        var set = new ProvisionSet([new Provision(_id1, 50f)], options);

        // Act
        set.Expend(new Provision(_id1, 40f));

        // Assert
        Assert.Equal(30f, set[_id1].Amount);
    }

    #endregion

    #region Clone

    [Fact]
    public void Clone_ReturnsNewInstanceWithSameContent()
    {
        // Arrange
        var original = new ProvisionSet([new Provision(_id1, 100f), new Provision(_id2, 200f)]);

        // Act
        var clone = original.Clone();

        // Assert
        Assert.NotSame(original, clone);
        Assert.Equal(original.Count(), clone.Count());
        Assert.True(original.SequenceEqual(clone));
    }

    [Fact]
    public void Clone_ModifyingOriginal_DoesNotAffectClone()
    {
        // Arrange
        var original = new ProvisionSet([new Provision(_id1, 100f)]);
        var clone = original.Clone();

        // Act
        original.Add(new Provision(_id2, 50f));

        // Assert
        Assert.Single(clone);
        Assert.Equal(100f, clone[_id1].Amount);
    }

    #endregion

    #region Compare

    [Fact]
    public void Compare_DelegatesToAppraiser()
    {
        // Arrange
        var set = new ProvisionSet([new Provision(_id1, 100f)]);
        var appraiser = new Mock<IAppraiser>();
        var provisions = new[] { new Provision(_id2, 50f) };

        // Act
        set.Compare(provisions, appraiser.Object);

        // Assert
        appraiser.Verify(m => m.Compare(It.IsAny<IEnumerable<Provision>>(), It.IsAny<IEnumerable<Provision>>()), Times.Once);
    }

    [Fact]
    public void Compare_WithNullAppraiser_ThrowsArgumentNullException()
    {
        // Arrange
        var set = new ProvisionSet([new Provision(_id1, 100f)]);
        var provisions = new[] { new Provision(_id2, 50f) };

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => set.Compare(provisions, null!));
    }

    #endregion

    #region Contains

    [Fact]
    public void Contains_ExistingId_ReturnsTrue()
    {
        // Arrange
        var set = new ProvisionSet([new Provision(_id1, 100f)]);

        // Act
        var result = set.Contains(_id1);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Contains_MissingId_ReturnsFalse()
    {
        // Arrange
        var set = new ProvisionSet([new Provision(_id1, 100f)]);

        // Act
        var result = set.Contains(_id2);

        // Assert
        Assert.False(result);
    }

    #endregion

    #region Remove

    [Fact]
    public void Remove_ExistingId_ReturnsTrueAndRemoves()
    {
        // Arrange
        var set = new ProvisionSet([new Provision(_id1, 100f)]);

        // Act
        var result = set.Remove(_id1);

        // Assert
        Assert.True(result);
        Assert.False(set.Contains(_id1));
    }

    [Fact]
    public void Remove_MissingId_ReturnsFalse()
    {
        // Arrange
        var set = new ProvisionSet([new Provision(_id1, 100f)]);

        // Act
        var result = set.Remove(_id2);

        // Assert
        Assert.False(result);
    }

    #endregion

    #region SetProvisions

    [Fact]
    public void SetProvisions_ClearsOldAndAddsNew()
    {
        // Arrange
        var set = new ProvisionSet([new Provision(_id1, 100f)]);

        // Act
        set.SetProvisions([new Provision(_id2, 200f)]);

        // Assert
        Assert.False(set.Contains(_id1));
        Assert.True(set.Contains(_id2));
        Assert.Equal(200f, set[_id2].Amount);
    }

    [Fact]
    public void SetProvisions_DuplicateIds_AggregatesAmounts()
    {
        // Arrange
        var set = new ProvisionSet();
        var provisions = new[] { new Provision(_id1, 50f), new Provision(_id1, 30f) };

        // Act
        set.SetProvisions(provisions);

        // Assert
        Assert.Single(set);
        Assert.Equal(80f, set[_id1].Amount);
    }

    [Fact]
    public void SetProvisions_Null_ThrowsArgumentNullException()
    {
        // Arrange
        var set = new ProvisionSet();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => set.SetProvisions(null!));
    }

    [Fact]
    public void SetProvisions_Empty_ClearsAllProvisions()
    {
        // Arrange
        var set = new ProvisionSet([new Provision(_id1, 100f), new Provision(_id2, 200f)]);

        // Act
        set.SetProvisions([]);

        // Assert
        Assert.Empty(set);
    }

    #endregion

    #region this[Guid]

    [Fact]
    public void Indexer_GetExisting_ReturnsStoredProvision()
    {
        // Arrange
        var set = new ProvisionSet([new Provision(_id1, 100f)]);

        // Act
        var provision = set[_id1];

        // Assert
        Assert.Equal(_id1, provision.Id);
        Assert.Equal(100f, provision.Amount);
    }

    [Fact]
    public void Indexer_GetMissing_ReturnsZeroAmountProvision()
    {
        // Arrange
        var set = new ProvisionSet([new Provision(_id1, 100f)]);

        // Act
        var provision = set[_id2];

        // Assert
        Assert.Equal(_id2, provision.Id);
        Assert.Equal(0f, provision.Amount);
    }

    [Fact]
    public void Indexer_SetExisting_UpdatesAmount()
    {
        // Arrange
        var set = new ProvisionSet([new Provision(_id1, 100f)]);

        // Act
        set[_id1] = new Provision(_id1, 150f);

        // Assert
        Assert.Equal(150f, set[_id1].Amount);
    }

    [Fact]
    public void Indexer_SetNewProvision_AddsToSet()
    {
        // Arrange
        var set = new ProvisionSet([new Provision(_id1, 100f)]);

        // Act
        set[_id2] = new Provision(_id2, 200f);

        // Assert
        Assert.True(set.Contains(_id2));
        Assert.Equal(200f, set[_id2].Amount);
    }

    #endregion

    #region GetEnumerator

    [Fact]
    public void GetEnumerator_YieldsAllProvisions()
    {
        // Arrange
        var provisions = new[] { new Provision(_id1, 100f), new Provision(_id2, 200f) };
        var set = new ProvisionSet(provisions);

        // Act
        var result = set.ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, p => p.Id == _id1 && p.Amount == 100f);
        Assert.Contains(result, p => p.Id == _id2 && p.Amount == 200f);
    }

    [Fact]
    public void GetEnumerator_EmptySet_YieldsNoProvisions()
    {
        // Arrange
        var set = new ProvisionSet();

        // Act
        var result = set.ToList();

        // Assert
        Assert.Empty(result);
    }

    #endregion
}
