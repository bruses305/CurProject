using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using System;
using UnityEngine.UIElements;
using UnityEngine.EventSystems;

public class TableObjectData : MonoBehaviour
{
    [SerializeField] private GameObject StudentPrefab;
    [SerializeField] private GameObject LassonNamePrefab;
    [SerializeField] private GameObject NPrefab;
    [SerializeField] private GameObject NParentPrefab;
    [SerializeField] private GameObject TimePrefab;

    [SerializeField] private GameObject StudentParent;
    [SerializeField] private GameObject LessonParent;
    [SerializeField] private GameObject NParent;
    [SerializeField] private GameObject TimeParent;
    [SerializeField] private GameObject GroupParent;

    [SerializeField] private SelectCells selectCells;

    public static event EventHandler updateTableData;
    private void Awake() {
        updateTableData += UpdateTableData;
        UpdateTMPData();

    }

    private void UpdateTableData(object sender, EventArgs e) {
        UpdateTMPData();
    }

    private void UpdateTMPData() {
        tableTextCell.GroupCell = ChiledTextObjectGroupName(GroupParent);
        tableTextCell.TablePersonCell = AllChiledTextObjectPerson(StudentParent);
        tableTextCell.TableDateCell = AllChiledTextObjectDate(TimeParent);
        tableTextCell.TableLessonCell = AllChiledTextObjectLesson(LessonParent);
        tableTextCell.TableNCell = AllChiledTextObjectN(NParent);
    }


    public TableTextCell tableTextCell = new();

    
    private TextMeshProUGUI ChiledTextObjectGroupName(GameObject parent) {
        return parent.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();
    }
    private List<List<TextMeshProUGUI>> AllChiledTextObjectLesson(GameObject AllDataParent) {
        List<List<TextMeshProUGUI>> dataTableCells = new();

        foreach (Transform dataParent in AllDataParent.transform)
        {
            List<TextMeshProUGUI> DataCellList = new();
            foreach (Transform DataCell in dataParent.GetChild(0))
            {
                DataCellList.Add(DataCell.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>());
            }
            dataTableCells.Add(DataCellList);
        }

        return dataTableCells;
    }
    private List<List<TextMeshProUGUI>> AllChiledTextObjectN(GameObject AllDataParent) {
        List<List<TextMeshProUGUI>> dataTableCells = new();

        foreach (Transform dataParent in AllDataParent.transform)
        {
            List<TextMeshProUGUI> DataCellList = new();
            foreach (Transform DataCell in dataParent)
            {
                DataCellList.Add(DataCell.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>());
            }
            dataTableCells.Add(DataCellList);
        }

        return dataTableCells;
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
            childs.Add(child.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>());
        }

