using System.Collections.Generic;
using Firebase.Database;

public class GroupParsing
{
    public string Name { get; set; }
    public List<DateParse> DateParses = new();
    public DatabaseReference Reference { get; set; }

    public void MergingObjectDate(List<DateParse> dateParses) {
        if (dateParses != null)
        {
            foreach (DateParse variable in dateParses)
            {
                DateParse temp = this.DateParses.Find(obj=>obj.dateTime==variable.dateTime);
                if (temp != null)
                {
                    temp = variable;
                }
                else
                {
                    this.DateParses.Add(variable);
                }
            }
            
        }
    }
}
