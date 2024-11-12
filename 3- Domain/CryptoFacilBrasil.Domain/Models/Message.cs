using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoFacilBrasil.Domain.Models
{
    public class Message
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required(ErrorMessage = "O texto da mensagem é obrigatório.")]
        public string TextValue { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.UtcNow;

        public int IdMessageTelegram { get; set; }

        public bool IsBot { get; set; }= false;

        /// <summary>
        /// Identificador do Chat, representando uma chave estrangeira.
        /// </summary>
        [Required]
        [ForeignKey(nameof(Chat))]  // Vincula ChatId com a entidade Chat
        public long ChatId { get; set; }
        public Chat? Chat { get; set; }
        
    }
}
