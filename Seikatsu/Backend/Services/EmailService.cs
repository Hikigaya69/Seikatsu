using Seikatsu.Backend.Services;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

public class EmailService(IConfiguration config, ILogger<EmailService> logger) : IEmailService
{
    private readonly HttpClient _httpClient = new();

    public async Task SendAsync(string to, string subject, string body)
    {
        try
        {
            var payload = new
            {
                sender = new
                {
                    email = config["Brevo:From"],
                    name = "Seikatsu"
                },
                to = new[] { new { email = to } },
                subject = subject,
                htmlContent = body
            };

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("api-key", config["Brevo:ApiKey"]);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("https://api.brevo.com/v3/smtp/email", content);
            var responseBody = await response.Content.ReadAsStringAsync();

            logger.LogInformation("Brevo status: {StatusCode}", response.StatusCode);

            if (!response.IsSuccessStatusCode)
                logger.LogError("Brevo error: {Body}", responseBody);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send email to {To}", to);
            throw;
        }
    }
}


//using Seikatsu.Backend.Services;

//using SendGrid;
//using SendGrid.Helpers.Mail;
//using System.Net.Mail;

//public class EmailService(IConfiguration config, ILogger<EmailService> logger) : IEmailService
//{
//    public async Task SendAsync(string to, string subject, string body)
//    {
//        try
//        {
//            var client = new SendGridClient(config["SendGrid:ApiKey"]);
//            var from = new EmailAddress(config["SendGrid:From"], "Seikatsu");
//            var toAddress = new EmailAddress(to);
//            var msg = MailHelper.CreateSingleEmail(from, toAddress, subject, plainTextContent: null, htmlContent: body);

//            var response = await client.SendEmailAsync(msg);
//            logger.LogInformation("SendGrid status: {StatusCode}", response.StatusCode);
//        }
//        catch (Exception ex)
//        {
//            logger.LogError(ex, "Failed to send email to {To}", to);
//            throw;
//        }
//    }
//}

//should be used after sendgrid completely fails
//using Seikatsu.Backend.Services;
//using System.Net.Http.Headers;
//using System.Text;
//using System.Text.Json;

//public class EmailService(IConfiguration config, ILogger<EmailService> logger) : IEmailService
//{
//    private readonly HttpClient _httpClient = new();

//    public async Task SendAsync(string to, string subject, string body)
//    {
//        try
//        {
//            var payload = new
//            {
//                from = "onboarding@resend.dev", // use this until you have a custom domain which is nevvver
//                to = new[] { to },
//                subject = subject,
//                html = body
//            };

//            _httpClient.DefaultRequestHeaders.Authorization =
//                new AuthenticationHeaderValue("Bearer", config["Resend:ApiKey"]);

//            var json = JsonSerializer.Serialize(payload);
//            var content = new StringContent(json, Encoding.UTF8, "application/json");

//            var response = await _httpClient.PostAsync("https://api.resend.com/emails", content);
//            var responseBody = await response.Content.ReadAsStringAsync();

//            logger.LogInformation("Resend status: {StatusCode}", response.StatusCode);

//            if (!response.IsSuccessStatusCode)
//                logger.LogError("Resend error: {Body}", responseBody);
//        }
//        catch (Exception ex)
//        {
//            logger.LogError(ex, "Failed to send email to {To}", to);
//            throw;
//        }
//    }
//}