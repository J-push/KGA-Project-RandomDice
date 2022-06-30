using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceBullet : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public ParticleSystem _particleSystem;

    public SerializeDiceData serializeDiceData;

    public float speed;
    public DiceData diceData => GameManager.Instance.diceSO.GetDiceData(serializeDiceData.code);
    Enemy targetEnemy;
    BossKnight targetBoss;
    public void SetupDiceBullet(SerializeDiceData serializeDiceData, Enemy targetEnemy, BossKnight targetBoss)
    {
        this.serializeDiceData = serializeDiceData;
        this.targetEnemy = targetEnemy;
        this.targetBoss = targetBoss;
        spriteRenderer.enabled = true;
        spriteRenderer.color = diceData.color;
        var particleMain = _particleSystem.main;
        particleMain.startColor = diceData.color;

        StartCoroutine(AttackCo());
    }

    IEnumerator AttackCo()
    {
        while(true)
        {
            if (targetBoss == null && targetEnemy == null)
            {
                break;
            }
            else if (targetBoss != null && targetEnemy == null)
            {
                transform.position = Vector2.MoveTowards(transform.position, targetBoss.transform.position, Time.deltaTime * speed);
                yield return null;

                if ((transform.position - targetBoss.transform.position).sqrMagnitude < speed * Time.deltaTime * speed * Time.deltaTime)
                {
                    transform.position = targetBoss.transform.position;
                    break;
                }
            }
            else
            {
                transform.position = Vector2.MoveTowards(transform.position, targetEnemy.transform.position, Time.deltaTime * speed);
                yield return null;

                if ((transform.position - targetEnemy.transform.position).sqrMagnitude < speed * Time.deltaTime * speed * Time.deltaTime)
                {
                    transform.position = targetEnemy.transform.position;
                    break;
                }
            }
        }

        // 데미지를 준다.
        int totalAttackDamage = Utils.TotalAttackDamage(diceData.basicAttackDamage, serializeDiceData.level);
        if (targetEnemy != null)
        {
            targetEnemy.Damaged(totalAttackDamage);
            var damageTMP = ObjectPooler.Inst._SpawnFromPool("damageTMP", targetEnemy.transform.position, Utils.QI).GetComponent<DamageTMP>();
            damageTMP.GetComponent<DamageTMP>().SetUp(targetEnemy.transform, totalAttackDamage);
            GameManager.Instance.damageTMPs.Add(damageTMP);
        }
        if (targetBoss != null)
        {
            targetBoss.Damaged(totalAttackDamage);
            var damageTMP = ObjectPooler.Inst._SpawnFromPool("damageTMP", targetBoss.transform.position, Utils.QI).GetComponent<DamageTMP>();
            damageTMP.GetComponent<DamageTMP>().SetUp(targetBoss.transform, totalAttackDamage);
            GameManager.Instance.damageTMPs.Add(damageTMP);
        }
        Break();
    }

    void Break()
    {
        spriteRenderer.enabled = false;
        _particleSystem.Play();
    }
    
    private void OnDisable()
    {
        serializeDiceData = null;
        targetEnemy = null;
        targetBoss = null;
        spriteRenderer.color = GameManager.Instance.diceSO.GetDiceData(0).color;
        var particleMain = _particleSystem.main;
        particleMain.startColor = GameManager.Instance.diceSO.GetDiceData(0).color;
        ObjectPooler.ReturnToPool(gameObject);
    }
}
