using CryptoFacilBrasil.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoFacilBrasil.Domain.IRepository
{
    public interface IChatRepository
    {
        Task<Chat> GetChatByIdAsync(long chatId); 
        Task<OrderDetail> GetLastOrderDetailByIdAsync(long chatId);
        Task AddChatAsync(Chat chat); 
        Task DeleteChatAsync(long chatId); 
    }

}
