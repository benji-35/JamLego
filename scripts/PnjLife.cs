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

    [SerializeField] private Image fgLifeBar;
    [SerializeField] private UnityEvent onDeath;

    // Start is called before the first frame update
    void Start()
    {
        life = maxLife;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void AddEventOnDeath(UnityAction action) {
        onDeath.AddListener(action);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Bullet") {
            life -= 10;
            if (fgLifeBar != null)
                fgLifeBar.fillAmount = Mathf.Clamp01((float)life / maxLife);
            if (life <= 0) {
                onDeath.Invoke();
                Destroy(gameObject);
            }            
        }
    }
}
