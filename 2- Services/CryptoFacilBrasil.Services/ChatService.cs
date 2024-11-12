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
    public class ChatService : IChatService
    {
        private IChatRepository _chatRepository;

        public ChatService(IChatRepository chatRepository)
        {
            _chatRepository = chatRepository;
        }

        public async Task CreateChat(Chat chat)
        {
            await _chatRepository.AddChatAsync(chat);
        }

        public async Task<Chat> GetChat(long chatId)
        {
            return await _chatRepository.GetChatByIdAsync(chatId);
        }

        public async Task<OrderDetail> GetLastOrderDetail(long chatId)
        {
            return await _chatRepository.GetLastOrderDetailByIdAsync(chatId);
        }
    }
}
