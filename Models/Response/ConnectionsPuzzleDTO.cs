namespace ConnectionsAPI.Models.Response;
public class ConnectionsPuzzleDTO
{
    public static ConnectionsPuzzleDTO FromEntity(Database.Entities.ConnectionsPuzzle dbPuzzle) =>
        new()
        {
            PuzzleNumber = dbPuzzle.Index,
            PrintDate = dbPuzzle.PrintDate,
            Editor = dbPuzzle.EditorName,

            NextPuzzle = dbPuzzle.NextPrintDate ?? string.Empty,
            PreviousPuzzle = dbPuzzle.PrevPrintDate ?? string.Empty,

            Categories = dbPuzzle.Categories.OrderBy(x => (int)x.Color).Select(ConnectionsCategoryDTO.FromEntity).ToList(),
        };

    public int PuzzleNumber { get; set; }
    public string PrintDate { get; set; } = string.Empty;
    public string PreviousPuzzle { get; set; } = string.Empty;
    public string NextPuzzle { get; set; } = string.Empty;
    public string Editor { get; set; } = string.Empty;
    public ICollection<ConnectionsCategoryDTO> Categories { get; set; } = [];
}

public class ConnectionsCategoryDTO
{
    public static ConnectionsCategoryDTO FromEntity(Database.Entities.ConnectionsCategory dbCategory) =>
        new()
        {
            Title = dbCategory.Name,
            Cards = dbCategory.Cards.OrderBy(x => x.Content).Select(ConnectionsCardDTO.FromEntity).ToList(),
            Color = dbCategory.Color.ToString().ToLower(),
            OrderingKey = (int)dbCategory.Color
        };

    public string Title { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public int OrderingKey { get; set; }
    public ICollection<ConnectionsCardDTO> Cards { get; set; } = [];
}

public class ConnectionsCardDTO
{
    public static ConnectionsCardDTO FromEntity(Database.Entities.ConnectionsCard dbCard) =>
        new()
        {
            Content = dbCard.Content,
            Position = dbCard.Position,
        };

    public string Content { get; set; } = string.Empty;
    public int Position { get; set; }
}
