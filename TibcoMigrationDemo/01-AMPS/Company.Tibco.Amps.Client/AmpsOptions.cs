namespace Company.Tibco.Amps.Client;

public sealed class AmpsOptions
{
    public string ClientName { get; set; } = "tibco-migration";
    public string ServerUrl { get; set; } = "tcp://localhost:9007/amps/json";
    public string Username { get; set; } = "testuser";
    public string Password { get; set; } = "password";
}
