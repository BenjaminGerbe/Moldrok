using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LifeManager : MonoBehaviour
{
    public TextMeshProUGUI lifeTxt;
        
    private float life = 10;
    public Transform player;
    public float distanceHit;
    public float GetLife()
    {
        return this.life;
    }

    public void dommage(float incrementation)
    {
        var position = player.position;
        if (Mathf.Sqrt( Mathf.Pow(position.x,2) + Mathf.Pow( position.z,2)) > distanceHit )
        {
            this.life -= incrementation;
        }
       
    }
    
    public void heal(float incrementation){
        this.life += incrementation;
    }
    


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        lifeTxt.text =  (Mathf.Round(life)).ToString();
    }
}
