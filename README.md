# Send Emails with ASP.NET Core

<img src="Help/download.png" style="width:100%; height:300px"/>

## Step 1 – Create a New ASP.NET Core Project

`For this tutorial, let’s build our Mail Service on to an ASP.NET Core 3.1 Web API Project. I am using Visual Studio 2019 Community as my IDE (The Best IDE Ever!). You can find the completed source code on my GitHub. This guide will be pretty easy to follow along, especially for the beginners. Create a new Controller and name it MailController.
`
## Step 2 – Add the Required Mail Models
```bash
  public class MailRequest
{
    public string ToEmail { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
    public List<IFormFile> Attachments { get; set; }
}
```
## Step 3 – Configure Mail Settings in appsettings.json
```bash
"MailSettings": {
  "Mail": "<fromemail>",
  "DisplayName": "<displayname>",
  "Password": "<yourpasswordhere>",
  "Host": "smtp.gmail.com",
  "Port": 587
}
```

```bash
public class MailSettings
{
    public string Mail { get; set; }
    public string DisplayName { get; set; }
    public string Password { get; set; }
    public string Host { get; set; }
    public int Port { get; set; }
}
```
```bash
services.Configure<MailSettings>(Configuration.GetSection("MailSettings"));
```
## Step 4 – Add a Service Layer to Send Mails

```bash
Install-Package MailKit
Install-Package MimeKit
```
Now with that out of the way, let’s build Service classes that is actually responsible to send Mails. Add an Interface first. Services/IMailService.cs
```bash
public interface IMailService
{
    Task SendEmailAsync(MailRequest mailRequest);
}
```
We will have a method that takes in the MailRequest object and sends the email. Next, Add a concrete class that will implement this interface. Services/MailService.cs
```bash
public class MailService : IMailService
{
    private readonly MailSettings _mailSettings;
    public MailService(IOptions<MailSettings> mailSettings)
    {
        _mailSettings = mailSettings.Value;
    }
}
```
You can see that we are Injecting the IOptions<MailSettings> to the constructor and assigning it’s value to the instance of MailSettings. Like this, we will be able to access the data from the JSON at runtime. Now, let’s add the missing Method.
```bash
public async Task SendEmailAsync(MailRequest mailRequest)
{
    var email = new MimeMessage();
    email.Sender = MailboxAddress.Parse(_mailSettings.Mail);
    email.To.Add(MailboxAddress.Parse(mailRequest.ToEmail));
    email.Subject = mailRequest.Subject;
    var builder = new BodyBuilder();
    if (mailRequest.Attachments != null)
    {
        byte[] fileBytes;
        foreach (var file in mailRequest.Attachments)
        {
            if (file.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    file.CopyTo(ms);
                    fileBytes = ms.ToArray();
                }
                builder.Attachments.Add(file.FileName, fileBytes, ContentType.Parse(file.ContentType));
            }
        }
    }
    builder.HtmlBody = mailRequest.Body;
    email.Body = builder.ToMessageBody();
    using var smtp = new SmtpClient();
    smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
    smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
    await smtp.SendAsync(email);
    smtp.Disconnect(true);
}
```
## Configuring Services
```bash
services.AddTransient<IMailService, Services.MailService>();
```
## Adding the Controller Method
```bash
[Route("api/[controller]")]
[ApiController]
public class MailController : ControllerBase
{
    private readonly IMailService mailService;
    public MailController(IMailService mailService)
    {
        this.mailService = mailService;
    }
    [HttpPost("send")]
    public async Task<IActionResult> SendMail([FromForm]MailRequest request)
    {
        try
        {
            await mailService.SendEmailAsync(request);
            return Ok();
        }
        catch (Exception ex)
        {
            throw;
        }
            
    }
}
```
PlayList : <a href="bddc1d57eb34aedcf264cecac667cb3e">Here ...</a> <br/>
Tutorial : <a href="https://code-maze.com/aspnetcore-send-email/">Here ...</a>
