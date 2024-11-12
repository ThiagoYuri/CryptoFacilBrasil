using CryptoFacilBrasil.Domain.IRepository;
using CryptoFacilBrasil.Domain.Models;
using CryptoFacilBrasil.Services.Interfaces.Services;

namespace CryptoFacilBrasil.Services
{
    public class OrderDetailService : IOrderDetailService
    {
        private IOrderDetailRepository _orderDetailRepository;

        public OrderDetailService(IOrderDetailRepository orderDetailRepository)
        {
            _orderDetailRepository = orderDetailRepository;
        }

        public Task CreateOrderDetail(OrderDetail orderDetail)
        {
           return _orderDetailRepository.AddOrderDetailAsync(orderDetail);
        }

        public Task<OrderDetail> GetOrderDetail(Guid id)
        {
            return _orderDetailRepository.GetOrderDetailByIdAsync(id);
        }

        public Task UpdateOrderDetail(OrderDetail orderDetail)
        {
            return _orderDetailRepository.UpdateOrderDetailAsync(orderDetail);
        }
    }
}
