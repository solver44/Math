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

    public ToggleGroup Pens;

    private void Start()
    {
        StartCoroutine(setUnitNumber());
        panelWinAnim = PanelWin.GetComponent<Animator>() as Animator;
    }

    private IEnumerator setUnitNumber()
    {
        yield return new WaitForSeconds(0f);
        unitText.text = PlayerPrefs.GetInt("ClickingUnit").ToString();
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

    public void SetSelectedColor()
    {
        if (Pens.AnyTogglesOn())
        {
            foreach (var item in Pens.GetComponentsInChildren<Transform>())
            {
                if (item)
                    item.localPosition = Vector3.MoveTowards(item.localPosition, new Vector3(item.localPosition.x, item.localPosition.y - 50), 1f);
            }
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
