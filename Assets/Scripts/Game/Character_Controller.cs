using UnityEngine.InputSystem;
using UnityEngine;
using System;
using System.Collections;
using UnityEngine.SceneManagement; 
using TMPro;
using Photon.Realtime; 
using Photon.Pun;
using UnityEngine.UI;

public class Character_Controller : MonoBehaviourPunCallbacks
{      
    private float movementX; // déplacement en x 
    [SerializeField] private float movespeed; // serialize pour pouvoir modifier sa valeur dans l'éditeur
    private bool canMove; // peut bouger ou pas
    private bool canBlock; // peu bloquer ou pas 
    private bool canJump; // touche sol ou pas  
    private bool canSpecialAttack;
    private bool canPunch;
    private bool canKick;
    private bool canCharge; // peut charger le ki 
    public bool canParry; // peut parrer
    public bool hit; // Pour declencher le son si on touche l'attaque
    private float tempKi; // charge 
    [SerializeField] private float jumpForce;
    [SerializeField] private float directionJumpForce;
    [SerializeField] private float fallMultiplier;
    [SerializeField] private float lowJumpMultiplier;
    public int damage; // damage 
    private bool  waitingForPunch = false;
    [SerializeField] private GameObject specialAttackPrefab;
    [SerializeField] private GameObject specialAttackPosition;
    [SerializeField] private GameObject kikohaPosition;
    public GameObject specialAttack;
    [SerializeField] private GameObject kikohaPrefab;
    [SerializeField] private Rigidbody2D rb; 
    public bool lookingRight;
    [HideInInspector] public GameObject opponent; // l'adversaire en face de moi 
    [SerializeField] private Animator anim; // animation pointeur
    private bool isOnline;

    void Awake()
    {
        Application.targetFrameRate = 60;
    }

    private void Start() // appeler juste après start awake = se reveiller au tout début
    { 
        if (SceneManager.GetActiveScene().name == "GameOnline")
        {
            isOnline = true;
        }
    }

    public void SetOnlinePlayer(int idPhotonP1, int idPhotonP2)
    {
        photonView.RPC("SendOnlinePlayer", RpcTarget.All, idPhotonP1, idPhotonP2);
    }

    [PunRPC]
    public void SendOnlinePlayer(int idPhotonP1, int idPhotonP2)
    {
        GameObject player1 = PhotonView.Find(idPhotonP1).gameObject;
        GameObject player2 = PhotonView.Find(idPhotonP2).gameObject;
        SetProperty(player1, player2);
    }

    public void SetProperty(GameObject player1, GameObject player2)
    {
        GameObject menuController = GameObject.FindWithTag("MenuController");
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
        Destroy(menuController);
    }

    public void UpdateOnlineHealhBar()
    {
        photonView.RPC("SendHealhBar", RpcTarget.Others, gameObject.GetComponentInChildren<HealthBar>().GetHealth());
    }

    [PunRPC]
    public void SendHealhBar(int health)
    {
        gameObject.GetComponentInChildren<HealthBar>().SetHealth(health); 
    }

