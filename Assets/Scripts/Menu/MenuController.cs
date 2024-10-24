using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Photon.Realtime; 
using Photon.Pun;

public class MenuController : MonoBehaviourPunCallbacks
{
    private GameObject goku;
    private GameObject vegeta;
    public int selectedCharacter = -1; // parmis les personnage
    public int selectedMap = -1; // parmis les personnage
    public GameObject currentSelection; // parmis toute la ui
    public int currentMenuArray = -1; 
    public GameObject playerBorder; 
    public Image playerImage;
    public Image playerImageMap;
    public MenuController otherPlayer;
    [HideInInspector] public GameObject[] characters = new GameObject[2]; 
    [HideInInspector] public GameObject[] map = new GameObject[3]; 
    private GameObject[][] menu = new GameObject[5][]; 
    public bool playVote = false;
    public bool quitVote = false;
    private GameObject cellTournament;
    private GameObject namek;
    private GameObject spiritAndTime;
    public bool isOnline;


    void Start()
    {
        if (SceneManager.GetActiveScene().name == "MenuOnline")
        {
            isOnline = true;
        }
        goku = GameObject.FindWithTag("GokuImage");        
        vegeta = GameObject.FindWithTag("VegetaImage");
        characters[0] = goku;
        characters[1] = vegeta;
        cellTournament = GameObject.FindWithTag("CellTournamentImage");        
        namek = GameObject.FindWithTag("NamekImage");
        spiritAndTime = GameObject.FindWithTag("SpiritAndTimeImage");
        map[0] = cellTournament;
        map[1] = namek;
        map[2] = spiritAndTime;
        menu[0] = new GameObject[characters.Length];
        menu[0][0] = characters[0];
        menu[0][1] = characters[1];
        menu[1] = new GameObject[map.Length];
        menu[1][0] = map[0];
        menu[1][1] = map[1];
        menu[1][2] = map[2];
        menu[2] = new GameObject[1];
        menu[2][0] = GameObject.FindWithTag("PlayButton"); 
        menu[3] = new GameObject[1];
        menu[3][0] = GameObject.FindWithTag("QuitButton"); 
        menu[4] = new GameObject[1];
        menu[4][0] = GameObject.FindWithTag("OnlineButton"); 
        currentSelection = menu[0][0];
        GameObject tagManager = GameObject.FindWithTag("Player1");
        if (!isOnline)
        {
            if (tagManager == null)
            {
                gameObject.tag = "Player1";
                playerImage = GameObject.FindWithTag("Player1Image").GetComponent<Image>();
                playerImageMap = GameObject.FindWithTag("Player1ImageMap").GetComponent<Image>();
                playerBorder = GameObject.FindWithTag("Player1Selection");   
                playerImage.enabled = true; 
                playerImageMap.enabled = true; 
            }
            else
            {
                gameObject.tag = "Player2";
                playerImage = GameObject.FindWithTag("Player2Image").GetComponent<Image>();
                playerImageMap = GameObject.FindWithTag("Player2ImageMap").GetComponent<Image>();
                playerBorder = GameObject.FindWithTag("Player2Selection");   
                playerImage.enabled = true; 
                otherPlayer = GameObject.FindWithTag("Player1").GetComponent<MenuController>();
                otherPlayer.otherPlayer = gameObject.GetComponent<MenuController>();
                playerImageMap.enabled = true; 
                Array.Resize(ref menu, menu.Length - 1);
                Array.Resize(ref otherPlayer.menu, otherPlayer.menu.Length - 1);
                if (SceneManager.GetActiveScene().name != "MenuOnline") 
                { 
                    GameObject.FindWithTag("OnlineButton").SetActive(false);
                }
            }
        }
        else
        {
            Array.Resize(ref menu, menu.Length - 1);
        }
        UpdateUI();
    }

    public void SetOnlinePlayerMenu(int idPhotonP1, int idPhotonP2)
    {
        photonView.RPC("SendOnlinePlayerMenu", RpcTarget.All, idPhotonP1, idPhotonP2);
    } 

