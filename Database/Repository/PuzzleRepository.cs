using ConnectionsAPI.Database.Entities;
using ConnectionsAPI.Models.Response;
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

    public async Task<PagedDataResponse<ConnectionsPuzzle>> QueryConnectionsPuzzles(int page,
                                                                                    int pageCount,
                                                                                    int? year,
                                                                                    int? month,
                                                                                    bool includeSolutions = true)
    {
        var query = _db.ConnectionsPuzzles
            .AsNoTracking();

        if (year != null)
        {
            string filterStr = $"{year}-%";
            if (month != null)
            {
                filterStr = $"{year}-{month.ToString()!.PadLeft(2, '0')}-%";
            }

            query = query.Where(x => EF.Functions.Like(x.PrintDate, filterStr));
        }

        if (includeSolutions)
        {
            query = query
                .Include(x => x.Categories)
                .ThenInclude(x => x.Cards);
        }

        query = query
            .OrderBy(x => x.PrintDate)
            .Skip((page - 1) * pageCount)
            .Take(pageCount);

        var puzzles = await query.ToListAsync();

        if (puzzles.Count > 0)
        {
            foreach (var puzzle in puzzles)
            {
                await EnhanceConnectionsWithDatesAsync(puzzle);
            }
        }

        int totalCount = await _db.ConnectionsPuzzles.AsNoTracking().CountAsync();

        return new PagedDataResponse<ConnectionsPuzzle>(page, puzzles.Count, totalCount, puzzles);
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
