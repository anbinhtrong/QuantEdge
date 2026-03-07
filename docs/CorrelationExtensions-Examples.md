# Correlation Extensions - Usage Examples

## Overview

`CorrelationExtensions` provides fluent, easy-to-use extension methods for calculating correlation coefficients on various collection types including arrays, lists, and enumerables.

## Quick Start

### Basic Usage with Arrays

```csharp
using QuantSharp.Statistics;

// Stock returns data
double[] stockA = [0.02, -0.01, 0.03, 0.01, -0.02];
double[] stockB = [0.01, -0.02, 0.04, 0.00, -0.01];

// Calculate Pearson correlation
double correlation = stockA.GetCorrelation(stockB);
Console.WriteLine($"Correlation: {correlation:F4}");
```

### Using with Lists

```csharp
var prices1 = new List<double> { 100, 102, 101, 103, 105 };
var prices2 = new List<double> { 50, 51, 50.5, 52, 53 };

double correlation = prices1.GetCorrelation(prices2);
```

### Working with IEnumerable (LINQ)

```csharp
var returns = GetStockReturns(); // IEnumerable<double>
var benchmarkReturns = GetBenchmarkReturns();

double correlation = returns.GetCorrelation(benchmarkReturns);
```

## Correlation Methods

### 1. Pearson Correlation (Linear Correlation)

Best for measuring linear relationships between variables.

```csharp
double[] x = [1, 2, 3, 4, 5];
double[] y = [2, 4, 6, 8, 10];

double pearson = x.GetCorrelation(y); // 1.0 (perfect positive)
```

### 2. Spearman Correlation (Rank Correlation)

Better for non-linear monotonic relationships and more robust to outliers.

```csharp
double[] x = [1, 2, 3, 4, 5];
double[] y = [1, 4, 9, 16, 25]; // y = x²

// Pearson would be < 1 due to non-linearity
double pearson = x.GetCorrelation(y); // ~0.976

// Spearman detects perfect monotonic relationship
double spearman = x.GetSpearmanCorrelation(y); // 1.0
```

### 3. R-Squared (Coefficient of Determination)

Measures the proportion of variance explained (0 to 1).

```csharp
double[] x = [1, 2, 3, 4, 5];
double[] y = [2, 4, 6, 8, 10];

double rSquared = x.GetRSquared(y); // 1.0 (100% variance explained)
```

## Correlation Matrix

Calculate pairwise correlations between multiple variables.

```csharp
// Returns for multiple stocks
double[][] stockReturns =
[
    [0.02, -0.01, 0.03, 0.01, -0.02],  // Stock A
    [0.015, -0.015, 0.035, 0.005, -0.025], // Stock B
    [-0.01, 0.02, -0.03, 0.01, 0.01],  // Stock C
    [0.0, 0.0, 0.0, 0.0, 0.0]          // Cash
];

double[,] correlationMatrix = stockReturns.GetCorrelationMatrix();

// Access correlation between stock i and j
double corrAB = correlationMatrix[0, 1]; // Correlation between A and B
```

## Real-World Examples

### Portfolio Diversification Analysis

```csharp
// Analyze correlation between assets in a portfolio
var assets = new Dictionary<string, double[]>
{
    ["Tech Stock"] = [0.02, -0.01, 0.03, 0.02, -0.02],
    ["Bond"] = [-0.005, 0.005, -0.005, 0.01, 0.005],
    ["Gold"] = [-0.01, 0.02, -0.015, 0.005, 0.01]
};

// Find correlations between each pair
foreach (var asset1 in assets)
{
    foreach (var asset2 in assets.Where(a => a.Key != asset1.Key))
    {
        double corr = asset1.Value.GetCorrelation(asset2.Value);
        Console.WriteLine($"{asset1.Key} vs {asset2.Key}: {corr:F4}");
    }
}
```

### Pair Trading Strategy

```csharp
// Find pairs of stocks with high correlation for pair trading
var stockPrices = GetStockPrices(); // Dictionary<string, double[]>

var correlations = new List<(string Stock1, string Stock2, double Correlation)>();

foreach (var stock1 in stockPrices)
{
    foreach (var stock2 in stockPrices.Where(s => s.Key != stock1.Key))
    {
        double corr = stock1.Value.GetCorrelation(stock2.Value);
        if (corr > 0.8) // High correlation threshold
        {
            correlations.Add((stock1.Key, stock2.Key, corr));
        }
    }
}

// Sort by correlation
var topPairs = correlations
    .OrderByDescending(c => c.Correlation)
    .Take(10);
```

### Factor Analysis

