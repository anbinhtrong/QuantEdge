using QuantEdge.Statistics;

namespace QuantEdge.Tests.Statistics;

/// <summary>
/// Unit tests for the CorrelationExtensions class.
/// </summary>
public class CorrelationExtensionsTests
{
    private const double Tolerance = 1e-10;

    #region Pearson Correlation Extension Tests

    [Fact]
    public void GetCorrelation_WithDoubleArrays_CalculatesCorrectly()
    {
        // Arrange
        double[] x = [1, 2, 3, 4, 5];
        double[] y = [2, 4, 6, 8, 10];

        // Act
        double result = x.GetCorrelation(y);

        // Assert
        Assert.Equal(1.0, result, Tolerance);
    }

    [Fact]
    public void GetCorrelation_WithLists_CalculatesCorrectly()
    {
        // Arrange
        var x = new List<double> { 1, 2, 3, 4, 5 };
        var y = new List<double> { 10, 8, 6, 4, 2 };

        // Act
        double result = x.GetCorrelation(y);

        // Assert
        Assert.Equal(-1.0, result, Tolerance);
    }

    [Fact]
    public void GetCorrelation_WithIEnumerable_CalculatesCorrectly()
    {
        // Arrange
        IEnumerable<double> x = [1, 2, 3, 4, 5];
        IEnumerable<double> y = [2, 4, 6, 8, 10];

        // Act
        double result = x.GetCorrelation(y);

        // Assert
        Assert.Equal(1.0, result, Tolerance);
    }

    [Fact]
    public void GetCorrelation_WithSpans_CalculatesCorrectly()
    {
        // Arrange
        double[] xArray = [1, 2, 3, 4, 5];
        double[] yArray = [2, 4, 6, 8, 10];
        ReadOnlySpan<double> x = xArray;
        ReadOnlySpan<double> y = yArray;

        // Act
        double result = x.GetCorrelation(y);

        // Assert
        Assert.Equal(1.0, result, Tolerance);
    }

