using WazeCredit.Utility.AppSettingsClasses;

namespace WazeCredit.Utility.DI_Config;

public class DIAppSettingsConfig
{
	public static void AddAppSettingConfig(WebApplicationBuilder builder)
	{

		builder.Services.Configure<WazeForecastSettings>(builder.Configuration.GetSection("WazeForecast"));
		builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));
		builder.Services.Configure<TwilioSettings>(builder.Configuration.GetSection("Twilio"));
		builder.Services.Configure<SendGridSettings>(builder.Configuration.GetSection("SendGrid"));
	}
}
