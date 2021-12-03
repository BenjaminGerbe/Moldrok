using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PIckMetal : MonoBehaviour
{

    public DetectIronScript DTS;

    public TextMeshProUGUI txt;

    public int counter = 0;
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
