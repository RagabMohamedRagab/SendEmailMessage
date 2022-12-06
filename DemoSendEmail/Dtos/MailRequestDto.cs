using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DemoSendEmail.Dtos {
    public class MailRequestDto {
        [Required]
        public string MailTo { get; set; }
        [Required]
        public string Subject { get; set; }
        [Required]
        public string Body { get; set; }
        public IList<IFormFile> Attachment { get; set; }
    }
}
