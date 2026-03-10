# Pearson Correlation Implementation

This document provides technical details on the Pearson Correlation implementation in the **QuantEdge** statistics library. It is designed for .NET engineers and quant developers using the library for statistical analysis.

## 1. Mathematical Definition

The Pearson correlation coefficient (\(r\)) is a measure of the linear correlation between two sets of data. It is the ratio between the covariance of two variables and the product of their standard deviations; thus, it is essentially a normalized measurement of the covariance, such that the result always has a value between -1 and 1.

The standard population formula is defined as:

$$r = \frac{\sum_{i=1}^{n} (x_i - \bar{x})(y_i - \bar{y})}{\sqrt{\sum_{i=1}^{n} (x_i - \bar{x})^2 \sum_{i=1}^{n} (y_i - \bar{y})^2}}$$

Where:
* \(n\) is the sample size.
* \(x_i, y_i\) are the individual sample points indexed with \(i\).
* \(\bar{x}\) is the sample mean of \(x\); and \(\bar{y}\) is the sample mean of \(y\).

## 2. Derivation of the Fast Computational Formula

The standard two-pass algorithm requires calculating the mean first, and then iterating again to compute the variances and covariance. To compute it in a single pass (\(\mathcal{O}(N)\) time), QuantEdge defaults to a **Fast** formula (`PearsonFast`).

**Derivation:**
Starting from the standard numerator component of the covariance:
$$ \sum (x_i - \bar{x})(y_i - \bar{y}) = \sum (x_i y_i - x_i \bar{y} - \bar{x} y_i + \bar{x} \bar{y}) $$
$$ = \sum x_i y_i - \bar{y} \sum x_i - \bar{x} \sum y_i + n \bar{x} \bar{y} $$

Substitute \(\bar{x} = \frac{\sum x}{n}\) and \(\bar{y} = \frac{\sum y}{n}\):
$$ = \sum x_i y_i - \frac{\sum x \sum y}{n} $$

Similarly, the denominator's variance components expand to:
$$ \sum (x_i - \bar{x})^2 = \sum x_i^2 - \frac{(\sum x)^2}{n} $$

To remove the fractional division inside the summation, we multiply the numerator and the denominator inside the square root by \(n\):

**Numerator:** \(n \sum x_i y_i - \sum x \sum y\)

**Denominator inside the root:** 
$$n \left( \sum x_i^2 - \frac{(\sum x)^2}{n} \right) \cdot n \left( \sum y_i^2 - \frac{(\sum y)^2}{n} \right) $$
which simplifies to:
$$ (n \sum x_i^2 - (\sum x)^2)(n \sum y_i^2 - (\sum y)^2) $$

Resulting in the fast direct-sum formula:
$$ r = \frac{n \sum xy - \sum x \sum y}{\sqrt{(n \sum x^2 - (\sum x)^2)(n \sum y^2 - (\sum y)^2)}} $$

## 3. Explanation of Variables Used in the Implementation

In the `PearsonFast` implementation within `QuantEdge.Statistics.Correlation`, the fast formula is calculated using variables corresponding to the terms derived above:

* `n`: Length of the data arrays (`x.Length`).
* `sumX` (\(\sum x\)): The accumulated sum of dataset `x`.
* `sumY` (\(\sum y\)): The accumulated sum of dataset `y`.
* `sumXY` (\(\sum xy\)): The accumulated product of `x_i` and `y_i`.
* `sumX2` (\(\sum x^2\)): The accumulated square of properties in `x`.
* `sumY2` (\(\sum y^2\)): The accumulated square of properties in `y`.
* `numerator`: Computed as `(n * sumXY) - (sumX * sumY)`.
* `denominator`: Computed as `Math.Sqrt(((n * sumX2) - (sumX * sumX)) * ((n * sumY2) - (sumY * sumY)))`.

## 4. Numerical Stability Considerations

While the "Fast" algorithm is highly optimized for performance and requires only one allocation-free pass through the data, it is susceptible to **catastrophic cancellation**.

If datasets exhibit very high magnitudes relative to their variance (for example, raw stock prices hovering around $3000 but only varying by $0.01), computing \(\sum x^2\) can overflow double-precision floating-point limits or swallow the variance entirely at the tail end of the significand. In scenarios with heavy floating-point truncation risk, you should use relative metrics like financial Returns rather than absolute Prices.

To alleviate this, QuantEdge provides an alternate algorithmic path via the `CorrelationAlgorithm.Welford` option.

## 5. Comparison with the Welford Algorithm

QuantEdge implements both `CorrelationAlgorithm.Fast` (the default) and `CorrelationAlgorithm.Welford`. 

**Welford's Online Algorithm (`PearsonWelford`):**
* Avoids large sum accumulations by dynamically updating the mean (`meanX`, `meanY`) and the squared deviations (`m2X`, `m2Y`, `cXY`) as each new element arrives.
* **Accuracy:** Extremely stable numerically; prevents loss of precision when variances are small relative to their dataset means.
* **Performance:** Still \(\mathcal{O}(N)\), but involves more arithmetic operations and divisions per iteration loop than the fast sum method.
* **Use Case:** Recommended strictly for very large sequence bounds or unscaled financial data where stability dominates the requirements.

## 6. Time Complexity and Memory Usage

Both implementations in QuantEdge guarantee the following bounds:
* **Time Complexity:** \(\mathcal{O}(N)\) where \(N\) is the length of the datasets. Both algorithms iterate straight through the sets precisely once.
* **Space Complexity:** \(\mathcal{O}(1)\). By utilizing `ReadOnlySpan<double>` natively throughout the API, QuantEdge avoids array copying and prevents heap allocations (GC friendly). Extensions exist for arrays and `List<T>`, utilizing `CollectionsMarshal.AsSpan` to ensure no memory is allocated during execution.

## 7. Example Using Financial Data

Below is a practical C# example evaluating correlation between two tech equities using QuantEdge Extension methods. When calculating correlations of financial data, doing so on **returns** (rather than raw prices) resolves many numerical and non-stationarity issues.

```csharp
using QuantEdge.Statistics;

public class CorrelationDemo
{
    public static void CalculateTechCorrelation()
    {
        // Sample daily returns for Asset A and Asset B
        double[] stockAReturns = [0.012, -0.005, 0.021, 0.008, -0.011];
        double[] stockBReturns = [0.009, -0.002, 0.018, 0.005, -0.008];

        // Utilizing QuantEdge extension methods for succinct API layout.
        // Defaults to CorrelationAlgorithm.Fast underneath using zero-alloc spans.
        double correlation = stockAReturns.GetCorrelation(stockBReturns);

        Console.WriteLine($"Asset Correlation: {correlation:F4}"); 
        // Expected Output: Highly correlated since the asset movements mirror each other.
    }
}
```
