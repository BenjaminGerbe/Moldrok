using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class telescopeScript : MonoBehaviour
{
    
    public GameObject Tele;
    public float Zoom;
    public float OriginFov;

    private Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
        OriginFov = cam.fieldOfView;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            Tele.gameObject.SetActive(true);
            cam.fieldOfView = 12.5f;
            RenderSettings.fogDensity = 0.0035f;
        }
        else
        {
            Tele.gameObject.SetActive(false);
            cam.fieldOfView = OriginFov;
            RenderSettings.fogDensity = 0.005f;
        }
    }
}
