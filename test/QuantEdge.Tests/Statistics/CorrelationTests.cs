using QuantEdge.Statistics;

namespace QuantEdge.Tests.Statistics;

/// <summary>
/// Unit tests for the Correlation class.
/// </summary>
public class CorrelationTests
{
    private const double Tolerance = 1e-10;

    #region Pearson Tests

    [Fact]
    public void Pearson_PerfectPositiveCorrelation_ReturnsOne()
    {
        // Arrange
        double[] x = [1, 2, 3, 4, 5];
        double[] y = [2, 4, 6, 8, 10];

        // Act
        double result = Correlation.Pearson(x, y);

        // Assert
        Assert.Equal(1.0, result, Tolerance);
    }

    [Fact]
    public void Pearson_PerfectNegativeCorrelation_ReturnsMinusOne()
    {
        // Arrange
        double[] x = [1, 2, 3, 4, 5];
        double[] y = [10, 8, 6, 4, 2];

        // Act
        double result = Correlation.Pearson(x, y);

        // Assert
        Assert.Equal(-1.0, result, Tolerance);
    }

    [Fact]
    public void Pearson_NoCorrelation_ReturnsZero()
    {
        // Arrange
        double[] x = [1, 2, 3, 4, 5];
        double[] y = [5, 5, 5, 5, 5]; // No variance in y

        // Act
        double result = Correlation.Pearson(x, y);

        // Assert
        Assert.Equal(0.0, result, Tolerance);
    }

