using SQLite4Unity3d;

/// <summary>
/// Stores user statistics.
/// Stored in database.
/// </summary>
public class Record  {

    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public int GamesWithBot { get; set; }
    public int WinsBot { get; set; }
    public int GamesNetwork { get; set; }
    public int WinsNetwork { get; set; }
}