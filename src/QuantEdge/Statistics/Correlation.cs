namespace QuantEdge.Statistics;

/// <summary>
/// Provides statistical correlation calculation methods.
/// </summary>
public static class Correlation
{
    /// <summary>
    /// Calculates the Pearson correlation coefficient between two datasets.
    /// </summary>
    /// <param name="x">The first dataset.</param>
    /// <param name="y">The second dataset.</param>
    /// <returns>
    /// The Pearson correlation coefficient in the range [-1, 1].
    /// Returns NaN if either dataset has fewer than 2 elements.
    /// Returns 0 if the denominator is zero (no variance in either dataset).
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown when the two datasets have different lengths.
    /// </exception>
    /// <remarks>
    /// The Pearson correlation coefficient measures the linear relationship between two variables.
    /// A value of 1 indicates perfect positive correlation, -1 indicates perfect negative correlation,
    /// and 0 indicates no linear correlation.
    /// 
    /// Formula: r = Σ((xi - x̄)(yi - ȳ)) / √(Σ(xi - x̄)² * Σ(yi - ȳ)²)
    /// 
    /// Time complexity: O(n)
    /// Space complexity: O(1)
    /// </remarks>
    public static double Pearson(ReadOnlySpan<double> x, ReadOnlySpan<double> y)
    {
        if (x.Length != y.Length)
        {
            throw new ArgumentException(
                $"Both datasets must have the same length. x.Length={x.Length}, y.Length={y.Length}",
                nameof(y));
        }

        int n = x.Length;
        if (n < 2)
        {
            return double.NaN;
        }

        double sumX = 0, sumY = 0, sumXY = 0, sumX2 = 0, sumY2 = 0;

        for (int i = 0; i < n; i++)
        {
            double xi = x[i];
            double yi = y[i];
            
            sumX += xi;
            sumY += yi;
            sumXY += xi * yi;
            sumX2 += xi * xi;
            sumY2 += yi * yi;
        }

        double numerator = (n * sumXY) - (sumX * sumY);
        double denominator = Math.Sqrt(((n * sumX2) - (sumX * sumX)) * ((n * sumY2) - (sumY * sumY)));

        if (denominator == 0)
        {
            return 0;
        }

        return numerator / denominator;
    }

    /// <summary>
    /// Calculates the correlation matrix from multiple feature arrays.
    /// </summary>
    /// <param name="features">
    /// An array of feature arrays, where each inner array represents a feature vector.
    /// All feature arrays must have the same length.
    /// </param>
    /// <returns>
    /// A symmetric correlation matrix where element [i,j] represents the correlation
    /// between feature i and feature j. Diagonal elements are always 1.0.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when features is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when features is empty or contains arrays of different lengths.
    /// </exception>
    /// <remarks>
    /// The resulting matrix is symmetric (matrix[i,j] = matrix[j,i]) and has 1.0 on the diagonal.
    /// This method is useful for analyzing relationships between multiple variables in financial data.
    /// 
    /// Time complexity: O(n² * m) where n is the number of features and m is the feature length.
    /// Space complexity: O(n²)
    /// </remarks>
    public static double[,] PearsonMatrix(double[][] features)
    {
        if (features == null)
        {
            throw new ArgumentNullException(nameof(features), "Features array cannot be null.");
        }

        if (features.Length == 0)
        {
            throw new ArgumentException("Features array cannot be empty.", nameof(features));
        }

        // Validate all features have the same length
        int featureLength = features[0].Length;
        for (int i = 1; i < features.Length; i++)
        {
            if (features[i].Length != featureLength)
            {
                throw new ArgumentException(
                    $"All feature arrays must have the same length. Feature[0].Length={featureLength}, Feature[{i}].Length={features[i].Length}",
                    nameof(features));
            }
        }

        int size = features.Length;
        double[,] matrix = new double[size, size];

        // Diagonal elements are always 1.0 (correlation with itself)
        for (int i = 0; i < size; i++)
        {
            matrix[i, i] = 1.0;
        }

        // Calculate upper triangle and mirror to lower triangle (symmetric matrix)
        for (int i = 0; i < size; i++)
        {
            for (int j = i + 1; j < size; j++)
            {
                double correlation = Pearson(features[i], features[j]);
                matrix[i, j] = correlation;
                matrix[j, i] = correlation;
            }
        }

        return matrix;
    }

