using QuantSharp.Statistics;

namespace QuantSharp.Examples;

/// <summary>
/// Demonstrates usage of Correlation and CorrelationExtensions for financial analysis.
/// </summary>
public static class CorrelationDemo
{
    /// <summary>
    /// Runs all correlation examples.
    /// </summary>
    public static void RunAllExamples()
    {
        Console.WriteLine("=== QuantSharp Correlation Examples ===\n");

        Example1_BasicCorrelation();
        Example2_PortfolioDiversification();
        Example3_CompareCorrelationMethods();
        Example4_CorrelationMatrix();
        Example5_FactorAnalysis();
        Example6_CovarianceAnalysis();
        Example7_BetaCalculation();
        Example8_RollingCorrelation();
    }

    /// <summary>
    /// Example 1: Basic correlation calculation between two stocks.
    /// </summary>
    public static void Example1_BasicCorrelation()
    {
        Console.WriteLine("--- Example 1: Basic Stock Correlation ---");
        
        // Daily returns for two stocks
        double[] stockA = [0.02, -0.01, 0.03, 0.01, -0.02, 0.04, -0.01];
        double[] stockB = [0.015, -0.015, 0.035, 0.005, -0.025, 0.045, -0.005];
        
        // Calculate correlation using extension method
        double correlation = stockA.GetCorrelation(stockB);
        
        Console.WriteLine($"Stock A Returns: [{string.Join(", ", stockA)}]");
        Console.WriteLine($"Stock B Returns: [{string.Join(", ", stockB)}]");
        Console.WriteLine($"Correlation: {correlation:F4}");
        Console.WriteLine($"Interpretation: {InterpretCorrelation(correlation)}\n");
    }

    /// <summary>
    /// Example 2: Portfolio diversification analysis.
    /// </summary>
    public static void Example2_PortfolioDiversification()
    {
        Console.WriteLine("--- Example 2: Portfolio Diversification ---");
        
        var portfolio = new Dictionary<string, double[]>
        {
            ["Tech Stock"] = [0.02, -0.01, 0.03, 0.02, -0.02],
            ["Financial Stock"] = [0.015, -0.005, 0.025, 0.015, -0.015],
            ["Government Bond"] = [-0.005, 0.005, -0.005, 0.01, 0.005],
            ["Gold"] = [-0.01, 0.02, -0.015, 0.005, 0.01]
        };
        
        Console.WriteLine("Asset Correlation Analysis:");
        Console.WriteLine(new string('-', 60));
        
        foreach (var asset1 in portfolio)
        {
            foreach (var asset2 in portfolio.Where(a => string.CompareOrdinal(a.Key, asset1.Key) > 0))
            {
                double corr = asset1.Value.GetCorrelation(asset2.Value);
                string interpretation = InterpretDiversification(corr);
                
                Console.WriteLine($"{asset1.Key,-20} vs {asset2.Key,-20}: {corr,6:F4} {interpretation}");
            }
        }
        Console.WriteLine();
    }

    /// <summary>
    /// Example 3: Compare Pearson vs Spearman correlation.
    /// </summary>
    public static void Example3_CompareCorrelationMethods()
    {
        Console.WriteLine("--- Example 3: Pearson vs Spearman Correlation ---");
        
        // Linear relationship
        double[] x1 = [1, 2, 3, 4, 5];
        double[] y1 = [2, 4, 6, 8, 10];
        
        // Non-linear but monotonic relationship
        double[] x2 = [1, 2, 3, 4, 5];
        double[] y2 = [1, 4, 9, 16, 25]; // y = x²
        
        // With outlier
        double[] x3 = [1, 2, 3, 4, 5];
        double[] y3 = [2, 4, 6, 8, 100]; // Last value is outlier
        
        Console.WriteLine("Linear Relationship:");
        Console.WriteLine($"  Pearson:  {x1.GetCorrelation(y1):F4}");
        Console.WriteLine($"  Spearman: {x1.GetSpearmanCorrelation(y1):F4}");
        Console.WriteLine($"  R²:       {x1.GetRSquared(y1):F4}\n");
        
        Console.WriteLine("Non-linear (y = x²):");
        Console.WriteLine($"  Pearson:  {x2.GetCorrelation(y2):F4}");
        Console.WriteLine($"  Spearman: {x2.GetSpearmanCorrelation(y2):F4}");
        Console.WriteLine($"  → Spearman detects perfect monotonic relationship!\n");
        
        Console.WriteLine("With Outlier:");
        Console.WriteLine($"  Pearson:  {x3.GetCorrelation(y3):F4}");
        Console.WriteLine($"  Spearman: {x3.GetSpearmanCorrelation(y3):F4}");
        Console.WriteLine($"  → Spearman is more robust to outliers!\n");
    }

