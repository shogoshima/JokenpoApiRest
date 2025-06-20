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
