using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeterDistance : MonoBehaviour
{
    private GameObject player;
    [SerializeField] private TMPro.TextMeshProUGUI distanceText;

    [SerializeField] private GameObject canvas;
    // Start is called before the first frame update
    void Start()
    {
        HideDistance();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
            return;
        float distance = Vector3.Distance(player.transform.position, transform.position);
        distanceText.text = "Distance: " + distance.ToString("0.00") + "m";                
    }
    
    public void DisplayDistance() {
        canvas.SetActive(true);
    }
    
    public void HideDistance() {
        canvas.SetActive(false);
    }
}
