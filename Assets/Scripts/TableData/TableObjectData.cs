using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TableObjectData : MonoBehaviour
{
    [SerializeField] private GameObject PersonPrefab;
    [SerializeField] private GameObject LassonNamePrefab;
    [SerializeField] private GameObject NPrefab;
    [SerializeField] private GameObject TimePrefab;

    [SerializeField] private GameObject styleTimePrefab;
    [SerializeField] private GameObject styleLassonNamePrefab;
    [SerializeField] private GameObject styleNPrefab;
    private void Awake() {
        tableColumn = AllChiledObject(gameObject);

        UpdateTMPData();

    }

    private void UpdateTMPData() {

        tableTextCell.GroupCell = ChiledTextObjectGroupName(tableColumn[0]);
        tableTextCell.TablePersonCell = AllChiledTextObjectPerson(tableColumn[1]);
        tableTextCell.TableDateCell = AllChiledTextObjectDate(tableColumn[2]);
        tableTextCell.TableCell = AllChiledTextObjectTable(tableColumn[2]);
    }

    private List<GameObject> tableColumn; //0-‘»ќ 1-¬рем€

    public TableTextCell tableTextCell = new();

    private List<GameObject> AllChiledObject(GameObject parent) {
        List<GameObject> childs = new List<GameObject>
        {
            parent.transform.GetChild(0).gameObject,
            parent.transform.GetChild(1).gameObject,
            parent.transform.GetChild(3).gameObject,
            parent.transform.GetChild(2).gameObject//style
        };
        /*
        foreach (Transform ColumnTime in parent.transform.GetChild(1))
        {
            foreach (Transform Column in ColumnTime) {
                childs.Add(Column.gameObject);
            }
        }*/

        return childs;
    }
    private TextMeshProUGUI ChiledTextObjectGroupName(GameObject parent) {
        return parent.transform.GetChild(0).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
    }
    private List<List<List<TextMeshProUGUI>>> AllChiledTextObjectTable(GameObject parent) {
        List<List<List<TextMeshProUGUI>>> childs = new ();

        foreach (Transform TimesCells in parent.transform)//просматриваем все даты
        {
            List<List<TextMeshProUGUI>> TimeCells = new();
            foreach(Transform LessonsNames in TimesCells) // просматриваем все колонки предметов в эту дату
            {
                List<TextMeshProUGUI> LessonsName = new();
                foreach (Transform LessonsCell in LessonsNames) // просматриваем €чейки в колонке
                {
                    LessonsName.Add(LessonsCell.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>());
                }
                TimeCells.Add(LessonsName);
            }
            childs.Add(TimeCells);
        }
        

        

        return childs;
    }
    private List<TextMeshProUGUI> AllChiledTextObjectPerson(GameObject parent) {
        List<TextMeshProUGUI> childs = new List<TextMeshProUGUI>();

        
            foreach (Transform child in parent.transform)
            {
                childs.Add(child.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>());
            }
        

        return childs;
    }
    private List<TextMeshProUGUI> AllChiledTextObjectDate(GameObject parent) {
        List<TextMeshProUGUI> childs = new List<TextMeshProUGUI>();


        foreach (Transform child in parent.transform)
        {
            childs.Add(child.gameObject.GetComponent<TextMeshProUGUI>());
        }

        return childs;
    }

    public void UpdatePersonCell(int personCount) {
        DestoroyChildObject(tableColumn[1]);
        CreateChildObject(tableColumn[1].transform, PersonPrefab,personCount);


        Transform timePerson = tableColumn[2].transform;
        Transform styleTimePerson = tableColumn[3].transform;

        foreach (Transform time in timePerson.transform)
        {
            foreach (Transform lessonTime in time.transform)
            {
                DestoroyChildObject(lessonTime.gameObject, 1);
            }
        }
        foreach (Transform styleTime in styleTimePerson.transform)
        {
            foreach (Transform styleLessonTime in styleTime.transform)
            {
                DestoroyChildObject(styleLessonTime.gameObject, 1);
            }
        }

        foreach (Transform time in timePerson.transform)
        {
            foreach (Transform lessonTime in time.transform)
            {
                CreateChildObject(lessonTime,NPrefab,personCount);
            }
        }
        foreach (Transform styleTime in styleTimePerson.transform)
        {
            foreach (Transform styleLessonTime in styleTime.transform)
            {
                CreateChildObject(styleLessonTime, styleNPrefab, personCount);
            }
        }

        UpdateTMPData();
    }
    public void UpdateLessonCell(int LessonCount) {
        DestoroyChildObject(tableColumn[1]);
        CreateChildObject(tableColumn[1].transform, PersonPrefab, LessonCount);


        Transform timePerson = tableColumn[2].transform;
        Transform styleTimePerson = tableColumn[3].transform;

        foreach (Transform time in timePerson.transform)
        {
            foreach (Transform lessonTime in time.transform)
            {
                DestoroyChildObject(lessonTime.gameObject, 1);
            }
        }
        foreach (Transform styleTime in styleTimePerson.transform)
        {
            foreach (Transform styleLessonTime in styleTime.transform)
            {
                DestoroyChildObject(styleLessonTime.gameObject, 1);
            }
        }

        foreach (Transform time in timePerson.transform)
        {
            foreach (Transform lessonTime in time.transform)
            {
                CreateChildObject(lessonTime, NPrefab, LessonCount);
            }
        }
        foreach (Transform styleTime in styleTimePerson.transform)
        {
            foreach (Transform styleLessonTime in styleTime.transform)
            {
                CreateChildObject(styleLessonTime, styleNPrefab, LessonCount);
            }
        }

        UpdateTMPData();
    }

    private void DestoroyChildObject(GameObject parent,int indexStart = 0, int indexEnd = int.MaxValue) {
        int personCellCount = parent.transform.childCount;
        if (indexEnd > personCellCount) indexEnd = personCellCount-1;
        for (int i = indexEnd; i >= indexStart; i--)
        {
            if (parent.transform.childCount <= indexStart)
            {
                Debug.LogError(" олличество parentPerson.childCount не совпадает");
                return;
            }
            GameObject chiled = parent.transform.GetChild(i).gameObject;
            chiled.transform.SetParent(null);
            Destroy(chiled);
        }
    }
    private void CreateChildObject(Transform parent, GameObject prefab, int count) {
        int personCellCount = parent.childCount;
        for (int i = 0; i < count; i++)
        {
            Instantiate(prefab, parent);
        }
    }
    public void FormingTableCell(int PersonCount, List<int> DateAndLessonCount) { 
        
        for(int DateCellID = 0; DateCellID< DateAndLessonCount.Count; DateCellID++)
        {
            //создание €чеек ƒаты

            for (int LessonCellID = 0; LessonCellID < DateAndLessonCount[DateCellID];LessonCellID++)
            {
                //создание €чейки зан€ти€

                for (int PersonCellID = 0; PersonCellID < PersonCount; PersonCellID++)
                {
                    // —оздание €чеек с Person

                    // —оздание €чеек N дл€ каждого Person
                }
            }
        }

    }
}
