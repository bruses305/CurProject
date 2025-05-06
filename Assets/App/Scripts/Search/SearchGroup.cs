using System;
using UnityEngine;
using TMPro;

public class SearchGroup : MonoBehaviour
{
    [SerializeField] private TMP_InputField searchPanel;
    [SerializeField] private Parsing parsingObject;

    public async void Search() {
        string groupName = searchPanel.text;
        LoadingScreenActivate();
        Debug.Log("Search: " + groupName + ";" + SelectedDates.DateStart + ";" + SelectedDates.DateEnd);
        await parsingObject.LoadingDefouldData(RedactSearchText.UpperText(groupName), true, SelectedDates.DateStart, SelectedDates.DateEnd);

    }

    private static void LoadingScreenActivate()
    {
        LoadingScreen.Instance.Activate();
    }
}
