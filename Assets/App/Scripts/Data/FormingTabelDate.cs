using System.Collections.Generic;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using System;
using System.Linq;
using UnityEngine.Tilemaps;
using System.Threading.Tasks;
using System.Collections;

public class FormingTabelDate : MonoBehaviour
{
    public static FormingTabelDate Instance;
    [SerializeField] TableObjectData tableObjectData;
    private TableTextCell tableTextCell => tableObjectData.tableTextCell;
    private static GroupParsing _lastGroupParsing;
    private static Group _lastGroup;

    public static Dictionary<Vector2Int, bool> MissingStudent;
    private void Awake() {
        Instance = this;
    }
    private void Start() {
        Parsing.PageEvent += EventFormingTable;
    }

    private void EventFormingTable(object sender, EventArgs e) {
        FormingTable(Parsing.ParsingGroupName);
    }

    public void FormingTable(string groupNameForming) {
        GroupParsing groupParsing = Parsing.ParsingData1[groupNameForming];
        _lastGroupParsing = groupParsing;
        
        Debug.Log($"Count Data in group {groupNameForming}: " + groupParsing.dateParses.Count);
        //TableObjectData.FormingTableCell(LoadingDataFireBase.StrudentName[Page].Count, Converter(groupParsing));


        PasteGroupName(groupParsing.Name); // ����������� ������
        string[] groupNameData = RedactSearchText.ConverterGroupNameData(groupParsing.Name);

        if (groupNameData == null) return;

        string yearName = groupNameData[0];
        string specializationName = groupNameData[1];
        string groupName = groupNameData[2];

        bool SpecializationName_ContainsKey = false;
        int FacultyNameID = -1;
        for (int i = 0; i < FireBase.fireBaseData.Faculties.Count; i++)
        {
            FacultyNameID = i;
            Faculty faculty = FireBase.fireBaseData.Faculties[i];
            SpecializationName_ContainsKey = faculty.Specializations.ContainsKey(specializationName);
            if (SpecializationName_ContainsKey)
            {
                break;
            }
        }

        if (!SpecializationName_ContainsKey) { return; }

        bool YearName_ContainsKey = FireBase.fireBaseData.Faculties[FacultyNameID].Specializations[specializationName].Years.ContainsKey(yearName);
        if (!YearName_ContainsKey) { return; }

        bool groupName_ContainsKey = FireBase.fireBaseData.Faculties[FacultyNameID].Specializations[specializationName].Years[yearName].Groups.ContainsKey(groupName);
        if (!groupName_ContainsKey) { return; }

        Group group = FireBase.fireBaseData.Faculties[FacultyNameID].Specializations[specializationName].Years[yearName].Groups[groupName];
        _lastGroup = group;
        if (groupName_ContainsKey)
        {
            if (tableTextCell.TablePersonCell.Count != group.Students.Count)
            {
                tableObjectData.UpdatePersonCell(group.Students.Count);
                Debug.Log("Fail Loading Student Name Data Or Error Table Cell");
            }
            for (int idPersonCell = 0; idPersonCell < tableTextCell.TablePersonCell.Count; idPersonCell++)
            {
                TextMeshProUGUI CellPerson = tableTextCell.TablePersonCell[idPersonCell];
                CellPerson.text = group.Students[idPersonCell].Name;
            }
        }
        else { Debug.LogError("Fail Loading Group"); }

        for (int idCell = 0; idCell < tableTextCell.TableDateCell.Count && idCell < groupParsing.dateParses.Count; idCell++) // ��������� ����
        {
            TextMeshProUGUI CellDate = tableTextCell.TableDateCell[idCell];

            CellDate.text = groupParsing.dateParses[idCell].dateTime;
        }
        List<int> lessonCells = new();
        bool isUpdate = false;
        for (int idDate = 0; idDate < tableTextCell.TableLessonCell.Count && idDate < groupParsing.dateParses.Count; idDate++)
        {
            lessonCells.Add(groupParsing.dateParses[idDate].Lessons.Count);
            if (!isUpdate && (tableTextCell.TableLessonCell[idDate].Count != groupParsing.dateParses[idDate].Lessons.Count)) isUpdate = true;
            
        }

        if (isUpdate)
        {
            tableObjectData.UpdateLessonCell(lessonCells);
        }
        MissingStudent = new();
        for (int idDate = 0, idNColumn = 0; idDate < tableTextCell.TableLessonCell.Count && idDate < groupParsing.dateParses.Count; idDate++) {
            for (int idColumn = 0; idColumn < groupParsing.dateParses[idDate].Lessons.Count; idColumn++,idNColumn++)
            {
                tableTextCell.TableLessonCell[idDate][idColumn].text = groupParsing.dateParses[idDate].Lessons[idColumn];

                // for (int idStudent = 0; idStudent<tableTextCell.TableNCell[idNColumn].Count; idStudent++)
                // {
                //     CellTextN();
                // }
                
                group.Dates.TryGetValue(groupParsing.dateParses[idDate].dateTime, out Dates date);
                if (date != null)
                {
                    List<StudentMissing> studentMissings = date.lessons[idColumn].StudentsMissing;

                    foreach (StudentMissing studentMissing in studentMissings)
                    {
                        Debug.Log(idNColumn + " " + studentMissing.ID + " " + studentMissing.Type);
                        MissingStudent.Add(new(idNColumn,studentMissing.ID), true);
                        CellTextN(tableTextCell.TableNCell[idNColumn][studentMissing.ID],
                            studentMissing.Type ? TypeCellN.valid : TypeCellN.disrespectful);
                    }
                }
                /*else
                {
                    TypeCellN typeCellN = SerchStudentCell(group, groupParsing, idDate, idColumn, idCell);
                    CellTextN(tableTextCell.TableCell[idDate][idColumn][idCell], typeCellN);
                }//����� N*/
            }
            
        }
        
        StartCoroutine(timerReloadingTable());

    }

