using CryptoFacilBrasil.Domain.IRepository;
using CryptoFacilBrasil.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Entityframework.Repository
{
    public class OrderDetailRepository : IOrderDetailRepository
    {
        private readonly AppDbContext _context;

        // Construtor para injetar o AppDbContext
        public OrderDetailRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // Recupera um OrderDetail pelo Id de forma assíncrona
        public async Task<OrderDetail> GetOrderDetailByIdAsync(Guid id)
        {
            var orderDetail = await _context.OrderDetails.FindAsync(id);

            if (orderDetail == null)
                throw new KeyNotFoundException($"OrderDetail with Id {id} not found.");

            return orderDetail;
        }

        // Recupera uma lista de OrderDetails associados a um ChatId específico
        public async Task<IEnumerable<OrderDetail>> GetOrderDetailsByChatIdAsync(long chatId)
        {
            return await _context.OrderDetails
                .Where(od => od.ChatId == chatId)
                .ToListAsync();
        }

        // Adiciona um novo OrderDetail de forma assíncrona
        public async Task AddOrderDetailAsync(OrderDetail orderDetail)
        {
            if (orderDetail == null)
                throw new ArgumentNullException(nameof(orderDetail));

            await _context.OrderDetails.AddAsync(orderDetail);
            await _context.SaveChangesAsync();
        }

        // Atualiza um OrderDetail existente de forma assíncrona
        public async Task UpdateOrderDetailAsync(OrderDetail orderDetail)
        {
            if (orderDetail == null)
                throw new ArgumentNullException(nameof(orderDetail));

            var existingOrderDetail = await GetOrderDetailByIdAsync(orderDetail.Id);

            if (existingOrderDetail == null)
                throw new KeyNotFoundException($"OrderDetail with Id {orderDetail.Id} not found.");

            // Atualiza as propriedades
            _context.Entry(existingOrderDetail).CurrentValues.SetValues(orderDetail);

            await _context.SaveChangesAsync();
        }

        // Deleta um OrderDetail pelo Id de forma assíncrona
        public async Task DeleteOrderDetailAsync(Guid id)
        {
            var orderDetail = await _context.OrderDetails.FindAsync(id);

            if (orderDetail == null)
                throw new KeyNotFoundException($"OrderDetail with Id {id} not found.");

            _context.OrderDetails.Remove(orderDetail);
            await _context.SaveChangesAsync();
        }

    }


}
