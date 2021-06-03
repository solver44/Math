using UnityEngine;
using UnityEngine.SceneManagement;

public class Intro : MonoBehaviour
{
    public GameObject panelInfo;
    public GameObject Menu;

    public GameObject PreSchool;
    public GameObject Class1;

    void Start()
    {
        if (!PlayerPrefs.HasKey("nameUser") && !PlayerPrefs.HasKey("surnameUser"))
        {
            panelInfo.SetActive(true);
            Menu.SetActive(false);
        }
        else
        {
            panelInfo.SetActive(false);
            Menu.SetActive(true);
        }
    }

    public void ActivePreSchool()
    {
        PreSchool.SetActive(true);
        Menu.SetActive(false);
    }

    public void ActiveClass1()
    {
        Class1.SetActive(true);
        Menu.SetActive(false);
    }

    public void BackToMenu()
    {
        PreSchool.SetActive(false);
        Class1.SetActive(false);
        Menu.SetActive(true);
    }

    public void PreSchoolSceneClick(int index)
    {
        PlayerPrefs.SetInt("ClickingLevel", 0);
        SceneManager.LoadScene(index + 1);
        PlayerPrefs.SetInt("ClickingUnit", index);
    }
    public void Class1SceneClick(int index)
    {
        PlayerPrefs.SetInt("ClickingLevel", 1);
        SceneManager.LoadScene(index + 3);
    }
}
