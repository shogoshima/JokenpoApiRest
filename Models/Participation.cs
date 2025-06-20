namespace JokenpoApiRest.Models;

public class Participation
{
  public int RoundId { get; set; }
  public int UserId { get; set; }
  public int? HandId { get; set; }

  public Round? Round { get; set; }
  public User? User { get; set; }
  public Hand? Hand { get; set; }
}