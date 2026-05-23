using System.Threading.Tasks;
using Domain.Models;

namespace Application.Interfaces;

public interface IMomoService
{
    Task<string> CreatePaymentUrlAsync(Order order, string redirectUrl, string ipnUrl);
    bool ValidateSignature(
        string partnerCode, 
        string orderId, 
        string requestId, 
        string amount, 
        string orderInfo, 
        string orderType, 
        string transId, 
        string resultCode, 
        string message, 
        string payType, 
        string responseTime, 
        string extraData, 
        string signature);
}


