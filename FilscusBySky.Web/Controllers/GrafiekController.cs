using FilscusBySky.Data;
using FilscusBySky.Models;
using FilscusBySky.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FilscusBySky.Web.Controllers;

[Authorize]
public class GrafiekController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public GrafiekController(AppDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index(int? rekeningId)
    {
        var userId = _userManager.GetUserId(User);

        var alleRekeningen = await _context.Rekeningen
            .Where(r => r.UserId == userId)
            .ToListAsync();

        var transacties = await _context.Transacties
            .Include(t => t.Rekening)
            .Where(t => t.Rekening!.UserId == userId
                && (rekeningId == null || t.RekeningId == rekeningId))
            .ToListAsync();

        var maanden = Enumerable.Range(0, 6)
            .Select(i => DateTime.UtcNow.AddMonths(-i))
            .Reverse()
            .ToList();

        var dezeMaand = DateTime.UtcNow;
        var vorigeMaand = DateTime.UtcNow.AddMonths(-1);

        var viewModel = new GrafiekViewModel
        {
            Rekeningen = alleRekeningen,
            GeselecteerdeRekeningId = rekeningId,

            Maanden = maanden.Select(m => m.ToString("MMM yyyy")).ToList(),

            Inkomsten = maanden.Select(m =>
                transacties
                    .Where(t => t.Type == TransactieType.Inkomen
                        && t.Datum.Month == m.Month
                        && t.Datum.Year == m.Year)
                    .Sum(t => t.Bedrag)).ToList(),

            Uitgaven = maanden.Select(m =>
                transacties
                    .Where(t => t.Type == TransactieType.Uitgave
                        && t.Datum.Month == m.Month
                        && t.Datum.Year == m.Year)
                    .Sum(t => t.Bedrag)).ToList(),

            NettoResultaat = maanden.Select(m =>
            {
                var ink = transacties
                    .Where(t => t.Type == TransactieType.Inkomen
                        && t.Datum.Month == m.Month
                        && t.Datum.Year == m.Year)
                    .Sum(t => t.Bedrag);
                var uit = transacties
                    .Where(t => t.Type == TransactieType.Uitgave
                        && t.Datum.Month == m.Month
                        && t.Datum.Year == m.Year)
                    .Sum(t => t.Bedrag);
                return ink - uit;
            }).ToList(),

            Spaarpercentage = maanden.Select(m =>
            {
                var ink = transacties
                    .Where(t => t.Type == TransactieType.Inkomen
                        && t.Datum.Month == m.Month
                        && t.Datum.Year == m.Year)
                    .Sum(t => t.Bedrag);
                var uit = transacties
                    .Where(t => t.Type == TransactieType.Uitgave
                        && t.Datum.Month == m.Month
                        && t.Datum.Year == m.Year)
                    .Sum(t => t.Bedrag);
                return ink == 0 ? 0 : Math.Round((ink - uit) / ink * 100, 1);
            }).ToList(),

            Categorieen = transacties
                .Where(t => t.Type == TransactieType.Uitgave)
                .GroupBy(t => t.Categorie)
                .Select(g => g.Key)
                .ToList(),

            UitgavenPerCategorie = transacties
                .Where(t => t.Type == TransactieType.Uitgave)
                .GroupBy(t => t.Categorie)
                .Select(g => g.Sum(t => t.Bedrag))
                .ToList(),

            InkomstenDezeMaand = transacties
                .Where(t => t.Type == TransactieType.Inkomen
                    && t.Datum.Month == dezeMaand.Month
                    && t.Datum.Year == dezeMaand.Year)
                .Sum(t => t.Bedrag),

            UitgavenDezeMaand = transacties
                .Where(t => t.Type == TransactieType.Uitgave
                    && t.Datum.Month == dezeMaand.Month
                    && t.Datum.Year == dezeMaand.Year)
                .Sum(t => t.Bedrag),

            InkomstenVorigeMaand = transacties
                .Where(t => t.Type == TransactieType.Inkomen
                    && t.Datum.Month == vorigeMaand.Month
                    && t.Datum.Year == vorigeMaand.Year)
                .Sum(t => t.Bedrag),

            UitgavenVorigeMaand = transacties
                .Where(t => t.Type == TransactieType.Uitgave
                    && t.Datum.Month == vorigeMaand.Month
                    && t.Datum.Year == vorigeMaand.Year)
                .Sum(t => t.Bedrag)
        };

        return View(viewModel);
    }
}