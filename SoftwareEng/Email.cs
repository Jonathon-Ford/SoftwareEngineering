using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftwareEng
{
    public static class Email
    {
        private static MailboxAddress _hotelEmailAddress = new MailboxAddress("Ophelia's Oasis", "annarose0987@gmail.com");
        public static void GenerateDailyEmails()
        {
            GeneratePaymentReminder();
            GenerateCancellationNotification();
        }

        private static void GeneratePaymentReminder()
        {
            var reservationsDue = PreparedStatements.GetReservationsForEmail();
            string subject = "", body = "";

            foreach (var res in reservationsDue)
            {
                subject = "Payment Due for Reservation at Ophelia's Oasis";
                body = $"We are looking forward to your arrival on {res.StartDate}. In order to retain your reservation, please pay the " +
                    $"full bill by {res.StartDate.AddDays(-30)}. Thank you for booking a stay with us!\n\n";
                body += ReportGenerator.GenerateReservationHistory(PreparedStatements.GetAllResosToBeBilled(res));

                SendEmail(MailboxAddress.Parse(res.Email), _hotelEmailAddress, subject, body);
            }
        }

        private static void GenerateCancellationNotification()
        {
            var reservationsOverdue = PreparedStatements.GetReservationsToCancelForEmail();
            string subject = "", body = "";

            foreach (var res in reservationsOverdue)
            {
                subject = "Unpaid Reservation Cancelled";
                body = $"This message is being sent to inform you that your reservation beginning {res.StartDate} has been cancelled because it was not paid by the due date.";

                SendEmail(MailboxAddress.Parse(res.Email), _hotelEmailAddress, subject, body);
            }
        }

        private static void SendEmail(MailboxAddress to, MailboxAddress from, string subject, string body)
        {            
            MimeMessage message = new MimeMessage();
            message.From.Add(from);
            message.To.Add(to);
            message.Subject = subject;
            message.Body = new TextPart(TextFormat.Plain) { Text = body };
            
            using SmtpClient client = new SmtpClient();

            try
            {
                client.Connect("smtp.gmail.com", 587, SecureSocketOptions.Auto);
                client.Authenticate("opheliaoasis2022@gmail.com", "SEoo2022");
                client.Send(message);
                client.Disconnect(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to send email to ");
                foreach(var addr in message.To)
                {
                    Console.WriteLine(addr);
                }
            }
        }
    }
}
