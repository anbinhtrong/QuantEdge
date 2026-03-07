namespace QuantEdge.Statistics;

/// <summary>
/// Provides extension methods for calculating correlation coefficients on various collection types.
/// </summary>
public static class CorrelationExtensions
{
    #region Pearson Correlation Extensions

    /// <summary>
    /// Calculates the Pearson correlation coefficient between this array and another array.
    /// </summary>
    /// <param name="x">The first dataset.</param>
    /// <param name="y">The second dataset.</param>
    /// <returns>The Pearson correlation coefficient in the range [-1, 1].</returns>
    /// <exception cref="ArgumentNullException">Thrown when either array is null.</exception>
    /// <exception cref="ArgumentException">Thrown when arrays have different lengths.</exception>
    /// <example>
    /// <code>
    /// double[] stockA = [0.02, -0.01, 0.03, 0.01, -0.02];
    /// double[] stockB = [0.01, -0.02, 0.04, 0.00, -0.01];
    /// double correlation = stockA.GetCorrelation(stockB);
    /// </code>
    /// </example>
    public static double GetCorrelation(this double[] x, double[] y)
    {
        ArgumentNullException.ThrowIfNull(x);
        ArgumentNullException.ThrowIfNull(y);
        
        return Correlation.Pearson(x, y);
    }

    /// <summary>
    /// Calculates the Pearson correlation coefficient between this list and another list.
    /// </summary>
    /// <param name="x">The first dataset.</param>
    /// <param name="y">The second dataset.</param>
    /// <returns>The Pearson correlation coefficient in the range [-1, 1].</returns>
    /// <exception cref="ArgumentNullException">Thrown when either list is null.</exception>
    /// <exception cref="ArgumentException">Thrown when lists have different lengths.</exception>
    /// <example>
    /// <code>
    /// var prices1 = new List&lt;double&gt; { 100, 102, 101, 103, 105 };
    /// var prices2 = new List&lt;double&gt; { 50, 51, 50.5, 52, 53 };
    /// double correlation = prices1.GetCorrelation(prices2);
    /// </code>
    /// </example>
    public static double GetCorrelation(this List<double> x, List<double> y)
    {
        ArgumentNullException.ThrowIfNull(x);
        ArgumentNullException.ThrowIfNull(y);
        
        return Correlation.Pearson(
            System.Runtime.InteropServices.CollectionsMarshal.AsSpan(x),
            System.Runtime.InteropServices.CollectionsMarshal.AsSpan(y));
    }

    /// <summary>
    /// Calculates the Pearson correlation coefficient between this enumerable and another enumerable.
    /// </summary>
    /// <param name="x">The first dataset.</param>
    /// <param name="y">The second dataset.</param>
    /// <returns>The Pearson correlation coefficient in the range [-1, 1].</returns>
    /// <exception cref="ArgumentNullException">Thrown when either enumerable is null.</exception>
    /// <exception cref="ArgumentException">Thrown when enumerables have different lengths.</exception>
    /// <remarks>
    /// This method materializes the enumerables into arrays. For better performance with
    /// large datasets that are already in array or list form, use the specific overloads.
    /// </remarks>
    public static double GetCorrelation(this IEnumerable<double> x, IEnumerable<double> y)
    {
        ArgumentNullException.ThrowIfNull(x);
        ArgumentNullException.ThrowIfNull(y);
        
        double[] xArray = x.ToArray();
        double[] yArray = y.ToArray();
        
        return Correlation.Pearson(xArray, yArray);
    }

    /// <summary>
    /// Calculates the Pearson correlation coefficient between this span and another span.
    /// </summary>
    /// <param name="x">The first dataset.</param>
    /// <param name="y">The second dataset.</param>
    /// <returns>The Pearson correlation coefficient in the range [-1, 1].</returns>
    /// <exception cref="ArgumentException">Thrown when spans have different lengths.</exception>
    /// <remarks>
    /// This is the most efficient overload as it works directly with spans without allocation.
    /// </remarks>
    public static double GetCorrelation(this ReadOnlySpan<double> x, ReadOnlySpan<double> y)
    {
        return Correlation.Pearson(x, y);
    }

    #endregion

    #region Spearman Correlation Extensions

