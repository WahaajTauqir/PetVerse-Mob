using UnityEngine;
using UnityEngine.UI;

public class PanelManager : MonoBehaviour
{
    [Header("Main Buttons")]
    public Button button1;
    public Button button2;

    [Header("Main Container Panel")]
    public GameObject panelB; 

    [Header("Sub Panels inside Panel B")]
    public GameObject subPanel1;
    public GameObject subPanel2;

    void Start()
    {
        // Hide panelB at start
        if (panelB != null)
            panelB.SetActive(false);

        // Hook up buttons
        if (button1 != null)
            button1.onClick.AddListener(OpenSubPanel1);

        if (button2 != null)
            button2.onClick.AddListener(OpenSubPanel2);
    }

    void OpenSubPanel1()
    {
        if (panelB != null) panelB.SetActive(true);
        if (subPanel1 != null) subPanel1.SetActive(true);
        if (subPanel2 != null) subPanel2.SetActive(false);
    }

    void OpenSubPanel2()
    {
        if (panelB != null) panelB.SetActive(true);
        if (subPanel1 != null) subPanel1.SetActive(false);
        if (subPanel2 != null) subPanel2.SetActive(true);
    }
}
