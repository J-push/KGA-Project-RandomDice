using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class SerializeDiceData
{
    public bool isFull;
    public int index;
    public int code;
    public int level;

    public SerializeDiceData(int index, bool isFull, int code, int level)
    {
        this.index = index;
        this.isFull = isFull;
        this.code = code;
        this.level = level;
    }
}


public class Utils
{
    public const int MAX_DICE_LEVEL = 6;
    public const int DICE_LAYER = 6;
    public static readonly Quaternion QI = Quaternion.identity;

    //마우스 좌표(2D상)
    public static Vector3 MousePos
    {
        get
        {
            var result = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            result.z = 0;
            return result;
        }
    }
    public static GameObject[] GetRayCastAll(int layerMask)
    {
        var mousePos = Utils.MousePos;
        mousePos.z = -100.0f;
        RaycastHit2D[] rayCastHit = Physics2D.RaycastAll(mousePos, Vector3.forward, float.MaxValue, 1 << layerMask);
        var results = Array.ConvertAll(rayCastHit, x => x.collider.gameObject);
        return results;
    }

    public static Vector2[] GetDotPositions(int level) =>
        level switch
        {
            1 => new Vector2[] { Vector2.zero },
            2 => new Vector2[] { new Vector2(-0.04f, -0.04f), new Vector2(0.04f, 0.04f) },
            3 => new Vector2[] { new Vector2(-0.04f, -0.04f), Vector2.zero, new Vector2(0.04f, 0.04f) },
            4 => new Vector2[] { new Vector2(-0.04f, -0.04f), new Vector2(-0.04f, 0.04f), new Vector2(0.04f, -0.04f), new Vector2(0.04f, 0.04f) },
            5 => new Vector2[] { new Vector2(-0.04f, -0.04f), new Vector2(-0.04f, 0.04f), Vector2.zero, new Vector2(0.04f, -0.04f), new Vector2(0.04f, 0.04f) },
            6 => new Vector2[] { new Vector2(-0.04f, -0.04f), new Vector2(-0.04f, 0.0f), new Vector2(-0.04f, 0.04f), new Vector2(0.04f, -0.04f), new Vector2(0.04f, 0.04f), new Vector2(0.04f, 0.0f) },
            _ => new Vector2[] { Vector2.zero}
        };

    public static readonly Vector2[] enemyWay = new Vector2[]
    {
        new Vector2(-2.2f, -3.0f),
        new Vector2(-2.2f, 0.345f),
        new Vector2(2.5f, 0.345f),
        new Vector2(2.5f, -3.0f),
        new Vector2(0.35f, 0.345f),
        new Vector2(-2.2f, -3.0f)
    };
    public static readonly float startWave = 5.0f;
    public static readonly WaitForSeconds delayWave = new WaitForSeconds(1);
    public static readonly WaitForSeconds delayDiceBulletSpawn = new WaitForSeconds(1);

    public static int TotalAttackDamage(int basicAttackDamage, int level)
    {
        int result = basicAttackDamage + level * 3;
        return result;
    }
}
 