    /// <summary>
    /// Calculates the Spearman rank correlation coefficient between this array and another array.
    /// </summary>
    /// <param name="x">The first dataset.</param>
    /// <param name="y">The second dataset.</param>
    /// <returns>The Spearman correlation coefficient in the range [-1, 1].</returns>
    /// <exception cref="ArgumentNullException">Thrown when either array is null.</exception>
    /// <exception cref="ArgumentException">Thrown when arrays have different lengths.</exception>
    /// <remarks>
    /// Spearman correlation is useful for non-linear monotonic relationships and is
    /// more robust to outliers than Pearson correlation.
    /// </remarks>
    public static double GetSpearmanCorrelation(this double[] x, double[] y)
    {
        ArgumentNullException.ThrowIfNull(x);
        ArgumentNullException.ThrowIfNull(y);
        
        return Correlation.Spearman(x, y);
    }

    /// <summary>
    /// Calculates the Spearman rank correlation coefficient between this list and another list.
    /// </summary>
    /// <param name="x">The first dataset.</param>
    /// <param name="y">The second dataset.</param>
    /// <returns>The Spearman correlation coefficient in the range [-1, 1].</returns>
    /// <exception cref="ArgumentNullException">Thrown when either list is null.</exception>
    /// <exception cref="ArgumentException">Thrown when lists have different lengths.</exception>
    public static double GetSpearmanCorrelation(this List<double> x, List<double> y)
    {
        ArgumentNullException.ThrowIfNull(x);
        ArgumentNullException.ThrowIfNull(y);
        
        return Correlation.Spearman(
            System.Runtime.InteropServices.CollectionsMarshal.AsSpan(x),
            System.Runtime.InteropServices.CollectionsMarshal.AsSpan(y));
    }

    /// <summary>
    /// Calculates the Spearman rank correlation coefficient between this enumerable and another enumerable.
    /// </summary>
    /// <param name="x">The first dataset.</param>
    /// <param name="y">The second dataset.</param>
    /// <returns>The Spearman correlation coefficient in the range [-1, 1].</returns>
    /// <exception cref="ArgumentNullException">Thrown when either enumerable is null.</exception>
    /// <exception cref="ArgumentException">Thrown when enumerables have different lengths.</exception>
    public static double GetSpearmanCorrelation(this IEnumerable<double> x, IEnumerable<double> y)
    {
        ArgumentNullException.ThrowIfNull(x);
        ArgumentNullException.ThrowIfNull(y);
        
        double[] xArray = x.ToArray();
        double[] yArray = y.ToArray();
        
        return Correlation.Spearman(xArray, yArray);
    }

    /// <summary>
    /// Calculates the Spearman rank correlation coefficient between this span and another span.
    /// </summary>
    /// <param name="x">The first dataset.</param>
    /// <param name="y">The second dataset.</param>
    /// <returns>The Spearman correlation coefficient in the range [-1, 1].</returns>
    /// <exception cref="ArgumentException">Thrown when spans have different lengths.</exception>
    public static double GetSpearmanCorrelation(this ReadOnlySpan<double> x, ReadOnlySpan<double> y)
    {
        return Correlation.Spearman(x, y);
    }

    #endregion

    #region R-Squared Extensions

    /// <summary>
    /// Calculates the coefficient of determination (R²) between this array and another array.
    /// </summary>
    /// <param name="x">The first dataset.</param>
    /// <param name="y">The second dataset.</param>
    /// <returns>The R² value in the range [0, 1].</returns>
    /// <exception cref="ArgumentNullException">Thrown when either array is null.</exception>
    /// <exception cref="ArgumentException">Thrown when arrays have different lengths.</exception>
    /// <remarks>
    /// R² represents the proportion of variance in the dependent variable that is
    /// predictable from the independent variable.
    /// </remarks>
    public static double GetRSquared(this double[] x, double[] y)
    {
        ArgumentNullException.ThrowIfNull(x);
        ArgumentNullException.ThrowIfNull(y);
        
        return Correlation.RSquared(x, y);
    }

