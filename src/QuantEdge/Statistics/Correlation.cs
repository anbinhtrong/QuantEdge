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
}
