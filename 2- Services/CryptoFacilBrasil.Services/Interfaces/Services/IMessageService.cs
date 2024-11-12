using CryptoFacilBrasil.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoFacilBrasil.Services.Interfaces.Services
{
    public interface IMessageService
    {
        Task<List<Message>> GetMessages(long chatId);

        Task CreateMessage(Message message);

    }
}
