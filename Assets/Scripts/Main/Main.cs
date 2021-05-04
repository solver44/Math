using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Main : MonoBehaviour
{
    public GameObject profilePanel = null, settingPanel = null;
    public Canvas canvas = null;
    public Text Name = null, Surname = null, Age = null;
    [Header("Showing Text In Canvas")]
    public Text UserName = null, CurrentLevel = null;
    [Header("Fill content")]
    public Image CurrentLevelImg = null;
    public Animator Menu;

    private Toggle UzbLang = null;
    private Toggle RusLang = null;

    bool isFirstTime = true;
    
    private void Awake()
    {
        Application.targetFrameRate = 500;
        Screen.orientation = ScreenOrientation.Landscape; //Сделать экран альбомный

        UserName.text = PlayerPrefs.GetString("nameUser") + " " + PlayerPrefs.GetString("surnameUser");//Поставить имя пользователя на Username Text
    }
    float rangeOfLevel = 0;
    private void Start()
    {
        profilePanel.SetActive(false);

        StartCoroutine(menuAnimTime());

        if (PlayerPrefs.HasKey("CurrentLevel")) //Если текущий лвл существует
        {
            int currentLVL = PlayerPrefs.GetInt("CurrentLevel");
            CurrentLevel.text = currentLVL.ToString();
            rangeOfLevel = currentLVL * 0.2f; //Начислить сумма заполнения лвл рисункок
            CurrentLevelImg.fillAmount = 0; //И поставить
        }
        else
        {
            PlayerPrefs.SetInt("CurrentLevel", 0);
        }

    }
    public bool StartLvlAnim = false;
    private void Update()
    {
        if (CurrentLevelImg.fillAmount <= rangeOfLevel && StartLvlAnim)
            CurrentLevelImg.fillAmount += Time.fixedDeltaTime * 0.1f;
        else if (CurrentLevelImg.fillAmount > rangeOfLevel && !StartLvlAnim)
            StartLvlAnim = false;
    }
    IEnumerator menuAnimTime()
    {
        yield return new WaitForSeconds(0.5f);
        Menu.SetTrigger("start");
        yield return new WaitForSeconds(2);
        StartLvlAnim = true;
    }

    [SerializeField] private LocalizationManager locManager;
    public void OnClickOkButton()
    {
        if (profilePanel.activeSelf)
        {
            profilePanel.GetComponent<Animator>().SetTrigger("exit");
            StartCoroutine(exitProfilePanel());
        }
        if (settingPanel.activeSelf)
        {
            settingPanel.SetActive(false);
        }
    }
    private IEnumerator exitProfilePanel()
    {
        yield return new WaitForSeconds(0.8f);
        profilePanel.SetActive(false);
    }
    public void OnClickRussianBtn()
    {
        if (settingPanel.activeSelf)
        {
            locManager.CurrentLanguage = "ru_RU";
        }
    }
    public void OnClickUzbekBtn()
    {
        if (settingPanel.activeSelf)
        {
            locManager.CurrentLanguage = "uz_UZ";
        }
    }

    public void OnClickProfileButton()
    {
        if (!Age.text.Contains(PlayerPrefs.GetInt("ageUser").ToString()))
        {
            Name.text += "\n" + PlayerPrefs.GetString("nameUser");
            Surname.text += "\n" + PlayerPrefs.GetString("surnameUser");
            Age.text += "\n" + PlayerPrefs.GetInt("ageUser");
        }
        profilePanel.SetActive(true);
        profilePanel.GetComponent<Animator>().SetTrigger("start");
    }

    public void OnClickSettingsButton()
    {
        settingPanel.SetActive(true);
        RusLang = GameObject.Find("RussianToggle").GetComponent<Toggle>();
        UzbLang = GameObject.Find("UzbekToggle").GetComponent<Toggle>() as Toggle;


        if (PlayerPrefs.GetString("Language") == "ru_RU")
        {
            UzbLang.isOn = false;
            RusLang.isOn = true;
        }
        else
        {
            UzbLang.isOn = true;
            RusLang.isOn = false;
        }
    }
}
