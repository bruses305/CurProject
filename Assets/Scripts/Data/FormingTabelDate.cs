using System.Collections.Generic;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using System;
using System.Linq;

public class FormingTabelDate : MonoBehaviour
{
    [SerializeField] Parsing parsing;
    [SerializeField] TableObjectData TableObjectData;

    private TableTextCell tableTextCell { get { return TableObjectData.tableTextCell; }}
    private void Start() {
        parsing.PageEvent += EventFormingTable;
    }

    private void EventFormingTable(object sender, EventArgs e) {
        FormingTable();
    }

    public void FormingTable(int Page = -1) {
        if (Page <0) Page = Parsing.Page;
        Debug.Log("Page Number: " + Page);
        GroupParsing groupParsing;
        if (Page < 0) groupParsing = parsing.ParsingData2;
        else groupParsing = parsing.ParsingData1[Page];


        //TableObjectData.FormingTableCell(LoadingDataFireBase.StrudentName[Page].Count, Converter(groupParsing));
        

        tableTextCell.GroupCell.text = groupParsing.Name; // присваиваем группу
        string[] groupNameData = ConverterGroupNameData(groupParsing.Name);

        if (groupNameData == null) return;

        int yearName = Convert.ToInt32(groupNameData[0]);
        string specializationName = groupNameData[1];
        string groupName = groupNameData[2];
        bool SpecializationName_ContainsKey = false;
        int FacultyName = -1;
        for (int i = 0; i< FireBase.fireBaseData.Faculties.Count;i++)
        {
            FacultyName = i;
            Faculty faculty = FireBase.fireBaseData.Faculties[i];
            SpecializationName_ContainsKey = faculty.Specializations.ContainsKey(specializationName);
            if (SpecializationName_ContainsKey)
            {
                break;
            }
        }
        if (!SpecializationName_ContainsKey) { return; }

        bool YearName_ContainsKey = FireBase.fireBaseData.Faculties[FacultyName].Specializations[specializationName].Years.ContainsKey(yearName.ToString());
        if (!YearName_ContainsKey) { return; }

        bool groupName_ContainsKey = FireBase.fireBaseData.Faculties[FacultyName].Specializations[specializationName].Years[yearName.ToString()].Groups.ContainsKey(groupName);
        if (!groupName_ContainsKey) {return; }

        Group group = FireBase.fireBaseData.Faculties[FacultyName].Specializations[specializationName].Years[yearName.ToString()].Groups[groupName];
        Debug.Log(group.Students.Count);
        if (groupName_ContainsKey)
        {
            if (tableTextCell.TablePersonCell.Count != group.Students.Count)
            {
                TableObjectData.UpdatePersonCell(group.Students.Count);
                Debug.Log("Fail Loading Student Name Data Or Error Table Cell");
            }
            for (int idPersonCell = 0; idPersonCell < tableTextCell.TablePersonCell.Count; idPersonCell++)
            {
                TextMeshProUGUI CellPerson = tableTextCell.TablePersonCell[idPersonCell];
                CellPerson.text = group.Students[idPersonCell].Name;
            }
        }
        else { Debug.LogError("Fail Loading Group"); }


        for (int idCell = 0; idCell< tableTextCell.TableDateCell.Count && idCell < groupParsing.dateParses.Count; idCell++) // добавляем даты
        {
            TextMeshProUGUI CellDate = tableTextCell.TableDateCell[idCell];

            CellDate.text = groupParsing.dateParses[idCell].dateTime;
        }

        for (int idDate = 0; idDate < tableTextCell.TableCell.Count && idDate < groupParsing.dateParses.Count; idDate++) {
            for (int idColumn = 0; idColumn < tableTextCell.TableCell[idDate].Count && idColumn < groupParsing.dateParses[idDate].Lessons.Count; idColumn++)
            {
                for (int idCell = 0; idCell < tableTextCell.TableCell[idDate][idColumn].Count/* && колличество строк в таблицах данных N + 1*/; idCell++)
                {
                    if (idCell == 0) // Вывод названий дисциплин
                    {
                        tableTextCell.TableCell[idDate][idColumn][0].text = groupParsing.dateParses[idDate].Lessons[idColumn];
                        break;
                    }
                    else
                    {
                        TypeCellN typeCellN = SerchStudentCell(group, groupParsing, idDate, idColumn, idCell);
                        CellTextN(tableTextCell.TableCell[idDate][idColumn][idCell], typeCellN);
                    }//вывод N
                }
            }
        }
    }

    public static string[] ConverterGroupNameData(string GroupName) { //0-год 1-специальность 2-группа
        try
        {
            if (GroupName == null) return new string[3] { null, null, null };
            int yearName = Convert.ToInt32(GroupName.Substring(0, 2));
            int index = GroupName.IndexOf('-');
            string specializationName = GroupName.Substring(2, index - 2);
            string groupName = GroupName.Substring(index + 1);

            if (yearName == 0 || specializationName == null || groupName == null) return null;

            return new string[3] { yearName.ToString(),specializationName,groupName};
        }
        catch {
            return null;
        }
    }
    private TypeCellN SerchStudentCell(Group group, GroupParsing groupParsing, int idDate, int idColumn, int idCell) {
        string date = groupParsing.dateParses[idDate].dateTime;
        group.Dates.TryGetValue(date, out Dates dates);
        string lesson = groupParsing.dateParses[idDate].Lessons[idColumn];
        StudentMissing student = dates?.lessons[idColumn].StudentMissings.FirstOrDefault(student => student.ID == idCell);
        if (student == default(StudentMissing))
            return TypeCellN.no;
        else if (student.Type == true)
            return TypeCellN.valid;
        else if (student.Type == false)
            return TypeCellN.disrespectful;
        else
            return TypeCellN.no;
    }
    private List<int> Converter(GroupParsing groupParsing) {

        List<DateParse> dateParses = groupParsing.dateParses;

        List<int> ConvertingData = new();

        foreach (DateParse dateParse in dateParses)
        {
            ConvertingData.Add(dateParse.Lessons.Count);
        }

        return ConvertingData;
    }

    private enum TypeCellN {
        disrespectful,
        valid,
        no,
    }

    private void CellTextN(TextMeshProUGUI text, TypeCellN typeCell) {
        switch(typeCell)
        {
            case TypeCellN.valid:
                text.color = Color.yellow;
                text.text = "N";
                break;
            case TypeCellN.disrespectful:
                text.color = Color.red;
                text.text = "N";
                break;
            case TypeCellN.no:
                text.color = Color.white;
                text.text = "";
                break;


        };
    }
}
