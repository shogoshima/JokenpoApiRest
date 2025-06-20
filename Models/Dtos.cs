namespace JokenpoApiRest.Models;

public class UserDto
{
  public required string Name { get; set; }
}

public class ParticipationDto
{
  public required int UserId { get; set; }
  public int? HandId { get; set; }
}

public class RoundDto
{
  public required Round Data { get; set; }
  public required IEnumerable<string> PlayedUsers { get; set; }
  public required IEnumerable<string> PendingUsers { get; set; }
}

public class RoundResultsDto
{
  public required List<string> Winners { get; set; }
}