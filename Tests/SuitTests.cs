using Simple.Cards;

namespace Tests {

  [TestClass]
  public class SuitTests {
  
    [TestMethod]
    public void SuitCounts() {
      int count = Suit.Items.Count;

      Assert.IsTrue(count == 5);
    }

  }

}