```csharp
// Calculate correlation between stock returns and market factors
double[] stockReturns = [0.02, -0.01, 0.03, 0.01, -0.02];
double[] marketReturns = [0.015, -0.008, 0.025, 0.012, -0.015];
double[] sizeFactor = [0.005, -0.002, 0.008, 0.003, -0.005];
double[] valueFactor = [-0.003, 0.004, -0.002, 0.001, 0.003];

// Calculate beta (correlation with market)
double marketCorr = stockReturns.GetCorrelation(marketReturns);
double marketRSquared = stockReturns.GetRSquared(marketReturns);

Console.WriteLine($"Market Correlation: {marketCorr:F4}");
Console.WriteLine($"Market R²: {marketRSquared:F4} ({marketRSquared * 100:F2}% variance explained)");

// Check factor exposures
double sizeCorr = stockReturns.GetCorrelation(sizeFactor);
double valueCorr = stockReturns.GetCorrelation(valueFactor);
```

### Rolling Correlation (Time Series)

```csharp
// Calculate rolling correlation over a sliding window
double[] prices1 = GetPriceHistory(); // Long time series
double[] prices2 = GetPriceHistory2();

int windowSize = 20;
var rollingCorrelations = new List<double>();

for (int i = windowSize; i <= prices1.Length; i++)
{
    var window1 = prices1[(i - windowSize)..i];
    var window2 = prices2[(i - windowSize)..i];
    
    double corr = window1.GetCorrelation(window2);
    rollingCorrelations.Add(corr);
}

// Analyze correlation stability
double avgCorr = rollingCorrelations.Average();
double stdCorr = CalculateStdDev(rollingCorrelations);
Console.WriteLine($"Average Correlation: {avgCorr:F4} ± {stdCorr:F4}");
```

### Risk Management - VaR Correlation

```csharp
// Calculate correlation for Value at Risk (VaR) calculations
var portfolioReturns = new List<double[]>
{
    stockA_returns,
    stockB_returns,
    stockC_returns
};

// Get correlation matrix for covariance calculation
double[,] correlationMatrix = portfolioReturns.GetCorrelationMatrix();

// Use in portfolio VaR calculation
double portfolioVaR = CalculatePortfolioVaR(
    returns: portfolioReturns,
    correlationMatrix: correlationMatrix,
    confidenceLevel: 0.95
);
```

## Performance Tips

### Best Performance: Use Spans

```csharp
double[] data1 = [/* large dataset */];
double[] data2 = [/* large dataset */];

// Most efficient - no allocation
ReadOnlySpan<double> span1 = data1;
ReadOnlySpan<double> span2 = data2;
double corr = span1.GetCorrelation(span2);
```

### Good Performance: Use Arrays or Lists

```csharp
// Good - CollectionsMarshal for zero-copy access
var list1 = new List<double> { /* data */ };
var list2 = new List<double> { /* data */ };
double corr = list1.GetCorrelation(list2);
```

### Avoid: IEnumerable for Large Datasets

```csharp
// Less efficient - materializes to array
IEnumerable<double> enum1 = GetData().Select(x => x.Value);
IEnumerable<double> enum2 = GetData2().Select(x => x.Value);
double corr = enum1.GetCorrelation(enum2); // Allocates arrays internally
```

## Choosing the Right Correlation Method

| Scenario | Recommended Method | Reason |
|----------|-------------------|---------|
| Linear relationships | Pearson (`GetCorrelation`) | Measures linear correlation |
| Non-linear but monotonic | Spearman (`GetSpearmanCorrelation`) | Rank-based, handles non-linearity |
| Data with outliers | Spearman | More robust to outliers |
| Measuring goodness of fit | R² (`GetRSquared`) | Shows % variance explained |
| Normal distribution assumed | Pearson | Optimal for normal data |
| Unknown distribution | Spearman | Non-parametric, distribution-free |

## API Summary

### Extension Methods on Collections

```csharp
// Pearson Correlation
double[] x, y;
List<double> lx, ly;
IEnumerable<double> ex, ey;
ReadOnlySpan<double> sx, sy;

double r1 = x.GetCorrelation(y);
double r2 = lx.GetCorrelation(ly);
double r3 = ex.GetCorrelation(ey);
double r4 = sx.GetCorrelation(sy);

// Spearman Correlation
double s1 = x.GetSpearmanCorrelation(y);
double s2 = lx.GetSpearmanCorrelation(ly);
double s3 = ex.GetSpearmanCorrelation(ey);
double s4 = sx.GetSpearmanCorrelation(sy);

// R-Squared
double rs1 = x.GetRSquared(y);
double rs2 = lx.GetRSquared(ly);
double rs3 = ex.GetRSquared(ey);
double rs4 = sx.GetRSquared(sy);

// Correlation Matrix
double[][] features;
List<List<double>> listFeatures;
IEnumerable<IEnumerable<double>> enumFeatures;

double[,] m1 = features.GetCorrelationMatrix();
double[,] m2 = listFeatures.GetCorrelationMatrix();
double[,] m3 = enumFeatures.GetCorrelationMatrix();
```

## See Also

- [Correlation.cs](./Correlation.cs) - Core correlation implementation
- [CorrelationTests.cs](../../test/QuantSharp.Tests/Statistics/CorrelationTests.cs) - Unit tests
- [CorrelationExtensionsTests.cs](../../test/QuantSharp.Tests/Statistics/CorrelationExtensionsTests.cs) - Extension tests
