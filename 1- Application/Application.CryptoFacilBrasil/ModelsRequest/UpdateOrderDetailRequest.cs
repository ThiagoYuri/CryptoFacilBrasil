using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using CryptoFacilBrasil.Domain.Models.Enums;

namespace Application.CryptoFacilBrasil.ModelsRequest
{
    public class UpdateOrderDetailRequest
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser maior que zero.")]
        public decimal Value { get; set; }
        [Required]
        public string SellOrBuy { get; set; }
        [Required]
        public long? ChatId { get; set; }
        [Required]
        public EnumNetwork Network { get; set; }
        [Required]
        public EnumMethodPay MethodPay { get; set; }
        [Required]
        public EnumStatusOrder StatusOrder { get; set; }
        [Required]
        public EnumTypeTransferNetwork TypeTransferNetwork { get; set; }
    }
}
