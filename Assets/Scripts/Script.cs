using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Script : MonoBehaviour
{
    public Transform loadingBar;

    [SerializeField]
    private float load;
    [SerializeField]
    private float range;

    public MoveToLocation transition;
    public float transitionTime = 2f;

    private void Awake()
    {
        loadImg = loadingBar.GetComponent<Image>();
        transitionTime = 0f;
        Screen.orientation = ScreenOrientation.Landscape; //Сделать экран альбомный
        if (!PlayerPrefs.HasKey("CurrentLevel")) //Проверить если нет текущий лвл
            PlayerPrefs.SetInt("CurrentLevel", 0); //То тогда создать его
    }
    bool loading = false;
    bool finishCurr = false;
    Image loadImg;
    void Update()
    {
        if (load < 100)
        {
            load += range * Time.fixedDeltaTime; //Сделать плавный скорость
            loadImg.fillAmount = load / 100;
        }
        else if(!finishCurr)
        {
            nextToMain(); //Метод #1
            finishCurr = true;
            return;
        }

        if (transition.FinishMove && !loading) {
            loading = true;
            StartCoroutine(LoadMain());
        }
    }

    public void nextToMain()//#1
    {
        transition.SetLocationY(0, true);
        //StartCoroutine(LoadMain()); //#2
    }

    IEnumerator LoadMain() //#2
    {
        //transition.SetTrigger("start");

        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene("Main"); //Активировать следующую сцену
    }
}
