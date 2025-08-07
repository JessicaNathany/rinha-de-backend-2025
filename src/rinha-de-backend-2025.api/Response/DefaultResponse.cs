using System.Text.Json.Serialization;

namespace rinha_de_backend_2025.api.Response
{
    public class DefaultResponse
    {
        public int TotalRequests { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
