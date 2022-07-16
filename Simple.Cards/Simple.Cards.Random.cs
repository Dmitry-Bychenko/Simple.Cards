using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Simple.Cards {

  #region Generators

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Standard Random Generation Interface 
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public interface IRandom {
    /// <summary>
    /// Next Random Uniform Distributed Number in [0..1[ range
    /// </summary>
    public double NextDouble();

    /// <summary>
    /// Next
    /// </summary>
    /// <param name="maxExcluded">Max (excluded) value</param>
    /// <returns>Uniform Random [0..maxExcluded) value</returns>
    public int Next(int maxExcluded) => (int)(NextDouble() * maxExcluded);

    /// <summary>
    /// Next 
    /// </summary>
    /// <param name="minIncluded">Min value (included)</param>
    /// <param name="maxExcluded">Max value (excluded)</param>
    /// <returns>Uniform Random [minIncluded..maxExcluded) value</returns>
    /// <exception cref="ArgumentOutOfRangeException">When minIncluded >= maxExcluded</exception>
    public int Next(int minIncluded, int maxExcluded) {
      
      if (minIncluded >= maxExcluded)
        throw new ArgumentOutOfRangeException(nameof(maxExcluded), $"{maxExcluded} should be greater than {minIncluded}");

      long diff = (long)maxExcluded - minIncluded;

      return (int)(int.MinValue + (NextDouble() * diff));
    }
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Predefined Random
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class PredefindedRandom : IRandom {
    #region Private Data

    private static readonly ThreadLocal<Random> s_Generator = new (() => new Random(0));

    private readonly Random m_Random = s_Generator.Value!;

    #endregion Private Data

    #region Public

    /// <summary>
    /// Standard Predefined Random Generator
    /// </summary>
    public static IRandom Default { get; } = new PredefindedRandom();

    /// <summary>
    /// Next Uniform Random [0..1) value
    /// </summary>
    public double NextDouble() => m_Random.NextDouble();

    /// <summary>
    /// Next Uniform Random [0..maxExcluded) value
    /// </summary>
    /// <param name="maxExcluded">Max Excluded</param>
    /// <returns>Integer Random Uniform [0..maxExcluded)</returns>
    public int Next(int maxExcluded) => m_Random.Next(maxExcluded);

    /// <summary>
    /// Next Uniform Random [minIncluded..maxExcluded) value
    /// </summary>
    /// <param name="minIncluded">Min Included</param>
    /// <param name="maxExcluded">Max Excluded</param>
    /// <returns>Integer Random Uniform [minIncluded..maxExcluded)</returns>
    public int Next(int minIncluded, int maxExcluded) => m_Random.Next(minIncluded, maxExcluded);

    #endregion Public
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Standard Random
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class StandardRandom : IRandom {
    #region Private Data

    private static readonly ThreadLocal<Random> s_Generator = new(() => {
      int seed;

      using (var generator = RandomNumberGenerator.Create()) {
        byte[] seedData = new byte[sizeof(int)];

        generator.GetBytes(seedData);

        seed = BitConverter.ToInt32(seedData, 0);
      }

      return new Random(seed);
    });

    private readonly Random m_Random = s_Generator.Value!;

    #endregion Private Data
        
    #region Public

    /// <summary>
    /// Standard Random Generator
    /// </summary>
    public static IRandom Default { get; } = new StandardRandom();

    /// <summary>
    /// Next Uniform Random [0..1) value
    /// </summary>
    public double NextDouble() => m_Random.NextDouble();

    /// <summary>
    /// Next Uniform Random [0..maxExcluded) value
    /// </summary>
    /// <param name="maxExcluded">Max Excluded</param>
    /// <returns>Integer Random Uniform [0..maxExcluded)</returns>
    public int Next(int maxExcluded) => m_Random.Next(maxExcluded);

    /// <summary>
    /// Next Uniform Random [minIncluded..maxExcluded) value
    /// </summary>
    /// <param name="minIncluded">Min Included</param>
    /// <param name="maxExcluded">Max Excluded</param>
    /// <returns>Integer Random Uniform [minIncluded..maxExcluded)</returns>
    public int Next(int minIncluded, int maxExcluded) => m_Random.Next(minIncluded, maxExcluded);

    #endregion Public
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Fixed Random
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class FixedRandom : IRandom {
    #region Private Data

    private readonly ThreadLocal<Random> m_Generator;

    #endregion Private Data

    #region Create

    /// <summary>
    /// Fixed Random with given Seed
    /// </summary>
    /// <param name="seed">Seed to use</param>
    public FixedRandom(int seed) {
      Seed = seed;

      m_Generator = new ThreadLocal<Random>(() => new Random(Seed));
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Seed
    /// </summary>
    public int Seed { get; }

    /// <summary>
    /// Next Uniform Random [0..1) value
    /// </summary>
    public double NextDouble() => m_Generator!.Value!.NextDouble();

    /// <summary>
    /// Next Uniform Random [0..maxExcluded) value
    /// </summary>
    /// <param name="maxExcluded">Max Excluded</param>
    /// <returns>Integer Random Uniform [0..maxExcluded)</returns>
    public int Next(int maxExcluded) => m_Generator!.Value!.Next(maxExcluded);

    /// <summary>
    /// Next Uniform Random [minIncluded..maxExcluded) value
    /// </summary>
    /// <param name="minIncluded">Min Included</param>
    /// <param name="maxExcluded">Max Excluded</param>
    /// <returns>Integer Random Uniform [minIncluded..maxExcluded)</returns>
    public int Next(int minIncluded, int maxExcluded) => m_Generator!.Value!.Next(minIncluded, maxExcluded);

    #endregion Public
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Cryptography Random 
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class CryptoRandom : IRandom {
    #region Public

    /// <summary>
    /// Standard Crypto Random Generator
    /// </summary>
    public static IRandom Default { get; } = new CryptoRandom();

    /// <summary>
    /// Next
    /// </summary>
    public double NextDouble() {
      using var generator = RandomNumberGenerator.Create();

      byte[] data = new byte[7];

      generator.GetBytes(data);

      return data.Aggregate(0.0, (s, a) => s / 256.0 + a / 256.0);
    }

    #endregion Public
  }

  #endregion Generators

  #region Extensions

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Extensions over IEnumerable<T>
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static partial class EnumerableExtensions {
    #region Public

    /// <summary>
    /// Shuffle 
    /// </summary>
    /// <param name="source">Enumerable to be shuffled</param>
    /// <param name="random">Random generator to be used</param>
    /// <returns>Shuffled enumerable</returns>
    /// <exception cref="ArgumentNullException">When source is null</exception>
    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source, IRandom? random = default) {
      if (source is null)
        throw new ArgumentNullException(nameof(source));

      random ??= StandardRandom.Default;

      T[] data = source.ToArray();

      for (int i = 0; i < data.Length; ++i) {
        int index = i + random.Next(data.Length - i + 1);

        (data[i], data[index]) = (data[index], data[i]);

        yield return data[i];
      }
    }

    /// <summary>
    /// Peek Random Item form source
    /// </summary>
    /// <param name="source">Source to peek random item from</param>
    /// <param name="random">Random generator to use</param>
    /// <returns>Random item from source</returns>
    /// <exception cref="ArgumentNullException">When source is null</exception>
    /// <exception cref="ArgumentOutOfRangeException">When source is empty</exception>
    public static T PeekRandom<T>(this IEnumerable<T> source, IRandom? random = default) {
      if (source is null)
        throw new ArgumentNullException(nameof(source));

      random ??= StandardRandom.Default;

      if (source is not IReadOnlyList<T> list)
        list = source.ToList();

      if (list.Count <= 0)
        throw new ArgumentOutOfRangeException(nameof(source), $"The {nameof(source)} is empty; no item can be peeked.");

      return list[random.Next(list.Count)];
    }

    #endregion Public
  }

  #endregion Extensions

}