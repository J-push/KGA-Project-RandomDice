using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Enemy : MonoBehaviour
{
    public TMP_Text healthTMP;
    public float speed;
    public int health;
    private int wayNum;
    public float distance;
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

        if(Health <= 0 && gameObject.activeSelf)
        {
            // Á×À½
            Debug.Log("Á×À½");
            GameManager.Instance.TotalSP += 20;
            gameObject.SetActive(false);
        }
    }

    public void Start()
    {
        StartCoroutine(MovePathCo());
    }

    private void OnEnable()
    {
        health = 100;
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
                Debug.Log("µµÂø");
                GameManager.Instance.DecreaseHeart();
                gameObject.SetActive(false);
                yield break;
            }

            yield return null;
        }
    }
    public void MovePathCenter()
    {
        transform.position = Vector2.MoveTowards(transform.position, Utils.enemyWay[4], speed * Time.deltaTime);
        distance += speed * Time.deltaTime;
        if ((Vector2)transform.position == Utils.enemyWay[4])
        {
            transform.position = Vector2.MoveTowards(transform.position, Utils.enemyWay[5], speed * Time.deltaTime);
            distance += speed * Time.deltaTime;
            gameObject.SetActive(false);
        }
    }

    public void StopMovePath()
    {
        StopCoroutine(MovePathCo());
    }


    private void OnDisable()
    {
        distance = 0;
        wayNum = 0;
        GameManager.Instance.enemies.Remove(this);
        ObjectPooler.ReturnToPool(gameObject);
        CancelInvoke();
    }
}