    [PunRPC]
    private void SendOnlinePlayerMenu(int idPhotonP1, int idPhotonP2)
    {  
        MenuController player1;
        MenuController player2;
        if (PhotonNetwork.IsMasterClient)
        {
            player1 = PhotonView.Find(idPhotonP1).GetComponent<MenuController>();
            player2 = PhotonView.Find(idPhotonP2).GetComponent<MenuController>();
            player1.tag = "Player1";
            player2.tag = "Player2";
        }
        else
        {
            player2 = PhotonView.Find(idPhotonP1).GetComponent<MenuController>();
            player1 = PhotonView.Find(idPhotonP2).GetComponent<MenuController>();
            player1.tag = "Player1";
            player2.tag = "Player2";
        }
        
        player1.playerImage = GameObject.FindWithTag("Player1Image").GetComponent<Image>();
        player1.playerImageMap = GameObject.FindWithTag("Player1ImageMap").GetComponent<Image>();
        player1.playerBorder = GameObject.FindWithTag("Player1Selection");   
        player1.playerImage.enabled = true; 
        player1.playerImageMap.enabled = true; 
        player1.otherPlayer =  player2;

        player2.playerImage = GameObject.FindWithTag("Player2Image").GetComponent<Image>();
        player2.playerImageMap = GameObject.FindWithTag("Player2ImageMap").GetComponent<Image>();
        player2.playerBorder = GameObject.FindWithTag("Player2Selection");   
        player2.playerImage.enabled = true; 
        player2.playerImageMap.enabled = true;
        player2.otherPlayer = player1;

        UpdateUI();
    }

    void Update()
    {
        if (otherPlayer != null)
        {
            if (otherPlayer.currentSelection != currentSelection)
            {
                playerBorder.GetComponent<RectTransform>().localScale = new Vector3(1.1f, 1.1f, 0f);
            }
            else if(gameObject.CompareTag("Player2"))
            {
                playerBorder.GetComponent<RectTransform>().localScale = new Vector3(1.2f, 1.2f, 0f);
            }
        }
    }

    public void UpdateUI()
    {
        playerImage.sprite = characters[Mathf.Abs((selectedCharacter + 1) % characters.Length)].GetComponent<Image>().sprite;
        playerImageMap.sprite = map[Mathf.Abs((selectedMap + 1) % map.Length)].GetComponent<Image>().sprite;
        playerBorder.GetComponent<RectTransform>().position = currentSelection.GetComponent<RectTransform>().position;
        playerBorder.GetComponent<RectTransform>().sizeDelta = new Vector2 (currentSelection.GetComponent<RectTransform>().sizeDelta.x, currentSelection.GetComponent<RectTransform>().sizeDelta.y);
    }


    [PunRPC]
    private void SendMenuSelection(int index1, int index2)
    {
        if(index1 == 0)
        {
            otherPlayer.playerImage.sprite = otherPlayer.characters[index2].GetComponent<Image>().sprite;
            otherPlayer.selectedCharacter = index2;
        } 
        else if (index1 == 1)
        {
            otherPlayer.playerImageMap.sprite = otherPlayer.map[index2].GetComponent<Image>().sprite;
            otherPlayer.selectedMap = index2;
        }
        otherPlayer.currentSelection = otherPlayer.menu[index1][index2];
        otherPlayer.playerBorder.GetComponent<RectTransform>().position = otherPlayer.currentSelection.GetComponent<RectTransform>().position;
        otherPlayer.playerBorder.GetComponent<RectTransform>().sizeDelta = new Vector2 (otherPlayer.currentSelection.GetComponent<RectTransform>().sizeDelta.x, otherPlayer.currentSelection.GetComponent<RectTransform>().sizeDelta.y);
    }

