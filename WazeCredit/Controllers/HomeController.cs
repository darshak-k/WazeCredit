using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WazeCredit.Models;
using WazeCredit.Models.ViewModels;
using WazeCredit.Services;
using WazeCredit.Utility.AppSettingsClasses;
using Microsoft.Extensions.Options;

namespace WazeCredit.Controllers
{
    public class HomeController : Controller
    {
        public HomeVM homeVM { get; set; }
        private readonly IMarketForecaster _marketForecaster;
        private readonly StripeSettings _stripeOptions;
        private readonly SendGridSettings _sendGridOptions;
        private readonly TwilioSettings _twilioOptions;
        private readonly WazeForecastSettings _wazeForecastOptions;


        public HomeController(IMarketForecaster marketForecaster, IOptions<StripeSettings> stripeOptions, IOptions<TwilioSettings> twilioOptions, IOptions<WazeForecastSettings> wazeForecastOptions, IOptions<SendGridSettings> sendGridOptions)
        {
            this.homeVM = new HomeVM();
            _marketForecaster = marketForecaster;
            _sendGridOptions = sendGridOptions.Value;
            _wazeForecastOptions = wazeForecastOptions.Value;
            _twilioOptions = twilioOptions.Value;
            _stripeOptions = stripeOptions.Value;
        }

        public IActionResult Index(MarketResult currenntMarket)
        {
            MarketResult currentMarket = _marketForecaster.GetMarketPrediction();

            switch (currenntMarket.MarketCondition)
            {
                case MarketCondition.STABLE_DOWN: 
                    homeVM.MarketForecast = "Stable down Market";
                    break;
                case MarketCondition.STABLE_UP: 
                    homeVM.MarketForecast = "Stable Up Market";
                    break;
                case MarketCondition.VOLATILE: 
                    homeVM.MarketForecast = "Volatile Market";
                    break;
                default:
                    homeVM.MarketForecast = "No Forecast";
                    break;
            }

            return View(homeVM);
        }

        public ActionResult AllConfigSettings()
        {
            List<string> Messages = new List<string>();
            Messages.Add($"Waze Config - Forecast Tracker: "+_wazeForecastOptions.ForecastTrackerEnabled);
            Messages.Add($"Stripe - Publishable Key: " + _stripeOptions.PublishableKey);
            Messages.Add($"Stripe - Secret Key: " + _stripeOptions.SecretKey);
            Messages.Add($"Send Grid - Key: " + _sendGridOptions.SendGridKey);
            Messages.Add($"Twilio - Phone: " + _twilioOptions.PhoneNumber);
            Messages.Add($"Twilio - SID: " + _twilioOptions.AccountSid);
            Messages.Add($"Twilio - Token: " + _twilioOptions.AuthToken);

            return View(Messages);

        }
    }
}