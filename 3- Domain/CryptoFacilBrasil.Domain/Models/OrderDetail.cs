using CryptoFacilBrasil.Domain.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;
using System.Text.Json.Serialization;

namespace CryptoFacilBrasil.Domain.Models
{
    public class OrderDetail
    {

        [Key]
        [Required]
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Valor da transação, que deve ser maior que zero.
        /// </summary>
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser maior que zero.")]
        public decimal Value { get; set; }

        /// <summary>
        /// Indica se a ordem é uma compra ou venda. Recomendado o uso de um enum para valores específicos.
        /// </summary>
        [Required]
        public string SellOrBuy { get; set; }



        /// <summary>
        /// Rede em que a transação vai ser realiada
        /// </summary>
        [Required]
        [Column(TypeName = "int")]
        public EnumNetwork Network { get; set; }

        [Required]
        [Column(TypeName = "int")]
        public EnumTypeTransferNetwork TypeTransferNetwork { get; set; }

        /// <summary>
        /// Método de pagamento opcional.
        /// </summary>
        [Required]
        [Column(TypeName = "int")]
        public EnumMethodPay MethodPay { get; set; }

        /// <summary>
        /// Relacionamento com o chat associado à ordem.
        /// </summary>
        [JsonIgnore]
        public Chat? Chat { get; set; }

        /// <summary>
        /// Identificador do Chat, representando uma chave estrangeira.
        /// </summary>
        [ForeignKey(nameof(Chat))]  // Vincula ChatId com a entidade Chat
        public long? ChatId { get; set; }

        /// <summary>
        /// Data de criação da ordem, definida automaticamente ao criar a instância.
        /// </summary>
        [Required]
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

        [Column(TypeName = "int")]
        public EnumStatusOrder StatusOrder { get; set; } = EnumStatusOrder.None;
    }




}
