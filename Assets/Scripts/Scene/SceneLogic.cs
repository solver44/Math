using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLogic : MonoBehaviour
{
    public GameObject GameObjects = null;
    public GameObject[] ForDestroying;
    public GameObject PanelWin = null;
    private Animator panelWinAnim = null;
    public Animator BGPanel = null;
    public Text unitText = null;

    bool stop = false;

    private void Start()
    {
        StartCoroutine(setUnitNumber());
        panelWinAnim = PanelWin.GetComponent<Animator>() as Animator;
    }

    private IEnumerator setUnitNumber()
    {
        yield return new WaitForSeconds(0.1f);
        unitText.text += " " + PlayerPrefs.GetInt("ClickingUnit");
    }
    void Update()
    {
        if(!stop && GameObjects.transform.childCount <= 0)
        {
            stop = true;
            for (int i = 0; i < ForDestroying.Length; i++)
            {
                Destroy(ForDestroying[i].transform.gameObject);
            }
            PanelWin.SetActive(true);
            panelWinAnim.SetTrigger("start");
            StartCoroutine(enterBGPanel());
            PlayerPrefs.SetInt("CurrentLevel", PlayerPrefs.GetInt("ClickingUnit"));
        }
    }
    private IEnumerator enterBGPanel()
    {
        yield return new WaitForSeconds(1);
        BGPanel.SetTrigger("start");
    }

    public void BackToMain()
    {
        SceneManager.LoadScene(1);
    }
}
