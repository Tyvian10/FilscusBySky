using FilscusBySky.Data;
using FilscusBySky.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FilscusBySky.Web.Controllers.Api;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class RekeningApiController : ControllerBase
{
	private readonly AppDbContext _context;
	private readonly UserManager<ApplicationUser> _userManager;

	public RekeningApiController(AppDbContext context,
								  UserManager<ApplicationUser> userManager)
	{
		_context = context;
		_userManager = userManager;
	}

	// GET: api/RekeningApi
	[HttpGet]
	public async Task<IActionResult> GetRekeningen()
	{
		var userId = _userManager.GetUserId(User);
		var rekeningen = await _context.Rekeningen
			.Where(r => r.UserId == userId)
			.Select(r => new
			{
				r.Id,
				r.Naam,
				r.Saldo
			})
			.ToListAsync();

		return Ok(rekeningen);
	}

	// GET: api/RekeningApi/5/transacties
	[HttpGet("{id}/transacties")]
	public async Task<IActionResult> GetTransacties(int id)
	{
		var userId = _userManager.GetUserId(User);
		var rekening = await _context.Rekeningen
			.FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);

		if (rekening == null)
			return NotFound();

		var transacties = await _context.Transacties
			.Where(t => t.RekeningId == id)
			.OrderByDescending(t => t.Datum)
			.Select(t => new
			{
				t.Id,
				t.Beschrijving,
				t.Bedrag,
				t.Type,
				t.Categorie,
				t.Datum
			})
			.ToListAsync();

		return Ok(transacties);
	}

	// POST: api/RekeningApi/5/transacties
	[HttpPost("{id}/transacties")]
	public async Task<IActionResult> AddTransactie(int id, [FromBody] TransactieDto dto)
	{
		var userId = _userManager.GetUserId(User);
		var rekening = await _context.Rekeningen
			.FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);

		if (rekening == null)
			return NotFound();

		var transactie = new Transactie
		{
			Beschrijving = dto.Beschrijving,
			Bedrag = dto.Bedrag,
			Type = dto.Type,
			Categorie = dto.Categorie,
			RekeningId = id,
			Datum = dto.Datum
		};

		if (dto.Type == TransactieType.Inkomen)
			rekening.Saldo += dto.Bedrag;
		else
			rekening.Saldo -= dto.Bedrag;

		_context.Transacties.Add(transactie);
		await _context.SaveChangesAsync();

		return Ok(transactie);
	}
}

public class TransactieDto
{
	public string Beschrijving { get; set; } = string.Empty;
	public decimal Bedrag { get; set; }
	public TransactieType Type { get; set; }
	public string Categorie { get; set; } = "Overig";
	public DateTime Datum { get; set; } = DateTime.UtcNow;
}