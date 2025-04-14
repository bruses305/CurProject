using UnityEngine;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System;
using System.Threading.Tasks;
using System.Collections;
using Unity.VisualScripting;
using System.Threading;
using System.Reflection;
using Unity.VisualScripting.Antlr3.Runtime;

public class Parsing : MonoBehaviour
{
    public FireBase fireBase = new();
    public event EventHandler PageEvent;
    public static int Page = -1;
    public List<GroupParsing> ParsingData1 { get; set; }
    public GroupParsing ParsingData2 { get; set; }


    private const int FAIL_LIMIT = 5;
    private const string POLESSU_URL_TYPE_1 = "https://www.polessu.by/ruz/term2ng/students.html";
    private const string POLESSU_URL_TYPE_2 = "https://www.polessu.by/ruz/ng/?";
    private const string POLESSU_FILE1 = ".//table[@id]";
    private const string POLESSU_FILE2 = ".//div[@class='container']";
    private const string GroupNamePlayerPrefs = "GROUP_NAME";
    public static string parsingGroupName = "22ИТ-3";
    private DateTime dateTime_Now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
    private DateTime dateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);


    private async void Awake() {
        if (PlayerPrefs.HasKey(GroupNamePlayerPrefs))
            parsingGroupName = PlayerPrefs.GetString(GroupNamePlayerPrefs);
        else
            PlayerPrefs.SetString(GroupNamePlayerPrefs, parsingGroupName);
        fireBase.Initialized();

        await LoadingAllData();
    }
    private void Start() {
        fireBase.ParsingFireBaseEnd += ParsingWebSite;
    }

    private void ParsingWebSite(object sender, EventArgs e) {
        Debug.Log("All Parsing Web Site");
        AllParsing();
    }
    private async Task<T> TimeTrigger<T>(Task<T> task, float timeOut = 10f) {
        CancellationTokenSource cancelTokenSource = new CancellationTokenSource();

        var fieldInfo = typeof(Task<T>).GetField("m_action", BindingFlags.NonPublic | BindingFlags.Instance);
        var value = fieldInfo.GetValue(task);


        Coroutine corotine = StartCoroutine(ResourceTickOver(cancelTokenSource, task.ToString(), timeOut));
        T Data = default(T);

        try
        {
            Data = await Task.Run(() => task, cancelTokenSource.Token);
        }
        catch
        {
            Debug.Log("Error Connecting");
        }
        
        StopCoroutine(corotine);
        Debug.Log(Data);
        return Data;
    }
    public async void ButtonEventLoadingAllData() {
        await LoadingAllData();
    }
    private async Task LoadingAllData() {
        int countParsingFail = 0;
        do
        {
            Debug.Log("StartParsing");

            ParsingData2 = await TimeTrigger(ParsingMetod2(parsingGroupName));
            if (ParsingData2 != default(GroupParsing))
            {
                bool FB = await TimeTrigger(fireBase.LoadingData());
                if (FB)
                {
                    Debug.Log("DataLoading");
                    PageEvent.Invoke(this, EventArgs.Empty);
                    await Task.Run(fireBase.LoadingAllData);
                    //ивент на закрытие загрузочного экрана
                    return;
                }
            }
            countParsingFail++;
        }
        while (countParsingFail < FAIL_LIMIT);

        // сделать чтобы пыталось повторно подключиться к fireBase
        Debug.LogError("Error Loading Data");

    }
    public void LastPage() {
        if (Page > 0)
        {
            Page--;
            PageEvent.Invoke(this, EventArgs.Empty);
        }
    }
    public void NextPage() {
        if (Page >= 0 && Page < ParsingData1.Count-1)
        {
            Page++;
            PageEvent.Invoke(this, EventArgs.Empty);
        }
        else if(Page == -1 && ParsingData1 != null)
        {
            Page = 1;
            PageEvent.Invoke(this, EventArgs.Empty);
        }
    }


    private async void AllParsing() {
        List<string> GroupName = new();
        foreach (var faculty in FireBase.fireBaseData.Faculties)
        {
            foreach (var specialithation in faculty.Specializations)
            {
                string name_specialithation = specialithation.Value.Name;
                foreach (var year in specialithation.Value.Years)
                {
                    string name_year = year.Value.Name;
                    foreach (var group in year.Value.Groups)
                    {
                        string name_group = group.Value.Name;
                        GroupName.Add(name_year + name_specialithation + "-" + name_group);
                    }
                }
            }
        }
        Debug.Log("GroupName" + GroupName.Count);
        Page = GroupName.FindIndex(obj=> obj== parsingGroupName);
        ParsingData1 = new();
        foreach (string groupName in GroupName)
        {
            GroupParsing groupParsing = await ParsingMetod2(groupName);
            if(groupParsing!=null) ParsingData1.Add(groupParsing);
        }

        Debug.Log("ParsingData1 CountGroup Parsing: " + ParsingData1.Count);
    }

    private async Task<GroupParsing> ParsingMetod2(string GroupName) {
        GroupParsing groupParsing = new();
        groupParsing.Name = GroupName;

        List<string> GroupAndDates = new();


        /*dateTime = dateTime_Now.Subtract(new TimeSpan(21, 0, 0, 0));
        if (dateTime.Month == dateTime_Now.Month)
        {
            string datePars_Last_3 = dateTime.Year + "-" + dateTime.Month + "-" + dateTime.Day;
            string GroupAndDate_3 = "group=" + GroupName + "&day=" + datePars_Last_3;

            GroupAndDates.Add(GroupAndDate_3);
        }

        dateTime = dateTime_Now.Subtract(new TimeSpan(14, 0, 0, 0));
        if (dateTime.Month == dateTime_Now.Month && dateTime.DayOfWeek != DayOfWeek.Sunday)
        {
            string datePars_Last_2 = dateTime_Now.Year + "-" + dateTime_Now.Month + "-" + dateTime.Day;
            string GroupAndDate_2 = "group=" + GroupName + "&day=" + datePars_Last_2;

            GroupAndDates.Add(GroupAndDate_2);
        }

        dateTime = dateTime_Now.Subtract(new TimeSpan(7, 0, 0, 0));
        if (dateTime.Month == dateTime_Now.Month)
        {
            string datePars_Last_1 = dateTime.Year + "-" + dateTime.Month + "-" + dateTime.Day;
            string GroupAndDate_1 = "group=" + GroupName + "&day=" + datePars_Last_1;

            GroupAndDates.Add(GroupAndDate_1);
        }*/

        string datePars_Now = dateTime_Now.Year + "-" + dateTime_Now.Month + "-" + dateTime_Now.Day;
        string GroupAndDate_0 = "group=" + GroupName + "&day=" + datePars_Now;

        GroupAndDates.Add(GroupAndDate_0);

        try
        {

            foreach (string GroupAndDate in GroupAndDates)
            {
                using (HttpClientHandler hdl = new HttpClientHandler { AllowAutoRedirect = false, AutomaticDecompression = System.Net.DecompressionMethods.Deflate | System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.None })
                {
                    using (var client = new HttpClient(hdl))
                    {
                        using (HttpResponseMessage resp = await client.GetAsync(POLESSU_URL_TYPE_2 + GroupAndDate))
                        {
                            if (resp.IsSuccessStatusCode)
                            {

                                var html = resp.Content.ReadAsStringAsync().Result;

                                if (!string.IsNullOrEmpty(html))
                                {
                                    HtmlAgilityPack.HtmlDocument document = new HtmlAgilityPack.HtmlDocument();
                                    document.LoadHtml(html);

                                    var tables = document.DocumentNode.SelectSingleNode(POLESSU_FILE2);
                                    try
                                    {
                                        WorkingTabelsType2(tables).dateParses.ForEach(groupParsing.dateParses.Add);
                                    }
                                    catch
                                    {
                                        continue;
                                    }

                                }
                            }
                        }
                    }
                }
            }
            if(groupParsing.dateParses.Count>0) return groupParsing;
            return null;
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }

        return null;
    }

    private List<string> ConvertToCollectionString(HtmlNodeCollection htmlNode, List<int> DontConvert = null ) {
        List<string> HTMLInnerText = new();
        for(int i = 0; i < htmlNode.Count; i++)
        {
            if(DontConvert==null||!DontConvert.All(obj => obj == i))
            {
                HTMLInnerText.Add(htmlNode[i].InnerText);
            }
        }
        return HTMLInnerText;
    }

    private HtmlNodeCollection SelectOnName(HtmlNodeCollection htmlNodes, HtmlNode parent, string name) {
        HtmlNodeCollection htmlNodesReturn = new HtmlNodeCollection(parent);
        foreach (var item in htmlNodes)
        {
            if (item.Name == name)
                htmlNodesReturn.Add(item);
        }
        return htmlNodesReturn;
    }

    private HtmlNodeCollection SelectOnNameParent(HtmlNodeCollection htmlNodes, int idChild) {
        HtmlNodeCollection htmlNodesReturn = htmlNodes;
        for (int i = htmlNodes[idChild].ChildNodes.Count - 1; i >= 0; i--)
        {
            var ChildNode = htmlNodes[idChild].ChildNodes[i];
            if (ChildNode.Name != "td")
            {
                htmlNodesReturn[idChild].ChildNodes.Remove(htmlNodesReturn[idChild].ChildNodes[i]);
            }
        }

        return htmlNodesReturn;
    }

    private HtmlNodeCollection RowspanOnID(HtmlNodeCollection htmlNodes, int idCell, int idStr, int CountLection) {
        if (CountLection == 3) { return htmlNodes; }
        for (int i = 1; i < CountLection; i++)
        {
            htmlNodes = SelectOnNameParent(htmlNodes, idStr + i);

            var parentNode = htmlNodes[idStr + i];

            if (parentNode.ChildNodes.Count > idCell) // Проверка, чтобы не выйти за пределы
            {
                parentNode.ChildNodes.Insert(idCell, htmlNodes[idStr].ChildNodes[idCell].CloneNode(true)); // Клонируем узел перед вставкой
            }
            else
            {
                parentNode.ChildNodes.Add(htmlNodes[idStr].ChildNodes[idCell].CloneNode(true)); // Добавляем в конец, если индекс выходит за пределы
            }

            parentNode.ChildNodes[idCell].Name = "td";


        }
        return htmlNodes;
    }




    private GroupParsing WorkingTabelsType2(HtmlNode table) {
        GroupParsing GroupData = new();
        
        if (table != null)
        {

            HtmlNodeCollection dateNodesName = table.SelectNodes(".//small[@class='text-muted']");
            if (dateNodesName != null && dateNodesName.Count > 1)
            {
                List<string> DateNodesName = ConvertToCollectionString(dateNodesName, new() {0}); // сохраняет даты на которые стоит расписание

                DateNodesName.ForEach(obj => GroupData.dateParses.Add(new DateParse { dateTime = obj }));

                HtmlNodeCollection DateNodes = table.SelectNodes(".//div[@class='row acty-group']"); // все строки tbody
                if (DateNodes != null && DateNodes.Count > 0)
                {
                    for (int DateID = 0; DateID < DateNodes.Count; DateID++)
                    {
                        HtmlNodeCollection LessonsNodes = DateNodes[DateID].SelectNodes(".//span[@class='acty-subjects']");
                        if (LessonsNodes != null && LessonsNodes.Count > 0)
                        {
                            List<string> LessonsName = ConvertToCollectionString(LessonsNodes);

                            LessonsName.ForEach(obj => GroupData.dateParses[DateID].Lessons.Add(obj));
                        }
                        else
                        {
                            Debug.Log($"Занятий на {DateID} нету");
                        }
                    }

                    return GroupData;
                }
                else
                {
                    Debug.LogError("Таблиц с данными о рассписании не существует");
                }
            }
            else
            {
                Debug.LogWarning("small[@class='text-muted дат не существует");
            }

        }
        else
        {
            Debug.LogWarning("Нет результата");
        }

        return null;
    }

    IEnumerator ResourceTickOver(CancellationTokenSource token, string taskType = "", float waitTime = 10) {
        yield return new WaitForSeconds(waitTime);
        if (token.IsCancellationRequested !=true)
        {
            token.Dispose();
            Debug.Log("Error Connecting " + taskType);
        }
        else
        {
            Debug.Log("Seccfulied Connecting " + taskType);
        }
    }
}
    

