using UnityEngine.UI;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public Slider healthBar; // référence 
    public Image healthColor; // référence pour la couleur de la barre de vie 

    public int GetHealth()
    {
        return (int)healthBar.value;  // retourne le nombre de hp 
    }

    public void SetHealth(int health)
    {
        healthBar.value = health; // modifie le nombre de hp 
        SetColor(health);
    }

    public void SetColor(int health)
    {
        int red = (int)(255 * (1 - healthBar.value / healthBar.maxValue)); // formule mathématique 
        int green = (int)(255 * (healthBar.value / healthBar.maxValue));
        healthColor.color = new Color32((byte)red, (byte)green, 0, 255); // changer la couleur en fonction du nombre de hp avec rgba a = transparence
    }

    
}
