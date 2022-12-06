using DemoSendEmail.Dtos;
using DemoSendEmail.Services;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;

namespace DemoSendEmail.Controllers {
    [Route("api/[Controller]")]
    [ApiController]
    public class MailingController : ControllerBase {
        private readonly IMailingService _mailingService;

        public MailingController(IMailingService mailingService)
        {
            _mailingService = mailingService;
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
            var filePath = $"{Directory.GetCurrentDirectory()}\\Templates\\EmailTemplate.html";
            var str = new StreamReader(filePath);

            var mailText = str.ReadToEnd();
            str.Close();

            mailText = mailText.Replace("[username]", dto.UserName).Replace("[email]", dto.Email);

            await _mailingService.SendEmailAsync(dto.Email, "Welcome to our channel", mailText);
            return Ok();
        }
    }
}
