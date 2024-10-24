using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParryHitBox : MonoBehaviour
{
    [HideInInspector] public Character_Controller cc; 

    private void Awake() 
    { 
        cc = gameObject.GetComponentInParent<Character_Controller>(); 
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {   
        if (cc.canParry)
        {
            if (other.CompareTag("Attack"))
            {
                other.GetComponentInChildren<Collider2D>().isTrigger = false; 
                other.GetComponentInParent<Rigidbody2D>().isKinematic = true;
            }
        }
    } 

    private void OnTriggerExit2D(Collider2D other) 
    {   
        if (other.CompareTag("Attack"))
        {
            other.GetComponentInChildren<Collider2D>().isTrigger = true; 
            other.GetComponentInParent<Rigidbody2D>().isKinematic = false;
        }
    } 
}
