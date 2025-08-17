using rinha_de_backend_2025.api.Entity;

namespace rinha_de_backend_2025.api.Configurations;

public static class HttpClientConfiguration
{
    public static void ConfigureHttpClient(this IServiceCollection service)
    {
        service.AddHttpClient(nameof(service_used.Default), client =>
        {
            var baseUrl = Environment.GetEnvironmentVariable("PAYMENT_PROCESSOR_DEFAULT");
            if (string.IsNullOrEmpty(baseUrl))
            {
                throw new InvalidOperationException("PAYMENT_PROCESSOR_DEFAULT environment variable is not set");
            }
            
            client.BaseAddress = new Uri(baseUrl);
            client.Timeout = TimeSpan.FromMilliseconds(800);
        });

        service.AddHttpClient(nameof(service_used.Fallback), client =>
        {
            var baseUrl = Environment.GetEnvironmentVariable("PAYMENT_PROCESSOR_FALLBACK");
            if (string.IsNullOrEmpty(baseUrl))
            {
                throw new InvalidOperationException("PAYMENT_PROCESSOR_FALLBACK environment variable is not set");
            }

            client.BaseAddress = new Uri(baseUrl);
            client.Timeout = TimeSpan.FromMilliseconds(5000);
        });
    }
}