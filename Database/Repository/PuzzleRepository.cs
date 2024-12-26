using ConnectionsAPI.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace ConnectionsAPI.Database.Repository;

public class PuzzleRepository(ConnectionsContext _db)
{
    private readonly ConnectionsContext _db = _db;

    public async Task<ConnectionsPuzzle?> GetConnectionsByDateAsync(string printDate, bool includeSolutions = true)
    {
        // query for the puzzle
        var query = _db.ConnectionsPuzzles.AsNoTracking();

        if (includeSolutions)
        {
            query = query
                .Include(x => x.Categories)
                .ThenInclude(x => x.Cards);
        }

        var puzzle = await query.FirstOrDefaultAsync(x => x.PrintDate == printDate);

        // if not found, we're done here
        if (puzzle == null)
        {
            return null;
        }

        await EnhanceConnectionsWithDatesAsync(puzzle);

        return puzzle;
    }

    public async Task<IEnumerable<ConnectionsPuzzle>> GetAllConnectionsAsync(bool includeSolutions = true)
    {
        // query all, ordered by print date
        var query = _db.ConnectionsPuzzles
            .AsNoTracking();

        if (includeSolutions)
        {
            query = query
                .Include(x => x.Categories)
                .ThenInclude(x => x.Cards);
        }

        var result = (await query.OrderBy(x => x.PrintDate).ToListAsync()) ?? [];

        foreach (var puzzle in result)
        {
            await EnhanceConnectionsWithDatesAsync(puzzle);
        }

        return result;
    }

    private async Task EnhanceConnectionsWithDatesAsync(ConnectionsPuzzle puzzle)
    {
        string? previousPuzzleDate = await _db.ConnectionsPuzzles.AsNoTracking()
            .Where(x => x.PrintDate.CompareTo(puzzle.PrintDate) < 0)
            .OrderByDescending(x => x.PrintDate)
            .Select(x => x.PrintDate)
            .FirstOrDefaultAsync();

        string? nextPuzzleDate = await _db.ConnectionsPuzzles.AsNoTracking()
            .Where(x => x.PrintDate.CompareTo(puzzle.PrintDate) > 0)
            .OrderBy(x => x.PrintDate)
            .Select(x => x.PrintDate)
            .FirstOrDefaultAsync();

        puzzle.PrevPrintDate = previousPuzzleDate;
        puzzle.NextPrintDate = nextPuzzleDate;
    }
}
