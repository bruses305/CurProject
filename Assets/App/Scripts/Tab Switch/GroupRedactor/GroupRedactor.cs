using UnityEngine;

public class GroupRedactor : MonoBehaviour
{
    private void Awake() {
        OpenGroupRedactor.LoadingEnd += OpenGroupRedactor_LoadingEnd;
    }

    private void OpenGroupRedactor_LoadingEnd(object sender, System.EventArgs e) {
        LoadingTable();
    }

    private void LoadingTable() {

    }
}
