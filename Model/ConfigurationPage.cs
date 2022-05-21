using System;

namespace server.Models;

public class ConfigAttribute : Attribute { }

public class ConfigurationPage : Entity<ConfigurationPage>
{
    public string? Token { get; set; }
    public string? UserId { get; set; }

    [ConfigAttribute]
    public string? gps_polling_period { get; set; }

    [ConfigAttribute]
    public string? generate_dummy_data { get; set; }

    [ConfigAttribute]
    public string? server_request_period { get; set; }

    public override ConfigurationPage self() => this;
}