using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "NewLinkemon", menuName = "ScriptableObjects/Linkemon", order = 1)]
public class LinkemonScriptable : ScriptableObject
{
    public string lkName;
    public Sprite battleIcon;
    public AudioClip verse;

    public Linkemon.LinkemonType lType;
    public Linkemon.LinkemonType weaknessType;
    public List<LinkemonAttack> attacks;

    public int startingLife = 100;
    public int startingSpeed = 50;
    public int startingAttack = 50;
    public int startingDefense = 50;
}
