using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.Cards {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Card
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class Card : IEquatable<Card> {
    #region Private Data

    private static readonly Dictionary<int, string> s_Titles = new() {
      {  0, "Joker"},
      {  1, "Ace"  },
      { 11, "Jack" },
      { 12, "Queen"},
      { 13, "King" },
    };

    #endregion Private Data

    #region Algorithm
    #endregion Algorithm

    #region Create

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="suit">Suit</param>
    /// <param name="value">Value</param>
    /// <exception cref="ArgumentOutOfRangeException">When value is out of [1..14] range</exception>
    public Card(Suit suit, int value) {
      Suit = suit ?? Suit.None;

      if (Suit.IsJoker)
        Value = 0;
      else {
        if (value < 1 || value > 14)
          throw new ArgumentOutOfRangeException(nameof(value));

        Value = value;
      }
    }

    /// <summary>
    /// Try Parse
    /// </summary>
    /// <param name="text">Text to Parse</param>
    /// <param name="card">Parsed card or null</param>
    /// <returns></returns>
    public static bool TryParse(string text, out Card? card) {
      card = null;

      if (string.IsNullOrWhiteSpace(text))
        return false;

      text = text.Trim();

      if (string.Equals(text, "joker", StringComparison.OrdinalIgnoreCase)) {
        card = new Card(Suit.None, 0);

        return true;
      }

      if (text.Length < 2)
        return false;
      
      Suit? suit = text[0];

      string mark = text[1..].Trim();

      if (suit is null) {
        suit = text[0];

        mark = text[0..(text.Length - 1)].Trim();
      }

      if (suit is null)
        return false;

      if (!int.TryParse(mark, out int code)) {
        if ("a".Equals(mark, StringComparison.OrdinalIgnoreCase))
          code = 1;
        else if ("j".Equals(mark, StringComparison.OrdinalIgnoreCase))
          code = 11;
        else if ("q".Equals(mark, StringComparison.OrdinalIgnoreCase))
          code = 12;
        else if ("k".Equals(mark, StringComparison.OrdinalIgnoreCase))
          code = 13;
        else
          code = -1;
      }

      if (code < 0)
        return false;

      card = new Card(suit, code);

      return true;
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Suit
    /// </summary>
    public Suit Suit { get; }

    /// <summary>
    /// Value
    /// </summary>
    public int Value { get; }

    /// <summary>
    /// Is Joker
    /// </summary>
    public bool IsJoker => Suit.IsJoker;

    /// <summary>
    /// Title
    /// </summary>
    public string Title {
      get {
        if (Suit.IsJoker)
          return s_Titles[0];

        if (s_Titles.TryGetValue(Value, out string? result))
          return result ?? "?";

        return Value.ToString(CultureInfo.InvariantCulture);
      }
    }

    /// <summary>
    /// Symbol
    /// </summary>
    public string Symbol {
      get {
        if (IsJoker)
          return Title;

        if (s_Titles.TryGetValue(Value, out string? result) && result is not null)
          return result[0..1];

        return Value.ToString(CultureInfo.InvariantCulture);
      }
    }

    /// <summary>
    /// Full Name
    /// </summary>
    public string FullName {
      get {
        if (Suit.IsJoker)
          return Title;

        return $"{Title} of {Suit}";
      }
    }
   
    /// <summary>
    /// Emoji
    /// </summary>
    public string Emoji {
      get {
        int code = Suit.EmojiPrefix + Value;

        return new string(new char[] {
          (char) (((code - 0x10000) >> 10) + 0xD800),
          (char) (((code - 0x10000) & 0b1111_111_111) + 0xDC00)
        });
      }
    }

    #endregion Public

    #region IEquatable<Card>

    /// <summary>
    /// Equals
    /// </summary>
    public bool Equals(Card? other) {
      if (ReferenceEquals(this, other))
        return true;
      if (other is null)
        return false;

      return Suit == other.Suit && Value == other.Value;
    }

    /// <summary>
    /// Equals
    /// </summary>
    public override bool Equals(object? obj) => obj is Card other && Equals(other);

    /// <summary>
    /// Hash code
    /// </summary>
    public override int GetHashCode() => unchecked(Suit.GetHashCode() ^ (Value << 6));

    #endregion IEquatable<Card>
  }

}
