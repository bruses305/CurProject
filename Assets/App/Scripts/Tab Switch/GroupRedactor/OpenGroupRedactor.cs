using System;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public static class OpenGroupRedactor
{
    [SerializeField] private static GameObject parentGroupsRedactor;
    [SerializeField] private static GameObject Redactor;
    public static event EventHandler LoadingEnd;
    private const int ID_WINDOW = 5;
    public static void ActivateListGroups() {
        parentGroupsRedactor.SetActive(!parentGroupsRedactor.activeSelf);
    }
    public async static void LoadingRedactor(string groupName) {
        TabsSwitcher.Instance.SwitchTabID(ID_WINDOW);
        await Parsing.Instance.LoadingDefouldData(groupName, true);
        LoadingEnd.Invoke(null,EventArgs.Empty);
    }
}
