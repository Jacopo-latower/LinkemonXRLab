using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewAttack", menuName = "ScriptableObjects/LinkemonAttack", order = 1)]
public class LinkemonAttack : ScriptableObject
{
    public enum LinkemonAttackGenre { Damage, Defense, Elusion, Speed}


    public string attackName;
    public LinkemonAttackGenre attackGenre;
    public Linkemon.LinkemonType attackType;
    public int value = 10;
    public int attackSpeed = 10;
    public int strikesNum = 1;
    public float successValue = 1;

}
