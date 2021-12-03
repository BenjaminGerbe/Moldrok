using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Light))]
[ExecuteInEditMode]
public class RotationSun : MonoBehaviour
{
    public float speed;

    private Light LT;

    [InspectorRange(0f,1f)]
    public float timeCount = 0;
  

    public Gradient AmbientColor;
    public Gradient FogColor;
    
    // Start is called before the first frame update
    void Start()
    {
        LT = GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {
      //  this.transform.Rotate(this.transform.right,1 * Time.deltaTime * speed);
      

        LT.transform.localRotation = Quaternion.Euler(new Vector3( (timeCount * 360f) -90f, 170f,0 ));
        
        LT.color = AmbientColor.Evaluate(timeCount);
        RenderSettings.fogColor = FogColor.Evaluate(timeCount);
        
        timeCount = (timeCount + speed * Time.deltaTime )%1;
    }
}
