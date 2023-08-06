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
        private readonly ICreditValidator _creditValidator;

        private readonly StripeSettings _stripeOptions;
        private readonly SendGridSettings _sendGridOptions;
        private readonly TwilioSettings _twilioOptions;
        private readonly WazeForecastSettings _wazeForecastOptions;


        [BindProperty]
        public CreditApplication CreditModel { get; set;}


        public HomeController(IMarketForecaster marketForecaster, IOptions<WazeForecastSettings> wazeForecastOptions, ICreditValidator creditValidator)
        {
            this.homeVM = new HomeVM();
            _creditValidator  = creditValidator;
            _marketForecaster = marketForecaster;
            _wazeForecastOptions = wazeForecastOptions.Value;
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

        public IActionResult AllConfigSettings(
            [FromServices] IOptions<StripeSettings> stripeOptions,
			[FromServices] IOptions<TwilioSettings> twilioOptions,
			[FromServices] IOptions<SendGridSettings> sendGridOptions
			)
        {
            List<string> Messages = new List<string>();
            Messages.Add($"Waze Config - Forecast Tracker: "+_wazeForecastOptions.ForecastTrackerEnabled);
            Messages.Add($"Stripe - Publishable Key: " + stripeOptions.Value.PublishableKey);
            Messages.Add($"Stripe - Secret Key: " + stripeOptions.Value.SecretKey);
            Messages.Add($"Send Grid - Key: " + sendGridOptions.Value.SendGridKey);
            Messages.Add($"Twilio - Phone: " + twilioOptions.Value.PhoneNumber);
            Messages.Add($"Twilio - SID: " + twilioOptions.Value.AccountSid);
            Messages.Add($"Twilio - Token: " + twilioOptions.Value.AuthToken);

            return View(Messages);

        }

		public IActionResult CreditApplication()
		{
            CreditModel = new CreditApplication();

			return View(CreditModel);
		}

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("CreditApplication")]
        public async Task<IActionResult> CreditApplicationPOST()
        {
            if(ModelState.IsValid)
            {
                var (validationPassed, errorMessages) = await _creditValidator.PassAllValidations(CreditModel);

                CreditResult creditResult = new CreditResult()
                {
                    ErrorList = errorMessages,
                    CreditID = 0,
                    Success = validationPassed
                };

                if(validationPassed)
                {
                    //Add record to database
                    return RedirectToAction(nameof(CreditApplication), creditResult);
                }
                else
                {
                    return RedirectToAction(nameof(CreditApplication), creditResult);
                }
            }

            return View(CreditModel);
        }

        public IActionResult CreditResult(CreditResult creditResult)
        {
            return View(creditResult);
        }

    }
}