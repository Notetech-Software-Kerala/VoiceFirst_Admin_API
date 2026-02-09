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
                if (string.IsNullOrWhiteSpace(emailDtoModel.from_email)) return false;
                if (string.IsNullOrWhiteSpace(emailDtoModel.to_email)) return false;
                if (string.IsNullOrWhiteSpace(emailDtoModel.email_subject)) return false;

                // Build email body
                var emailBody = new StringBuilder("<html><body>");
                if (!string.IsNullOrEmpty(emailDtoModel.email_html_body))
                {
                    emailBody.Append(emailDtoModel.email_html_body);
                }
                if (!string.IsNullOrEmpty(emailDtoModel.signature_content))
                {
                    emailBody.Append(emailDtoModel.signature_content);
                }
                emailBody.Append("</body></html>");

                // Create SMTP message
                var mail = new MailMessage
                {
                    From = new MailAddress(emailDtoModel.from_email),
                    Subject = emailDtoModel.email_subject,
                    Body = emailBody.ToString(),
                    IsBodyHtml = true
                };
                mail.To.Add(new MailAddress(emailDtoModel.to_email));

                using var smtp = new SmtpClient("smtp.gmail.com", 587)
                {
                    EnableSsl = true
                };
                if (!string.IsNullOrWhiteSpace(emailDtoModel.from_email_password))
                {
                    smtp.Credentials = new System.Net.NetworkCredential(emailDtoModel.from_email, emailDtoModel.from_email_password);
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
