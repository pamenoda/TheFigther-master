using UnityEngine;
using System.Collections;


public class SpecialAttack : MonoBehaviour
{
    [SerializeField] private float scale; // champ fixe définit
    [SerializeField] private float positionX; // champ fixe définit 
    [SerializeField] private SpriteRenderer kamehameha; // image du kamehameha 
    private int alpha = 255; // transparence rgba 
    private float maxScaleX = 12; // le scale maximum en x 
    private float speedRatio = 10f; // le ratio de la vitesse en x 
    private float speedRatioCompetition = 3f; // le ratio de la vitesse en x 
    private float delay = 5.0f;
    private Coroutine destroy;
    private bool hasCollided = false;
    private SpecialAttack otherSpecialAttack;
    public int powerSpecialAttack;
    [HideInInspector] public Animator anim; 
    [SerializeField] private AudioSource specialAttackCollision;
    [SerializeField] private AudioSource specialAttackExplosion;
    private UnityEngine.Rendering.Universal.Light2D light; 

    void Start() 
    {
        light = GameObject.FindGameObjectsWithTag("Light")[0].GetComponentInChildren<UnityEngine.Rendering.Universal.Light2D>(); // Reference vers le composant Light2D de l'objet Global light 2d de la scene
        destroy = StartCoroutine(Destroy()); // Coroutine pour détruire la special attack
        if (!GetComponent<SpriteRenderer>().flipX) // si flipx true position -x 
        {
            positionX = -positionX;    
            gameObject.GetComponent<Collider2D>().offset  = new Vector2(-gameObject.GetComponent<Collider2D>().offset.x, gameObject.GetComponent<Collider2D>().offset.y);
        }
        
    }
    void FixedUpdate()
    {
        if (!hasCollided)
        {
            if (transform.localScale.x < maxScaleX) 
            {
                transform.position = new Vector3(transform.position.x + positionX * speedRatio, transform.position.y, transform.position.z); // on avance en x 
                transform.localScale = new Vector3(transform.localScale.x + scale * speedRatio, transform.localScale.y, transform.localScale.z); // on augmente le scale en x
            }
            else
            {   // on avance en x mais plus vite une fois le scale maximum atteint 
                transform.position = new Vector3(transform.position.x + positionX * speedRatio * 2, transform.position.y, transform.position.z); 
                if (alpha > 0) alpha -= 2; // alpha = transparence rgba Alpha vaut 255 de base
                kamehameha.color = new Color32(255, 255, 255, (byte)alpha);// la transparence diminue petit a petit et donc devient invisible a la fin 
            }
        }
        else
        {
            light.intensity = Mathf.PingPong(Time.time, 1f) + 1; // Effet visuel de luminosité 
            //powerSpecialAttack = Random.Range(1, 101); // Random pour savoir quelle special attack gagne
            if (otherSpecialAttack != null)
            {
                if (transform.localScale.x > otherSpecialAttack.transform.localScale.x)
                {
                    GetComponent<SpriteRenderer>().sortingOrder = 1; // Special attack la plus grande au premier plan
                }
                else
                {
                    GetComponent<SpriteRenderer>().sortingOrder = 0; // Special attack la moins grande au premier plan
                }
            }
            
            if (powerSpecialAttack > otherSpecialAttack.powerSpecialAttack) 
            {
                
                if (transform.localScale.x < maxScaleX) 
                {
                    transform.position = new Vector3(transform.position.x + positionX * speedRatioCompetition, transform.position.y, transform.position.z); // on avance en x 
                    transform.localScale = new Vector3(transform.localScale.x + scale * speedRatioCompetition, transform.localScale.y, transform.localScale.z); // on augmente le scale en x
                }
                else
                {   // taille maximum atteinte, la special attack avance mais ne grandit plus 
                    transform.position = new Vector3(transform.position.x + positionX * speedRatioCompetition * 2, transform.position.y, transform.position.z); 
                }
            }
            else
            {
                if (transform.localScale.x < 0) 
                {
                    Destroy(gameObject); 
                }
                if (transform.localScale.x < maxScaleX) 
                {
                    transform.position = new Vector3(transform.position.x - positionX * speedRatioCompetition, transform.position.y, transform.position.z); // on recule en x 
                    transform.localScale = new Vector3(transform.localScale.x - scale * speedRatioCompetition, transform.localScale.y, transform.localScale.z); // on reduit le scale en x
                }
            }
        }
    }

    private IEnumerator Destroy()
    {
        yield return new WaitForSeconds(delay); // Attend 5 sec
        Destroy(gameObject); // Detruit l'object
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.gameObject.CompareTag("SpecialAttack")) 
        {
            anim.SetBool("HoldSpecial", true);
            if (!specialAttackCollision.isPlaying) specialAttackCollision.Play();
            StopCoroutine(destroy); // Annule la coroutine si la specialattack entre en colision avec une autre special attack
            hasCollided = true;
            otherSpecialAttack = other.GetComponentInChildren<SpecialAttack>();
        }

        if (other.gameObject.CompareTag("Player")) 
        {
            anim.SetBool("HoldSpecial", false);
            if (specialAttackCollision.isPlaying) specialAttackCollision.Stop();
            if (!specialAttackExplosion.isPlaying) specialAttackExplosion.Play();
            otherSpecialAttack.anim.SetBool("HoldSpecial", false);
            Destroy(otherSpecialAttack.gameObject);
            Destroy(gameObject, 5);
            light.intensity = 1;
        } 
    }

    private void OnTriggerExit2D(Collider2D other) 
    {
        if (other.gameObject.CompareTag("SpecialAttack")) 
        {
            hasCollided = false;
        } 
    }
}
