using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandStromManager : MonoBehaviour
{

    public float speedChange = 0.001f;

    

    [Header("Tour manager")] 
    public float PourcentageHit;
    public float HitIcrementation;
    
    [Header("Good Day ")] 
    public Color BottomColorGD;
    public Color TopColorGD;
    public float fogOpaGD;
    public Color fogColorGD;
    public float ValueFadeGD;
    public float WindForceGD;
    
    [Header("SandStrom ")] 
    public Color BottomColorSS;
    public Color TopColorSS;
    public float fogOpaSS;
    public Color fogColorSS;
    public float ValueFadeSS;
    public float WindForceSS;
    [Header(" ")] 
    private float countPercentage;

    public LifeManager LifeManager;
    public WindZone WZ;
    public Material SandStormChange;
    
    
    private bool activate = false;
    // Start is called before the first frame update
    void Start()
    {
        SandStormChange.SetColor("_BottomColor",BottomColorGD);
        SandStormChange.SetColor("_TopColor",TopColorGD);
        RenderSettings.fogDensity = fogOpaGD;
        RenderSettings.fogColor = fogColorGD;
        SandStormChange.SetFloat("_Value",ValueFadeGD);
        WZ.windMain = WindForceGD;
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.F))
        {
            activate = !activate;
        }
        
        if (activate && countPercentage <= 1 )  
        {
             countPercentage = (countPercentage + speedChange);
        }
        else if(!activate && countPercentage >= 0)
        {
            countPercentage = (countPercentage - speedChange) ;
        }


        if (countPercentage >= PourcentageHit)
        {
            LifeManager.dommage(HitIcrementation * Time.deltaTime);
        }
        
        
        SandStormChange.SetColor("_BottomColor",Color.Lerp(BottomColorGD,BottomColorSS,countPercentage)  );
        SandStormChange.SetColor("_TopColor",Color.Lerp(TopColorGD,TopColorSS,countPercentage)  );
        RenderSettings.fogDensity = Mathf.Lerp(fogOpaGD,fogOpaSS,countPercentage);
        RenderSettings.fogColor = Color.Lerp(fogColorGD,fogColorSS,countPercentage) ;
        SandStormChange.SetFloat("_Value",Mathf.Lerp(ValueFadeGD,ValueFadeSS,countPercentage));
        WZ.windMain = Mathf.Lerp(WindForceGD, WindForceSS, countPercentage);

    }
}
