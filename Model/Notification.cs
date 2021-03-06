using System;

namespace server.Models;

public class Notification : Entity<Notification>
{
    public string? Token { get; set; }
    public string? UserId { get; set; }
    public string? Message { get; set; }
    public string? Type { get; set; }
    public DateTime? Moment { get; set; }
    public override Notification self() => this;
}