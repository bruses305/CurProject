using System.Collections.Generic;
using System.Linq;

public class GroupParsing
{
    public string Name { get; set; }
    public List<DateParse> dateParses = new();

    public void MergingObjectDate(List<DateParse> dateParses) {
        if (dateParses != null)
        {
            foreach (DateParse variable in dateParses)
            {
                DateParse temp = this.dateParses.Find(obj=>obj.dateTime==variable.dateTime);
                if (temp != null)
                {
                    temp = variable;
                }
                else
                {
                    this.dateParses.Add(variable);
                }
            }
            
        }
    }
}
