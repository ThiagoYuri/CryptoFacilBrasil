using CryptoFacilBrasil.Domain.IRepository;
using CryptoFacilBrasil.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Entityframework.Repository
{
    public class MessageRepository : IMessageRepository
    {
        private readonly AppDbContext _context;

        // Constructor to inject the AppDbContext
        public MessageRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Task AddMessageAsync(Message message)
        {
            _context.Messages.Add(message);
            _context.SaveChanges();
            return Task.CompletedTask;
        }

        public Task DeleteMessageAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Message> GetMessageByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Message>> GetMessagesByChatIdAsync(long chatId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateMessageAsync(Message message)
        {
            throw new NotImplementedException();
        }
    }
}
