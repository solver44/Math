using UnityEngine;
using UnityEngine.UI;

public class LocalizedText : MonoBehaviour
{
    [SerializeField] private string key;
    [SerializeField] private bool isMesh = false;

    private LocalizationManager locManager;
    private Text text;
    private TextMesh mesh;

    private void Awake()
    {
        if(locManager == null)
        {
            locManager = GameObject.FindGameObjectWithTag("LocalizationManager").GetComponent<LocalizationManager>();
        }
        if(text == null)
        {
            if (!isMesh)
                text = GetComponent<Text>();
            else
                mesh = GetComponent<TextMesh>();
        }
        locManager.ValueChanged += updateText;
    }

    private void Start()
    {
        updateText();
    }

    public void updateText()
    {
        if (gameObject == null) return;

        if (locManager == null)
        {
            locManager = GameObject.FindGameObjectWithTag("LocalizationManager").GetComponent<LocalizationManager>();
        }
        if (!isMesh)
            text.text = locManager.GetLocalizedValue(key);
        else
            mesh.text = locManager.GetLocalizedValue(key);
    }
}
