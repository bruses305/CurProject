using System.Collections.Generic;

public class Group : IFireBaseData
{
    public const string key = "Groups";
    public string Key => key;
    public const string Key_NAME = "Name";
    public string Name { get; set; }

    public Dictionary<string,Dates> Dates = new();
    public List<Student> Students = new();
}
