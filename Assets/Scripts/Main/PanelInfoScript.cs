using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PanelInfoScript : MonoBehaviour
{
    public Button NextButton = null;
    public GameObject PanelLanguage = null;
    public Animator PanelLanguageAnim = null;
    public InputField NameUser = null;
    public Animator NamePanelAnim = null;
    public InputField SurnameUser = null;
    public Animator SurnamePanelAnim = null;
    public InputField AgeUser = null;
    public Animator AgePanelAnim = null;
    public Animator Elements = null;
    public Animator SuccesText = null;

    #region Toggle Group
    public Toggle uzb, rus, eng;
    #endregion

    public GameObject MenuPanel = null;
    public Text UserName = null;

    public Animator PanelInfoAnim = null;

    public Text Name, Surname, Age;

    void Start()
    {
        //NextButton.interactable = false;

        NextButton.onClick.AddListener(OnClickNextBtn);

        if (locManage == null)
            locManage = GameObject.FindGameObjectWithTag("LocalizationManager").gameObject.GetComponent<LocalizationManager>();
    }
    LocalizationManager locManage;
    private void OnClickNextBtn()
    {
        if (PanelLanguage.transform.localPosition.x == 0)
        {
            if (uzb.isOn)
            {
                PlayerPrefs.SetString("Language", "uz_UZ");
                locManage.CurrentLanguage = "uz_UZ";
            }
            else if (rus.isOn)
            {
                PlayerPrefs.SetString("Language", "ru_RU");
                locManage.CurrentLanguage = "ru_RU";
            }

            StartCoroutine(makeDisablePanelLang());
        }
        if (!string.IsNullOrEmpty(NameUser.text))
        {
            //Сохранения данных
            PlayerPrefs.SetString("nameUser", NameUser.text);
            
            //Анимация панели
            NamePanelAnim.SetTrigger("exit");
            SurnamePanelAnim.SetTrigger("start");

            NameUser.text = null;
        }
        else if (!string.IsNullOrEmpty(SurnameUser.text))
        {
            PlayerPrefs.SetString("surnameUser", SurnameUser.text);

            SurnamePanelAnim.SetTrigger("exit");
            AgePanelAnim.SetTrigger("start");

            SurnameUser.text = null;
        }
        else if (!string.IsNullOrEmpty(AgeUser.text))
        {
            PlayerPrefs.SetInt("ageUser", int.Parse(AgeUser.text));

            StartCoroutine(leavePanelInfo());

            AgeUser.text = null;
        }

    }

    IEnumerator makeDisablePanelLang()
    {
        PanelLanguageAnim.SetTrigger("start");
        NamePanelAnim.SetTrigger("start");
        yield return new WaitForSeconds(1);
        PanelLanguage.SetActive(false);
    }
    IEnumerator leavePanelInfo()
    {
        AgePanelAnim.SetTrigger("exit");
        Elements.SetTrigger("start");
        PanelInfoAnim.SetTrigger("start");
        yield return new WaitForSeconds(2);
        SuccesText.SetTrigger("start");
        UserName.text = PlayerPrefs.GetString("nameUser") + " " + PlayerPrefs.GetString("surnameUser");
        yield return new WaitForSeconds(1.5f);
        MenuPanel.SetActive(true);
        MenuPanel.GetComponent<Animator>().SetTrigger("start");
    }

    public void ChangeValueField(string text)
    {
        if (string.IsNullOrEmpty(text))
            NextButton.interactable = false;
        else
            NextButton.interactable = true;
    }

    public void ResetData()
    {
        PlayerPrefs.DeleteAll();

        Name.text = "Имя:";
        Surname.text = "Фамилия:";
        Age.text = "Возраст:";
    }
}
