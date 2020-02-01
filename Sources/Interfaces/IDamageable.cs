using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DamageType
{
    Shoot,
    Collision
}

public interface IDamageable
{
    void TakeDamage(float damage, DamageType type);
    void Death();
}