    /// <summary>
    /// Calculates the Spearman rank correlation coefficient between two datasets.
    /// </summary>
    /// <param name="x">The first dataset.</param>
    /// <param name="y">The second dataset.</param>
    /// <returns>
    /// The Spearman correlation coefficient in the range [-1, 1].
    /// Returns NaN if either dataset has fewer than 2 elements.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown when the two datasets have different lengths.
    /// </exception>
    /// <remarks>
    /// The Spearman correlation coefficient is a non-parametric measure of rank correlation.
    /// It assesses how well the relationship between two variables can be described using a monotonic function.
    /// This is useful when dealing with non-linear relationships or outliers.
    /// 
    /// The method converts values to ranks and then applies Pearson correlation on the ranks.
    /// 
    /// Time complexity: O(n log n) due to sorting
    /// Space complexity: O(n)
    /// </remarks>
    public static double Spearman(ReadOnlySpan<double> x, ReadOnlySpan<double> y)
    {
        if (x.Length != y.Length)
        {
            throw new ArgumentException(
                $"Both datasets must have the same length. x.Length={x.Length}, y.Length={y.Length}",
                nameof(y));
        }

        int n = x.Length;
        if (n < 2)
        {
            return double.NaN;
        }

        // Convert to ranks
        double[] ranksX = ConvertToRanks(x);
        double[] ranksY = ConvertToRanks(y);

        // Calculate Pearson correlation on ranks
        return Pearson(ranksX, ranksY);
    }

    /// <summary>
    /// Converts a dataset to its rank representation.
    /// </summary>
    /// <param name="data">The input dataset.</param>
    /// <returns>An array where each element is replaced by its rank.</returns>
    /// <remarks>
    /// Ranks start from 1. Tied values receive the average of their ranks.
    /// For example: [10, 20, 20, 40] becomes [1, 2.5, 2.5, 4]
    /// </remarks>
    private static double[] ConvertToRanks(ReadOnlySpan<double> data)
    {
        int n = data.Length;
        var indexed = new (double Value, int Index)[n];
        
        for (int i = 0; i < n; i++)
        {
            indexed[i] = (data[i], i);
        }

        // Sort by value
        Array.Sort(indexed, (a, b) => a.Value.CompareTo(b.Value));

        double[] ranks = new double[n];
        
        int i0 = 0;
        while (i0 < n)
        {
            // Find range of tied values
            int i1 = i0;
            while (i1 < n && indexed[i1].Value == indexed[i0].Value)
            {
                i1++;
            }

            // Assign average rank to all tied values
            double avgRank = (i0 + i1 + 1) / 2.0; // Ranks are 1-based
            for (int i = i0; i < i1; i++)
            {
                ranks[indexed[i].Index] = avgRank;
            }

            i0 = i1;
        }

        return ranks;
    }

    /// <summary>
    /// Calculates the coefficient of determination (R²) for the correlation.
    /// </summary>
    /// <param name="x">The first dataset.</param>
    /// <param name="y">The second dataset.</param>
    /// <returns>
    /// The R² value in the range [0, 1], indicating the proportion of variance explained.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown when the two datasets have different lengths.
    /// </exception>
    /// <remarks>
    /// R² is the square of the Pearson correlation coefficient.
    /// It represents the proportion of the variance in the dependent variable 
    /// that is predictable from the independent variable.
    /// 
    /// Values closer to 1 indicate a better fit.
    /// </remarks>
    public static double RSquared(ReadOnlySpan<double> x, ReadOnlySpan<double> y)
    {
        double r = Pearson(x, y);
        return double.IsNaN(r) ? double.NaN : r * r;
    }

    /// <summary>
    /// Calculates the covariance between two datasets.
    /// </summary>
    /// <param name="x">The first dataset.</param>
    /// <param name="y">The second dataset.</param>
    /// <returns>
    /// The covariance value.
    /// Returns NaN if either dataset has fewer than 2 elements.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown when the two datasets have different lengths.
    /// </exception>
    /// <remarks>
    /// Covariance measures how two variables change together.
    /// Positive covariance indicates that the variables tend to move in the same direction,
    /// while negative covariance indicates they move in opposite directions.
    /// 
    /// Formula: Cov(X,Y) = Σ((xi - x̄)(yi - ȳ)) / (n - 1)
    /// 
    /// Note: This uses sample covariance (n-1 in denominator) rather than population covariance (n).
    /// 
    /// Time complexity: O(n)
    /// Space complexity: O(1)
    /// </remarks>
    public static double Covariance(ReadOnlySpan<double> x, ReadOnlySpan<double> y)
    {
        if (x.Length != y.Length)
        {
            throw new ArgumentException(
                $"Both datasets must have the same length. x.Length={x.Length}, y.Length={y.Length}",
                nameof(y));
        }

        int n = x.Length;
        if (n < 2)
        {
            return double.NaN;
        }

        double meanX = 0, meanY = 0;
        for (int i = 0; i < n; i++)
        {
            meanX += x[i];
            meanY += y[i];
        }
        meanX /= n;
        meanY /= n;

        double covariance = 0;
        for (int i = 0; i < n; i++)
        {
            covariance += (x[i] - meanX) * (y[i] - meanY);
        }

        return covariance / (n - 1);
    }

