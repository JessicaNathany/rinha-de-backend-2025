namespace rinha_de_backend_2025.api.Entity;

public record HealthCheckStatus
{
    public string ServiceName { get; set; } = string.Empty;
    public bool IsHealthy { get; set; }
    public decimal MinResponseTime { get; set; }
    public DateTime LastChecked { get; set; }
    public string CheckedBy { get; set; } = string.Empty;
}