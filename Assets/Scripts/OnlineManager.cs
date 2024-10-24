using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime; 
using Photon.Pun;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;


public class OnlineManager : MonoBehaviourPunCallbacks 
{
    public GameObject prefab;
    private GameObject menuController;
    private GameObject player1;
    private GameObject player2;

    void Start()
    {
        if (SceneManager.GetActiveScene().name == "MenuOnline")
        {
            if (PhotonNetwork.IsMasterClient)
            {
                player1 = PhotonNetwork.Instantiate(this.prefab.name, new Vector3(0, 0, 0), Quaternion.identity, 0);
                player2 = PhotonNetwork.Instantiate(this.prefab.name, new Vector3(0, 0, 0), Quaternion.identity, 0);
                player2.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.PlayerList[1].ActorNumber);
                player1.GetComponent<MenuController>().SetOnlinePlayerMenu(player1.GetComponent<PhotonView>().ViewID, player2.GetComponent<PhotonView>().ViewID);
                player2.GetComponent<MenuController>().SetOnlinePlayerMenu(player1.GetComponent<PhotonView>().ViewID, player2.GetComponent<PhotonView>().ViewID);
            }
        }
        else
        {
            menuController = GameObject.FindWithTag("MenuController");   
            if (PhotonNetwork.IsMasterClient)
            {
                player1 = PhotonNetwork.Instantiate(menuController.GetComponent<VoteController>().player1Character.name, new Vector3(-25, -11, 0), Quaternion.identity, 0);
                player2 = PhotonNetwork.Instantiate(menuController.GetComponent<VoteController>().player2Character.name, new Vector3(25, -11, 0), Quaternion.identity, 0);
                PhotonNetwork.Instantiate(menuController.GetComponent<VoteController>().map.name, new Vector3(0, 0, 0), Quaternion.identity, 0);
                player2.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.PlayerList[1].ActorNumber);
                player1.GetComponent<Character_Controller>().SetOnlinePlayer(player1.GetComponent<PhotonView>().ViewID, player2.GetComponent<PhotonView>().ViewID);
                player2.GetComponent<Character_Controller>().SetOnlinePlayer(player1.GetComponent<PhotonView>().ViewID, player2.GetComponent<PhotonView>().ViewID); 
            }  
        }
    }

}