    /// <summary>
    /// Example 4: Correlation matrix for multiple assets.
    /// </summary>
    public static void Example4_CorrelationMatrix()
    {
        Console.WriteLine("--- Example 4: Correlation Matrix ---");
        
        // Returns for multiple stocks over 5 days
        double[][] stockReturns =
        [
            [0.02, -0.01, 0.03, 0.01, -0.02],     // Tech Stock
            [0.015, -0.015, 0.035, 0.005, -0.025], // Similar to Tech
            [-0.01, 0.02, -0.03, 0.01, 0.01],     // Defensive Stock
            [0.0, 0.0, 0.0, 0.0, 0.0]             // Cash
        ];
        
        string[] assetNames = ["Tech", "Tech2", "Defensive", "Cash"];
        
        // Calculate correlation matrix using extension method
        double[,] matrix = stockReturns.GetCorrelationMatrix();
        
        Console.WriteLine("Correlation Matrix:");
        Console.Write("         ");
        foreach (var name in assetNames)
        {
            Console.Write($"{name,10}");
        }
        Console.WriteLine();
        
        for (int i = 0; i < assetNames.Length; i++)
        {
            Console.Write($"{assetNames[i],-8} ");
            for (int j = 0; j < assetNames.Length; j++)
            {
                Console.Write($"{matrix[i, j],10:F4}");
            }
            Console.WriteLine();
        }
        Console.WriteLine();
    }

    /// <summary>
    /// Example 5: Factor analysis - stock returns vs market factors.
    /// </summary>
    public static void Example5_FactorAnalysis()
    {
        Console.WriteLine("--- Example 5: Factor Analysis ---");
        
        double[] stockReturns = [0.02, -0.01, 0.03, 0.01, -0.02];
        double[] marketReturns = [0.015, -0.008, 0.025, 0.012, -0.015];
        double[] sizeFactor = [0.005, -0.002, 0.008, 0.003, -0.005];
        double[] valueFactor = [-0.003, 0.004, -0.002, 0.001, 0.003];
        
        double marketCorr = stockReturns.GetCorrelation(marketReturns);
        double marketR2 = stockReturns.GetRSquared(marketReturns);
        double sizeCorr = stockReturns.GetCorrelation(sizeFactor);
        double valueCorr = stockReturns.GetCorrelation(valueFactor);
        
        Console.WriteLine("Factor Exposures:");
        Console.WriteLine($"Market Correlation: {marketCorr:F4}");
        Console.WriteLine($"Market R²:          {marketR2:F4} ({marketR2 * 100:F2}% variance explained)");
        Console.WriteLine($"Size Factor:        {sizeCorr:F4}");
        Console.WriteLine($"Value Factor:       {valueCorr:F4}\n");
    }

    #endregion

    /// <summary>
    /// Example 6: Covariance analysis for portfolio construction.
    /// </summary>
    public static void Example6_CovarianceAnalysis()
    {
        Console.WriteLine("--- Example 6: Covariance Analysis ---");

        double[] stockA = [0.02, -0.01, 0.03, 0.01, -0.02, 0.04, -0.01];
        double[] stockB = [0.015, -0.015, 0.035, 0.005, -0.025, 0.045, -0.005];
        double[] stockC = [-0.01, 0.02, -0.03, 0.01, 0.01, -0.02, 0.015];

        double covAB = stockA.GetCovariance(stockB);
        double covAC = stockA.GetCovariance(stockC);
        double covBC = stockB.GetCovariance(stockC);

        double corrAB = stockA.GetCorrelation(stockB);
        double corrAC = stockA.GetCorrelation(stockC);

        Console.WriteLine("Covariance vs Correlation:");
        Console.WriteLine($"Stock A vs B - Cov: {covAB:F6}, Corr: {corrAB:F4}");
        Console.WriteLine($"Stock A vs C - Cov: {covAC:F6}, Corr: {corrAC:F4}");
        Console.WriteLine($"Stock B vs C - Cov: {covBC:F6}, Corr: {covBC:F4}");
        Console.WriteLine("\n💡 Covariance shows absolute relationship strength,");
        Console.WriteLine("   Correlation normalizes it to [-1, 1] range.\n");
    }