    private void FixedUpdate() // fixupdate pour fixer le nombre de rafraichissement par seconde 
    {   
        if (isOnline && gameObject.GetComponent<PlayerInput>().enabled == false)
        {
            if (gameObject.GetComponent<PhotonView>().IsMine)
            {
                gameObject.GetComponent<PlayerInput>().enabled = true;
                InvokeRepeating("UpdateOnlineHealhBar", 0f, 1f / 1);
            }
        }
        SetAvailableAction();
        if (waitingForPunch)
        {
            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Punch1"))
            {
                anim.SetInteger("PunchCounter", 2);
                anim.SetTrigger("Punch");
                waitingForPunch = false;
            }   
        }
        Facing(opponent); // on appele a chaque frame pour être l'un en face de l'autre 

        if (canMove) // déplacement et Animation 
        {                                                                            
            transform.position = new Vector2(transform.position.x + movementX * movespeed * Time.deltaTime, transform.position.y);
            
            if (opponent.transform.position.x > transform.position.x) // dans le cas ou l'adversaire se trouve à droite 
            {
                RunAnimation("RunFront", "RunBack");
            }
            else
            {
                RunAnimation("RunBack", "RunFront");
            }
        }

        if (rb.velocity.y < 0 ) // si  en train de tomber 
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime; // optimisation du saut plus réaliste
            anim.ResetTrigger("Jump");
            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("HighKick") && !anim.GetCurrentAnimatorStateInfo(0).IsName("LowKickw"))
            {
                anim.SetTrigger("Fall");
            }
        }
        else if (rb.velocity.y > 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime; // optimisation du saut plus réaliste 
        } 

        
        if (gameObject.GetComponentInChildren<HealthBar>().GetHealth() <= 0)
        {
            anim.SetBool("IsDead", true);
            FindObjectOfType<TextMeshProUGUI>().enabled = true;
            //GameObject.FindWithTag("End").GetComponent<Text>().enabled = true;
            StartCoroutine(WaitDeath());
        }
        if (opponent.GetComponentInChildren<HealthBar>().GetHealth() <= 0)
        {
            anim.SetBool("KiCharge", true);
        }
    }

    [PunRPC]
    private void SendPosition(float x, float y)
    {
        transform.position = new Vector2(x, y);
    }

    IEnumerator WaitDeath()
    {
        yield return new WaitForSeconds(5);
        if (isOnline)
        {
            SceneManager.LoadScene("MenuOnline");
        }
        else
        {
            SceneManager.LoadScene("Menu");
        }
        
    }
    
    public void SetAvailableAction()
    {  
        switch(anim.GetCurrentAnimatorClipInfo(0)[0].clip.name)
        {
            case "Idle":
                canSpecialAttack = true;
                canPunch = true;
                canKick = true;
                canCharge = true;
                canBlock = true;
                break;
            case "Guard":
                canSpecialAttack = false;
                canPunch = false;
                canKick = false;
                canMove = false;
                canCharge = false;
                canJump = false;
                break;
            case "GuardFloor":
                canSpecialAttack = false;
                canPunch = false;
                canKick = false;
                canMove = false;
                canCharge = false;
                canJump = false;
                break;
            case "KiCharge":
                canSpecialAttack = false;
                canPunch = false;
                canKick = false;
                canMove = false;
                tempKi += 0.5f;
                if (Mathf.Approximately(tempKi, Mathf.Round(tempKi)))
                {
                    gameObject.GetComponent<EnergieBar>().SetEnergie(gameObject.GetComponent<EnergieBar>().GetEnergie() + 1);
                    tempKi = 0;
                }
                canJump = false;
                break;
            case "Jump":
                canCharge = false;
                canBlock = false;
                canPunch = false;
                canKick = true;
                canSpecialAttack = false;
                break;
            case "Fall":
                canCharge = false;
                canBlock = false;
                canPunch = false;
                canKick = true;
                canSpecialAttack = false; 
                break;
            case "Land":
                canCharge = false;
                canBlock = false;
                canSpecialAttack = false;
                canPunch = false;
                canKick = false;
                break;
            case "Punch1":
                canCharge = false;
                canBlock = false;
                canJump = false;
                canMove = false;
                break;
            case "Punch2":
                canCharge = false;
                canBlock = false;
                canJump = false;
                canMove = false;
                canSpecialAttack = false;
                canPunch = false;
                canKick = false;
                break;
            case "HighKick":
                canCharge = false;
                canBlock = false;
                canJump = false;
                canMove = false;
                canSpecialAttack = false;
                canPunch = false;
                canKick = false;
                break;
            case "LowKick":
                canCharge = false;
                canBlock = false;
                canJump = false;
                canMove = false;
                canSpecialAttack = false;
                canPunch = false;
                canKick = false;
                break;
            case "SpecialAttack":
                canCharge = false;
                canBlock = false;
                canSpecialAttack = false;
                canPunch = false;
                canKick = false;
                canJump = false;
                canMove = false;
                break;
            case "HoldSpecialAttack":
                canCharge = false;
                canBlock = false;
                canSpecialAttack = false;
                canPunch = false;
                canKick = false;
                canJump = false;
                canMove = false;
                break;
            case "Kikoha":
                canCharge = false;
                canBlock = false;
                canSpecialAttack = false;
                canPunch = false;
                canKick = false;
                canJump = false;
                canMove = false;
                break;
            case "Death":
                canCharge = false;
                canBlock = false;
                canSpecialAttack = false;
                canPunch = false;
                canKick = false;
                canJump = false;
                canMove = false;
                break;
        } 
    }
    public void RunAnimation(string front, string back) // front = droite si opposant à droite et gauche si opposant à gauche 
    {
        if (movementX > 0) // dans le cas ou on veut se déplacer à droite
        {
            anim.SetFloat(front, movementX);
            anim.SetFloat(back, 0);
        }
        else if (movementX < 0) // on veut aller a gauche
        {
            anim.SetFloat(back, Mathf.Abs(movementX));
            anim.SetFloat(front, 0);
        }
        else // animation pas bouger 
        {
            anim.SetFloat(front, 0); // annulle animation de course
            anim.SetFloat(back, 0); // idem 
        }
    }


    public void Facing(GameObject opponent) // faire face a l'opposant constamment 
    {
        Vector2 directionPlayer = opponent.transform.position - transform.position;
        lookingRight = opponent.transform.position.x > transform.position.x ? true : false;
        if(directionPlayer.x > 0 && transform.localScale.x < 0 || directionPlayer.x < 0 && transform.localScale.x > 0)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
    }
    
    // type InputAction.callbackcontext renvoi l'événement qui va appeler ma fonction 
    public void Movement(InputAction.CallbackContext context) 
    {
        movementX = context.ReadValue<Vector2>().x; // valeur du context vaut soit 1 => aller a droite  soit -1 => aller a gauche      
        if (isOnline)
        {
            photonView.RPC("SendPosition", RpcTarget.Others, transform.position.x, transform.position.y);
            photonView.RPC("SendMovement", RpcTarget.Others, movementX);
        }
    }

    [PunRPC]
    private void SendMovement(float value)
    {
        gameObject.GetComponent<Character_Controller>().movementX = value;
    }
   
    public void Jump(InputAction.CallbackContext context)
    {  
        if (canJump && context.started) 
        {    
            anim.ResetTrigger("Land");
            rb.AddForce(new Vector2(movementX * directionJumpForce, jumpForce), ForceMode2D.Impulse); 
            anim.SetTrigger("Jump");
            if (isOnline)
            {
                photonView.RPC("SendPosition", RpcTarget.Others, transform.position.x, transform.position.y);
                photonView.RPC("SendJump", RpcTarget.Others);
            }
        }  
    }

    [PunRPC]
    private void SendJump()
    {
        gameObject.GetComponent<Character_Controller>().anim.ResetTrigger("Land");
        gameObject.GetComponent<Character_Controller>().rb.AddForce(new Vector2(movementX * directionJumpForce, jumpForce), ForceMode2D.Impulse); 
        gameObject.GetComponent<Character_Controller>().anim.SetTrigger("Jump");
    }

    public void Punch(InputAction.CallbackContext context) // événement appuie sur E
    {
        if (context.started && canPunch)
        {
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Punch1"))
            {
                waitingForPunch = true;
            }
            else if (!waitingForPunch)
            {
                anim.SetInteger("PunchCounter", 1);
                anim.SetTrigger("Punch");
            }
            if (isOnline)
            {
                photonView.RPC("SendPosition", RpcTarget.Others, transform.position.x, transform.position.y);
                photonView.RPC("SendPunch", RpcTarget.Others);
            }
        }     
    }

    [PunRPC]
    private void SendPunch()
    {
        if (gameObject.GetComponent<Character_Controller>().anim.GetCurrentAnimatorStateInfo(0).IsName("Punch1"))
        {
            gameObject.GetComponent<Character_Controller>().waitingForPunch = true;
        }
        else if (!gameObject.GetComponent<Character_Controller>().waitingForPunch)
        {
            gameObject.GetComponent<Character_Controller>().anim.SetInteger("PunchCounter", 1);
            gameObject.GetComponent<Character_Controller>().anim.SetTrigger("Punch");
        }
    }

    public void SetOpponentDmg(int isSpecialAttack)
    { // méthode qui applique les damages a la barre de vie de l'opposant
        if (isSpecialAttack == 0) 
        {
            opponent.GetComponentInChildren<HealthBar>().SetHealth(opponent.GetComponentInChildren<HealthBar>().GetHealth()-damage); 
        }
        else
        {
            opponent.GetComponentInChildren<HealthBar>().SetHealth(opponent.GetComponentInChildren<HealthBar>().GetHealth()-isSpecialAttack); 
        } 
    }

    public void SetAttackDamage(int dmg)
    { // set les damages en fonction des attaques dans les événements des animations 
        damage = dmg ;
    }


    public void Kick(InputAction.CallbackContext context) // événement appuie sur A
    {   
        if (context.started && canKick)
        {
            anim.SetBool("HighKick", true);
            if (isOnline)
            {
                photonView.RPC("SendPosition", RpcTarget.Others, transform.position.x, transform.position.y);
                photonView.RPC("SendKick", RpcTarget.Others);
            }
        } 
    }

    [PunRPC]
    private void SendKick()
    {
        gameObject.GetComponent<Character_Controller>().anim.SetBool("HighKick", true);
    }

    public void KickDown(InputAction.CallbackContext context) // événement de kick du bas sur v 
    {
        if (context.started && canKick)
        {
            anim.SetBool("LowKick", true);
            if (isOnline)
            {
                photonView.RPC("SendPosition", RpcTarget.Others, transform.position.x, transform.position.y);
                photonView.RPC("SendKickDown", RpcTarget.Others);
            }
        }
    }

    [PunRPC]
    private void SendKickDown()
    {
        gameObject.GetComponent<Character_Controller>().anim.SetBool("LowKick", true);
    }

    public void ResetHighKickBool()
    {
        anim.SetBool("HighKick", false);
    }

    public void ResetLowKickBool()
    {
        anim.SetBool("LowKick", false);
    }


    public void KiCharge(InputAction.CallbackContext context)
    {
        if (context.performed && canCharge)
        {
            anim.SetBool("KiCharge",true);  
            if (isOnline)
            {
                photonView.RPC("SendPosition", RpcTarget.Others, transform.position.x, transform.position.y);
                photonView.RPC("SendKiCharge", RpcTarget.Others, true);
            }
        } 
        else
        {
            anim.SetBool("KiCharge",false);
            gameObject.GetComponent<AnimationSounds>().ChargeKiSoundCancel();
            if (isOnline)
            {
                photonView.RPC("SendPosition", RpcTarget.Others, transform.position.x, transform.position.y);
                photonView.RPC("SendKiCharge", RpcTarget.Others, false);
            }
        }
        
    }

    [PunRPC]
    private void SendKiCharge(bool value)
    {
        gameObject.GetComponent<Character_Controller>().anim.SetBool("KiCharge", value); 
        if (value == false) 
        {
            gameObject.GetComponent<AnimationSounds>().ChargeKiSoundCancel();
        }
    }

    public void GuardFloor(InputAction.CallbackContext context)
    {
        if (context.performed && canBlock)
        {             
            anim.SetBool("GuardFloor",true);
            if (isOnline)
            {
                photonView.RPC("SendPosition", RpcTarget.Others, transform.position.x, transform.position.y);
                photonView.RPC("SendGuardFloor", RpcTarget.Others, true);
            }
        }   
        else 
        {
            anim.SetBool("GuardFloor",false);   
            if (isOnline)
            {
                photonView.RPC("SendPosition", RpcTarget.Others, transform.position.x, transform.position.y);
                photonView.RPC("SendGuardFloor", RpcTarget.Others, false);
            }
        }
    }

    [PunRPC]
    private void SendGuardFloor(bool value)
    {
        gameObject.GetComponent<Character_Controller>().anim.SetBool("GuardFloor",value);
    }

    public void Guard(InputAction.CallbackContext context)
    {
        if (context.performed && canBlock)
        {   
            anim.SetBool("Guard",true);
            if (isOnline)
            {
                photonView.RPC("SendPosition", RpcTarget.Others, transform.position.x, transform.position.y);
                photonView.RPC("SendGuard", RpcTarget.Others, true);
            }
        }
        else
        {
            anim.SetBool("Guard",false); 
            if (isOnline)
            {
                photonView.RPC("SendPosition", RpcTarget.Others, transform.position.x, transform.position.y);
                photonView.RPC("SendGuard", RpcTarget.Others, false);
            }
        }
    }

    [PunRPC]
    private void SendGuard(bool value)
    {
        gameObject.GetComponent<Character_Controller>().anim.SetBool("Guard",value);
    }

    public void SpecialAttack(InputAction.CallbackContext context)
    {
        if (context.started && canSpecialAttack)
        {
            if (gameObject.GetComponent<EnergieBar>().GetEnergie() >= 50) // verifie si assez de ki pour l'attaque
            { 
                gameObject.GetComponent<EnergieBar>().SetEnergie(gameObject.GetComponent<EnergieBar>().GetEnergie()-50); // retrait de ki
                anim.SetBool("SpecialAttack",true);
                if (isOnline)
                {
                    photonView.RPC("SendPosition", RpcTarget.Others, transform.position.x, transform.position.y);
                    photonView.RPC("SendSpecialAttack", RpcTarget.Others);
                }
            } 

        }     
    }

    [PunRPC]
    private void SendSpecialAttack()
    {
        gameObject.GetComponent<EnergieBar>().SetEnergie(gameObject.GetComponent<EnergieBar>().GetEnergie()-50);
        gameObject.GetComponent<Character_Controller>().anim.SetBool("SpecialAttack",true);
    }

    public void SpamSpecialAttack(InputAction.CallbackContext context)
    {
        if (context.started && anim.GetCurrentAnimatorClipInfo(0)[0].clip.name == "HoldSpecialAttack")
        {
            specialAttack.GetComponent<SpecialAttack>().powerSpecialAttack ++;
            if (isOnline)
            {
                photonView.RPC("SendSpamSpecialAttack", RpcTarget.Others, specialAttack.transform.position.x, specialAttack.transform.localScale.x);
            }
        }     
    }

    [PunRPC]
    private void SendSpamSpecialAttack(float posX, float scaleX)
    {
        specialAttack.GetComponent<SpecialAttack>().powerSpecialAttack ++;
        specialAttack.transform.position = new Vector2(posX, specialAttack.transform.position.y);
        specialAttack.transform.localScale = new Vector3(scaleX, 2f, 1f);
    }
    
    public void Kikoha(InputAction.CallbackContext context)
    {
        if (context.started && canSpecialAttack)
        {
            if (gameObject.GetComponent<EnergieBar>().GetEnergie() >= 10) // verifie si assez de ki pour l'attaque
            { 
                anim.SetBool("Kikoha", true);
                if (isOnline)
                {
                    photonView.RPC("SendPosition", RpcTarget.Others, transform.position.x, transform.position.y);
                    photonView.RPC("SendKikoha", RpcTarget.Others);
                }
            } 
        }     
    }
    
    [PunRPC]
    private void SendKikoha()
    {
        gameObject.GetComponent<Character_Controller>().anim.SetBool("Kikoha", true);
    }

    public void ResetKikohaBool()
    {
        anim.SetBool("Kikoha", false);
    }

    public void KikohaSpawn()
    {
        gameObject.GetComponent<EnergieBar>().SetEnergie(gameObject.GetComponent<EnergieBar>().GetEnergie()-10); // retrait de ki
        GameObject kikoha = Instantiate(kikohaPrefab, new Vector3(kikohaPosition.transform.position.x, kikohaPosition.transform.position.y, kikohaPosition.transform.position.z), Quaternion.identity);
        kikoha.GetComponent<AttackHitBox>().cc = gameObject.GetComponent<Character_Controller>();
        if (!lookingRight) 
        {
            kikoha.GetComponent<SpriteRenderer>().flipX = true;
            kikoha.GetComponent<Rigidbody2D>().AddForce(Vector2.left * 0.002f, ForceMode2D.Impulse);
        }
        else
        {
            kikoha.GetComponent<Rigidbody2D>().AddForce(Vector2.right * 0.002f, ForceMode2D.Impulse);
        }
    }

    public void ResetSpecialAttackBool()
    {
        anim.SetBool("SpecialAttack", false);
    }

    public void SpecialAttackSpawn()
    {
        specialAttack = Instantiate(specialAttackPrefab, new Vector3(specialAttackPosition.transform.position.x, 
        specialAttackPosition.transform.position.y, specialAttackPosition.transform.position.z), Quaternion.identity);
        if (!lookingRight) specialAttack.GetComponent<SpriteRenderer>().flipX = false;
        specialAttack.GetComponent<AttackHitBox>().cc = gameObject.GetComponent<Character_Controller>(); // recupère le character controller 
        specialAttack.GetComponent<SpecialAttack>().anim = anim; // on recupère l'animator pour l'animation holdspecial
    }


    public void GotAttacked()
    {
        anim.SetTrigger("GotHit");
    }


    private void OnCollisionStay2D(Collision2D other) // vérifie la collision en cours (other = à l'autre object )
    {
        if (other.gameObject.CompareTag("Map")) // si le tag de l'object en collision est map 
        {
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") 
                || anim.GetCurrentAnimatorStateInfo(0).IsName("RunFront") 
                || anim.GetCurrentAnimatorStateInfo(0).IsName("RunBack")) 
            {
                canJump = true;
            } 
            canMove = true; 
            
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Fall") || anim.GetBool("Fall")) // on vérifie si l'animation de chute est en cours 
            {
                anim.ResetTrigger("Fall"); // on annule pour finir l'animation chute 
                anim.SetTrigger("Land"); // on lance l'animation d'atterissage
            }
            
        } 
        if (other.gameObject.CompareTag("BorderMap"))
        {
            canParry = false;
        }
        else if (other.gameObject.CompareTag("Player")) // si le tag de l'object en collision est player
        {   // ca veut dire qu'on se trouve en collision avec l'adversaire  et du coup on peut bouger mais pas sauter
            canMove = true;
            canJump = false;
        }
    }

    private void OnCollisionExit2D(Collision2D other) // a la sortie d'une collision 
    {  
        if (other.gameObject.CompareTag("Map") || other.gameObject.CompareTag("Player")){ // vérifie si on n'est plus en collision avec map ou joueur 

            canJump = false; // on peut pas sauter
            canMove = false; // on peut pas bouger 
            anim.SetFloat("RunFront", 0); // on annulle les animations de course
            anim.SetFloat("RunBack", 0); // idem ici 
        }
        if (other.gameObject.CompareTag("BorderMap"))
        {
            canParry = true;
        }
    }


}
