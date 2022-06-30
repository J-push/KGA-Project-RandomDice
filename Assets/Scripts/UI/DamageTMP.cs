using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
public class DamageTMP : MonoBehaviour
{
    public TMP_Text damageTMP;
    public float minOffsetY;
    public float maxOffsetY;

    private Transform target;
    private float totalTime;
    public void SetUp(Transform target, int damageAmount)
    {
        gameObject.SetActive(true);
        this.target = target;
        totalTime = 0f;
        damageTMP.text = damageAmount.ToString();
        
        StartCoroutine(DamageTMPCo());
    }

    IEnumerator DamageTMPCo()
    {
        //위에 숫자뜨는거 
        while(totalTime <= 0.3f)
        {
            if (target != null)
            {
                var targetPos = target.position;
                targetPos.y += minOffsetY;
                transform.position = targetPos;
            }
            totalTime += Time.deltaTime;
            yield return null;
        }
        //점점 올라가고 페이드아웃
        totalTime = 0f;
        while (totalTime <= 0.2f)
        {
            if (target != null)
            {
                float lerpTime = totalTime * 4;
                var targetPos = target.position;
                targetPos.y += Mathf.Lerp(minOffsetY, maxOffsetY, lerpTime);
                transform.position = targetPos;
            }
            totalTime += Time.deltaTime;
            yield return null;
        }

        gameObject.SetActive(false); 
    }

    private void OnDisable()
    {
        GameManager.Instance.damageTMPs.Remove(this);
        target = null;
        totalTime = 0f;
        damageTMP.text = "";
        ObjectPooler.ReturnToPool(gameObject);
        CancelInvoke();
    }
}
