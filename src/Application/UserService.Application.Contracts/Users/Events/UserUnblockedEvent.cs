using Itmo.Dev.Platform.Events;

namespace UserService.Application.Contracts.Users.Events;

public sealed record UserUnblockedEvent(
    long UserId,
    DateTimeOffset UnblockedAt) : IEvent;