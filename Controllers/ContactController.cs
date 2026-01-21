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
        try
        {
            var fromEmail = Environment.GetEnvironmentVariable("SMTP_EMAIL");
            var appPassword = Environment.GetEnvironmentVariable("SMTP_PASSWORD");

            if (string.IsNullOrEmpty(fromEmail) || string.IsNullOrEmpty(appPassword))
            {
                ModelState.AddModelError("", "Email service is temporarily unavailable.");
                return View("~/Views/Home/Contact.cshtml", model);
            }
        
            var mail = new MailMessage
            {
                From = new MailAddress(fromEmail),
                Subject = "New Contact Message from Portfolio",
                Body = $@"Name: {model.Name} \nEmail: {model.Email} \n\n Message:\n{model.Message}"
            };

            mail.To.Add(fromEmail);

            var smtp = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential(fromEmail, appPassword),
                EnableSsl = true
            };

            smtp.Send(mail);

            TempData["Success"] = "Message sent successfully!";
            return RedirectToAction("Index");
        }
        catch(Exception ex)
        {
            // This prevents app crash
            ModelState.AddModelError("", "Failed to send message. Please try again later.");
            return View("~/Views/Home/Contact.cshtml", model);
        }
    }
}
