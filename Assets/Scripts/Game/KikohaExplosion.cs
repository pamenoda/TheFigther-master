using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KikohaExplosion : MonoBehaviour
{
    [SerializeField] private AudioSource kikoExplosion;
   
    private void OnTriggerEnter2D(Collider2D other) 
    {   
       if (other.CompareTag("Player"))
       {
            kikoExplosion.Play();
       }
       else if (other.CompareTag("Kikoha"))
       {
            kikoExplosion.Play();
            Destroy(gameObject);

       }
       else if (other.CompareTag("SpecialAttack"))
       {
            kikoExplosion.Play();
            Destroy(gameObject);

       }
    } 

}
