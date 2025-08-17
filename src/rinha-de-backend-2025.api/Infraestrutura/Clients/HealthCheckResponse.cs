namespace rinha_de_backend_2025.api.Infraestrutura.Clients;

public record HealthCheckResponse(bool Failing, decimal MinResponseTime);
