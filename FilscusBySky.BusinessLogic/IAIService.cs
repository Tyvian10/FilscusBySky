using FilscusBySky.Models;

namespace FilscusBySky.BusinessLogic;

public interface IAIService
{
    Task<string> GenereerAdviesAsync(Rekening rekening, List<Transactie> transacties);
}