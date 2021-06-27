using UnityEngine;
using Photon.Pun;

public class Client : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject enemy;
    public GameObject parent;

    public void Leave()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftLobby()
    {
        PhotonNetwork.LoadLevel("Main");
    }
}
