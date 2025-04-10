using DataAccess.PaymentService.IPaymentService;
using Models.Donations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Models.EnumsClass;
namespace DataAccess.PaymentService
{
    public class PaymentService : IPaymentServices
    {
        public async Task<PaymentResult> ProcessPaymentAsync(Donation donation , Campaign? campaign = null)
        {
            try
            {
                // Simulate payment processing
                await Task.Delay(1000); // Simulate network delay

                // For demo purposes, fail if the amount is a multiple of 100
                if (donation.Amount  >= 10000)
                {
                    return new PaymentResult(false, "Payment declined (simulated failure for amounts that are multiples of 100).");
                }
                if (campaign.CollectedAmount == campaign.TargetAmount)
                {
                    return new PaymentResult(false, "the TargetAmount already collected and Completed");
                }

                return new PaymentResult(true);
            }
            catch (Exception ex)
            {
                return new PaymentResult(false, $"Payment processing error: {ex.Message}");
            }
        }
    }
}
