using UnityEngine;
using Photon.Pun;

public class Client : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject enemy;
    public GameObject parent;
    private void Start()
    {
        //PhotonNetwork.Instantiate(enemy.name, parent.transform.position, Quaternion.identity);
    }

    public void Leave()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftLobby()
    {
        PhotonNetwork.LoadLevel("Main");
    }
}
