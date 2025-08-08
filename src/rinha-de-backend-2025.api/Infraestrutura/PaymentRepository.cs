using rinha_de_backend_2025.api.Entity;

namespace rinha_de_backend_2025.api.Infraestrutura
{
    public class PaymentRepository : IPaymentRepository
    {

        public Task<List<Payments>> Get()
        {
            /*
             select count(1) as request,
                   sum(amount) as amount,
                   service_used
            from payments group by service_used;

             */

            throw new NotImplementedException();
        }

        public Task Save(Payments entity)
        {
            /*
             * savar com o serviceType
             */
            
            throw new NotImplementedException();
        }
    }
}
