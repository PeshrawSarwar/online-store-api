using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using online_store_api.Helpers;

namespace online_store_api.Controllers
{
    // Route Products
    [Route("[controller]")]
    // change the behaviour of the controller
    [ApiController]

    public class CurrencyController : ControllerBase {
        private readonly CurrencyService _currencyService;

        public CurrencyController(CurrencyService currencyService){
            this._currencyService = currencyService;
        }


       [HttpGet]
        public async Task<CurrencyResponse> Get(){
            var response = await _currencyService.GetLatestRatesAsync();
            var rate = response.Rates["IQD"];


            return new CurrencyResponse{
                Timestamp = response.Date,
                USDRate = rate
            };

            
        }



        
    }

    public class CurrencyResponse {
        public DateTime Timestamp { get; set; }
    
        public decimal USDRate { get; set; }
    }

    // Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
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