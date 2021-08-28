using System;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class EnemyObject : MonoBehaviourPunCallbacks, IOnEventCallback
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
    CreatorMapFirst map;

    private void Start()
    {
        view = GetComponent<PhotonView>();
        map = GameObject.Find("Creator").GetComponent<CreatorMapFirst>();
    }
    [PunRPC]
    public void GoUp(int index, bool isOwn)
    {
        if ((!view.IsMine && isOwn) || (view.IsMine && !isOwn)) {
            Transform nextParent = GameObject.Find("l" + (index + 1)).transform.GetChild(3);
            this.transform.parent = nextParent;
            this.transform.localPosition = new Vector3(0, 0, 0);
        }
    }
    [PunRPC]
    public void CheckWinOrLose(int[] results)
    {
        if (view.IsMine)
        {
            int cntR = 0;
            for (int i = 0; i < results.Length; i++)
            {
                if (results[i] == 1)
                    cntR++;
            }
            int currentCntR = 0;
            for (int i = 0; i < map.ExamsResults.Length; i++)
            {
                if (map.ExamsResults[i] == 1)
                    currentCntR++;
            }
            Debug.Log("CntR: " + cntR + "\nCurrentCNTR: " + currentCntR);

            RaiseEventOptions options = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            SendOptions sendOption = new SendOptions { Reliability = true };
            PhotonNetwork.RaiseEvent(100, currentCntR, options, sendOption);

            if (cntR > currentCntR)
                map.WinOrLose(false);
            else
                map.WinOrLose(true);
        }
    }

    public void OnEvent(EventData photonEvent)
    {
        if(photonEvent.Code == 100)
        {
            int cntR = (int)photonEvent.CustomData;
            int currentCntR = 0;
            for (int i = 0; i < map.ExamsResults.Length; i++)
            {
                if (map.ExamsResults[i] == 1)
                    currentCntR++;
            }

            Debug.Log(cntR + "\n" + currentCntR);
            if (cntR > currentCntR)
                map.WinOrLose(false);
            else
                map.WinOrLose(true);
        }
    }
}
