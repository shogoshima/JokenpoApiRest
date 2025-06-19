namespace JokenpoApiRest.Models;

public class HandRelation
{
  public int WinnerHandId { get; set; }
  public int LoserHandId { get; set; }

  public required Hand WinnerHand { get; set; }
  public required Hand LoserHand { get; set; }
}