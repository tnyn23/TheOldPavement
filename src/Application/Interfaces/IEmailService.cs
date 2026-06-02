using System.Threading.Tasks;
using Domain.Models;

namespace Application.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(string toEmail, string subject, string body, bool isHtml = true);
    Task SendOrderConfirmationEmailAsync(Order order);
    Task SendAccountCreationEmailAsync(string toEmail, string fullName, string rawPassword);
    Task SendPasswordRecoveryEmailAsync(string toEmail, string fullName, string tempPassword, string loginUrl = "");
}


