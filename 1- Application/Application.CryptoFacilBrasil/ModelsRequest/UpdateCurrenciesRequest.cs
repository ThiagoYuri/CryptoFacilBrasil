namespace Application.CryptoFacilBrasil.ModelsRequest
{
    public class UpdateCurrenciesRequest
    {
        public string name { get; set; }
        public string original_value { get; set; }
        public string converted_to_usd { get; set; }
    }
}
