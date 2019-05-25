using SQLite4Unity3d;

public class Record  {

    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Login { get; set; }
    public int WinsBot { get; set; }

    public override string ToString ()
    {
        return $"[Person: Id={Id}, Login={Login},  WinsBot={WinsBot}]";
    }
}