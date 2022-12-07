using DemoSendEmail.Settings;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MimeKit;
using System.IO;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;

namespace DemoSendEmail.Services {
    public class MailingService : IMailingService {
        private readonly MailSettings _mailSettings;
        private readonly IWebHostEnvironment _host;

        public MailingService(IOptions<MailSettings> mailSettings, IWebHostEnvironment host)
        {
            _mailSettings = mailSettings.Value;
            _host = host;
        }

        public async Task SendEmailAsync(string mailto, string subject, string body, IList<IFormFile> attachments = null)
        {
            var email = new MimeMessage()
            {
                Sender = MailboxAddress.Parse(_mailSettings.Email),
                Subject = subject ?? string.Empty,
            };
            email.To.Add(MailboxAddress.Parse(mailto));
            var builder = new BodyBuilder();
            if (attachments != null)
            {
                byte[] FileBytes;
                foreach (var file in attachments)
                {
                    if (file.Length > 0)
                    {
                        using var ms = new MemoryStream();
                        file.CopyTo(ms);
                        FileBytes = ms.ToArray();
                        builder.Attachments.Add(file.FileName, FileBytes, ContentType.Parse(file.ContentType));
                    }

                }
            }
            builder.HtmlBody = body;
            email.Body=builder.ToMessageBody();
            email.From.Add(new MailboxAddress(_mailSettings.DisplayName,_mailSettings.Email));
            using var smtp = new SmtpClient();
            smtp.Connect(_mailSettings.Host, _mailSettings.Port,SecureSocketOptions.StartTls);
            smtp.Authenticate(_mailSettings.Email, _mailSettings.Password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }
    }
}


