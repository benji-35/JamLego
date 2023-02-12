using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PnjLife : MonoBehaviour
{
    [SerializeField] private int maxLife = 100;
    [SerializeField] int life = 100;
    [SerializeField] bool respawnable = false;
    [SerializeField] List<Renderer> renderers;
    [SerializeField] List<Collider> colliders;

    [SerializeField] private Image fgLifeBar;
    [SerializeField] private UnityEvent onDeath;
    [SerializeField] private int yellowCoins = 0;
    [SerializeField] private int blueCoins = 0;
    [SerializeField] private int purpleCoins = 0;
    [SerializeField] private GameObject coin;

    // Start is called before the first frame update
    void Start()
    {
        life = maxLife;
    }

    public void AddEventOnDeath(UnityAction action) {
        onDeath.AddListener(action);
    }

    private void GenerateCoin()
    {
        if (coin == null)
            return;
        for (int i = 0; i < yellowCoins; i++) {
            GameObject obj = Instantiate(coin, transform.position, Quaternion.identity);
            int amount = UnityEngine.Random.Range(1, 100);
            obj.GetComponent<Coin>().SetCoinValue(amount);
        }
        for (int i = 0; i < blueCoins; i++) {
            GameObject obj = Instantiate(coin, transform.position, Quaternion.identity);
            int amount = UnityEngine.Random.Range(100, 10000);
            obj.GetComponent<Coin>().SetCoinValue(amount);
        }
        for (int i = 0; i < purpleCoins; i++) {
            GameObject obj = Instantiate(coin, transform.position, Quaternion.identity);
            int amount = UnityEngine.Random.Range(10000, 1000000);
            obj.GetComponent<Coin>().SetCoinValue(amount);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Bullet") {
            life -= 10;
            Destroy(other.gameObject);
            if (fgLifeBar != null)
                fgLifeBar.fillAmount = Mathf.Clamp01((float)life / maxLife);
            if (life <= 0) {
                GenerateCoin();
                onDeath.Invoke();
                if (respawnable) {
                    StartCoroutine(respawn());
                } else {
                    Destroy(gameObject);
                }
            }            
        }
    }
    
    IEnumerator respawn()
    {
        foreach (var VARIABLE in renderers)
        {
            VARIABLE.enabled = false;
        }
        foreach (var VARIABLE in colliders)
        {
            VARIABLE.enabled = false;
        }
        yield return new WaitForSeconds(30f);
        life = maxLife;
        if (fgLifeBar != null)
            fgLifeBar.fillAmount = Mathf.Clamp01((float)life / maxLife);
        foreach (var VARIABLE in renderers)
        {
            VARIABLE.enabled = true;
        }
        foreach (var VARIABLE in colliders)
        {
            VARIABLE.enabled = true;
        }
    }

    public bool IsRespawnable()
    {
        return respawnable;
    }
    
    public GameObject GetCoin()
    {
        return coin;
    }
}
