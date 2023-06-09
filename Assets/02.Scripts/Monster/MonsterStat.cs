using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="MonsterData",menuName="Scriptable Object/MonsterData",order=int.MaxValue)]
public class MonsterStat : ScriptableObject
{
    [SerializeField]
    string monsterName;
    public string MonsterName{get{return monsterName;}}

    [SerializeField]
    int hp;
    public int HP{get{return hp;}}

    [SerializeField]
    float sightRange;
    public float SightRange{get{return sightRange;}}

    [SerializeField]
    float moveSpeed;
    public float MoveSpeed{get{return moveSpeed;}}
}


