namespace JokenpoApiRest.Models;

public class HandRelation
{
  public int WinnerHandId { get; set; }
  public int LoserHandId { get; set; }

  public Hand? WinnerHand { get; set; }
  public Hand? LoserHand { get; set; }
}