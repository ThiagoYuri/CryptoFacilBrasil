using CryptoFacilBrasil.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoFacilBrasil.Domain.IRepository
{
    public interface IOrderDetailRepository
    {
        Task<OrderDetail> GetOrderDetailByIdAsync(Guid id);          
        Task<IEnumerable<OrderDetail>> GetOrderDetailsByChatIdAsync(long chatId); 
        Task AddOrderDetailAsync(OrderDetail orderDetail);             
        Task UpdateOrderDetailAsync(OrderDetail orderDetail);         
        Task DeleteOrderDetailAsync(Guid id);                          
    }

}
