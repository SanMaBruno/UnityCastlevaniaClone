using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDemo : MonoBehaviour, ITargetCombat
{
    [SerializeField] int health;

    public void TakeDamage(int damagePoints)
    {
        health -= damagePoints;
    }
}
