using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewAttack", menuName = "ScriptableObjects/LinkemonAttack", order = 1)]
public class LinkemonAttack : ScriptableObject
{
    public enum LinkemonAttackGenre { Damage, Attack, Defense, Elusion, Speed, OpponentAttack, OpponentDefense, OpponentElusion, OpponentSpeed, RestoreHealth }


    public string attackName;
    public LinkemonAttackGenre attackGenre;
    public Linkemon.LinkemonType attackType;
    public int value = 10;
    public int attackSpeed = 10;
    public int strikesNum = 1;
    public float successValue = 1f;
    public int ppValue = 10;

    public string GetAttackGenreName()
    {
        string ret = "";
        switch (attackGenre) 
        {
            case LinkemonAttackGenre.Damage:
                ret = "danno";
                break;
            case LinkemonAttackGenre.Attack:
            case LinkemonAttackGenre.OpponentAttack:
                ret = "attacco";
                break;
            case LinkemonAttackGenre.Defense:
            case LinkemonAttackGenre.OpponentDefense:
                ret = "difesa";
                break;
            case LinkemonAttackGenre.Speed:
            case LinkemonAttackGenre.OpponentSpeed:
                ret = "velocità";
                break;
            case LinkemonAttackGenre.Elusion:
            case LinkemonAttackGenre.OpponentElusion:
                ret = "elusione";
                break;
            case LinkemonAttackGenre.RestoreHealth:
                ret = "cura";
                break;
        }

        return ret;
    }

    public bool IsSelf()
    {
        bool ret = false;

        if(attackGenre == LinkemonAttackGenre.Attack ||
            attackGenre == LinkemonAttackGenre.Defense ||
            attackGenre == LinkemonAttackGenre.Elusion ||
            attackGenre == LinkemonAttackGenre.Speed ||
            attackGenre == LinkemonAttackGenre.RestoreHealth
            )
        {
            ret = true;
        }
            return ret;
    }

}