    /// <summary>
    /// Example 7: Beta calculation for CAPM analysis.
    /// </summary>
    public static void Example7_BetaCalculation()
    {
        Console.WriteLine("--- Example 7: Beta Calculation (CAPM) ---");

        // Market returns (e.g., S&P 500)
        double[] marketReturns = [0.01, -0.005, 0.015, 0.01, -0.01, 0.02, -0.005];

        // Different types of stocks
        double[] defensiveStock = [0.005, -0.002, 0.008, 0.004, -0.005, 0.01, -0.002];  // β < 1
        double[] marketStock = [0.01, -0.005, 0.015, 0.01, -0.01, 0.02, -0.005];        // β ≈ 1
        double[] aggressiveStock = [0.02, -0.01, 0.03, 0.02, -0.02, 0.04, -0.01];      // β > 1

        double betaDefensive = defensiveStock.GetBeta(marketReturns);
        double betaMarket = marketStock.GetBeta(marketReturns);
        double betaAggressive = aggressiveStock.GetBeta(marketReturns);

        Console.WriteLine("Beta Analysis:");
        Console.WriteLine($"Defensive Stock:  β = {betaDefensive:F4} → {InterpretBeta(betaDefensive)}");
        Console.WriteLine($"Market Stock:     β = {betaMarket:F4} → {InterpretBeta(betaMarket)}");
        Console.WriteLine($"Aggressive Stock: β = {betaAggressive:F4} → {InterpretBeta(betaAggressive)}");

        Console.WriteLine("\n💡 Beta measures systematic risk:");
        Console.WriteLine("   β > 1: More volatile than market (higher risk/return)");
        Console.WriteLine("   β = 1: Moves with market");
        Console.WriteLine("   β < 1: Less volatile than market (lower risk/return)\n");
    }

    /// <summary>
    /// Example 8: Rolling correlation for time-varying relationships.
    /// </summary>
    public static void Example8_RollingCorrelation()
    {
        Console.WriteLine("--- Example 8: Rolling Correlation ---");

        // Simulated stock returns over 20 days
        // Correlation changes over time (high → low → negative)
        double[] stockA = [0.02, 0.03, 0.01, -0.01, 0.02, 0.03, 0.01, 0.00, -0.01, -0.02,
                          0.01, 0.02, 0.00, -0.01, 0.01, -0.02, -0.03, -0.01, 0.00, 0.01];
        double[] stockB = [0.025, 0.035, 0.015, -0.005, 0.025, 0.02, 0.01, 0.005, 0.01, 0.015,
                          0.00, -0.01, -0.015, 0.02, -0.02, 0.03, 0.04, 0.02, -0.01, -0.02];

        int window = 5;
        double[] rollingCorr = stockA.GetRollingCorrelation(stockB, window);

        Console.WriteLine($"Rolling {window}-day Correlation (Total {rollingCorr.Length} windows):");
        Console.WriteLine(new string('-', 60));

        for (int i = 0; i < rollingCorr.Length; i++)
        {
            int dayEnd = i + window;
            string trend = i > 0 
                ? (rollingCorr[i] > rollingCorr[i - 1] ? "↑" : "↓")
                : " ";

            Console.WriteLine($"Days {i + 1,2}-{dayEnd,2}: Corr = {rollingCorr[i],6:F4} {trend}");
        }

        Console.WriteLine("\n💡 Rolling correlation helps detect:");
        Console.WriteLine("   - Regime changes in market conditions");
        Console.WriteLine("   - Dynamic hedging opportunities");
        Console.WriteLine("   - Time-varying portfolio risk\n");
    }

    #region Helper Methods

    private static string InterpretCorrelation(double correlation)
    {
        return Math.Abs(correlation) switch
        {
            >= 0.9 => "Very strong correlation",
            >= 0.7 => "Strong correlation",
            >= 0.5 => "Moderate correlation",
            >= 0.3 => "Weak correlation",
            _ => "Very weak or no correlation"
        };
    }

    private static string InterpretDiversification(double correlation)
    {
        return correlation switch
        {
            < 0 => "✓ Good (negative correlation)",
            < 0.3 => "✓ Good (low correlation)",
            < 0.7 => "⚠ Moderate (some correlation)",
            _ => "✗ Poor (high correlation)"
        };
    }

    #endregion
}
