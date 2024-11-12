using CryptoFacilBrasil.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoFacilBrasil.Domain.IRepository
{
    public interface IMessageRepository
    {
        Task<Message> GetMessageByIdAsync(int id);               
        Task<IEnumerable<Message>> GetMessagesByChatIdAsync(long chatId); 
        Task AddMessageAsync(Message message);                  
        Task UpdateMessageAsync(Message message);                
        Task DeleteMessageAsync(int id);                         
    }

}
