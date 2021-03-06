using System.Collections;
using UnityEditor;
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

    private void Awake()
    {
        Application.targetFrameRate = 1000;
    }
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

    private float timer;
    void Update()
    {
        if (Time.unscaledTime > timer + .5f)
        {
            if (!stop && GameObjects.transform.childCount <= 0)
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
            timer = Time.unscaledTime;
        }

        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                SceneManager.LoadScene(1);
            }
        }
    }
    public void SetSelectedColor()
    {
        int cnt = 0;
        foreach (var item in Pens.GetComponentsInChildren<Transform>())
        {
            var item1 = Pens.GetComponentsInChildren<Toggle>()[cnt];
            if(item1.isOn)
                item.localPosition = Vector3.MoveTowards(item.localPosition, new Vector3(item.localPosition.x, item.localPosition.y + 20), 2f);
            cnt++;
        }
    }

    private IEnumerator enterBGPanel()
    {
        yield return new WaitForSeconds(0.5f);
        BGPanel.SetTrigger("start");
    }

    public void BackToMain()
    {
        SceneManager.LoadScene(1);
    }

    public void NextLevel()
    {
        if (PlayerPrefs.GetInt("ClickingUnit") >= 38)
            return;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        PlayerPrefs.SetInt("ClickingUnit", SceneManager.GetActiveScene().buildIndex);
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(SceneLogic))]
public class SceneLogicEditor : Editor
{
    private GameObject parent;
    private string title;
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        SceneLogic sceneScript = (SceneLogic)target;

        EditorGUILayout.Space(50);
        parent = (GameObject)EditorGUILayout.ObjectField("Parent:", parent, typeof(GameObject), true);
        title = EditorGUILayout.TextField("Title:", title);
        if (GUILayout.Button("Sorting"))
        {
            for (int i = 0; i < parent.transform.childCount; i++)
            {
                parent.transform.GetChild(i).name = title + (i+1).ToString();
            }
        }
    }
}
#endif
