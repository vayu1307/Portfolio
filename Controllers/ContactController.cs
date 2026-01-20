using Microsoft.AspNetCore.Mvc;
using MyPortfolio.Models;
using System.Net;
using System.Net.Mail;

public class ContactController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View("~/Views/Home/Contact.cshtml");
    }

    [HttpPost]
    public IActionResult Send(ContactViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("~/Views/Home/Contact.cshtml", model);
        }

        var fromEmail = Environment.GetEnvironmentVariable("SMTP_EMAIL");
        var appPassword = Environment.GetEnvironmentVariable("SMTP_PASSWORD");

        var mail = new MailMessage
        {
            From = new MailAddress(fromEmail),
            Subject = "New Contact Message from Portfolio",
            Body = $@"Name: {model.Name}
                    Email: {model.Email}

                    Message:
                    {model.Message}"
        };

        mail.To.Add(fromEmail);

        var smtp = new SmtpClient("smtp.gmail.com", 587)
        {
            Credentials = new NetworkCredential(fromEmail, appPassword),
            EnableSsl = true
        };

        smtp.Send(mail);

        TempData["Success"] = "Message sent successfully!";
        return RedirectToAction("Index"); // ✅ REQUIRED
    }
}
