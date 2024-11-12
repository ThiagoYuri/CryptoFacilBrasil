namespace Application.CryptoFacilBrasil.ModelsRequest
{
    public class SendTelegramManualMessageRequest
    {
        public long ChatId { get; set; }
        public string Message { get; set; }
    }
}