    /// <summary>
    /// Calculates the coefficient of determination (R²) between this list and another list.
    /// </summary>
    /// <param name="x">The first dataset.</param>
    /// <param name="y">The second dataset.</param>
    /// <returns>The R² value in the range [0, 1].</returns>
    /// <exception cref="ArgumentNullException">Thrown when either list is null.</exception>
    /// <exception cref="ArgumentException">Thrown when lists have different lengths.</exception>
    public static double GetRSquared(this List<double> x, List<double> y)
    {
        ArgumentNullException.ThrowIfNull(x);
        ArgumentNullException.ThrowIfNull(y);
        
        return Correlation.RSquared(
            System.Runtime.InteropServices.CollectionsMarshal.AsSpan(x),
            System.Runtime.InteropServices.CollectionsMarshal.AsSpan(y));
    }

    /// <summary>
    /// Calculates the coefficient of determination (R²) between this enumerable and another enumerable.
    /// </summary>
    /// <param name="x">The first dataset.</param>
    /// <param name="y">The second dataset.</param>
    /// <returns>The R² value in the range [0, 1].</returns>
    /// <exception cref="ArgumentNullException">Thrown when either enumerable is null.</exception>
    /// <exception cref="ArgumentException">Thrown when enumerables have different lengths.</exception>
    public static double GetRSquared(this IEnumerable<double> x, IEnumerable<double> y)
    {
        ArgumentNullException.ThrowIfNull(x);
        ArgumentNullException.ThrowIfNull(y);
        
        double[] xArray = x.ToArray();
        double[] yArray = y.ToArray();
        
        return Correlation.RSquared(xArray, yArray);
    }

    /// <summary>
    /// Calculates the coefficient of determination (R²) between this span and another span.
    /// </summary>
    /// <param name="x">The first dataset.</param>
    /// <param name="y">The second dataset.</param>
    /// <returns>The R² value in the range [0, 1].</returns>
    /// <exception cref="ArgumentException">Thrown when spans have different lengths.</exception>
    public static double GetRSquared(this ReadOnlySpan<double> x, ReadOnlySpan<double> y)
    {
        return Correlation.RSquared(x, y);
    }

    #endregion

    #region Correlation Matrix Extensions

    /// <summary>
    /// Calculates the Pearson correlation matrix for this collection of feature arrays.
    /// </summary>
    /// <param name="features">A collection of feature arrays.</param>
    /// <returns>A symmetric correlation matrix.</returns>
    /// <exception cref="ArgumentNullException">Thrown when features is null.</exception>
    /// <exception cref="ArgumentException">Thrown when features is empty or contains arrays of different lengths.</exception>
    /// <example>
    /// <code>
    /// double[][] features = 
    /// [
    ///     [1, 2, 3, 4, 5],      // Stock A returns
    ///     [2, 4, 6, 8, 10],     // Stock B returns
    ///     [5, 4, 3, 2, 1]       // Stock C returns
    /// ];
    /// double[,] correlationMatrix = features.GetCorrelationMatrix();
    /// </code>
    /// </example>
    public static double[,] GetCorrelationMatrix(this double[][] features)
    {
        ArgumentNullException.ThrowIfNull(features);
        
        return Correlation.PearsonMatrix(features);
    }

    /// <summary>
    /// Calculates the Pearson correlation matrix for this collection of feature lists.
    /// </summary>
    /// <param name="features">A collection of feature lists.</param>
    /// <returns>A symmetric correlation matrix.</returns>
    /// <exception cref="ArgumentNullException">Thrown when features is null.</exception>
    /// <exception cref="ArgumentException">Thrown when features is empty or contains lists of different lengths.</exception>
    public static double[,] GetCorrelationMatrix(this IEnumerable<List<double>> features)
    {
        ArgumentNullException.ThrowIfNull(features);
        
        double[][] featureArrays = features.Select(f => f.ToArray()).ToArray();
        return Correlation.PearsonMatrix(featureArrays);
    }

    /// <summary>
    /// Calculates the Pearson correlation matrix for this collection of feature enumerables.
    /// </summary>
    /// <param name="features">A collection of feature enumerables.</param>
    /// <returns>A symmetric correlation matrix.</returns>
    /// <exception cref="ArgumentNullException">Thrown when features is null.</exception>
    /// <exception cref="ArgumentException">Thrown when features is empty or contains enumerables of different lengths.</exception>
    public static double[,] GetCorrelationMatrix(this IEnumerable<IEnumerable<double>> features)
    {
        ArgumentNullException.ThrowIfNull(features);
        
        double[][] featureArrays = features.Select(f => f.ToArray()).ToArray();
        return Correlation.PearsonMatrix(featureArrays);
    }

    #endregion
}