    /// <summary>
    /// Calculates the Beta coefficient of an asset relative to the market.
    /// </summary>
    /// <param name="assetReturns">The returns of the asset.</param>
    /// <param name="marketReturns">The returns of the market or benchmark.</param>
    /// <returns>
    /// The Beta coefficient.
    /// Returns NaN if calculation is not possible (e.g., market has no variance).
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown when the two datasets have different lengths.
    /// </exception>
    /// <remarks>
    /// Beta measures the systematic risk of an asset relative to the market.
    /// It indicates how sensitive the asset's returns are to market movements.
    /// 
    /// Formula: β = Cov(Asset, Market) / Var(Market)
    /// 
    /// Interpretation:
    /// - β = 1: Asset moves with the market
    /// - β > 1: Asset is more volatile than the market
    /// - β &lt; 1: Asset is less volatile than the market
    /// - β &lt; 0: Asset moves opposite to the market
    /// 
    /// Beta is a key component of the Capital Asset Pricing Model (CAPM).
    /// 
    /// Time complexity: O(n)
    /// Space complexity: O(1)
    /// </remarks>
    public static double Beta(ReadOnlySpan<double> assetReturns, ReadOnlySpan<double> marketReturns)
    {
        if (assetReturns.Length != marketReturns.Length)
        {
            throw new ArgumentException(
                $"Both datasets must have the same length. assetReturns.Length={assetReturns.Length}, marketReturns.Length={marketReturns.Length}",
                nameof(marketReturns));
        }

        int n = assetReturns.Length;
        if (n < 2)
        {
            return double.NaN;
        }

        double meanMarket = 0;
        for (int i = 0; i < n; i++)
        {
            meanMarket += marketReturns[i];
        }
        meanMarket /= n;

        double meanAsset = 0;
        for (int i = 0; i < n; i++)
        {
            meanAsset += assetReturns[i];
        }
        meanAsset /= n;

        double covariance = 0;
        double marketVariance = 0;
        for (int i = 0; i < n; i++)
        {
            double marketDiff = marketReturns[i] - meanMarket;
            covariance += (assetReturns[i] - meanAsset) * marketDiff;
            marketVariance += marketDiff * marketDiff;
        }

        if (marketVariance == 0)
        {
            return double.NaN;
        }

        return covariance / marketVariance;
    }

    /// <summary>
    /// Calculates the rolling (moving window) correlation between two datasets.
    /// </summary>
    /// <param name="x">The first dataset.</param>
    /// <param name="y">The second dataset.</param>
    /// <param name="window">The size of the rolling window.</param>
    /// <returns>
    /// An array of correlation coefficients, where each value represents
    /// the correlation over the corresponding window.
    /// The output length is (x.Length - window + 1).
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown when the two datasets have different lengths.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when window size is less than 2 or greater than data length.
    /// </exception>
    /// <remarks>
    /// Rolling correlation is useful for analyzing how the relationship between
    /// two variables changes over time. This is particularly important in financial
    /// markets where correlations can vary significantly during different market conditions.
    /// 
    /// Example: A 30-day rolling correlation can show how stock relationships
    /// change during volatile periods vs. stable periods.
    /// 
    /// Time complexity: O(n * window) where n is the number of windows
    /// Space complexity: O(n)
    /// </remarks>
    public static double[] RollingCorrelation(ReadOnlySpan<double> x, ReadOnlySpan<double> y, int window)
    {
        if (x.Length != y.Length)
        {
            throw new ArgumentException(
                $"Both datasets must have the same length. x.Length={x.Length}, y.Length={y.Length}",
                nameof(y));
        }

        if (window < 2)
        {
            throw new ArgumentOutOfRangeException(
                nameof(window),
                $"Window size must be at least 2. Provided: {window}");
        }

        if (window > x.Length)
        {
            throw new ArgumentOutOfRangeException(
                nameof(window),
                $"Window size ({window}) cannot be greater than data length ({x.Length})");
        }

        int outputLength = x.Length - window + 1;
        double[] results = new double[outputLength];

        for (int i = 0; i < outputLength; i++)
        {
            ReadOnlySpan<double> windowX = x.Slice(i, window);
            ReadOnlySpan<double> windowY = y.Slice(i, window);
            results[i] = Pearson(windowX, windowY);
        }

        return results;
    }
}
