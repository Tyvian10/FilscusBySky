using FilscusBySky.Data;
using FilscusBySky.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FilscusBySky.Web.Controllers;

[Authorize]
public class RekeningController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public RekeningController(AppDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // GET: /Rekening
    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);
        var rekeningen = await _context.Rekeningen
            .Where(r => r.UserId == userId)
            .ToListAsync();

        return View(rekeningen);
    }

    // GET: /Rekening/Toevoegen
    public IActionResult Toevoegen() => View();

    // POST: /Rekening/Toevoegen
    [HttpPost]
    public async Task<IActionResult> Toevoegen(string naam, decimal saldo)
    {
        var userId = _userManager.GetUserId(User);

        var rekening = new Rekening
        {
            Naam = naam,
            Saldo = saldo,
            UserId = userId!
        };

        _context.Rekeningen.Add(rekening);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // POST: /Rekening/Verwijderen/5
    [HttpPost]
    public async Task<IActionResult> Verwijderen(int id)
    {
        var userId = _userManager.GetUserId(User);
        var rekening = await _context.Rekeningen
            .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);

        if (rekening != null)
        {
            _context.Rekeningen.Remove(rekening);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }
}