using System.Collections.Generic;

public class GroupParsing
{
    public string Name { get; set; }
    public List<DateParse> dateParses = new();

    public void MergingObjectDate(List<DateParse> dateParses) {
        if(dateParses != null) dateParses.ForEach(this.dateParses.Add);
    }
}
