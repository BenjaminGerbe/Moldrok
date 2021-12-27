using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PickMetal : MonoBehaviour
{

    public DetectIronScript DTS;

    public TextMeshProUGUI txt;

    private int counter = 0;

    public int getCounter()
    {
        return this.counter;
    }

    public bool Decriment(int i)
    {
        if (i > counter)
        {
            return false;
        }

        this.counter -= i;
        txt.text = counter.ToString();

        return true;

    }



    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
     
        if (Input.GetKeyDown(KeyCode.E))
        {
            
            if (DTS.getDetected() != null)
            {
                Destroy(DTS.getDetected() );
                counter++;
                txt.text = counter.ToString();
            }
            
            
            
            
            
            
        }
    }
}
