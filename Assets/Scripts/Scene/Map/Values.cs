using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Values : MonoBehaviour
{
    public GameObject UserImage = null;
    public bool MakeNull = true;
    public bool MakeInactive = false;
    public GameObject Effect = null;
    public GameObject Additional = null;
    public Text Text = null;

    private void Awake()
    {
        //Debug.Log(this.GetComponentInChildren<Image>().sprite.name);
        if (Effect != null)
            Effect.SetActive(false);
        if (UserImage != null)
        {
            if (MakeNull)
            {
                UserImage.GetComponent<Image>().sprite = null;
                UserImage.GetComponent<Image>().color = new Color(1, 1, 1, 0);
            }
            if(MakeInactive)
                UserImage.SetActive(false);
        }
    }
}
