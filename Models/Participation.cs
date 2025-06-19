namespace JokenpoApiRest.Models;

public class Participation
{
  public int RoundId { get; set; }
  public int UserId { get; set; }
  public int? HandId { get; set; }

  public required Round Round { get; set; }
  public required User User { get; set; }
  public Hand? Hand { get; set; }
}