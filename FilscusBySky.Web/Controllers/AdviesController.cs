using FilscusBySky.BusinessLogic;
using FilscusBySky.Data;
using FilscusBySky.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FilscusBySky.Web.Controllers;

[Authorize]
public class AdviesController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IAIService _aiService;

    public AdviesController(AppDbContext context,
                            UserManager<ApplicationUser> userManager,
                            IAIService aiService)
    {
        _context = context;
        _userManager = userManager;
        _aiService = aiService;
    }

    // GET: /Advies
    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);
        var rekeningen = await _context.Rekeningen
            .Where(r => r.UserId == userId)
            .ToListAsync();

        return View(rekeningen);
    }

    // POST: /Advies/Genereer
    [HttpPost]
    public async Task<IActionResult> Genereer(int rekeningId)
    {
        var userId = _userManager.GetUserId(User);

        var rekening = await _context.Rekeningen
            .FirstOrDefaultAsync(r => r.Id == rekeningId && r.UserId == userId);

        if (rekening == null)
            return RedirectToAction(nameof(Index));

        var transacties = await _context.Transacties
            .Where(t => t.RekeningId == rekeningId)
            .ToListAsync();

        var advies = await _aiService.GenereerAdviesAsync(rekening, transacties);

        ViewBag.Advies = advies;
        ViewBag.RekeningNaam = rekening.Naam;

        var rekeningen = await _context.Rekeningen
            .Where(r => r.UserId == userId)
            .ToListAsync();

        return View("Index", rekeningen);
    }
}