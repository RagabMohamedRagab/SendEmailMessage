using Microsoft.AspNetCore.Http;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DemoSendEmail.Services {
    public interface IMailingService {
        Task SendEmailAsync(string mailto,string subject,string body,IList<IFormFile> attachments=null);
    }
}
