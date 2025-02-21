using UnityEngine;
using TMPro;

public class CubeButton : MonoBehaviour
{
    public GameObject mainPanel;
    public GameObject detailPanel;
    public TMP_Text detailText;


    void OnMouseDown()
    {
        // Cacher le panneau principal
        mainPanel.SetActive(false);
        // Montrer le panneau de détail
        detailPanel.SetActive(true);

    }
}