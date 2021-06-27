using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyObject : MonoBehaviour
{
    [PunRPC]
    public void Setting(string Name, string Parent)
    {
        this.name = Name;
        this.transform.parent = GameObject.Find(Parent).transform;
        this.transform.localScale = new Vector3(0.5f, 0.5f, 1);
        this.transform.localPosition = new Vector3(0, 0, 0);
    }

    PhotonView view;

    private void Start()
    {
        view = GetComponent<PhotonView>();
    }
    [PunRPC]
    public void GoUp(int index, bool isOwn)
    {
        if (!view.IsMine || (view.IsMine && isOwn && !Server.isOwnerRoom)) {
            Transform nextParent = GameObject.Find("l" + (index + 1)).transform.GetChild(3);
            this.transform.parent = nextParent;
            this.transform.localPosition = new Vector3(0, 0, 0);
        }
    }
}
