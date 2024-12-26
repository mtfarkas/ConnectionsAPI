namespace ConnectionsAPI.Database.Entities;

public class ConnectionsCard
{
    /// <summary>
    /// Primary key of the entity
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The contents of this card (the word)
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// The initial position of this card on the grid
    /// </summary>
    public int Position { get; set; }

    /// <summary>
    /// The ID of the associated Connections category
    /// </summary>
    public int ConnectionsCategoryId { get; set; }

    /// <summary>
    /// The associated category instance
    /// </summary>
    public virtual ConnectionsCategory? Category { get; set; }
}
