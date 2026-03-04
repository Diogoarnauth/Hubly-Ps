namespace Hubly.api.Domain.Entities;

public record AuthenticatedUser(User User, string Token);