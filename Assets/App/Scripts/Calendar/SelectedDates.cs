using System;
using Bitsplash.DatePicker;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class SelectedDates : MonoBehaviour
{
    [SerializeField] private DatePickerSettings datePickerSettings;
    [SerializeField] private TextMeshProUGUI selectedDateText;
    public static DateTime? DateStart;
    public static DateTime? DateEnd;
    
    private void Start()
    {
        if (datePickerSettings == null) return;
        // handle selection change using a unity event
        datePickerSettings.Content.OnSelectionChanged.AddListener(OnSelectionChanged);
        datePickerSettings.Content.OnDisplayChanged.AddListener(OnDisplayChanged);
        ShowAllSelectedDates();// show all the selected days in the begining
    }
    private void ShowAllSelectedDates()
    {
            var selection = datePickerSettings.Content.Selection;
            if (selection.Count == 0) {
                DateStart = null;
                DateEnd = null;
            }
            else {
                DateStart = selection.GetItem(0);
                DateEnd = selection.GetItem(selection.Count - 1);
            }
            selectedDateText.text = ConvertToDateTime(DateStart) + ConvertToDateTime(DateEnd);
    }
    private void OnSelectionChanged()
    {
        ShowAllSelectedDates();
    }
    private void OnDisplayChanged()
    {
        var cell = datePickerSettings.Content.GetCellObjectByDate(DateTime.Now);
        if (cell != null)
        {
            cell.CellEnabled = false;
        }
    }

    private string ConvertToDateTime(DateTime? dateTime)
    {
        if(dateTime == null) return DateTime.Today.ToString("D") + '\n';
        
        return dateTime.Value.ToString("D") + '\n';
    }
}
