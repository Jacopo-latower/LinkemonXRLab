using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewAttack", menuName = "ScriptableObjects/LinkemonAttack", order = 1)]
public class LinkemonAttack : ScriptableObject
{
    public string attackName;
    public Linkemon.LinkemonType attackType;
    public int damage = 10;
    public int strikesNum = 1;

}
