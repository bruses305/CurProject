using System.Collections.Generic;

public class Year : IFireBaseData
{
    public const string key = "Years";
    public string Key { get => key; }
    public const string Key_NAME = "Name";
    public string Name { get; set; }

    public Dictionary<string, Group> Groups = new();
}
