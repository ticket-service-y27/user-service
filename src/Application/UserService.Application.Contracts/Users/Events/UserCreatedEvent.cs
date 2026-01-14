using Itmo.Dev.Platform.Events;

namespace UserService.Application.Contracts.Users.Events;

public record UserCreatedEvent(
    long UserId,
    DateTimeOffset CreatedAt) : IEvent;