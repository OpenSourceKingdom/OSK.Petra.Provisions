using OSK.Petra.Provisions.Appraisers;
using OSK.Petra.Provisions.Models;

namespace OSK.Petra.Provisions.UnitTests.Appraisers;

public class ProvisionSumAppraiserTests
{
    #region Variables

    private readonly Guid _id1 = new("11111111-1111-1111-1111-111111111111");
    private readonly Guid _id2 = new("22222222-2222-2222-2222-222222222222");
    private readonly Guid _id3 = new("33333333-3333-3333-3333-333333333333");

    #endregion

    #region Compare

    [Fact]
    public void Compare_FirstGreaterThanSecond_ReturnsOne()
    {
        // Arrange
        var appraiser = new ProvisionSumAppraiser();
        var first = new[] { new Provision(_id1, 100f) };
        var second = new[] { new Provision(_id2, 50f) };

        // Act
        var result = appraiser.Compare(first, second);

        // Assert
        Assert.Equal(1, result);
    }

    [Fact]
    public void Compare_FirstLessThanSecond_ReturnsNegativeOne()
    {
        // Arrange
        var appraiser = new ProvisionSumAppraiser();
        var first = new[] { new Provision(_id1, 50f) };
        var second = new[] { new Provision(_id2, 100f) };

        // Act
        var result = appraiser.Compare(first, second);

        // Assert
        Assert.Equal(-1, result);
    }

    [Fact]
    public void Compare_EqualSums_ReturnsZero()
    {
        // Arrange
        var appraiser = new ProvisionSumAppraiser();
        var first = new[] { new Provision(_id1, 50f), new Provision(_id2, 50f) };
        var second = new[] { new Provision(_id3, 100f) };

        // Act
        var result = appraiser.Compare(first, second);

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public void Compare_BothEmpty_ReturnsZero()
    {
        // Arrange
        var appraiser = new ProvisionSumAppraiser();
        var first = Array.Empty<Provision>();
        var second = Array.Empty<Provision>();

        // Act
        var result = appraiser.Compare(first, second);

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public void Compare_FirstEmpty_ReturnsNegativeOne()
    {
        // Arrange
        var appraiser = new ProvisionSumAppraiser();
        var first = Array.Empty<Provision>();
        var second = new[] { new Provision(_id1, 100f) };

        // Act
        var result = appraiser.Compare(first, second);

        // Assert
        Assert.Equal(-1, result);
    }

    [Fact]
    public void Compare_SecondEmpty_ReturnsOne()
    {
        // Arrange
        var appraiser = new ProvisionSumAppraiser();
        var first = new[] { new Provision(_id1, 100f) };
        var second = Array.Empty<Provision>();

        // Act
        var result = appraiser.Compare(first, second);

        // Assert
        Assert.Equal(1, result);
    }

    [Theory]
    [InlineData(100f, 200f)]
    [InlineData(-50f, -100f)]
    public void Compare_WithNegativeAmounts_ReturnsCorrectResult(float firstAmount, float secondAmount)
    {
        // Arrange
        var appraiser = new ProvisionSumAppraiser();
        var first = new[] { new Provision(_id1, firstAmount) };
        var second = new[] { new Provision(_id2, secondAmount) };

        // Act
        var result = appraiser.Compare(first, second);

        // Assert
        if (firstAmount > secondAmount)
        {
            Assert.Equal(1, result);
        }
        else if (firstAmount < secondAmount)
        {
            Assert.Equal(-1, result);
        }
        else
        {
            Assert.Equal(0, result);
        }
    }

    [Fact]
    public void Compare_WithConverter_UsesConverterValues()
    {
        // Arrange
        var converter = new Func<Provision, float>(p => 2.0f);
        var appraiser = new ProvisionSumAppraiser(converter);
        var first = new[] { new Provision(_id1, 1f), new Provision(_id2, 1f) };
        var second = new[] { new Provision(_id3, 1f) };

        // Act
        var result = appraiser.Compare(first, second);

        // Assert
        Assert.Equal(1, result);
    }

    [Fact]
    public void Compare_MultipleProvisionsPerSide_SumsAllCorrectly()
    {
        // Arrange
        var appraiser = new ProvisionSumAppraiser();
        var first = new[] { new Provision(_id1, 30f), new Provision(_id2, 40f), new Provision(_id3, 50f) };
        var second = new[] { new Provision(_id1, 80f), new Provision(_id2, 20f) };

        // Act
        var result = appraiser.Compare(first, second);

        // Assert
        Assert.Equal(1, result);
    }

    #endregion
}
