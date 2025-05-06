using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CalendarEvents : MonoBehaviour
{
    [SerializeField] private Button exitButton;
    [SerializeField] private Button openCalendarButton;
    private GameObject _calendarObject;
    private void Awake()
    {
        _calendarObject = exitButton.gameObject;
        openCalendarButton.onClick.AddListener(ActivateCalendar);
        exitButton.onClick.AddListener(DeactivateCalendar);
        DeactivateCalendar();
    }

    private void ActivateCalendar()
    {
        _calendarObject.SetActive(true);
    }
    private void DeactivateCalendar()
    {
        _calendarObject.SetActive(false);
    }
    
    
}
