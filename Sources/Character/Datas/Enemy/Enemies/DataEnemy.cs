using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataEnemy
{
    public enum EnemyType
    {
        type1,
        type2
    }

    protected float damage;
    protected float walkingSpeed;
    protected float runningSpeed;
    protected float life;
    protected float cdAttack;
    protected float stunTimer;
    protected int type;
    protected int xp;

    static private DataEnemy[] arrayOfPrefab;
    private static DataEnemy[] ArrayOfPrefab
    {
        get
        {
            return arrayOfPrefab;
        }

        set
        {
            if (arrayOfPrefab == null)
            {
                arrayOfPrefab = value;
            }
            else
            {
                throw new System.InvalidOperationException("cannot reatribuate array of prefabs");
            }
        }
      
    }

    public DataEnemy(EnemyType EnemyType)
    {
        int DataEnemyID = (int)EnemyType;

        damage = ArrayOfPrefab[DataEnemyID].damage;
        walkingSpeed = ArrayOfPrefab[DataEnemyID].walkingSpeed;
        runningSpeed = ArrayOfPrefab[DataEnemyID].runningSpeed;
        life = ArrayOfPrefab[DataEnemyID].life;
        cdAttack = ArrayOfPrefab[DataEnemyID].cdAttack;
        stunTimer = ArrayOfPrefab[DataEnemyID].stunTimer;
        type = ArrayOfPrefab[DataEnemyID].type;
        xp = ArrayOfPrefab[DataEnemyID].xp;
    }

 

    public float Damage
    {
        get
        {
            return damage;
        }

        set
        {
            damage = value;
        }
    }

    public float WalkingSpeed
    {
        get
        {
            return walkingSpeed;
        }

        set
        {
            walkingSpeed = value;
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
        }
    }

    public float CdAttack
    {
        get
        {
            return cdAttack;
        }

        set
        {
            cdAttack = value;
        }
    }

    public float StunTimer
    {
        get
        {
            return stunTimer;
        }

        set
        {
            stunTimer = value;
        }
    }

    public int Type
    {
        get
        {
            return type;
        }

        set
        {
            type = value;
        }
    }

    public int Xp
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
}
