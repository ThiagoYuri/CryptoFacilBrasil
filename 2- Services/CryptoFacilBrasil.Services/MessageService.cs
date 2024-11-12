using CryptoFacilBrasil.Domain.IRepository;
using CryptoFacilBrasil.Domain.Models;
using CryptoFacilBrasil.Services.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoFacilBrasil.Services
{
    public class MessageService : IMessageService
    {

        private IMessageRepository _messageRepository;

        public MessageService(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }

        public async Task CreateMessage(Message message)
        {           
            await _messageRepository.AddMessageAsync(message);
        }

        public Task<List<Message>> GetMessages(long chatId)
        {
            throw new NotImplementedException();
        }
    }
}
