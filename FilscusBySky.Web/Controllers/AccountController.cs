using FilscusBySky.BusinessLogic;
using FilscusBySky.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FilscusBySky.Web.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly EmailService _emailService;

    public AccountController(UserManager<ApplicationUser> userManager,
                             SignInManager<ApplicationUser> signInManager,
                             EmailService emailService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _emailService = emailService;
    }

    // GET: /Account/Register
    public IActionResult Register() => View();

    // POST: /Account/Register
    [HttpPost]
    public async Task<IActionResult> Register(string volledigeNaam, string email,
                                              string password, bool isVisueleBeperking)
    {
        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            VolledigeNaam = volledigeNaam,
            IsVisueleBeperking = isVisueleBeperking
        };

        var result = await _userManager.CreateAsync(user, password);

        if (result.Succeeded)
            return RedirectToAction("Index", "Home");

        foreach (var error in result.Errors)
            ModelState.AddModelError("", error.Description);

        return View();
    }

    // GET: /Account/Login
    public IActionResult Login() => View();

    // POST: /Account/Login
    [HttpPost]
    public async Task<IActionResult> Login(string email, string password, bool rememberMe)
    {
        var result = await _signInManager.PasswordSignInAsync(email, password, rememberMe, false);

        if (result.Succeeded)
            return RedirectToAction("Index", "Home");

        ModelState.AddModelError("", "Ongeldig e-mailadres of wachtwoord.");
        return View();
    }

    // POST: /Account/Logout
    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    // GET: /Account/WachtwoordVergeten
    public IActionResult WachtwoordVergeten() => View();

    // POST: /Account/WachtwoordVergeten
    [HttpPost]
    public async Task<IActionResult> WachtwoordVergeten(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user != null)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetLink = Url.Action("ResetWachtwoord", "Account",
                new { token, email }, Request.Scheme)!;
            await _emailService.StuurWachtwoordResetAsync(email, resetLink);
        }

        TempData["Bericht"] = "Als dit e-mailadres bestaat, ontvang je een e-mail.";
        return View();
    }

    // GET: /Account/ResetWachtwoord
    public IActionResult ResetWachtwoord(string token, string email) => View();

    // POST: /Account/ResetWachtwoord
    [HttpPost]
    public async Task<IActionResult> ResetWachtwoord(string token, string email,
                                                       string nieuwWachtwoord)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            return RedirectToAction(nameof(Login));

        var result = await _userManager.ResetPasswordAsync(user, token, nieuwWachtwoord);
        if (result.Succeeded)
        {
            TempData["Succes"] = "Wachtwoord succesvol gewijzigd!";
            return RedirectToAction(nameof(Login));
        }

        foreach (var error in result.Errors)
            ModelState.AddModelError("", error.Description);

        return View();
    }
}