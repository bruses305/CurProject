using System;
using System.Collections.Generic;
using System.Linq;
using DocumentFormat.OpenXml.Drawing.Charts;

public static class RedactSearchText
{

    private static Dictionary<char, char> LIBRARY_KEY = new()
    {
        { 'Q','Й'},
        { 'W','Ц'},
        { 'E','У'},
        { 'R','К'},
        { 'T','Е'},
        { 'Y','Н'},
        { 'U','Г'},
        { 'I','Ш'},
        { 'O','Щ'},
        { 'P','З'},
        { 'A','Ф'},
        { 'S','Ы'},
        { 'D','В'},
        { 'F','А'},
        { 'G','П'},
        { 'H','Р'},
        { 'J','О'},
        { 'K','Л'},
        { 'L','Д'},
        { 'Z','Я'},
        { 'X','Ч'},
        { 'C','С'},
        { 'V','М'},
        { 'B','И'},
        { 'N','Т'},
        { 'M','Ь'},
    };

    public static string UpperText(string groupName) {
        groupName = string.Concat(groupName.Select(
            c => {
                if (LIBRARY_KEY.TryGetValue(char.ToUpper(c), out char ch)) return ch;
                else return c;
                }
            ));
        groupName = groupName.ToUpper();
        return groupName;
    }

    public static string[] ConverterGroupNameData(string groupName) { //0-год 1-специальность 2-группа
        try
        {
            if (groupName == null) return new string[3] { null, null, null };
            UpperText(groupName);
            int yearName = Convert.ToInt32(groupName.Substring(0, 2));
            int index = groupName.IndexOf('-');
            string specializationName;
            string groupNumber = null;
            if (index >= 2)
            {
                specializationName = groupName.Substring(2, index - 2);
                groupNumber = groupName.Substring(index + 1);
            }
            else
            {
                specializationName = groupName.Substring(2);
            }
            return new string[3] { yearName.ToString(), specializationName, groupNumber };
        }
        catch
        {
            return new string[3] { null, null, null };
        }
    }
}
