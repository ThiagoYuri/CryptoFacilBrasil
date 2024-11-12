using CryptoFacilBrasil.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoFacilBrasil.Services.Interfaces.Services
{
    public interface IOrderDetailService
    {
        Task CreateOrderDetail(OrderDetail orderDetail);

        Task<OrderDetail> GetOrderDetail(Guid id);

        Task UpdateOrderDetail( OrderDetail orderDetail);
    }
}
