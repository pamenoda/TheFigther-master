using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement; // importer la classe SceneManager
using Photon.Realtime; 
using Photon.Pun;

public class VoteController : MonoBehaviourPunCallbacks
{
    public GameObject player1Character;
    public GameObject player2Character;
    public Image player1Image;
    public Image player2Image;
    public GameObject map;
    public InputDevice player1Device;
    public InputDevice player2Device;

    void Update()
    {
        GameObject player1 = GameObject.FindWithTag("Player1");
        GameObject player2 = GameObject.FindWithTag("Player2");
        int mapPlayer2;
        int characterPlayer2;

        if (player1 != null && player2 != null)
        {
            if (player1.GetComponent<MenuController>().playVote && player2.GetComponent<MenuController>().playVote)
            {
                player1Image = player1.GetComponent<MenuController>().playerImage;
                player2Image = player2.GetComponent<MenuController>().playerImage;

                if (SceneManager.GetActiveScene().name == "MenuOnline")
                {
                    mapPlayer2 = player2.GetComponent<MenuController>().selectedMap;
                    characterPlayer2 = player2.GetComponent<MenuController>().selectedCharacter;
                    if (mapPlayer2 == -1) mapPlayer2 = 0;
                    if (characterPlayer2 == -1) characterPlayer2 = 0;
                }
                else
                {
                    mapPlayer2 = Mathf.Abs((player2.GetComponent<MenuController>().selectedMap + 1) % player2.GetComponent<MenuController>().map.Length);
                    characterPlayer2 = Mathf.Abs((player2.GetComponent<MenuController>().selectedCharacter + 1) % player2.GetComponent<MenuController>().characters.Length);
                    player1Device = player1.GetComponent<PlayerInput>().devices[0];
                    player2Device = player2.GetComponent<PlayerInput>().devices[0];
                }

                player1Character = player1.GetComponent<MenuController>().characters[Mathf.Abs((player1.GetComponent<MenuController>().selectedCharacter + 1) % player1.GetComponent<MenuController>().characters.Length)].GetComponent<GetPrefab>().prefab;
                player2Character = player2.GetComponent<MenuController>().characters[characterPlayer2].GetComponent<GetPrefab>().prefab;
                

                if (player1.GetComponent<MenuController>().map[Mathf.Abs((player1.GetComponent<MenuController>().selectedMap + 1) % player1.GetComponent<MenuController>().map.Length)] == player2.GetComponent<MenuController>().map[mapPlayer2])
                {
                    map = player1.GetComponent<MenuController>().map[Mathf.Abs((player1.GetComponent<MenuController>().selectedMap + 1) % player1.GetComponent<MenuController>().map.Length)].GetComponent<GetPrefab>().prefab;
                }
                else
                {
                    int randomNumber = Random.Range(0, 2);
                    if (randomNumber == 0)
                    {
                        map = player1.GetComponent<MenuController>().map[Mathf.Abs((player1.GetComponent<MenuController>().selectedMap + 1) % player1.GetComponent<MenuController>().map.Length)].GetComponent<GetPrefab>().prefab;
                    }
                    else
                    {
                        map = player2.GetComponent<MenuController>().map[mapPlayer2].GetComponent<GetPrefab>().prefab;
                    }
                }

                DontDestroyOnLoad(gameObject);
                if (SceneManager.GetActiveScene().name == "MenuOnline")
                {
                    if (PhotonNetwork.IsMasterClient)
                    {
                        SceneManager.LoadScene("GameOnline");
                    }
                }
                else
                {
                    SceneManager.LoadScene("Game");
                }
                
            }

            if (player1.GetComponent<MenuController>().quitVote && player2.GetComponent<MenuController>().quitVote)
            {
                Application.Quit();
            }
        }
    }
}
