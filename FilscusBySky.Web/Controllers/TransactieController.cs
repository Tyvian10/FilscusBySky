using FilscusBySky.Data;
using FilscusBySky.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FilscusBySky.Web.Controllers;

[Authorize]
public class TransactieController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public TransactieController(AppDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // GET: /Transactie/Index/3
    public async Task<IActionResult> Index(int rekeningId)
    {
        var userId = _userManager.GetUserId(User);

        var rekening = await _context.Rekeningen
            .FirstOrDefaultAsync(r => r.Id == rekeningId && r.UserId == userId);

        if (rekening == null)
            return RedirectToAction("Index", "Rekening");

        var transacties = await _context.Transacties
            .Where(t => t.RekeningId == rekeningId)
            .OrderByDescending(t => t.Datum)
            .ToListAsync();

        ViewBag.Rekening = rekening;
        return View(transacties);
    }

    // GET: /Transactie/Toevoegen/3
    public async Task<IActionResult> Toevoegen(int rekeningId)
    {
        var userId = _userManager.GetUserId(User);
        var rekening = await _context.Rekeningen
            .FirstOrDefaultAsync(r => r.Id == rekeningId && r.UserId == userId);

        if (rekening == null)
            return RedirectToAction("Index", "Rekening");

        ViewBag.RekeningId = rekeningId;
        ViewBag.RekeningNaam = rekening.Naam;
        return View();
    }

    // POST: /Transactie/Toevoegen
    [HttpPost]
    public async Task<IActionResult> Toevoegen(int rekeningId, string beschrijving,
                                            decimal bedrag, TransactieType type,
                                            string categorie, DateTime datum)
    {
        var userId = _userManager.GetUserId(User);
        var rekening = await _context.Rekeningen
            .FirstOrDefaultAsync(r => r.Id == rekeningId && r.UserId == userId);

        if (rekening == null)
            return RedirectToAction("Index", "Rekening");

        var transactie = new Transactie
        {
            Beschrijving = beschrijving,
            Bedrag = bedrag,
            Type = type,
            Categorie = categorie,
            RekeningId = rekeningId,
            Datum = datum
        };

        if (type == TransactieType.Inkomen)
            rekening.Saldo += bedrag;
        else
            rekening.Saldo -= bedrag;

        _context.Transacties.Add(transactie);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index), new { rekeningId });
    }

    // POST: /Transactie/Verwijderen
    [HttpPost]
    public async Task<IActionResult> Verwijderen(int id, int rekeningId)
    {
        var userId = _userManager.GetUserId(User);
        var transactie = await _context.Transacties
            .Include(t => t.Rekening)
            .FirstOrDefaultAsync(t => t.Id == id && t.Rekening!.UserId == userId);

        if (transactie != null)
        {
            // Saldo terugzetten
            if (transactie.Type == TransactieType.Inkomen)
                transactie.Rekening!.Saldo -= transactie.Bedrag;
            else
                transactie.Rekening!.Saldo += transactie.Bedrag;

            _context.Transacties.Remove(transactie);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index), new { rekeningId });
    }
}