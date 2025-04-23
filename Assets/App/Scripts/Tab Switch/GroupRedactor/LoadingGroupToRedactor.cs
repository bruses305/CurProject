using UnityEngine;
using UnityEngine.UI;

public class LoadingGroupToRedactor : MonoBehaviour
{
    [SerializeField] Button activatorListGrops;
    [SerializeField] GameObject GroupsRedactorParent;
    [SerializeField] GameObject imageiconList;
    [SerializeField] GameObject prefabGroupRedacror;

    [SerializeField] FireBase fireBase;

    private void Awake() {
        fireBase.ParsingFireBaseEnd += FireBase_ParsingFireBaseEnd;
        activatorListGrops.onClick.AddListener(()=>OpenGroupRedactor.ActivateListGroups());
    }

    private void FireBase_ParsingFireBaseEnd(object sender, System.EventArgs e) {
        CreateGroupsRedactor();
    }

    private void CreateGroupsRedactor() {
        Instantiate(prefabGroupRedacror, GroupsRedactorParent.transform);
    }
}
