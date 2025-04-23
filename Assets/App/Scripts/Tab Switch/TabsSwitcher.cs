using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class TabsSwitcher : MonoBehaviour
{
    public static TabsSwitcher Instance;
    [SerializeField] private Button buttonJournal;
    [SerializeField] private Button buttonStatistic;
    [SerializeField] private Button buttonExport;
    [SerializeField] private Button buttonSettings;

    private Transform tabsParent;
    private Transform windowParent;

    public static Transform tabSelect;
    public static GameObject windowSelect;

    private const string TABS_PARENT_TAG = "Tabs Parent";
    private const string WINDOW_PARENT_TAG = "Window Parent";

    private const float colorAlphaUnselect = 0.5f;
    private const float colorAlphaSelect = 1;

    private void Awake() {
        Instance = this;
        tabsParent = GameObject.FindWithTag(TABS_PARENT_TAG).transform;
        windowParent = GameObject.FindWithTag(WINDOW_PARENT_TAG).transform;

        tabSelect = tabsParent.GetChild(0);
        windowSelect = windowParent.GetChild(0).gameObject;
    }
    private void Start() {
        UnityAction actionJournal = () => {SwitchTabID(0);};
        buttonJournal.onClick.AddListener(actionJournal);

        UnityAction actionStatistic = () => { SwitchTabID(1); };
        buttonStatistic.onClick.AddListener(actionStatistic);

        UnityAction actionExport = () => { SwitchTabID(2); };
        buttonExport.onClick.AddListener(actionExport);

        UnityAction actionSettings = () => { SwitchTabID(3); };
        buttonSettings.onClick.AddListener(actionSettings);
    }
    public void SwitchTabID(int idTab) {
        ReSelect(idTab);
    }
    private void ReSelect(int SelectingObjectID) {
        // Disable old
        if (tabSelect != null)
            ToggleTab(false);

        if(windowSelect != null)
            windowSelect.SetActive(false);

        // Switch references
        if (SelectingObjectID != 5)
        {
            tabSelect = tabsParent.GetChild(SelectingObjectID);
            windowSelect = windowParent.GetChild(SelectingObjectID).gameObject;

            // Enable new
            ToggleTab(true);
            windowSelect.SetActive(true);
        }
        else
        {
            tabSelect = null;
            windowSelect = windowParent.GetChild(SelectingObjectID).gameObject;
            windowSelect.SetActive(true);

        }

    }
    private void ToggleTab(bool isActive) {

        GameObject SelectableObject = tabSelect.GetChild(1).gameObject;
        tabSelect.GetChild(0).GetChild(0).TryGetComponent(out TextMeshProUGUI textWindow);

        bool isSwitchActive = !SelectableObject.activeSelf;
        float colorAlpha = isSwitchActive ? colorAlphaSelect : colorAlphaUnselect;

        textWindow.color = new Color(textWindow.color.r, textWindow.color.g, textWindow.color.b, colorAlpha);
        SelectableObject.SetActive(isSwitchActive);
    }
}
