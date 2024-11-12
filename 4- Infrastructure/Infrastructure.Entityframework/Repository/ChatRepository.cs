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
    public class ChatRepository : IChatRepository
    {
        private readonly AppDbContext _context;

        // Constructor to inject the AppDbContext
        public ChatRepository(AppDbContext context) 
        {
            _context = context ?? throw new ArgumentNullException(nameof(context)); 
        }

        public async Task AddChatAsync(Chat chat)
        {
            if (chat == null)
            {
                throw new ArgumentNullException(nameof(chat), "O chat não pode ser nulo.");
            }

            await _context.Chats.AddAsync(chat); 
            await _context.SaveChangesAsync();  
        }

        public async Task DeleteChatAsync(long chatId)
        {
            throw new NotImplementedException();
        }

        public async Task<Chat> GetChatByIdAsync(long chatId)
        {
            return await _context.Set<Chat>().FirstOrDefaultAsync(chat => chat.IdChat == chatId);
        }

        public async Task<OrderDetail> GetLastOrderDetailByIdAsync(long chatId)
        {
            var chat = await _context.Set<Chat>()
                .Include(c => c.OrderDetails) 
                .FirstOrDefaultAsync(c => c.IdChat == chatId);

            if (chat == null) return null;            

            // Se existir OrderDetails, obtém o último; caso contrário, retorna null
            var lastOrderDetail = chat.OrderDetails?
                .OrderByDescending(orderDetail => orderDetail.CreatedAt)
                .FirstOrDefault();

            return lastOrderDetail;
        }

    }
}
