public interface IFireBaseData
{
    public string Key { get;}
    public string Name { get; set; }

    public void SetName(string name) {
        this.Name = name;
    }
}
