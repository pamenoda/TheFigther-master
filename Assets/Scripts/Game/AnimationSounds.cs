using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationSounds : MonoBehaviour
{
    [SerializeField] private AudioSource specialAttack;
    [SerializeField] private AudioSource jump;
    [SerializeField] private AudioSource land;
    [SerializeField] private AudioSource death;
    [SerializeField] private AudioSource punch;
    [SerializeField] private AudioSource punch1;
    [SerializeField] private AudioSource lowKick;
    [SerializeField] private AudioSource highKick;
    [SerializeField] private AudioSource chargeKi;
    [SerializeField] private AudioSource kikoha;

    public void SpecialAttackSound()
    {
        specialAttack.Play();
    }

    public void JumpSound()
    {
        jump.Play();
    }

    public void LandSound()
    {
        land.Play();
    }

    public void DeathSound()
    {
        death.Play();
    }

    public void Punchound()
    {
        if (gameObject.GetComponentInParent<Character_Controller>().hit)
        {
            punch.Play();
        }
    }

    public void Punch1Sound()
    {
        if (gameObject.GetComponentInParent<Character_Controller>().hit)
        {
            punch1.Play();
        }
    }

    public void LowKickSound()
    {
        if (gameObject.GetComponentInParent<Character_Controller>().hit)
        {
            lowKick.Play();
        }
    }

    public void HighKickSound()
    {
        if (gameObject.GetComponentInParent<Character_Controller>().hit)
        {
            highKick.Play();
        }
    }

    public void ChargeKiSound()
    {
        chargeKi.Play();
    }

    public void ChargeKiSoundCancel()
    {
        chargeKi.Stop();
    }

    public void KikohaSound()
    {
        kikoha.Play();
    }

}
