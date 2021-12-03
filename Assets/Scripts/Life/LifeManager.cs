using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LifeManager : MonoBehaviour
{
    public TextMeshProUGUI lifeTxt;

    private float life = 10;

    public float GetLife()
    {
        return this.life;
    }
    


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        lifeTxt.text = life.ToString();
    }
}
