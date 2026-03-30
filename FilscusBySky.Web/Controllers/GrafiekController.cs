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

    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);

        var transacties = await _context.Transacties
            .Include(t => t.Rekening)
            .Where(t => t.Rekening!.UserId == userId)
            .ToListAsync();

        // Laatste 6 maanden
        var maanden = Enumerable.Range(0, 6)
            .Select(i => DateTime.UtcNow.AddMonths(-i))
            .Reverse()
            .ToList();

        var viewModel = new GrafiekViewModel
        {
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

            Categorieën = transacties
                .Where(t => t.Type == TransactieType.Uitgave)
                .GroupBy(t => t.Categorie)
                .Select(g => g.Key)
                .ToList(),

            UitgavenPerCategorie = transacties
                .Where(t => t.Type == TransactieType.Uitgave)
                .GroupBy(t => t.Categorie)
                .Select(g => g.Sum(t => t.Bedrag))
                .ToList()
        };

        return View(viewModel);
    }
}