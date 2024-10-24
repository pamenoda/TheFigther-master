using System.Collections; 
using System.Collections.Generic; 
using UnityEngine; 
using UnityEngine.UI; 
using Photon.Realtime; 
using Photon.Pun;

public class Launcher : MonoBehaviourPunCallbacks 
{ 
    [SerializeField] private byte maxPlayersPerRoom;

    private string gameVersion;
    public bool isConnecting;
    public bool host = false;
    private bool MenuOnlineLoaded = false;

    private void Awake()
    {
        maxPlayersPerRoom = 2;
        gameVersion = "1.0.0";
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    void Update()
    {
        if (PhotonNetwork.CurrentRoom != null)
        {
            if(PhotonNetwork.CurrentRoom.PlayerCount == 2 && host && !MenuOnlineLoaded)
            {
                MenuOnlineLoaded = true;
                PhotonNetwork.LoadLevel("MenuOnline");
            }
        }
        
    }

    public void Connect()
    {
        isConnecting = true;

        if(PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = gameVersion;
        }
    }

    public override void OnConnectedToMaster()
    {
        if(isConnecting)
        {
            PhotonNetwork.JoinRandomRoom();
        }
    }

    public override void OnJoinedRoom()
    {
        if(PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            host = true;
        }
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });
    }

    public void StopResearch()
    {
        if (PhotonNetwork.CurrentRoom != null)
        {
            PhotonNetwork.LeaveRoom();
            isConnecting = false;
        }
    }
}