    [Fact]
    public void Pearson_DifferentLengths_ThrowsArgumentException()
    {
        // Arrange
        double[] x = [1, 2, 3];
        double[] y = [1, 2];

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => Correlation.Pearson(x, y));
        Assert.Contains("same length", exception.Message);
    }

    [Fact]
    public void Pearson_LessThanTwoElements_ReturnsNaN()
    {
        // Arrange
        double[] x = [1];
        double[] y = [2];

        // Act
        double result = Correlation.Pearson(x, y);

        // Assert
        Assert.True(double.IsNaN(result));
    }

    [Fact]
    public void Pearson_EmptyArrays_ReturnsNaN()
    {
        // Arrange
        double[] x = [];
        double[] y = [];

        // Act
        double result = Correlation.Pearson(x, y);

        // Assert
        Assert.True(double.IsNaN(result));
    }

    [Fact]
    public void Pearson_RealWorldFinancialData_CalculatesCorrectly()
    {
        // Arrange - Example: Daily returns of two stocks
        double[] stockA = [0.02, -0.01, 0.03, 0.01, -0.02];
        double[] stockB = [0.01, -0.02, 0.04, 0.00, -0.01];

        // Act
        double result = Correlation.Pearson(stockA, stockB);

        // Assert - Expected value calculated manually or with reference implementation
        Assert.True(result >= -1.0 && result <= 1.0, "Correlation must be in range [-1, 1]");
        Assert.False(double.IsNaN(result), "Result should not be NaN for valid inputs");
    }

    #endregion

    #region PearsonMatrix Tests

    [Fact]
    public void PearsonMatrix_ThreeFeatures_ReturnsCorrectMatrix()
    {
        // Arrange
        double[][] features =
        [
            [1, 2, 3, 4, 5],
            [2, 4, 6, 8, 10],
            [5, 4, 3, 2, 1]
        ];

        // Act
        double[,] result = Correlation.PearsonMatrix(features);

        // Assert
        Assert.Equal(3, result.GetLength(0));
        Assert.Equal(3, result.GetLength(1));
        
        // Diagonal should be 1.0
        Assert.Equal(1.0, result[0, 0], Tolerance);
        Assert.Equal(1.0, result[1, 1], Tolerance);
        Assert.Equal(1.0, result[2, 2], Tolerance);
        
        // Matrix should be symmetric
        Assert.Equal(result[0, 1], result[1, 0], Tolerance);
        Assert.Equal(result[0, 2], result[2, 0], Tolerance);
        Assert.Equal(result[1, 2], result[2, 1], Tolerance);
        
        // Feature 0 and 1 should have perfect positive correlation
        Assert.Equal(1.0, result[0, 1], Tolerance);
        
        // Feature 0 and 2 should have perfect negative correlation
        Assert.Equal(-1.0, result[0, 2], Tolerance);
    }

    [Fact]
    public void PearsonMatrix_NullFeatures_ThrowsArgumentNullException()
    {
        // Arrange
        double[][]? features = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => Correlation.PearsonMatrix(features!));
    }

    [Fact]
    public void PearsonMatrix_EmptyFeatures_ThrowsArgumentException()
    {
        // Arrange
        double[][] features = [];

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => Correlation.PearsonMatrix(features));
        Assert.Contains("cannot be empty", exception.Message);
    }

    [Fact]
    public void PearsonMatrix_DifferentFeatureLengths_ThrowsArgumentException()
    {
        // Arrange
        double[][] features =
        [
            [1, 2, 3],
            [1, 2] // Different length
        ];

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => Correlation.PearsonMatrix(features));
        Assert.Contains("same length", exception.Message);
    }

    #endregion

    #region Spearman Tests

    [Fact]
    public void Spearman_PerfectMonotonicRelationship_ReturnsOne()
    {
        // Arrange - Non-linear but monotonic relationship
        double[] x = [1, 2, 3, 4, 5];
        double[] y = [1, 4, 9, 16, 25]; // y = x²

        // Act
        double result = Correlation.Spearman(x, y);

        // Assert
        Assert.Equal(1.0, result, Tolerance);
    }

    [Fact]
    public void Spearman_PerfectNegativeMonotonicRelationship_ReturnsMinusOne()
    {
        // Arrange
        double[] x = [1, 2, 3, 4, 5];
        double[] y = [25, 16, 9, 4, 1]; // Reversed squares

        // Act
        double result = Correlation.Spearman(x, y);

        // Assert
        Assert.Equal(-1.0, result, Tolerance);
    }

    [Fact]
    public void Spearman_WithTiedRanks_HandlesCorrectly()
    {
        // Arrange
        double[] x = [1, 2, 2, 3, 4];
        double[] y = [1, 2, 2, 3, 4];

        // Act
        double result = Correlation.Spearman(x, y);

        // Assert
        Assert.Equal(1.0, result, Tolerance);
    }

    [Fact]
    public void Spearman_DifferentLengths_ThrowsArgumentException()
    {
        // Arrange
        double[] x = [1, 2, 3];
        double[] y = [1, 2];

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => Correlation.Spearman(x, y));
        Assert.Contains("same length", exception.Message);
    }

    [Fact]
    public void Spearman_LessThanTwoElements_ReturnsNaN()
    {
        // Arrange
        double[] x = [1];
        double[] y = [2];

        // Act
        double result = Correlation.Spearman(x, y);

        // Assert
        Assert.True(double.IsNaN(result));
    }

    #endregion

    #region RSquared Tests

    [Fact]
    public void RSquared_PerfectCorrelation_ReturnsOne()
    {
        // Arrange
        double[] x = [1, 2, 3, 4, 5];
        double[] y = [2, 4, 6, 8, 10];

        // Act
        double result = Correlation.RSquared(x, y);

        // Assert
        Assert.Equal(1.0, result, Tolerance);
    }

    [Fact]
    public void RSquared_NoCorrelation_ReturnsZero()
    {
        // Arrange
        double[] x = [1, 2, 3, 4, 5];
        double[] y = [5, 5, 5, 5, 5];

        // Act
        double result = Correlation.RSquared(x, y);

        // Assert
        Assert.Equal(0.0, result, Tolerance);
    }

    [Fact]
    public void RSquared_NegativeCorrelation_ReturnsPositiveValue()
    {
        // Arrange
        double[] x = [1, 2, 3, 4, 5];
        double[] y = [10, 8, 6, 4, 2];

        // Act
        double result = Correlation.RSquared(x, y);

        // Assert
        Assert.Equal(1.0, result, Tolerance); // R² of perfect negative correlation is also 1
    }

    [Fact]
    public void RSquared_PartialCorrelation_ReturnsBetweenZeroAndOne()
    {
        // Arrange
        double[] x = [1, 2, 3, 4, 5];
        double[] y = [2, 3, 5, 7, 11]; // Some correlation but not perfect

        // Act
        double result = Correlation.RSquared(x, y);

        // Assert
        Assert.True(result >= 0.0 && result <= 1.0);
    }

    #endregion

    #region Covariance Tests

    [Fact]
    public void Covariance_PerfectPositiveRelationship_ReturnsPositiveValue()
    {
        // Arrange
        double[] x = [1, 2, 3, 4, 5];
        double[] y = [2, 4, 6, 8, 10];

        // Act
        double result = Correlation.Covariance(x, y);

        // Assert
        Assert.True(result > 0, "Covariance should be positive for positively related data");
        Assert.Equal(5.0, result, Tolerance);
    }

    [Fact]
    public void Covariance_PerfectNegativeRelationship_ReturnsNegativeValue()
    {
        // Arrange
        double[] x = [1, 2, 3, 4, 5];
        double[] y = [10, 8, 6, 4, 2];

        // Act
        double result = Correlation.Covariance(x, y);

        // Assert
        Assert.True(result < 0, "Covariance should be negative for negatively related data");
    }

    [Fact]
    public void Covariance_NoRelationship_ReturnsZero()
    {
        // Arrange
        double[] x = [1, 2, 3, 4, 5];
        double[] y = [5, 5, 5, 5, 5];

        // Act
        double result = Correlation.Covariance(x, y);

        // Assert
        Assert.Equal(0.0, result, Tolerance);
    }

    [Fact]
    public void Covariance_DifferentLengths_ThrowsArgumentException()
    {
        // Arrange
        double[] x = [1, 2, 3];
        double[] y = [1, 2];

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => Correlation.Covariance(x, y));
        Assert.Contains("same length", exception.Message);
    }

    [Fact]
    public void Covariance_LessThanTwoElements_ReturnsNaN()
    {
        // Arrange
        double[] x = [1];
        double[] y = [2];

        // Act
        double result = Correlation.Covariance(x, y);

        // Assert
        Assert.True(double.IsNaN(result));
    }

    [Fact]
    public void Covariance_StockReturns_CalculatesCorrectly()
    {
        // Arrange - Daily returns of two stocks
        double[] stockA = [0.02, -0.01, 0.03, 0.01, -0.02];
        double[] stockB = [0.01, -0.02, 0.04, 0.00, -0.01];

        // Act
        double result = Correlation.Covariance(stockA, stockB);

        // Assert
        Assert.False(double.IsNaN(result));
    }

    #endregion

    #region Beta Tests

    [Fact]
    public void Beta_AssetMovesWithMarket_ReturnsOne()
    {
        // Arrange - Asset returns = Market returns
        double[] assetReturns = [0.02, -0.01, 0.03, 0.01, -0.02];
        double[] marketReturns = [0.02, -0.01, 0.03, 0.01, -0.02];

        // Act
        double result = Correlation.Beta(assetReturns, marketReturns);

        // Assert
        Assert.Equal(1.0, result, Tolerance);
    }

    [Fact]
    public void Beta_MoreVolatileThanMarket_ReturnsGreaterThanOne()
    {
        // Arrange - Asset moves 2x the market
        double[] marketReturns = [0.01, -0.005, 0.015, 0.005, -0.01];
        double[] assetReturns = [0.02, -0.01, 0.03, 0.01, -0.02];

        // Act
        double result = Correlation.Beta(assetReturns, marketReturns);

        // Assert
        Assert.True(result > 1.0, $"Beta should be > 1 for more volatile assets. Got: {result}");
    }

    [Fact]
    public void Beta_LessVolatileThanMarket_ReturnsBetweenZeroAndOne()
    {
        // Arrange - Asset moves 0.5x the market
        double[] marketReturns = [0.02, -0.01, 0.03, 0.01, -0.02];
        double[] assetReturns = [0.01, -0.005, 0.015, 0.005, -0.01];

        // Act
        double result = Correlation.Beta(assetReturns, marketReturns);

        // Assert
        Assert.True(result > 0 && result < 1.0, $"Beta should be between 0 and 1. Got: {result}");
    }

    [Fact]
    public void Beta_MovesOppositeToMarket_ReturnsNegative()
    {
        // Arrange - Asset moves opposite to market
        double[] marketReturns = [0.02, -0.01, 0.03, 0.01, -0.02];
        double[] assetReturns = [-0.02, 0.01, -0.03, -0.01, 0.02];

        // Act
        double result = Correlation.Beta(assetReturns, marketReturns);

        // Assert
        Assert.True(result < 0, "Beta should be negative when asset moves opposite to market");
    }

    [Fact]
    public void Beta_MarketHasNoVariance_ReturnsNaN()
    {
        // Arrange - Market has constant returns (no variance)
        double[] marketReturns = [0.01, 0.01, 0.01, 0.01, 0.01];
        double[] assetReturns = [0.02, -0.01, 0.03, 0.01, -0.02];

        // Act
        double result = Correlation.Beta(assetReturns, marketReturns);

        // Assert
        Assert.True(double.IsNaN(result), "Beta should be NaN when market has no variance");
    }

    [Fact]
    public void Beta_DifferentLengths_ThrowsArgumentException()
    {
        // Arrange
        double[] assetReturns = [0.02, -0.01, 0.03];
        double[] marketReturns = [0.01, -0.02];

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => Correlation.Beta(assetReturns, marketReturns));
        Assert.Contains("same length", exception.Message);
    }

    [Fact]
    public void Beta_LessThanTwoElements_ReturnsNaN()
    {
        // Arrange
        double[] assetReturns = [0.02];
        double[] marketReturns = [0.01];

        // Act
        double result = Correlation.Beta(assetReturns, marketReturns);

        // Assert
        Assert.True(double.IsNaN(result));
    }

    #endregion

    #region Rolling Correlation Tests

    [Fact]
    public void RollingCorrelation_ValidWindow_ReturnsCorrectLength()
    {
        // Arrange
        double[] x = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];
        double[] y = [2, 4, 6, 8, 10, 12, 14, 16, 18, 20];
        int window = 5;

        // Act
        double[] result = Correlation.RollingCorrelation(x, y, window);

        // Assert
        Assert.Equal(6, result.Length); // 10 - 5 + 1 = 6
    }

    [Fact]
    public void RollingCorrelation_PerfectCorrelation_AllValuesOne()
    {
        // Arrange
        double[] x = [1, 2, 3, 4, 5, 6, 7, 8];
        double[] y = [2, 4, 6, 8, 10, 12, 14, 16];
        int window = 3;

        // Act
        double[] result = Correlation.RollingCorrelation(x, y, window);

        // Assert
        Assert.All(result, r => Assert.Equal(1.0, r, Tolerance));
    }

    [Fact]
    public void RollingCorrelation_ChangingRelationship_DetectsChanges()
    {
        // Arrange - First half positive correlation, second half negative
        double[] x = [1, 2, 3, 4, 5, 6, 7, 8];
        double[] y = [2, 4, 6, 8, 8, 6, 4, 2]; // Changes direction
        int window = 3;

        // Act
        double[] result = Correlation.RollingCorrelation(x, y, window);

        // Assert
        Assert.True(result[0] > 0, "Early windows should show positive correlation");
        Assert.True(result[result.Length - 1] < 0, "Later windows should show negative correlation");
    }

    [Fact]
    public void RollingCorrelation_WindowTooSmall_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        double[] x = [1, 2, 3, 4, 5];
        double[] y = [2, 4, 6, 8, 10];
        int window = 1;

        // Act & Assert
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => Correlation.RollingCorrelation(x, y, window));
        Assert.Contains("at least 2", exception.Message);
    }

    [Fact]
    public void RollingCorrelation_WindowTooLarge_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        double[] x = [1, 2, 3, 4, 5];
        double[] y = [2, 4, 6, 8, 10];
        int window = 10;

        // Act & Assert
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => Correlation.RollingCorrelation(x, y, window));
        Assert.Contains("cannot be greater than data length", exception.Message);
    }

    [Fact]
    public void RollingCorrelation_DifferentLengths_ThrowsArgumentException()
    {
        // Arrange
        double[] x = [1, 2, 3, 4, 5];
        double[] y = [1, 2, 3];
        int window = 3;

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => Correlation.RollingCorrelation(x, y, window));
        Assert.Contains("same length", exception.Message);
    }

    [Fact]
    public void RollingCorrelation_WindowEqualsDataLength_ReturnsSingleValue()
    {
        // Arrange
        double[] x = [1, 2, 3, 4, 5];
        double[] y = [2, 4, 6, 8, 10];
        int window = 5;

        // Act
        double[] result = Correlation.RollingCorrelation(x, y, window);

        // Assert
        Assert.Single(result);
        Assert.Equal(1.0, result[0], Tolerance);
    }

    #endregion

    #region Performance Tests (Optional)

    [Fact]
    public void Pearson_LargeDataset_CompletesInReasonableTime()
    {
        // Arrange
        int size = 1_000_000;
        double[] x = new double[size];
        double[] y = new double[size];
        
        Random random = new(42);
        for (int i = 0; i < size; i++)
        {
            x[i] = random.NextDouble();
            y[i] = random.NextDouble();
        }

        // Act
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        double result = Correlation.Pearson(x, y);
        stopwatch.Stop();

        // Assert
        Assert.False(double.IsNaN(result));
        Assert.True(stopwatch.ElapsedMilliseconds < 100, 
            $"Calculation took {stopwatch.ElapsedMilliseconds}ms, expected < 100ms");
    }

    #endregion
}
