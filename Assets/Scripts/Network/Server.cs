using Photon.Pun;
using UnityEngine;

public class Server : MonoBehaviourPunCallbacks
{
    private const byte MAX_USER = 2;
    private void Start()
    {
        PhotonNetwork.NickName = PlayerPrefs.GetString("nameUser");
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.GameVersion = "1.0";
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connect");
    }

    public void CreateRoom()
    {
        Debug.Log("Creating Room...");
        PhotonNetwork.CreateRoom(null, new Photon.Realtime.RoomOptions { MaxPlayers = 2});
        Debug.Log("Created Room");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room");
        PhotonNetwork.LoadLevel("Battle");
    }
    public void JoinRoom()
    {
        Debug.Log("Joining Room...");
        PhotonNetwork.JoinRandomRoom();
    }

    public void CreateOrJoin()
    {
        if (PhotonNetwork.JoinRandomOrCreateRoom(null, MAX_USER, Photon.Realtime.MatchmakingMode.FillRoom, null, null, null, new Photon.Realtime.RoomOptions { MaxPlayers = 2 }, null))
        {

        }
    }
}
