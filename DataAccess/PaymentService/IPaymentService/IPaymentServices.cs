using Models.Donations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.PaymentService.IPaymentService
{
    public class PaymentResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }

        public PaymentResult(bool success, string errorMessage = null)
        {
            Success = success;
            ErrorMessage = errorMessage;
        }
    }

    public interface IPaymentServices
    {
        Task<PaymentResult> ProcessPaymentAsync(Donation donation, Campaign? campaign);
    }
}
