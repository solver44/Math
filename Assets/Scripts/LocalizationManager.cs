using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;

public class LocalizationManager : MonoBehaviour
{
    private Dictionary<string, string> localizedText;
    private string currentLanguage = null;
    private bool isReady = false;

    public Action ValueChanged;
    protected virtual void OnLanguageChanged() => ValueChanged?.Invoke();

    private void Awake()
    {
        if (!PlayerPrefs.HasKey("Language"))
        {
            if (Application.systemLanguage == SystemLanguage.Russian)
                PlayerPrefs.SetString("Language", "ru_RU");
            else
                PlayerPrefs.SetString("Language", "uz_UZ");
        }
        currentLanguage = PlayerPrefs.GetString("Language");
        LoadLocalizatedText(currentLanguage);
    }

    public string CurrentLanguage
    {
        get { return currentLanguage; }
        set { PlayerPrefs.SetString("Language", value); currentLanguage = value; OnLanguageChanged(); }

    }

    public string GetLocalizedValue(string key)
    {
        LoadLocalizatedText(currentLanguage);
        if (localizedText != null && localizedText.ContainsKey(key))
        {
            return localizedText[key];
        }
        else
        {
            throw new Exception("Can't get localized text");
        }
    }

    public void LoadLocalizatedText(string lang)
    {
        var path = "";

        string dataAJson = null;
#if UNITY_EDITOR
        path = Application.streamingAssetsPath + "/" + lang + ".json";
#elif UNITY_IOS
        path = Path.Combine (Application.streamingAssetsPath + "/Raw", lang + ".json");
#elif UNITY_ANDROID
        path = "jar:file://" + Application.dataPath + "!/assets/" + lang + ".json";
#endif

        //path = Application.streamingAssetsPath + "/Languages/" + lang + ".json";
        //WWW reader = new WWW(path);
        //while(!reader.isDone){}
        //dataAJson = reader.text;

        //if (Application.platform == RuntimePlatform.Android)
        //{
        //UnityWebRequest www = UnityWebRequest.Get(path);
        //www.SendWebRequest();
        //while (!www.isDone) ;
        //dataAJson = www.downloadHandler.text;
        //}

#if UNITY_EDITOR || UNITY_IOS
        dataAJson = File.ReadAllText(path);
        using(StreamReader stream = new StreamReader(path))
        {
            dataAJson = stream.ReadToEnd();
        }
#elif UNITY_ANDROID 
        
        WWW reader = new WWW(path);
        while(!reader.isDone){}
        dataAJson = reader.text;

#endif
        LocalizationData loadData = new LocalizationData();

        int numLines = dataAJson.Split('\n').Length;
        List<int> countRange = new List<int>();
        for (int i = 0; i < numLines; i++)
        {
            string line = GetLine(dataAJson, i);
            if (line.Contains("\": \"") && (line.Contains("key") || line.Contains("value")))
            {
                countRange.Add(i);
            }
        }

        loadData.Items = new LocalizationItem[countRange.Count / 2];

        int cntOfitems = 0;
        for (int i = 0; i < countRange.Count; i++)
        {
            string line = GetLine(dataAJson, countRange[i]);
            string keyT = "", valueT = "";
            if (line.Contains("\"key\""))
            {
                keyT = line;
                valueT = GetLine(dataAJson, countRange[i] + 1);
            }

            if (keyT != "")
            {
                loadData.Items[cntOfitems] = new LocalizationItem(GetTextBetweenQuotes(keyT)[1],
                GetTextBetweenQuotes(valueT)[1]);
                cntOfitems++;
            }
        }

        localizedText = new Dictionary<string, string>();
        for (int i = 0; i < loadData.Items.Length; i++)
        {
            localizedText.Add(loadData.Items[i].key, loadData.Items[i].value);
        }

        PlayerPrefs.SetString("Language", lang);
        currentLanguage = lang;
        isReady = true;
    }
    private List<string> GetTextBetweenQuotes(string str)
    {
        List<string> results = new List<string>();
        var reg = new Regex("\".*?\"");
        var matches = reg.Matches(str);
        foreach (var item in matches)
        {
            results.Add(item.ToString().Replace("\"", "").Replace("||", Environment.NewLine));
        }

        return results;
    }
    string GetLine(string text, int lineNo)
    {
        string[] strLines = text.Split('\n');
        return strLines[lineNo];

    }
}
