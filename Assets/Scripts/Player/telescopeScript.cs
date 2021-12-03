using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class telescopeScript : MonoBehaviour
{
    
    public GameObject Tele;
    public float Zoom;
    public float OriginFov;
    public float FogStorm;

    private Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
        OriginFov = cam.fieldOfView;
        FogStorm = RenderSettings.fogDensity;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Tele.gameObject.SetActive(true);
            cam.fieldOfView = 12.5f;
            RenderSettings.fogDensity = FogStorm - 0.015f;
        }
        else  if (Input.GetMouseButtonUp(1))
        {
            Tele.gameObject.SetActive(false);
            cam.fieldOfView = OriginFov;
            RenderSettings.fogDensity = FogStorm;
        }
    }
}
