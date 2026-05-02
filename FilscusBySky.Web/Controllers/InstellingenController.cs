using FilscusBySky.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FilscusBySky.Web.Controllers;

[Authorize]
public class InstellingenController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;

    public InstellingenController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    // GET: /Instellingen
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        return View(user);
    }

    // POST: /Instellingen
    [HttpPost]
    public async Task<IActionResult> Index(int meldingDag, int meldingUur)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return RedirectToAction("Index");

        user.MeldingDag = meldingDag;
        user.MeldingTijd = new TimeSpan(meldingUur, 0, 0);

        await _userManager.UpdateAsync(user);

        TempData["Succes"] = "Instellingen opgeslagen!";
        return RedirectToAction(nameof(Index));
    }
}