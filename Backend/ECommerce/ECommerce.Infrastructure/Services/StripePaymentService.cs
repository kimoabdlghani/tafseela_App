using ECommerce.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Stripe;

namespace ECommerce.Infrastructure.Services;

public class StripePaymentService : IPaymentService
{
    private readonly IConfiguration _configuration;
    private readonly PaymentIntentService _paymentIntentService;

    public StripePaymentService(IConfiguration configuration)
    {
        _configuration = configuration;
        
        StripeConfiguration.ApiKey = _configuration["StripeSettings:SecretKey"];
        
        _paymentIntentService = new PaymentIntentService();
    }

    public async Task<string> CreatePaymentTransactionAsync(int orderId, decimal amount, string customerEmail, string currency = "EGP", CancellationToken cancellationToken = default)
    {
        
        var options = new PaymentIntentCreateOptions
        {
            Amount = (long)(amount * 100),
            Currency = currency.ToLower(),
            PaymentMethodTypes = new List<string> { "card" },
            ReceiptEmail = customerEmail,
            Metadata = new Dictionary<string, string>
            {
                { "OrderId", orderId.ToString() }
            }
        };

        var paymentIntent = await _paymentIntentService.CreateAsync(options, cancellationToken: cancellationToken);

        return paymentIntent.ClientSecret;
    }

    public async Task<bool> VerifyPaymentAsync(string paymentIntentId, CancellationToken cancellationToken = default)
    {
        var paymentIntent = await _paymentIntentService.GetAsync(paymentIntentId, cancellationToken: cancellationToken);

        return paymentIntent.Status == "succeeded";
    }

    public bool ValidateStripeWebhook(string jsonPayload, string stripeSignatureHeader)
    {
        try
        {
            var endpointSecret = _configuration["StripeSettings:WebhookSecret"];
            
            var stripeEvent = EventUtility.ConstructEvent(
                jsonPayload, 
                stripeSignatureHeader, 
                endpointSecret
            );

            return true;
        }
        catch (StripeException)
        {
            return false;
        }
    }
}