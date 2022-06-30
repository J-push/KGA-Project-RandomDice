using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
using TMPro;
using UnityEngine.SceneManagement;

public class BossKnight : MonoBehaviour
{
    public TMP_Text healthTMP;
    public float speed;
    public int health;
    private int wayNum;
    public float distance;
    public SerializeDiceData serializeDiceData;
    public DiceData diceData => GameManager.Instance.diceSO.GetDiceData(serializeDiceData.code);
    public int Health
    {
        get => health;
        set
        {
            health = value;
            healthTMP.text = value.ToString();
        }
    }
    public void Damaged(int damage)
    {
        Health -= damage;
        Health = Mathf.Max(0, Health);

        if (Health <= 0 && gameObject.activeSelf)
        {
            // 보스죽음
            Debug.Log("보스죽음");
            //GameClear
            gameObject.SetActive(false);
        }
    }
    public void Start()
    {
        StartCoroutine(MovePathCo());
    }

    public void Update()
    {
        StartCoroutine(AccelSpeed());
        Debug.Log(speed);
    }
    private void OnEnable()
    {
        health = 10000;
    }
    IEnumerator MovePathCo()
    {
        while (true)
        {
            transform.position = Vector2.MoveTowards(transform.position, Utils.enemyWay[wayNum], speed * Time.deltaTime);
            distance += speed * Time.deltaTime;
            if ((Vector2)transform.position == Utils.enemyWay[wayNum])
            {
                wayNum++;
            }

            if (wayNum == Utils.enemyWay.Length - 2)
            {
                gameObject.SetActive(false);
                //게임오버 실행

                yield break;
            }

            yield return null;
        }
    }

    private void OnDisable()
    {
        distance = 0;
        wayNum = 0;
        GameManager.Instance.boss.Remove(this);
        ObjectPooler.ReturnToPool(gameObject);
        CancelInvoke();
    }
    //IEnumerator RemoveDices()
    //{
    //    yield return new WaitForSeconds(5.0f);  
    //    GameManager.Instance.RemoveDice();
    //}
    IEnumerator AccelSpeed()
    {
        yield return new WaitForSeconds(7f);
        speed += 0.01f;
    }
}

