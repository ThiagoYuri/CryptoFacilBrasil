using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using CryptoFacilBrasil.Domain.Models.Enums;

namespace Application.CryptoFacilBrasil.ModelsRequest
{
    public class CreateOrderDetailRequest
    {      
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser maior que zero.")]
        public decimal Value { get; set; }
        [Required]
        public string SellOrBuy { get; set; }
        [Required]
        public EnumNetwork Network { get; set; }
        [Required]
        public EnumMethodPay MethodPay { get; set; }

        [Required]
        public EnumTypeTransferNetwork TypeTransferNetwork { get; set; }
    }
}
