namespace Application.CryptoFacilBrasil.ModelsResponse
{
    public class OrderDetailResponse
    {
        public Guid Id { get; set; }
        public decimal Value { get; set; }
        public int SellOrBuy { get; set; }
        public int Network { get; set; }
        public int TypeTransferNetwork { get; set; }
        public int MethodPay { get; set; }
        public DateTime CreatedAt { get; set; }
        public int StatusOrder { get; set; }
    }
}
