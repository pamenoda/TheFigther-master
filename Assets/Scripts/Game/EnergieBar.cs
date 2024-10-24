using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnergieBar : MonoBehaviour
{
    public Slider energieBar; // référence 
    public Text energieText;

    public int GetEnergie()
    {
        return (int)energieBar.value;  // retourne le nombre de hp 
    }

    public void SetEnergie(int energie)
    {
        energieBar.value = energie; // modifie le nombre de hp 
        energieText.text = (energieBar.value / energieBar.maxValue * 100).ToString("F0") + "%";
    }


}
