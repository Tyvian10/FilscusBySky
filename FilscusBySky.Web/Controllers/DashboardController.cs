using FilscusBySky.Data;
using FilscusBySky.Models;
using FilscusBySky.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FilscusBySky.Web.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public DashboardController(AppDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);
        var user = await _userManager.GetUserAsync(User);

        var rekeningen = await _context.Rekeningen
            .Where(r => r.UserId == userId)
            .Include(r => r.Transacties)
            .ToListAsync();

        var alleTransacties = rekeningen.SelectMany(r => r.Transacties).ToList();

        var viewModel = new DashboardViewModel
        {
            Rekeningen = rekeningen,
            TotaalSaldo = rekeningen.Sum(r => r.Saldo),
            TotaalInkomen = alleTransacties
                .Where(t => t.Type == TransactieType.Inkomen)
                .Sum(t => t.Bedrag),
            TotaalUitgaven = alleTransacties
                .Where(t => t.Type == TransactieType.Uitgave)
                .Sum(t => t.Bedrag),
            GebruikersNaam = user?.VolledigeNaam ?? "Gebruiker"
        };

        return View(viewModel);
    }
}