using FilscusBySky.Data;
using FilscusBySky.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FilscusBySky.Web.Controllers;

[Authorize]
public class DomiciliatieController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public DomiciliatieController(AppDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // GET: /Domiciliatie
    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);
        var domiciliaties = await _context.Domiciliaties
            .Include(d => d.Rekening)
            .Where(d => d.Rekening!.UserId == userId)
            .ToListAsync();

        return View(domiciliaties);
    }

    // GET: /Domiciliatie/Toevoegen
    public async Task<IActionResult> Toevoegen()
    {
        var userId = _userManager.GetUserId(User);
        var rekeningen = await _context.Rekeningen
            .Where(r => r.UserId == userId)
            .ToListAsync();

        ViewBag.Rekeningen = rekeningen;
        return View();
    }

    // POST: /Domiciliatie/Toevoegen
    [HttpPost]
    public async Task<IActionResult> Toevoegen(string naam, decimal bedrag,
                                               string categorie, int dag, int rekeningId)
    {
        var userId = _userManager.GetUserId(User);
        var rekening = await _context.Rekeningen
            .FirstOrDefaultAsync(r => r.Id == rekeningId && r.UserId == userId);

        if (rekening == null)
            return RedirectToAction(nameof(Index));

        var domiciliatie = new Domiciliatie
        {
            Naam = naam,
            Bedrag = bedrag,
            Categorie = categorie,
            Dag = dag,
            IsActief = true,
            RekeningId = rekeningId
        };

        _context.Domiciliaties.Add(domiciliatie);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // POST: /Domiciliatie/ToggleActief
    [HttpPost]
    public async Task<IActionResult> ToggleActief(int id)
    {
        var userId = _userManager.GetUserId(User);
        var domiciliatie = await _context.Domiciliaties
            .Include(d => d.Rekening)
            .FirstOrDefaultAsync(d => d.Id == id && d.Rekening!.UserId == userId);

        if (domiciliatie != null)
        {
            domiciliatie.IsActief = !domiciliatie.IsActief;
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    // POST: /Domiciliatie/Verwijderen
    [HttpPost]
    public async Task<IActionResult> Verwijderen(int id)
    {
        var userId = _userManager.GetUserId(User);
        var domiciliatie = await _context.Domiciliaties
            .Include(d => d.Rekening)
            .FirstOrDefaultAsync(d => d.Id == id && d.Rekening!.UserId == userId);

        if (domiciliatie != null)
        {
            _context.Domiciliaties.Remove(domiciliatie);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }
}