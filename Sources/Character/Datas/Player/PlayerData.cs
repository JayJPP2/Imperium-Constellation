using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData
{
    private float speed;
    private float runningSpeed;
    private float life;
    private float lifeMax;
    private float jumpHeigh;
    private float stamina;
    private float staminaMax;
    private float weight;
    private float weightMax;
    private bool isCrouch;
    private bool isInTheAir;
    private bool isDead;
    private float xp;

    CreditsManager creditsPlayer = new CreditsManager();
    
    public PlayerData()
    {
        speed = 4.0f;
        runningSpeed = 8.0f;
        life = 20.0f;
        LifeMax = 20.0f;
        jumpHeigh = 5.0f;
        stamina = 100.0f;
        StaminaMax = 100.0f;
        weight = 0.0f;
        WeightMax = 20.0f;
        isCrouch = false;
        isInTheAir = false;
        isDead = false;
        Xp = 0.0f;
        creditsPlayer.credits = 0;
    }

    public int CreditsPlayer
    {
        get { return creditsPlayer.credits; }
        set { creditsPlayer.credits = value; }
    }

    public float Speed
    {
        get
        {
            return speed;
        }

        set
        {
            speed = value;
        }
    }

    public float RunningSpeed
    {
        get
        {
            return runningSpeed;
        }

        set
        {
            runningSpeed = value;
        }
    }

    public float Life
    {
        get
        {
            return life;
        }

        set
        {
            life = value;
            if (life <= 0f)
            {
                isDead = true;
            }
            else if (life > lifeMax)
            {
                life = LifeMax;
            }
        }
    }

    public float JumpHeigh
    {
        get
        {
            return jumpHeigh;
        }

        set
        {
            jumpHeigh = value;
        }
    }

    public float Stamina
    {
        get
        {
            return stamina;
        }

        set
        {
            stamina = value;
            if (stamina <= 0f)
            {
                stamina = 0f;
            }
            else if (stamina > staminaMax)
            {
                stamina = staminaMax;
            }
        }
    }

    public float Weight
    {
        get
        {
            return weight;
        }

        set
        {
            weight = value;
        }
    }

    public bool IsCrouch
    {
        get
        {
            return isCrouch;
        }

        set
        {
            isCrouch = value;
        }
    }

    public bool IsInTheAir
    {
        get
        {
            return isInTheAir;
        }

        set
        {
            isInTheAir = value;
        }
    }

    public bool IsDead
    {
        get
        {
            return isDead;
        }

        set
        {
            isDead = value;
        }
    }

    public float Xp
    {
        get
        {
            return xp;
        }

        set
        {
            xp = value;
        }
    }

    public float WeightMax
    {
        get
        {
            return weightMax;
        }

        set
        {
            weightMax = value;
        }
    }

    public float StaminaMax
    {
        get
        {
            return staminaMax;
        }

        set
        {
            staminaMax = value;
        }
    }

    public float LifeMax
    {
        get
        {
            return LifeMax1;
        }

        set
        {
            LifeMax1 = value;
        }
    }

    public float LifeMax1
    {
        get
        {
            return lifeMax;
        }

        set
        {
            lifeMax = value;
        }
    }
}
