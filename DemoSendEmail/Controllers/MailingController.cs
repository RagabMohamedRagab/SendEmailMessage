using DemoSendEmail.Dtos;
using DemoSendEmail.Services;
using DemoSendEmail.Settings;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MimeKit;
using System.IO;
using System.Threading.Tasks;

namespace DemoSendEmail.Controllers {
    [Route("api/[Controller]")]
    [ApiController]
    public class MailingController : ControllerBase {
        private readonly IMailingService _mailingService;
        private readonly IWebHostEnvironment _web;
        private readonly MailSettings _mailSettings;

        public MailingController(IMailingService mailingService, IWebHostEnvironment web, IOptions<MailSettings> mailSettings)
        {
            _mailingService = mailingService;
            _web = web;
            _mailSettings = mailSettings.Value;
        }

        [HttpPost("Send")]
        public async Task<IActionResult> Index([FromForm] MailRequestDto dto)
        {
            await _mailingService.SendEmailAsync(dto.MailTo, dto.Subject, dto.Body, dto.Attachment); ;
            return Ok();
        }
        [HttpPost("welcome")]
        public async Task<IActionResult> SendWelcomeEmail([FromBody] WelcomeRequestDto dto)
        {
            var builder = new BodyBuilder();
            var pathinfoFile = _web.WebRootPath
                 + Path.DirectorySeparatorChar.ToString()
                 + "Templates"
                 + Path.DirectorySeparatorChar.ToString()
                 + "index.html";
            using (StreamReader streamReader = System.IO.File.OpenText(pathinfoFile))
            {
                builder.HtmlBody = streamReader.ReadToEnd();
            }
            builder.HtmlBody = builder.HtmlBody.Replace("[username]", _mailSettings.Email);
            builder.HtmlBody = builder.HtmlBody.Replace("[email]", _mailSettings.DisplayName);
            await _mailingService.SendEmailAsync(dto.Email, "Welcome to our channel", builder.HtmlBody);
            return Ok();
        }
    }
}
