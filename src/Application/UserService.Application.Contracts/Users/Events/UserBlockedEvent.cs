using Itmo.Dev.Platform.Events;

namespace UserService.Application.Contracts.Users.Events;

public sealed record UserBlockedEvent(
    long UserId,
    DateTimeOffset BlockedAt) : IEvent;