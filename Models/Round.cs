namespace JokenpoApiRest.Models;

public enum RoundStatus
{
  Open,
  Closed
}

public class Round
{
  public int Id { get; set; }
  public RoundStatus Status { get; set; } = RoundStatus.Open;
  public DateTime CreatedAt { get; set; } = DateTime.Now;
}