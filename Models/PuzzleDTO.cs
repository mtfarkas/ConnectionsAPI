namespace ConnectionsAPI.Models
{
    public class PuzzleDTO
    {
        public static PuzzleDTO FromEntity(Database.Entities.Puzzle dbPuzzle) =>
            new()
            {
                PuzzleNumber = dbPuzzle.Index,
                PrintDate = dbPuzzle.PrintDate,
                Editor = dbPuzzle.EditorName,
                Categories = dbPuzzle.Categories.OrderBy(x => (int)x.Color).Select(PuzzleCategoryDTO.FromEntity).ToList()
            };

        public int PuzzleNumber { get; set; }
        public string PrintDate { get; set; } = string.Empty;
        public string Editor { get; set;} = string.Empty;
        public ICollection<PuzzleCategoryDTO> Categories { get; set; } = [];
    }

    public class PuzzleCategoryDTO
    {
        public static PuzzleCategoryDTO FromEntity(Database.Entities.PuzzleCategory dbCategory) =>
            new()
            {
                Title = dbCategory.Name,
                Cards = dbCategory.PuzzleCards.OrderBy(x => x.Content).Select(PuzzleCardDTO.FromEntity).ToList(),
                Color = dbCategory.Color.ToString().ToLower(),
                OrderingKey = (int)dbCategory.Color
            };

        public string Title { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public int OrderingKey { get; set; }
        public ICollection<PuzzleCardDTO> Cards { get; set; } = [];
    }

    public class PuzzleCardDTO
    {
        public static PuzzleCardDTO FromEntity(Database.Entities.PuzzleCard dbCard) =>
            new()
            {
                Content = dbCard.Content,
                Position = dbCard.Position,
            };

        public string Content { get; set; } = string.Empty;
        public int Position { get; set; }
    }
}
