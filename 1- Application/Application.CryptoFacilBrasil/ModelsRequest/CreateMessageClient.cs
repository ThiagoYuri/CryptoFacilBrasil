using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Application.CryptoFacilBrasil.ModelsRequest
{
    public class CreateMessageClient
    {

        [Required(ErrorMessage = "O ChatId da mensagem é obrigatório.")]
        public long ChatId { get; set; }
        [Required(ErrorMessage = "O texto da mensagem é obrigatório.")]
        public string TextValue { get; set; }
        [Required(ErrorMessage = "O CreateAt da mensagem é obrigatório.")]
        public DateTime CreateAt { get; set; } = DateTime.UtcNow;
        [Required(ErrorMessage = "O IdMessageTelegram da mensagem é obrigatório.")]
        public int IdMessageTelegram { get; set; }

    }
}
