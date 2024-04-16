namespace ConnectionsAPI.Database.Entities
{
    public class PuzzleCategory
    {
        /// <summary>
        /// Primary key of the entity
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The name of the category in this Connections puzzle
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The color of the category in this Connections puzzle; Also used for sorting
        /// </summary>
        public PuzzleCategoryColor Color { get; set; }

        /// <summary>
        /// The ID of the associated Connections puzzle
        /// </summary>
        public int PuzzleId { get; set; }
        
        /// <summary>
        /// The associated puzzle instance
        /// </summary>
        public virtual Puzzle? Puzzle { get; set; }

        /// <summary>
        /// The cards associated with this category
        /// </summary>
        public ICollection<PuzzleCard> PuzzleCards { get; set; } = [];
    }
}
