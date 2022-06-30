using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class DiceData
{
    public int code;
    public Sprite sprite;
    public Color color;
    public int basicAttackDamage;
}

[CreateAssetMenu(fileName = "DiceSO", menuName = "ScriptableObject/DiceSO")]
public class DiceSO : ScriptableObject
{
    public DiceData[] diceDatas;
    public Vector2[] originDicePositions;
    public DiceData GetDiceData(int code) => Array.Find(diceDatas, x => x.code == code);
    public DiceData GetRandomDiceData() => diceDatas[UnityEngine.Random.Range(1,diceDatas.Length)];
    public Vector2 GetOriginDicePosition(int index) => originDicePositions[index];
}
