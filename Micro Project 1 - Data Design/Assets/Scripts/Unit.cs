﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public string UnitName;
    public int UnitLevel;

    public int MaxDamage;
    public int Damage;

    public int MaxHP;
    public int CurrentHP;


    public int MaxAcc;
    public int CurrentAcc;

    public int MaxSpeed;
    public int CurrentSpeed;

    public int MaxDef;
    public int CurrentDef;

    public bool TakeDamage(int damage)
    {
        CurrentHP -= damage;

        if (CurrentHP <= 0)
            return true;
        else
            return false;
    }

    public void Heal(int amount)
    {
        CurrentHP += amount;
        if (CurrentHP > MaxHP)
        {
            CurrentHP = MaxHP;
        }
        else if(CurrentHP <= 0)
        {
            CurrentHP = 0;
        }
            
        
    }

    public void Smoke(int amount)
    {
        //lowers speed but raises defence
        CurrentSpeed -= amount;
        if (CurrentSpeed > MaxSpeed)
        {
            CurrentSpeed = MaxSpeed;
        }
        else if(CurrentSpeed <= 0)
        {
            CurrentSpeed = 0;
        }
            

        CurrentDef += amount;
        if (CurrentDef > MaxDef)
        {
            CurrentDef = MaxDef;
        }
        else if (CurrentDef <= 0)
        {
            CurrentDef = 0;
        }
    }

    public void Doppel(int amount)
    {
        //lowers acc but raises attack damage
        CurrentAcc -= amount;
        if (CurrentAcc > MaxAcc)
        {
            CurrentAcc = MaxAcc;
        }
        else if (CurrentAcc <= 0)
        {
            CurrentAcc = 0;
        }

        Damage += amount;
        if (Damage > MaxDamage)
        {
            Damage = MaxDamage;
        }
        else if (Damage <= 0)
        {
            Damage = 0;
        }
    }
}
