using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    void Start()
    {   
        GameObject menuController = GameObject.FindWithTag("MenuController");      
        GameObject player1 = PlayerInput.Instantiate(menuController.GetComponent<VoteController>().player1Character, pairWithDevice: menuController.GetComponent<VoteController>().player1Device).gameObject;
        GameObject player2 = PlayerInput.Instantiate(menuController.GetComponent<VoteController>().player2Character, pairWithDevice: menuController.GetComponent<VoteController>().player2Device).gameObject;
        player1.transform.position = new Vector3(-25, -11, 0);
        player2.transform.position = new Vector3(25, -11, 0);
        player1.GetComponent<Character_Controller>().opponent = player2; // Set opponent 
        GameObject.FindWithTag("Player1Image").GetComponent<Image>().sprite = menuController.GetComponent<VoteController>().player1Image.sprite;
        player1.GetComponent<HealthBar>().healthBar = GameObject.FindWithTag("Player1HealthBar").GetComponent<Slider>(); // Set healthbar util
        player1.GetComponent<HealthBar>().healthColor = GameObject.FindWithTag("Player1HealthBar").transform.GetChild(0).GetComponent<Image>(); // Set healthbar util
        player1.GetComponent<EnergieBar>().energieBar = GameObject.FindWithTag("Player1EnergieBar").GetComponent<Slider>(); // Set energiebar util
        player1.GetComponent<EnergieBar>().energieText = GameObject.FindWithTag("Player1EnergieBar").transform.GetChild(2).GetComponent<Text>(); // Set energiebar util
        player2.GetComponent<Character_Controller>().opponent = player1; // Set opponent 
        GameObject.FindWithTag("Player2Image").GetComponent<Image>().sprite = menuController.GetComponent<VoteController>().player2Image.sprite;
        player2.GetComponent<HealthBar>().healthBar = GameObject.FindWithTag("Player2HealthBar").GetComponent<Slider>(); // Set healthbar util
        player2.GetComponent<HealthBar>().healthColor = GameObject.FindWithTag("Player2HealthBar").transform.GetChild(0).GetComponent<Image>(); // Set healthbar util
        player2.GetComponent<EnergieBar>().energieBar = GameObject.FindWithTag("Player2EnergieBar").GetComponent<Slider>(); // Set energiebar util
        player2.GetComponent<EnergieBar>().energieText = GameObject.FindWithTag("Player2EnergieBar").transform.GetChild(2).GetComponent<Text>(); // Set energiebar util
        Instantiate(menuController.GetComponent<VoteController>().map, new Vector3(0, 0, 0), Quaternion.identity);
        Destroy(menuController);
    }
    

}
