namespace Hubly.Domain.Entities;

public record AuthenticatedUser(User User, string Token);