using SQLite4Unity3d;

public class Record  {

    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Login { get; set; }
    public int WinsBot { get; set; }

    public override string ToString ()
    {
        return string.Format ("[Person: Id={0}, Login={1},  WinsBot={2}]", Id, Login, WinsBot);
    }
}