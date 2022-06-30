using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class DiceManager : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public SerializeDiceData serializeDiceData
    {
        get;
        private set;
    }
    public Transform[] dots;
    public Order order;
    public DiceData diceData => GameManager.Instance.diceSO.GetDiceData(serializeDiceData.code);
    private int dotCount;

    public void SetUpDice(SerializeDiceData serializeDiceData)
    {
        this.serializeDiceData = serializeDiceData;
        var diceData = GameManager.Instance.diceSO.GetDiceData(serializeDiceData.code);
        spriteRenderer.sprite = diceData.sprite;    
        SetDots(serializeDiceData.level);

        for(int i = 0; i < Utils.MAX_DICE_LEVEL; i++)
        {
            dots[i].GetComponent<SpriteRenderer>().color = diceData.color; 
        }

        if (serializeDiceData.code == 0)
        {
            gameObject.SetActive(false);
        }
        if (gameObject.activeSelf)
        {
            StartCoroutine(AttackCo());
        }
    }

    public void SetDots(int level)  // 레벨에 따라서 닷 개수가 달라지므로
    {
        int dotCount = 0;

        for (int i = 0; i < Utils.MAX_DICE_LEVEL; i++)
        {
            dots[i].gameObject.SetActive(i < level);
            if (i < level)
            {
                dotCount++;
            }
            this.dotCount = dotCount;
        }

        // dot 위치 정하기
        Vector2[] positions = new Vector2[1];
        switch (level)
        {
            case 1:
                positions = new Vector2[] {Vector2.zero };
                break;
            case 2:
                positions = new Vector2[] { new Vector2(-0.04f, -0.04f), new Vector2(0.04f, 0.04f) };
                break;
            case 3:
                positions = new Vector2[] { new Vector2(-0.04f, -0.04f), Vector2.zero, new Vector2(0.04f, 0.04f) };
                break;
            case 4:
                positions = new Vector2[] { new Vector2(-0.04f, -0.04f), new Vector2(-0.04f, 0.04f), new Vector2(0.04f, -0.04f), new Vector2(0.04f, 0.04f) };
                break;
            case 5:
                positions = new Vector2[] { new Vector2(-0.04f, -0.04f), new Vector2(-0.04f, 0.04f), Vector2.zero ,new Vector2(0.04f, -0.04f), new Vector2(0.04f, 0.04f) };
                break;
            case 6:
                positions = new Vector2[] { new Vector2(-0.04f, -0.04f), new Vector2(-0.04f, 0.0f), new Vector2(-0.04f, 0.04f), new Vector2(0.04f, -0.04f), new Vector2(0.04f, 0.04f), new Vector2(0.04f, 0.0f) };
                break;
        }

        for (int i = 0; i < positions.Length; i++)
        {
            dots[i].localPosition = positions[i];
        }
    } 

    private void OnDisable()
    {
        serializeDiceData = null;
        spriteRenderer.sprite = null; 
        SetDots(0);
        for (int i = 0; i < Utils.MAX_DICE_LEVEL; i++)
        {
            dots[i].GetComponent<SpriteRenderer>().color = Color.white;
        }
        ObjectPooler.ReturnToPool(gameObject);
        CancelInvoke();
    }

    public void OnMouseDown()
    {
        order.SetMostFrontOrder(true);
    }

    public void OnMouseDrag()
    {
        transform.position = Utils.MousePos;
    }

    public void OnMouseUp()
    {
        // 원래 자리로 이동한다. (만약 같은 숫자, 다른 종류면)
        MoveTransform(GameManager.Instance.ReturnDicePosition(serializeDiceData.index), true ,0.5f, () => order.SetMostFrontOrder(false));

        // 같은 code고 같은 level이면 합쳐진다.
        GameObject[] raycastAll = Utils.GetRayCastAll(Utils.DICE_LAYER);
        GameObject targetDiceObject = Array.Find(raycastAll, x => x.gameObject != gameObject);
        if(targetDiceObject != null)
        { 
            var targetDice = targetDiceObject.GetComponent<DiceManager>();
            int nextLevel = serializeDiceData.level + 1;
            if(serializeDiceData.code == targetDice.serializeDiceData.code && serializeDiceData.level == targetDice.serializeDiceData.level && nextLevel<= Utils.MAX_DICE_LEVEL)
            {
                var targetSerializeDiceData = new SerializeDiceData(targetDice.serializeDiceData.index, true, GameManager.Instance.diceSO.GetRandomDiceData().code, nextLevel);
                targetDice.SetUpDice(targetSerializeDiceData);

                var currentSerializeDiceData = new SerializeDiceData(serializeDiceData.index,false,0,0);
                SetUpDice(currentSerializeDiceData);
            }
        }
    }

    private void MoveTransform(Vector2 targetPos, bool useDotween, float duration = 0f, TweenCallback action = null)
    {
        if (useDotween)
        {
            transform.DOMove(targetPos, duration).OnComplete(() => action.Invoke());
        }
        else
        {
            transform.position = targetPos;
        }
    }
    void OnEnable()
    {
        StartCoroutine(AttackCo());
    }

    IEnumerator AttackCo()
    {
        while (true)
        {
            var delayDiceBulletSpawn = new WaitForSeconds(1f / dotCount);
            for (int i = 0; i < dotCount; i++)
            {
                Enemy targetEnemy = GameManager.Instance.GetRandomEnemy();
                BossKnight bossKnight = GameManager.Instance.GetBoss();
                if (targetEnemy != null || bossKnight != null)
                {
                    var diceBulletObject = ObjectPooler.Inst._SpawnFromPool("diceBullet", dots[0].position, Utils.QI);
                    diceBulletObject.GetComponent<DiceBullet>().SetupDiceBullet(serializeDiceData, targetEnemy, bossKnight);
                }
            }
            yield return delayDiceBulletSpawn;
        }
    }
}
  