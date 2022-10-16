using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;

namespace online_store_api.Helpers{
    public class CurrencyService{
        private readonly HttpClient _httpClient;
        private readonly CurrencyScoopOptions _options;

        public CurrencyService(HttpClient httpClient, IOptionsSnapshot<CurrencyScoopOptions> options){
            this._httpClient = httpClient;
            this._options = options.Value;
        }
    public async Task<CurrencyScoopResponse> GetLatestRatesAsync(){
        
            var url = $"latest?base=USD&symbols=IQD&api_key={_options.ApiKey}";

            var root = await _httpClient.GetFromJsonAsync<CurrencyScoopRoot>(url);

            return root.Response;
         }
        
    }


    public class CurrencyScoopMeta
    {
        [JsonPropertyName("code")]
        public int Code { get; set; }

        [JsonPropertyName("disclaimer")]
        public string Disclaimer { get; set; }
    }

  

    public class CurrencyScoopResponse
    {
        [JsonPropertyName("date")]
        public DateTime Date { get; set; }

        [JsonPropertyName("base")]
        public string Base { get; set; }

        [JsonPropertyName("rates")]
        public Dictionary<string, decimal> Rates { get; set; }
    }

    public class CurrencyScoopRoot
    {
        [JsonPropertyName("meta")]
        public CurrencyScoopMeta Meta { get; set; }

        [JsonPropertyName("response")]
        public CurrencyScoopResponse Response { get; set; }
    }
}