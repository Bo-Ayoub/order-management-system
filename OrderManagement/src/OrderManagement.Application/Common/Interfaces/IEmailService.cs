using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagement.Application.Common.Interfaces
{
    public interface IEmailService
    {
        Task SendOrderConfirmationEmailAsync(
            string toEmail,
            string customerName,
            string orderNumber,
            string totalAmount,
            CancellationToken cancellationToken = default);

        Task SendOrderStatusUpdateEmailAsync(
            string toEmail,
            string customerName,
            string orderNumber,
            string newStatus,
            CancellationToken cancellationToken = default);

        Task SendWelcomeEmailAsync(
            string toEmail,
            string customerName,
            CancellationToken cancellationToken = default);
    }
}
