using UnityEngine;

public class BackButton : MonoBehaviour
{
    public GameObject mainPanel;
    public GameObject detailPanel;

    void OnMouseDown()
    {
        mainPanel.SetActive(true);
        detailPanel.SetActive(false);
    }
}