    public static void CanselMissingStudent()
    {
        foreach (var localSelectedCell in SelectCells.LocalSelectedCells)
        {
            Instance.TimeMissing(localSelectedCell.Key, !localSelectedCell.Value);
        }

        SelectCells.LocalSelectedCells = new();
    }
    public static async Task MergingMissingStudent()
    {
        foreach (var localSelectedCell in SelectCells.LocalSelectedCells)
        {
            if(!localSelectedCell.Value) MissingStudent.Remove(localSelectedCell.Key);
            else MissingStudent.Add(localSelectedCell.Key, localSelectedCell.Value);
            
            FindDateAndLessonAndStudent(localSelectedCell.Key,out string date, out int lessonID, out int studentID);
            await FireBase.UpdateMissingStudents(_lastGroupParsing.Name,date,lessonID,studentID, localSelectedCell.Value);
            
            SelectCells.LocalSelectedCells = new();
        }
        
    }

    private static void FindDateAndLessonAndStudent(Vector2Int position,out string date, out int lessonID, out int studentID)
    {
        date = DateTime.Today.ToString("dd-MM-yyyy");
        lessonID = -1;
        List<DateParse> dateParses = _lastGroupParsing.dateParses;
        for (int idDate = 0,idLessonColum = 0; idDate < dateParses.Count; idDate++)
        {
            List<string> lessons = dateParses[idDate].Lessons;
            for (int idLesson = 0; idLesson < lessons.Count; idLesson++,idLessonColum++)
            {
                if (idLessonColum == position.x)
                {
                    date = dateParses[idDate].dateTime;
                    lessonID = idLesson;
                }
            }
        }

        studentID = _lastGroup.Students[position.y].ID;
    }

    public void TimeMissing(Vector2Int idPersonCell,bool isCreate)
    {
        CellTextN(tableTextCell.TableNCell[idPersonCell.x][idPersonCell.y],
            isCreate ? TypeCellN.yes : TypeCellN.no);
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
        yes,
    }

    private static void CellTextN(TextMeshProUGUI text, TypeCellN typeCell) {
        switch(typeCell)
        {
            case TypeCellN.valid:
                text.color = Color.green;
                text.text = "N";
                break;
            case TypeCellN.disrespectful:
                text.color = Color.red;
                text.text = "N";
                break;
            case TypeCellN.no:
                text.text = "";
                break;
            case TypeCellN.yes:
                text.text = "N";
                break;


        };
    }

    private void PasteGroupName(string Name) {
        tableTextCell.GroupCell.text = Name;
    }

    public IEnumerator timerReloadingTable() {
        yield return new WaitForSeconds(0f);
        tableObjectData.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        tableObjectData.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        tableObjectData.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        tableObjectData.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        tableObjectData.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        tableObjectData.gameObject.SetActive(true);
    }

}