    public void MenuSelection(InputAction.CallbackContext context) // Deplacement dans le menu (Haut, bas changement de tableau) (gauche droite deplacement dans le tableau)
    {
        if (context.started && !playVote && !quitVote && currentSelection!= null) // Si les joueurs n'ont pas voté
        {
            currentMenuArray += (int)context.ReadValue<Vector2>().y ;  // Changer de tableau en cours (personnage ou map ou jouer ou quitter)
            if (Mathf.Abs((currentMenuArray +1) % menu.Length) == 0) 
            {
                selectedCharacter += (int)context.ReadValue<Vector2>().x; 
                currentSelection = menu[Mathf.Abs((currentMenuArray +1) % menu.Length)][Mathf.Abs((selectedCharacter + 1) % menu[Mathf.Abs((currentMenuArray +1) % menu.Length)].Length)];
                if (isOnline) photonView.RPC("SendMenuSelection", RpcTarget.Others, Mathf.Abs((currentMenuArray +1) % menu.Length), Mathf.Abs((selectedCharacter + 1) % menu[Mathf.Abs((currentMenuArray +1) % menu.Length)].Length));
            }
            else if (Mathf.Abs((currentMenuArray +1) % menu.Length) == 1) 
            {
                selectedMap += (int)context.ReadValue<Vector2>().x ;   
                currentSelection = menu[Mathf.Abs((currentMenuArray +1) % menu.Length)][Mathf.Abs((selectedMap + 1) % menu[Mathf.Abs((currentMenuArray +1) % menu.Length)].Length)];
                if (isOnline) photonView.RPC("SendMenuSelection", RpcTarget.Others, Mathf.Abs((currentMenuArray +1) % menu.Length), Mathf.Abs((selectedMap + 1) % menu[Mathf.Abs((currentMenuArray +1) % menu.Length)].Length));
            }
            else
            {
                currentSelection = menu[Mathf.Abs((currentMenuArray +1) % menu.Length)][0];
                if (isOnline) photonView.RPC("SendMenuSelection", RpcTarget.Others, Mathf.Abs((currentMenuArray +1) % menu.Length), 0);
            }    
            UpdateUI();
        }
        
    }

    public void MenuVote(InputAction.CallbackContext context) // appuie sur x(manette) et entrer 
    {
        if (context.started)
        {
            if (Mathf.Abs((currentMenuArray +1) % menu.Length) == 2)
            {
                playVote = true;
                if (isOnline) photonView.RPC("SendVote", RpcTarget.Others, true, "jouer");
                playerImage.transform.GetChild(0).GetComponent<Image>().enabled = true; 
            }
            else if (Mathf.Abs((currentMenuArray +1) % menu.Length) == 3)
            {
                if (isOnline) photonView.RPC("SendVote", RpcTarget.Others, true, "quitter");
                quitVote = true;
                playerImage.transform.GetChild(1).GetComponent<Image>().enabled = true; 
            }
            else if (Mathf.Abs((currentMenuArray +1) % menu.Length) == 4)
            {
                gameObject.GetComponent<Launcher>().Connect();
                playerImage.transform.GetChild(2).GetComponent<Image>().enabled = true; 
            }
        }
    }

    public void MenuCancel(InputAction.CallbackContext context) // appuie sur O(manette) et echap
    {
        if (context.started)
        {
            if (Mathf.Abs((currentMenuArray +1) % menu.Length) == 2)
            {
                if (isOnline) photonView.RPC("SendVote", RpcTarget.Others, false, "jouer");
                playVote = false;
                playerImage.transform.GetChild(0).GetComponent<Image>().enabled = false; 
            }
            else if (Mathf.Abs((currentMenuArray +1) % menu.Length) == 3)
            {
                if (isOnline) photonView.RPC("SendVote", RpcTarget.Others, false, "quitter");
                quitVote = false;
                playerImage.transform.GetChild(1).GetComponent<Image>().enabled = false; 
            }
            else if (Mathf.Abs((currentMenuArray +1) % menu.Length) == 4)
            {
                gameObject.GetComponent<Launcher>().StopResearch();
                playerImage.transform.GetChild(2).GetComponent<Image>().enabled = false; 
            }
        }
    }

    [PunRPC]
    private void SendVote(bool result, string vote)
    {
        if (vote == "jouer")
        {
            otherPlayer.playVote = result;
            otherPlayer.playerImage.transform.GetChild(0).GetComponent<Image>().enabled = result; 
        }
        else if (vote == "quitter")
        {
            otherPlayer.quitVote = result;
            otherPlayer.playerImage.transform.GetChild(1).GetComponent<Image>().enabled = result; 
        }
    }
}
