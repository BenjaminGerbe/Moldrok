using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FPS : MonoBehaviour
{
    public TextMeshProUGUI fps;
    private float timer = 0.25f;
    private float counter = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        counter = timer;
    }

    // Update is called once per frame
    void Update()
    {
        counter -= Time.deltaTime;


        if (counter < 0)
        {
            fps.text = Mathf.Round( (1.0f / Time.deltaTime)).ToString();
            counter = timer;
        }
        
      
    }
}
