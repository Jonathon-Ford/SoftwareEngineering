using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;

namespace SoftwareEng
{
    public static class Email
    {
        private static MailboxAddress _hotelEmailAddress = new MailboxAddress("Ophelia's Oasis", "opheliaoasis2022@gmail.com");
        
        /// <summary>
        /// This function calls other functions to generate any emails that must be sent daily.
        /// </summary>
        /// Author: AS
        public static void GenerateDailyEmails()
        {
            GeneratePaymentReminder();
            GenerateCancellationNotification();
        }

        /// <summary>
        /// This function gets all the reservations that need to be sent a payment reminder, generates a customized email for each, and passes it to the appropriate function
        /// to be sent.
        /// </summary>
        /// Author: AS
        private static void GeneratePaymentReminder()
        {
            var reservationsDue = PreparedStatements.GetReservationsForEmail();
            string subject = "", body = "";

            foreach (var res in reservationsDue)
            {
                subject = "Payment Due for Reservation at Ophelia's Oasis";
                body = $"We are looking forward to your arrival on {res.StartDate:mm/dd/yyyy)}. In order to retain your reservation, please pay the " +
                    $"full bill by {res.StartDate.AddDays(-30):mm/dd/yyyy}. Thank you for booking a stay with us!\n\n";
                body += ReportGenerator.GenerateReservationHistory(PreparedStatements.GetAllResosToBeBilled(res));

                SendEmail(MailboxAddress.Parse(res.Email), _hotelEmailAddress, subject, body);
            }
        }

        /// <summary>
        /// This function gets all the reservations that need to be cancelled because they missed the payment deadline, cancels them, and generates notification emails. It
        /// passes each email to the appropriate function to be sent.
        /// </summary>
        /// Author: AS
        private static void GenerateCancellationNotification()
        {
            var reservationsOverdue = PreparedStatements.GetReservationsToCancelForEmail();
            string subject = "", body = "";

            foreach (var res in reservationsOverdue)
            {
                subject = "Unpaid Reservation Cancelled";
                body = $"This message is being sent to inform you that your reservation beginning {res.StartDate:mm/dd/yyyy} has been cancelled because it was not paid by the due date.";

                SendEmail(MailboxAddress.Parse(res.Email), _hotelEmailAddress, subject, body);
                PreparedStatements.MarkReservationAsCanceled(res);
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
                client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                client.Authenticate(_hotelEmailAddress.Address, "SEoo2022");
                client.Send(message);
                client.Disconnect(true);
                Console.WriteLine("Emails sent");
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
