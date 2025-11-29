namespace EventRelayWorker;

public class Tenants
{
    public List<TenantConfig> TenantConfigs { get; set; } = [];
}

public class TenantConfig
{
    public string Name { get; set; }
    public string ConnectionString { get; set; }
    public string OutboxTable { get; set; }
}