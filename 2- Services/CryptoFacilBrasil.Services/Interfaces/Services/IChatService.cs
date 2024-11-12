using CryptoFacilBrasil.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoFacilBrasil.Services.Interfaces.Services
{
    public interface IChatService
    {
        Task CreateChat(Chat chat);
        Task<Chat> GetChat(long chatId);
        Task<OrderDetail> GetLastOrderDetail(long chatId);
    }
}
