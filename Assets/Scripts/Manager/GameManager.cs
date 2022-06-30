using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Random = UnityEngine.Random;
using TMPro;
using DG.Tweening;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance
    {
        get; 
        private set;
    }
    void Awake() => Instance = this;
    public DiceSO diceSO;
    public Vector2[] originDicePositions;
    // 모든 주사위 정보를 직렬화 시킨다.
    public SerializeDiceData[] serializeDiceDatas;
    public List<Enemy> enemies;
    public List<DamageTMP> damageTMPs;
    public List<BossKnight> boss;
    public GameObject[] heartImages;
    public TMP_Text totalSPTMP;
    public TMP_Text spawnSPTMP;
    public TMP_Text requiredSPTMP;
    public TMP_Text requiredSPTMP1;
    public TMP_Text requiredSPTMP2;
    public TMP_Text requiredSPTMP3;
    public TMP_Text requiredSPTMP4;
    public SerializeDiceData serializeDiceData;
    public Enemy enemy;
    public DiceData diceData => diceSO.GetDiceData(serializeDiceData.code);
    //total
    private int totalSP;
    public int TotalSP
    {
        get => totalSP;
        set
        {
            totalSPTMP.text = value.ToString();
            totalSP = value;
        }
    }
    //current
    private int spawnSP;
    public int SpawnSP
    {
        get => spawnSP;
        set
        {
            spawnSPTMP.text = value.ToString();
            spawnSP = value;
        }
    }

    private int requiredSP;

    public int RequiredSP
    {
        get => requiredSP;
        set
        {
            requiredSPTMP.text = value.ToString();
            requiredSP = value;
        }
    }
    public int RequiredSP1
    {
        get => requiredSP;
        set
        {
            requiredSPTMP1.text = value.ToString();
            requiredSP = value;
        }
    }
    public int RequiredSP2
    {
        get => requiredSP;
        set
        {
            requiredSPTMP2.text = value.ToString();
            requiredSP = value;
        }
    }
    public int RequiredSP3
    {
        get => requiredSP;
        set
        {
            requiredSPTMP3.text = value.ToString();
            requiredSP = value;
        }
    }
    public int RequiredSP4
    {
        get => requiredSP;
        set
        {
            requiredSPTMP4.text = value.ToString();
            requiredSP = value;
        }
    }
    private bool isDie;

    public bool TryRandomSpawn(int level = 1)
    {
        // 빈 슬롯배열에서 랜덤할 걸 찾아 스폰한다.
        var emptySerializeDicedatas = Array.FindAll(serializeDiceDatas, x => x.isFull == false);

        if (emptySerializeDicedatas.Length <= 0)
        {
            return false;
        }
        int randomIndex = emptySerializeDicedatas[Random.Range(0, emptySerializeDicedatas.Length)].index;
        var randomDiceData = diceSO.GetRandomDiceData();
        var dice = ObjectPooler.Inst._SpawnFromPool("dice", diceSO.GetOriginDicePosition(randomIndex), Utils.QI);
        var serializeDiceData = new SerializeDiceData(randomIndex, true, randomDiceData.code, level);
        dice.GetComponent<DiceManager>().SetUpDice(serializeDiceData);
        serializeDiceDatas[randomIndex] = serializeDiceData;

        return true;
    }

    public void SpawnDice()
    {
        if (TotalSP >= SpawnSP)
        {
            if (TryRandomSpawn())
            {
                TotalSP -= SpawnSP;
                SpawnSP += 10;
            }
        }
    }
    public void UpgradeButton()
    {
        if (TotalSP >= requiredSP)
        {
            TotalSP -= requiredSP;
            RequiredSP *= 2;
            diceSO.diceDatas[1].basicAttackDamage *= 2;
            if (requiredSP > 800)
            {
                Debug.Log("Full Upgrade");
                return;
            }
        }
    }

    public void UpgradeButton1()
    {
        if (TotalSP >= requiredSP)
        {
            TotalSP -= requiredSP;
            RequiredSP1 *= 2;
            diceSO.diceDatas[2].basicAttackDamage *= 2;
            if (requiredSP > 800)
            {
                Debug.Log("Full Upgrade");
                return;
            }
        }
    }
    public void UpgradeButton2()
    {
        if (TotalSP >= requiredSP)
        {
            TotalSP -= requiredSP;
            RequiredSP2 *= 2;
            diceSO.diceDatas[3].basicAttackDamage *= 2;
            if (requiredSP > 800)
            {
                Debug.Log("Full Upgrade");
                return;
            }
        }
    }

    public void UpgradeButton3()
    {
        if (TotalSP >= requiredSP)
        {
            TotalSP -= requiredSP;
            RequiredSP3 *= 2;
            diceSO.diceDatas[4].basicAttackDamage *= 2;
            if (requiredSP > 800)
            {
                Debug.Log("Full Upgrade");
                return;
            }
        }
    }

    public void UpgradeButton4()
    {
        if (TotalSP >= requiredSP)
        {
            TotalSP -= requiredSP;
            RequiredSP4 *= 2;
            diceSO.diceDatas[5].basicAttackDamage *= 2;
            if (requiredSP > 800)
            {
                Debug.Log("Full Upgrade");
                return;
            }
        }
    }
    private void Start()
    {
        GameStart();
        StartCoroutine(StartWaveCo(30));
    }
    private void Update()
    {
        ArrangeEnemies();
        ArrangeDamages();
    }
    //주사위 위치를 다시 돌려놓기 위해 originDicePosition을 할당해놓는다.
    public Vector2 ReturnDicePosition(int index)
    {
        transform.position = originDicePositions[index];
        return transform.position;
    }

    IEnumerator StartWaveCo(float maxTime)
    {
        Debug.Log("웨이브1 시작");
        int time = 0;
        while (time < maxTime)
        {
            yield return Utils.delayWave;
            SpawnEnemy();
            time++;
        }
        Debug.Log("웨이브1 끝");
        if (time == maxTime)
        {
            SpawnBoss();
        }
    }

    
    private void SpawnEnemy()
    {
        var enemyObject = ObjectPooler.Inst._SpawnFromPool("enemy", Utils.enemyWay[0], Utils.QI);
        enemies.Add(enemyObject.GetComponent<Enemy>());
    }

    private void SpawnBoss()
    {
        var enemyObject = ObjectPooler.Inst._SpawnFromPool("bossKnight", Utils.enemyWay[0], Utils.QI);
        boss.Add(enemyObject.GetComponent<BossKnight>());
    }

    void ArrangeEnemies()
    {
        // 거리가 작은게 오더가 낮고 거리가 큰게 오더가 크다
        enemies.Sort((x, y) => x.distance.CompareTo(y.distance));

        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].GetComponent<Order>().SetOrder(i);
        }
    }
   
    void ArrangeDamages()
    {
        for (int i = 0; i < damageTMPs.Count; i++)
        {
            damageTMPs[i].GetComponent<Order>().SetOrder(i);
        }
    }
    public Enemy GetRandomEnemy()
    {
        if(enemies.Count <= 0)
        {
            return null;
        }
        return enemies[Random.Range(0, enemies.Count)];
    }

    public BossKnight GetBoss()
    {
        if (boss.Count <= 0)
        {
            return null;
        }
        return boss[0];
    }

    public void DecreaseHeart()
    {
        if(isDie)
        {
            return;
        }
        for (int i = 0; i < heartImages.Length; i++)
        {
            if(heartImages[i].activeSelf == true)
            {
                heartImages[i].SetActive(false);
                break;
            }
        }
        if(Array.TrueForAll(heartImages, x => x.activeSelf == false))
        {
            isDie = true;
            Debug.Log("게임 끝 진짜 죽음");
        }
    }

    public void GameStart()
    {
        TotalSP = 200;
        spawnSP = 10;
        RequiredSP = 100;
        RequiredSP1 = 100;
        RequiredSP2 = 100;
        RequiredSP3 = 100;
        RequiredSP4 = 100;
    }

    //public void RemoveDice()
    //{
    //    var fullSerializeDicedatas = Array.FindAll(serializeDiceDatas, x => x.isFull == true);
    //    int randomIndex = fullSerializeDicedatas[Random.Range(0, fullSerializeDicedatas.Length)].index;
        
    //}
}
