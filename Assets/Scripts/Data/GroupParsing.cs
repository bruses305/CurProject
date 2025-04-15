using System.Collections.Generic;

public class GroupParsing
{
    public string Name { get; set; }
    public List<DateParse> dateParses = new();

    public void MergingObjectDate(List<DateParse> dateParses) {
        dateParses.ForEach(this.dateParses.Add);
    }
}