        return childs;
    }
    public void UpdatePersonCell(int personCount) {
        CRCChildObject(StudentParent.transform, StudentPrefab, personCount);
        Transform NParent = this.NParent.transform;

        for (int i = 0; i< NParent.transform.childCount;i++)
        {
            CRCChildObject(NParent.transform.GetChild(i), NPrefab, i, personCount);
        }

        updateTableData.Invoke(this, EventArgs.Empty);
    }
    public void UpdateLessonCell(List<int> DatesLesson) {
        int studentCount = tableTextCell.TablePersonCell.Count;
        int countLesson = 0;
        ClearDate(DatesLesson.Count);
        for (int idDate = 0;idDate < DatesLesson.Count; countLesson += DatesLesson[idDate], idDate++)
        {
            Transform parentLessons = tableTextCell.TableDateCell[idDate].transform;
            CRCChildObject(parentLessons, LassonNamePrefab, DatesLesson[idDate]);
        }
        CRCChildObject(NParent.transform, NParentPrefab, countLesson);
        for (int i = 0; i< NParent.transform.childCount;i++)
        {
            CRCChildObject(NParent.transform.GetChild(i), NPrefab, i, studentCount);
        }

        updateTableData.Invoke(this, EventArgs.Empty);
    }

    private void ClearDate(int countLesson)
    {
        for (int idCell = countLesson; idCell < tableTextCell.TableDateCell.Count; idCell++) // ��������� ����
        {
            TextMeshProUGUI dateCell = tableTextCell.TableDateCell[idCell];
            dateCell.text = "";
            DestoroyChildObject(dateCell.gameObject);
            
        }
    }

    private void CRCChildObject(Transform parent, GameObject prefab, int count = 1) {
        if (count < parent.childCount)
        {
            DestoroyChildObject(parent.gameObject, count);
            ClearChildObject(parent.gameObject);
        }
        else
        {
            ClearChildObject(parent.gameObject);
            CreateChildObject(parent, prefab, count - parent.childCount);
        }
    }
    private void CRCChildObject(Transform parent, GameObject prefab, int positionColum, int count = 1) {
        if (count < parent.childCount)
        {
            DestoroyChildObject(parent.gameObject, count);
            ClearChildObject(parent.gameObject);
        }
        else
        {
            ClearChildObject(parent.gameObject);
            CreateChildObject(parent, prefab, positionColum, count - parent.childCount);
        }
    }

    public static void DestoroyChildObject(GameObject parent,int indexStart = 0, int indexEnd = int.MaxValue) {
        int personCellCount = parent.transform.childCount;
        if (indexEnd > personCellCount) indexEnd = personCellCount-1;
        for (int i = indexEnd; i >= indexStart; i--)
        {
            if (parent.transform.childCount <= indexStart)
            {
                Debug.LogError("����������� parentPerson.childCount �� ���������");
                return;
            }
            GameObject chiled = parent.transform.GetChild(i).gameObject;
            chiled.transform.SetParent(null);
            Destroy(chiled);
        }
    }

    private void ClearChildObject(GameObject parent, int indexStart = 0, int indexEnd = int.MaxValue) {
        int personCellCount = parent.transform.childCount;
        if (indexEnd > personCellCount) indexEnd = personCellCount - 1;
        for (int i = indexEnd; i >= indexStart; i--)
        {
            if (parent.transform.childCount <= indexStart)
            {
                Debug.LogError("����������� parentPerson.childCount �� ���������");
                return;
            }
            GameObject TMPObject = parent.transform.GetChild(i).GetChild(0).gameObject;
            TMPObject.TryGetComponent(out TextMeshProUGUI TMP);
            if(TMP!=null)
                TMP.text = "";
        }
    }

    private void CreateChildObject(Transform parent, GameObject prefab, int count = 1){
        for (int i = 0; i < count; i++)
        {
            Instantiate(prefab, parent);
        }
        Debug.Log(parent.childCount);
    }
    private void CreateChildObject(Transform parent, GameObject prefab, int positionColum, int count = 1) {
        for (int i = 0; i < count; i++)
        {
            GameObject gameObject = Instantiate(prefab, parent);
            EventTrigger eventTrigger = gameObject.GetComponent<EventTrigger>();
            int y = i;
            EventTrigger.Entry entrySelect = new EventTrigger.Entry();
            {
                entrySelect.eventID = EventTriggerType.PointerEnter;
            }
            entrySelect.callback.AddListener(data  => selectCells.SelectNCells((PointerEventData)data, new(positionColum, y)));

            EventTrigger.Entry entryUnSelect = new EventTrigger.Entry();
            {
                entryUnSelect.eventID = EventTriggerType.PointerExit;
            }
            entryUnSelect.callback.AddListener(data => selectCells.UnSelectNCells((PointerEventData)data, new(positionColum, y)));

            EventTrigger.Entry entryPinned = new EventTrigger.Entry();
            {
                entryPinned.eventID = EventTriggerType.PointerClick;
            }
            entryPinned.callback.AddListener(data => selectCells.isPinned((PointerEventData)data, new(positionColum, y)));

            eventTrigger.triggers.Add(entrySelect);
            eventTrigger.triggers.Add(entryUnSelect);
            eventTrigger.triggers.Add(entryPinned);
        }
    }

    public void FormingTableCell(int PersonCount, List<int> DateAndLessonCount) { 
        
        for(int DateCellID = 0; DateCellID< DateAndLessonCount.Count; DateCellID++)
        {
            //�������� ����� ����

            for (int LessonCellID = 0; LessonCellID < DateAndLessonCount[DateCellID];LessonCellID++)
            {
                //�������� ������ �������

                for (int PersonCellID = 0; PersonCellID < PersonCount; PersonCellID++)
                {
                    // �������� ����� � Person

                    // �������� ����� N ��� ������� Person
                }
            }
        }

    }
}
