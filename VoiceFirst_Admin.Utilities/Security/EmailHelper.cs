using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using VoiceFirst_Admin.Utilities.DTOs.Shared;

namespace VoiceFirst_Admin.Utilities.Security
{
    public static class EmailHelper
    {
        public  static bool SendMail(EmailDTO emailDtoModel)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(emailDtoModel.t8_from_email)) return false;
                if (string.IsNullOrWhiteSpace(emailDtoModel.t8_to_email)) return false;
                if (string.IsNullOrWhiteSpace(emailDtoModel.t8_email_subject)) return false;

                // Build email body
                var emailBody = new StringBuilder("<html><body>");
                if (!string.IsNullOrEmpty(emailDtoModel.t8_email_html_body))
                {
                    emailBody.Append(emailDtoModel.t8_email_html_body);
                }
                if (!string.IsNullOrEmpty(emailDtoModel.t8_signature_content))
                {
                    emailBody.Append(emailDtoModel.t8_signature_content);
                }
                emailBody.Append("</body></html>");

                // Create SMTP message
                var mail = new MailMessage
                {
                    From = new MailAddress(emailDtoModel.t8_from_email),
                    Subject = emailDtoModel.t8_email_subject,
                    Body = emailBody.ToString(),
                    IsBodyHtml = true
                };
                mail.To.Add(new MailAddress(emailDtoModel.t8_to_email));

                using var smtp = new SmtpClient("smtp.gmail.com", 587)
                {
                    EnableSsl = true
                };
                if (!string.IsNullOrWhiteSpace(emailDtoModel.t8_from_email_password))
                {
                    smtp.Credentials = new System.Net.NetworkCredential(emailDtoModel.t8_from_email, emailDtoModel.t8_from_email_password);
                }
                smtp.Send(mail);

                return true; // Success

            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                Console.WriteLine($"Error sending email: {ex.Message}");
                return false; // Failure
            }
        }
    }
}
