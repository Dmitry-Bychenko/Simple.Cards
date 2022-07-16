using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.Cards {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Suit Color 
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public enum SuitColor {
    /// <summary>
    /// None (Joker)
    /// </summary>
    None = 0,
    /// <summary>
    /// Red
    /// </summary>
    Red = 1,
    /// <summary>
    /// Black
    /// </summary>
    Black = 2
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Suit
  /// </summary>
  // https://en.wikipedia.org/wiki/Playing_cards_in_Unicode
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class Suit : IEquatable<Suit>, IComparable<Suit> {
    #region Private Data

    private static readonly List<Suit> s_Items = new(); 

    private static readonly Dictionary<string, Suit> s_Names = new (StringComparer.OrdinalIgnoreCase);

    private static readonly Dictionary<int, Suit> s_Codes = new ();

    private readonly char m_DarkSymbol;

    private readonly char m_LightSymbol;

    #endregion Private Data

    #region Algorithm
    #endregion Algorithm

    #region Create

    static Suit() {
      None = new Suit(0, "-", '-', '-', SuitColor.None, 0x1F0CF);
      Clubs = new Suit(1, "Clubs", '\u2663', '\u2667', SuitColor.Black, 0x1F0A0);
      Diamonds = new Suit(2, "Diamonds", '\u2666', '\u2662', SuitColor.Red, 0x1F0C0);
      Hearts = new Suit(3, "Hearts", '\u2665', '\u2661', SuitColor.Red, 0x1F0B0);
      Spades = new Suit(4, "Spades", '\u2660', '\u2664', SuitColor.Black, 0x1F0A0);
    }

    private Suit(int code, string title, char darkSymbol, char lightSymbol, SuitColor color, int emojiPrefix) {
      Code = code;
      Title = title;

      m_DarkSymbol = darkSymbol;
      m_LightSymbol = lightSymbol;

      Color = color;
      EmojiPrefix = emojiPrefix;

      s_Names.Add(title, this);
            
      s_Codes.Add(code, this);

      s_Codes.TryAdd(m_DarkSymbol, this);
      s_Codes.TryAdd(m_LightSymbol, this);
      s_Codes.TryAdd(char.ToLower(Acronym), this);
      s_Codes.TryAdd(char.ToUpper(Acronym), this);
      
      s_Items.Add(this);
    }

    /// <summary>
    /// From Integer
    /// </summary>
    public static Suit? FromInt(int value) => s_Codes.TryGetValue(value, out var result) ? result : null;

    /// <summary>
    /// From String
    /// </summary>
    public static Suit? FromString(string value) {
      if (string.IsNullOrWhiteSpace(value))
        return None;

      value = value.Trim();

      if (value.Length == 1)
        return FromInt(value[0]);

      return s_Names.TryGetValue(value, out var result) ? result : null;
    }

    /// <summary>
    /// None (e.g. for Joker)
    /// </summary>
    public static Suit None { get; }

    /// <summary>
    /// Clubs
    /// </summary>
    public static Suit Clubs { get; }

    /// <summary>
    /// Diamonds
    /// </summary>
    public static Suit Diamonds { get; }

    /// <summary>
    /// Clubs
    /// </summary>
    public static Suit Hearts { get; }

    /// <summary>
    /// Spades
    /// </summary>
    public static Suit Spades { get; }

    #endregion Create

    #region Public

    /// <summary>
    /// Compare
    /// </summary>
    public static int Compare(Suit? left, Suit? right) {
      if (ReferenceEquals(left, right))
        return 0;
      if (left is null)
        return -1;
      if (right is null)
        return 1;

      return left.Code.CompareTo(right.Code);
    }

    /// <summary>
    /// Items
    /// </summary>
    public static IReadOnlyList<Suit> Items => s_Items; 

    /// <summary>
    /// Acronym
    /// </summary>
    public char Acronym => Title.Length > 0 ? Title[0] : '-';

    /// <summary>
    /// Is Joker
    /// </summary>
    public bool IsJoker => Code <= 0;

    /// <summary>
    /// Symbol (Dark)
    /// </summary>
    public char SymbolDark => m_DarkSymbol;

    /// <summary>
    /// Symbol (Light)
    /// </summary>
    public char SymbolLight => m_LightSymbol;

    /// <summary>
    /// Title
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// Code
    /// </summary>
    public int Code { get; }

    /// <summary>
    /// Emoji Prefix 
    /// </summary>
    public int EmojiPrefix { get; }

    /// <summary>
    /// Color
    /// </summary>
    public SuitColor Color { get; }

    /// <summary>
    /// To String (Title)
    /// </summary>
    public override string ToString() => Title;

    #endregion Public

    #region Operators

    /// <summary>
    /// From Character
    /// </summary>
    public static implicit operator Suit?(char code) => FromInt(code);

    /// <summary>
    /// From Integer
    /// </summary>
    public static implicit operator Suit? (int code) => FromInt(code);

    /// <summary>
    /// From String
    /// </summary>
    public static implicit operator Suit?(string name) => FromString(name);


    #endregion Operators

    #region IEquatable<Suit>

    /// <summary>
    /// Equals
    /// </summary>
    public bool Equals(Suit? other) {
      if (ReferenceEquals(this, other))
        return true;
      if (other is null)
        return false;

      return Code == other.Code;
    }

    /// <summary>
    /// Equals
    /// </summary>
    public override bool Equals(object? obj) => obj is Suit suit && Equals(suit);

    /// <summary>
    /// Code
    /// </summary>
    public override int GetHashCode() => Code;

    #endregion IEquatable<Suit>

    #region IComparable<Suit>

    /// <summary>
    /// Compare To
    /// </summary>
    public int CompareTo(Suit? other) => Compare(this, other);

    #endregion IComparable<Suit>
  }

}