    [Fact]
    public void GetCorrelation_WithNullArray_ThrowsArgumentNullException()
    {
        // Arrange
        double[]? x = null;
        double[] y = [1, 2, 3];

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => x!.GetCorrelation(y));
    }

    [Fact]
    public void GetCorrelation_WithNullList_ThrowsArgumentNullException()
    {
        // Arrange
        List<double>? x = null;
        var y = new List<double> { 1, 2, 3 };

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => x!.GetCorrelation(y));
    }

    [Fact]
    public void GetCorrelation_RealWorldScenario_StockReturns()
    {
        // Arrange - Daily returns for two stocks
        double[] stockA = [0.02, -0.01, 0.03, 0.01, -0.02, 0.04, -0.01];
        double[] stockB = [0.015, -0.015, 0.035, 0.005, -0.025, 0.045, -0.005];

        // Act
        double correlation = stockA.GetCorrelation(stockB);

        // Assert
        Assert.True(correlation >= -1.0 && correlation <= 1.0);
        Assert.True(correlation > 0.9, "Expected high positive correlation for similar stock returns");
    }

    #endregion

    #region Spearman Correlation Extension Tests

    [Fact]
    public void GetSpearmanCorrelation_WithDoubleArrays_CalculatesCorrectly()
    {
        // Arrange - Non-linear but monotonic
        double[] x = [1, 2, 3, 4, 5];
        double[] y = [1, 4, 9, 16, 25]; // y = x²

        // Act
        double result = x.GetSpearmanCorrelation(y);

        // Assert
        Assert.Equal(1.0, result, Tolerance);
    }

    [Fact]
    public void GetSpearmanCorrelation_WithLists_CalculatesCorrectly()
    {
        // Arrange
        var x = new List<double> { 1, 2, 3, 4, 5 };
        var y = new List<double> { 25, 16, 9, 4, 1 };

        // Act
        double result = x.GetSpearmanCorrelation(y);

        // Assert
        Assert.Equal(-1.0, result, Tolerance);
    }

    [Fact]
    public void GetSpearmanCorrelation_WithIEnumerable_CalculatesCorrectly()
    {
        // Arrange
        IEnumerable<double> x = [1, 2, 3, 4, 5];
        IEnumerable<double> y = [1, 4, 9, 16, 25];

        // Act
        double result = x.GetSpearmanCorrelation(y);

        // Assert
        Assert.Equal(1.0, result, Tolerance);
    }

    [Fact]
    public void GetSpearmanCorrelation_WithSpans_CalculatesCorrectly()
    {
        // Arrange
        double[] xArray = [1, 2, 3, 4, 5];
        double[] yArray = [1, 4, 9, 16, 25];
        ReadOnlySpan<double> x = xArray;
        ReadOnlySpan<double> y = yArray;

        // Act
        double result = x.GetSpearmanCorrelation(y);

        // Assert
        Assert.Equal(1.0, result, Tolerance);
    }

    [Fact]
    public void GetSpearmanCorrelation_WithOutliers_MoreRobustThanPearson()
    {
        // Arrange - Data with outlier
        double[] x = [1, 2, 3, 4, 5];
        double[] y = [2, 4, 6, 8, 100]; // Last value is outlier

        // Act
        double spearman = x.GetSpearmanCorrelation(y);
        double pearson = x.GetCorrelation(y);

        // Assert - Spearman should be less affected by outlier
        Assert.True(Math.Abs(spearman - 1.0) < Math.Abs(pearson - 1.0),
            "Spearman correlation should be more robust to outliers");
    }

    #endregion

    #region R-Squared Extension Tests

    [Fact]
    public void GetRSquared_WithDoubleArrays_CalculatesCorrectly()
    {
        // Arrange
        double[] x = [1, 2, 3, 4, 5];
        double[] y = [2, 4, 6, 8, 10];

        // Act
        double result = x.GetRSquared(y);

        // Assert
        Assert.Equal(1.0, result, Tolerance);
    }

    [Fact]
    public void GetRSquared_WithLists_CalculatesCorrectly()
    {
        // Arrange
        var x = new List<double> { 1, 2, 3, 4, 5 };
        var y = new List<double> { 10, 8, 6, 4, 2 };

        // Act
        double result = x.GetRSquared(y);

        // Assert
        Assert.Equal(1.0, result, Tolerance); // R² of perfect negative correlation is 1
    }

    [Fact]
    public void GetRSquared_WithIEnumerable_CalculatesCorrectly()
    {
        // Arrange
        IEnumerable<double> x = [1, 2, 3, 4, 5];
        IEnumerable<double> y = [2, 4, 6, 8, 10];

        // Act
        double result = x.GetRSquared(y);

        // Assert
        Assert.Equal(1.0, result, Tolerance);
    }

    [Fact]
    public void GetRSquared_WithSpans_CalculatesCorrectly()
    {
        // Arrange
        double[] xArray = [1, 2, 3, 4, 5];
        double[] yArray = [2, 4, 6, 8, 10];
        ReadOnlySpan<double> x = xArray;
        ReadOnlySpan<double> y = yArray;

        // Act
        double result = x.GetRSquared(y);

        // Assert
        Assert.Equal(1.0, result, Tolerance);
    }

    [Fact]
    public void GetRSquared_PartialCorrelation_ReturnsBetweenZeroAndOne()
    {
        // Arrange
        double[] x = [1, 2, 3, 4, 5];
        double[] y = [2, 3, 5, 7, 11];

        // Act
        double result = x.GetRSquared(y);

        // Assert
        Assert.True(result >= 0.0 && result <= 1.0);
    }

    #endregion

    #region Covariance Extension Tests

    [Fact]
    public void GetCovariance_WithDoubleArrays_CalculatesCorrectly()
    {
        // Arrange
        double[] x = [1, 2, 3, 4, 5];
        double[] y = [2, 4, 6, 8, 10];

        // Act
        double result = x.GetCovariance(y);

        // Assert
        Assert.Equal(5.0, result, Tolerance);
    }

    [Fact]
    public void GetCovariance_WithLists_CalculatesCorrectly()
    {
        // Arrange
        var x = new List<double> { 1, 2, 3, 4, 5 };
        var y = new List<double> { 10, 8, 6, 4, 2 };

        // Act
        double result = x.GetCovariance(y);

        // Assert
        Assert.True(result < 0, "Covariance should be negative for negatively related data");
    }

    [Fact]
    public void GetCovariance_WithIEnumerable_CalculatesCorrectly()
    {
        // Arrange
        IEnumerable<double> x = [1, 2, 3, 4, 5];
        IEnumerable<double> y = [2, 4, 6, 8, 10];

        // Act
        double result = x.GetCovariance(y);

        // Assert
        Assert.Equal(5.0, result, Tolerance);
    }

    [Fact]
    public void GetCovariance_WithSpans_CalculatesCorrectly()
    {
        // Arrange
        double[] xArray = [1, 2, 3, 4, 5];
        double[] yArray = [2, 4, 6, 8, 10];
        ReadOnlySpan<double> x = xArray;
        ReadOnlySpan<double> y = yArray;

        // Act
        double result = x.GetCovariance(y);

        // Assert
        Assert.Equal(5.0, result, Tolerance);
    }

    #endregion

    #region Beta Extension Tests

    [Fact]
    public void GetBeta_WithDoubleArrays_CalculatesCorrectly()
    {
        // Arrange - Asset returns = Market returns
        double[] assetReturns = [0.02, -0.01, 0.03, 0.01, -0.02];
        double[] marketReturns = [0.02, -0.01, 0.03, 0.01, -0.02];

        // Act
        double result = assetReturns.GetBeta(marketReturns);

        // Assert
        Assert.Equal(1.0, result, Tolerance);
    }

    [Fact]
    public void GetBeta_WithLists_CalculatesCorrectly()
    {
        // Arrange
        var assetReturns = new List<double> { 0.02, -0.01, 0.03, 0.01, -0.02 };
        var marketReturns = new List<double> { 0.01, -0.005, 0.015, 0.005, -0.01 };

        // Act
        double result = assetReturns.GetBeta(marketReturns);

        // Assert
        Assert.True(result > 1.0, "Asset is more volatile than market");
    }

    [Fact]
    public void GetBeta_WithIEnumerable_CalculatesCorrectly()
    {
        // Arrange
        IEnumerable<double> assetReturns = [0.02, -0.01, 0.03, 0.01, -0.02];
        IEnumerable<double> marketReturns = [0.02, -0.01, 0.03, 0.01, -0.02];

        // Act
        double result = assetReturns.GetBeta(marketReturns);

        // Assert
        Assert.Equal(1.0, result, Tolerance);
    }

    [Fact]
    public void GetBeta_WithSpans_CalculatesCorrectly()
    {
        // Arrange
        double[] assetArray = [0.02, -0.01, 0.03, 0.01, -0.02];
        double[] marketArray = [0.02, -0.01, 0.03, 0.01, -0.02];
        ReadOnlySpan<double> assetReturns = assetArray;
        ReadOnlySpan<double> marketReturns = marketArray;

        // Act
        double result = assetReturns.GetBeta(marketReturns);

        // Assert
        Assert.Equal(1.0, result, Tolerance);
    }

    [Fact]
    public void GetBeta_RealWorldScenario_TechStockVsMarket()
    {
        // Arrange - Tech stock typically has Beta > 1
        double[] techStockReturns = [0.03, -0.02, 0.05, 0.02, -0.03, 0.06, -0.01];
        double[] marketReturns = [0.02, -0.01, 0.03, 0.01, -0.02, 0.04, -0.01];

        // Act
        double beta = techStockReturns.GetBeta(marketReturns);

        // Assert
        Assert.True(beta > 1.0, $"Tech stocks typically have Beta > 1. Got: {beta:F4}");
    }

    #endregion

    #region Rolling Correlation Extension Tests

    [Fact]
    public void GetRollingCorrelation_WithDoubleArrays_CalculatesCorrectly()
    {
        // Arrange
        double[] x = [1, 2, 3, 4, 5, 6, 7, 8];
        double[] y = [2, 4, 6, 8, 10, 12, 14, 16];
        int window = 3;

        // Act
        double[] result = x.GetRollingCorrelation(y, window);

        // Assert
        Assert.Equal(6, result.Length); // 8 - 3 + 1 = 6
        Assert.All(result, r => Assert.Equal(1.0, r, Tolerance));
    }

    [Fact]
    public void GetRollingCorrelation_WithLists_CalculatesCorrectly()
    {
        // Arrange
        var x = new List<double> { 1, 2, 3, 4, 5, 6, 7, 8 };
        var y = new List<double> { 2, 4, 6, 8, 10, 12, 14, 16 };
        int window = 3;

        // Act
        double[] result = x.GetRollingCorrelation(y, window);

        // Assert
        Assert.Equal(6, result.Length);
    }

    [Fact]
    public void GetRollingCorrelation_WithIEnumerable_CalculatesCorrectly()
    {
        // Arrange
        IEnumerable<double> x = [1, 2, 3, 4, 5, 6];
        IEnumerable<double> y = [2, 4, 6, 8, 10, 12];
        int window = 3;

        // Act
        double[] result = x.GetRollingCorrelation(y, window);

        // Assert
        Assert.Equal(4, result.Length);
    }

    [Fact]
    public void GetRollingCorrelation_WithSpans_CalculatesCorrectly()
    {
        // Arrange
        double[] xArray = [1, 2, 3, 4, 5, 6];
        double[] yArray = [2, 4, 6, 8, 10, 12];
        ReadOnlySpan<double> x = xArray;
        ReadOnlySpan<double> y = yArray;
        int window = 3;

        // Act
        double[] result = x.GetRollingCorrelation(y, window);

        // Assert
        Assert.Equal(4, result.Length);
    }

    [Fact]
    public void GetRollingCorrelation_DetectsChangingMarketConditions()
    {
        // Arrange - Correlation changes over time
        double[] stockA = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];
        double[] stockB = [2, 4, 6, 8, 10, 10, 8, 6, 4, 2]; // Direction changes
        int window = 5;

        // Act
        double[] rollingCorr = stockA.GetRollingCorrelation(stockB, window);

        // Assert
        Assert.Equal(6, rollingCorr.Length);

        // First window should show positive correlation
        Assert.True(rollingCorr[0] > 0, "Early period shows positive correlation");

        // Last window should show negative correlation
        Assert.True(rollingCorr[rollingCorr.Length - 1] < 0, "Later period shows negative correlation");
    }

    #endregion

    #region Correlation Matrix Extension Tests

    [Fact]
    public void GetCorrelationMatrix_WithDoubleArrays_CalculatesCorrectly()
    {
        // Arrange
        double[][] features =
        [
            [1, 2, 3, 4, 5],
            [2, 4, 6, 8, 10],
            [5, 4, 3, 2, 1]
        ];

        // Act
        double[,] result = features.GetCorrelationMatrix();

        // Assert
        Assert.Equal(3, result.GetLength(0));
        Assert.Equal(3, result.GetLength(1));
        
        // Diagonal should be 1.0
        Assert.Equal(1.0, result[0, 0], Tolerance);
        Assert.Equal(1.0, result[1, 1], Tolerance);
        Assert.Equal(1.0, result[2, 2], Tolerance);
        
        // Perfect correlations
        Assert.Equal(1.0, result[0, 1], Tolerance);
        Assert.Equal(-1.0, result[0, 2], Tolerance);
    }

    [Fact]
    public void GetCorrelationMatrix_WithLists_CalculatesCorrectly()
    {
        // Arrange
        var features = new List<List<double>>
        {
            new() { 1, 2, 3, 4, 5 },
            new() { 2, 4, 6, 8, 10 }
        };

        // Act
        double[,] result = features.GetCorrelationMatrix();

        // Assert
        Assert.Equal(2, result.GetLength(0));
        Assert.Equal(2, result.GetLength(1));
        Assert.Equal(1.0, result[0, 1], Tolerance);
    }

    [Fact]
    public void GetCorrelationMatrix_WithIEnumerable_CalculatesCorrectly()
    {
        // Arrange
        IEnumerable<IEnumerable<double>> features =
        [
            new[] { 1.0, 2, 3, 4, 5 },
            new[] { 2.0, 4, 6, 8, 10 }
        ];

        // Act
        double[,] result = features.GetCorrelationMatrix();

        // Assert
        Assert.Equal(2, result.GetLength(0));
        Assert.Equal(2, result.GetLength(1));
        Assert.Equal(1.0, result[0, 1], Tolerance);
    }

    [Fact]
    public void GetCorrelationMatrix_RealWorldScenario_MultipleStocks()
    {
        // Arrange - Returns for 4 stocks over 5 days
        double[][] stockReturns =
        [
            [0.02, -0.01, 0.03, 0.01, -0.02],  // Stock A
            [0.015, -0.015, 0.035, 0.005, -0.025], // Stock B (similar to A)
            [-0.01, 0.02, -0.03, 0.01, 0.01],  // Stock C (different pattern)
            [0.0, 0.0, 0.0, 0.0, 0.0]          // Stock D (cash, no correlation)
        ];

        // Act
        double[,] correlationMatrix = stockReturns.GetCorrelationMatrix();

        // Assert
        Assert.Equal(4, correlationMatrix.GetLength(0));
        Assert.Equal(4, correlationMatrix.GetLength(1));
        
        // Matrix should be symmetric
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                Assert.Equal(correlationMatrix[i, j], correlationMatrix[j, i], Tolerance);
            }
        }
        
        // Stock A and B should have high correlation
        Assert.True(correlationMatrix[0, 1] > 0.9);
        
        // Stock D (all zeros) should have zero correlation with others
        Assert.Equal(0.0, correlationMatrix[0, 3], Tolerance);
        Assert.Equal(0.0, correlationMatrix[1, 3], Tolerance);
        Assert.Equal(0.0, correlationMatrix[2, 3], Tolerance);
    }

    [Fact]
    public void GetCorrelationMatrix_WithNull_ThrowsArgumentNullException()
    {
        // Arrange
        double[][]? features = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => features!.GetCorrelationMatrix());
    }

    #endregion

    #region Fluent API Integration Tests

    [Fact]
    public void FluentAPI_ChainMultipleCorrelations_WorksCorrectly()
    {
        // Arrange
        double[] prices = [100, 102, 101, 103, 105];
        double[] volumes = [1000, 1100, 900, 1200, 1300];
        double[] momentum = [0, 2, -1, 2, 2];

        // Act
        double priceVolumeCorr = prices.GetCorrelation(volumes);
        double priceMomentumCorr = prices.GetCorrelation(momentum);
        double volumeMomentumCorr = volumes.GetCorrelation(momentum);

        // Assert
        Assert.True(priceVolumeCorr >= -1.0 && priceVolumeCorr <= 1.0);
        Assert.True(priceMomentumCorr >= -1.0 && priceMomentumCorr <= 1.0);
        Assert.True(volumeMomentumCorr >= -1.0 && volumeMomentumCorr <= 1.0);
    }

    [Fact]
    public void FluentAPI_CompareCorrelationMethods_ReturnsConsistentResults()
    {
        // Arrange
        double[] x = [1, 2, 3, 4, 5];
        double[] y = [2, 4, 6, 8, 10];

        // Act
        double pearson = x.GetCorrelation(y);
        double spearman = x.GetSpearmanCorrelation(y);
        double rSquared = x.GetRSquared(y);

        // Assert - For perfect linear relationship, all should indicate perfect correlation
        Assert.Equal(1.0, pearson, Tolerance);
        Assert.Equal(1.0, spearman, Tolerance);
        Assert.Equal(1.0, rSquared, Tolerance);
        Assert.Equal(pearson * pearson, rSquared, Tolerance);
    }

    [Fact]
    public void FluentAPI_LINQ_Integration_WorksSeamlessly()
    {
        // Arrange
        var dataset = new[]
        {
            (X: new[] { 1.0, 2, 3, 4, 5 }, Y: new[] { 2.0, 4, 6, 8, 10 }),
            (X: new[] { 1.0, 2, 3, 4, 5 }, Y: new[] { 5.0, 4, 3, 2, 1 }),
            (X: new[] { 1.0, 2, 3, 4, 5 }, Y: new[] { 3.0, 3, 3, 3, 3 })
        };

        // Act - Calculate correlations using LINQ
        var correlations = dataset
            .Select(d => d.X.GetCorrelation(d.Y))
            .ToList();

        // Assert
        Assert.Equal(3, correlations.Count);
        Assert.Equal(1.0, correlations[0], Tolerance);   // Perfect positive
        Assert.Equal(-1.0, correlations[1], Tolerance);  // Perfect negative
        Assert.Equal(0.0, correlations[2], Tolerance);   // No correlation
    }

    #endregion
}
