using Photon.Pun;
using UnityEngine;

public class Server : MonoBehaviourPunCallbacks
{
    public delegate void GetOwner(bool isOwn);
    public static event GetOwner isOwner;

    private static bool _isOwner = false;
    public static bool isOwnerRoom
    {
        get { return _isOwner; }
        set { _isOwner = value; isOwner?.Invoke(_isOwner); }
    }

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
        Debug.Log("On Master");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        isOwnerRoom = false;
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Created Room");
        PhotonNetwork.CreateRoom(null, new Photon.Realtime.RoomOptions { MaxPlayers = 2}, Photon.Realtime.TypedLobby.Default);
        isOwnerRoom = true;
    }
    private void CreateRoom()
    {
        Debug.Log("Created Room");
        PhotonNetwork.CreateRoom(null, new Photon.Realtime.RoomOptions { MaxPlayers = 2 }, Photon.Realtime.TypedLobby.Default);
        isOwnerRoom = true;
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room");
        PhotonNetwork.LoadLevel("FirstBattle");
    }

    public void CreateOrJoin()
    {
        try {
            PhotonNetwork.JoinRandomRoom();
        }
        catch { CreateRoom(); }
    }
}
