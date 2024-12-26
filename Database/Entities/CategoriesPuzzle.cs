using System.ComponentModel.DataAnnotations.Schema;

namespace ConnectionsAPI.Database.Entities
{
    public class CategoriesPuzzle
    {
        /// <summary>
        /// Primary key of the entity
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// When the entity was created (is the sync date)
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// When the puzzle was "printed" online
        /// </summary>
        public string PrintDate { get; set; } = string.Empty;

        /// <summary>
        /// The name of the editor for the puzzle
        /// </summary>
        public string EditorName { get; set; } = string.Empty;

        /// <summary>
        /// The actual count of the puzzle
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// The MD5 hash for the source content used to sync this puzzle
        /// </summary>
        public string ContentMD5 { get; set; } = string.Empty;

        /// <summary>
        /// The categories associated with this puzzle
        /// </summary>
        public virtual ICollection<CategoriesCategory> Categories { get; set; } = [];

        [NotMapped]
        public string? PrevPrintDate { get; set; }
        [NotMapped]
        public string? NextPrintDate { get; set; }
    }
}
