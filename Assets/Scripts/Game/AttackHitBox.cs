using UnityEngine;

public class AttackHitBox : MonoBehaviour
{
    [HideInInspector] public Character_Controller cc;   // soi même

    private void Awake() 
    { // on récupére le script character controller de soi même 
        cc = gameObject.GetComponentInParent<Character_Controller>(); 
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {   
        if (other.CompareTag("Player"))
        {
            if (gameObject.CompareTag("Kikoha"))
            {
                cc.SetOpponentDmg(10); // on appelle la méthode de celui qui attaque pour enlever les points de vie de celui qui se fait attaquer
            }
            else if (gameObject.CompareTag("SpecialAttack"))
            {
                cc.SetOpponentDmg(50); // on appelle la méthode de celui qui attaque pour enlever les points de vie de celui qui se fait attaquer
            }
            else
            {
                cc.hit = true;
                cc.SetOpponentDmg(0);
            } 
            other.GetComponentInChildren<Character_Controller>().GotAttacked(); 
        }
    } 

    private void OnTriggerExit2D(Collider2D other) 
    {   
        if (other.CompareTag("Player"))
        {
            if (!gameObject.CompareTag("Kikoha") && !gameObject.CompareTag("SpecialAttack"))
            {
                cc.hit = false;
            }
        }
    } 
}
