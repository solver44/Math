using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Client : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject enemy;
    public GameObject parent;

    public void Leave()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(1);
    }
}
