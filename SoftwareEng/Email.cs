using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace SoftwareEng
{
    public static class Email
    {
        private static MailAddress _hotelEmailAddress = new MailAddress("ophelias.oasis@gmail.com", "Ophelia's Oasis");
        public static void GenerateDailyEmails()
        {
            GeneratePaymentReminder();
            GenerateCancellationNotification();
        }

        private static void GeneratePaymentReminder()
        {
            var reservationsDue = PreparedStatements.GetReservationsForEmail();
            var toAddresses = new MailAddressCollection();
            string subject = "", body = "";

            foreach (var res in reservationsDue)
            {
                toAddresses.Add(res.Email);
                subject = "Payment Due for Reservation at Ophelia's Oasis";
                body = $"We are looking forward to your arrival on {res.StartDate}. In order to retain your reservation, please pay the " +
                    $"full bill by {res.StartDate.AddDays(-30)}. Thank you for booking a stay with us!\n\n";
                body += ReportGenerator.GenerateReservationHistory(PreparedStatements.GetAllResosToBeBilled(res));

                SendEmail(toAddresses, _hotelEmailAddress, subject, body);
            }
        }

        private static void GenerateCancellationNotification()
        {
            var reservationsOverdue = PreparedStatements.GetReservationsToCancelForEmail();
            var toAddresses = new MailAddressCollection();
            string subject = "", body = "";

            foreach (var res in reservationsOverdue)
            {
                toAddresses.Add(res.Email);
                subject = "Unpaid Reservation Cancelled";
                body = $"This message is being sent to inform you that your reservation beginning {res.StartDate} has been cancelled because it was not paid by the due date.";

                SendEmail(toAddresses, _hotelEmailAddress, subject, body);
            }
        }

        private static void SendEmail(MailAddressCollection to, MailAddress from, string subject, string body)
        {
            MailMessage message = new MailMessage();
            message.To.Concat(to);
            message.From = from;
            message.Subject = subject;
            message.Body = body;
            SmtpClient client = new SmtpClient("smtp.gmail.com");
            // Credentials are necessary if the server requires the client
            // to authenticate before it will send email on the client's behalf.
            client.UseDefaultCredentials = true;

            try
            {
                client.Send(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
