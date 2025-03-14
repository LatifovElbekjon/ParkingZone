using System.Net;
using System.Net.Mail;

namespace ParkingZone.Helpers
{
    public class EEmailService
    {
        public async Task SendVerificationEmailAsync(string email, string code)
        {
            var fromAddress = new MailAddress("remind.me.via.bot@gmail.com", "Parking zone");
            var toAddress = new MailAddress(email);
            const string subject = "Email tasdiqlash";
            string body = $"Sizning tasdiqlash kodingiz: {code}";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("remind.me.via.bot@gmail.com", "zqbclmhxycseuuzi")
            };

            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
            {
                await smtp.SendMailAsync(message);
            }
            /*
              


             */
        }
    }

}
