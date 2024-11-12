using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoFacilBrasil.Domain.Models
{
    public class Chat
    {
        [Key]
        [Required]
        public long IdChat { get; set; }
        public DateTime CreateAt { get; private set; } = DateTime.UtcNow;
        public List<Message>? Messages { get; set; }        
        public List<OrderDetail>? OrderDetails { get; set; }

    }
}
