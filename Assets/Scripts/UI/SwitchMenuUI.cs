using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchMenuUI : MonoBehaviour
{
    public List<Image> btns;
    private Image selectedButton;
    public Color Active;
    public Color Selected;
    public Transform target;
    private int idx = 0;
    public Transform Player;
    private GameObject child;
    public void select(Image btn)
    {
        
      
        if (selectedButton != null)
        {
            if (selectedButton.GetComponent<EvenementClick>())
            {
                selectedButton.GetComponent<EvenementClick>().Active = false;
            }

            
            selectedButton.color = Active;
        }
        
        

        if (btn.GetComponent<EvenementClick>())
        {
            btn.GetComponent<EvenementClick>().Active = true;
        }
        
        btn.color = Selected;
        selectedButton = btn;

    }
    
    // Start is called before the first frame update
    void Start()
    {

        child = this.transform.GetChild(0).gameObject;

        foreach (Image btn in btns)
        {
            btn.color = Active;
        }
        
        this.select(btns[0]);
    }
    
    
    private  void DeltaMouse()
    {
        if (idx >= btns.Count)
        {
            idx = 0;
        }
        else if (idx < 0)
        {
                
            idx = btns.Count-1;
        }
    }

    private bool DrawElements()
    {
        var position =  (target.position - Player.transform.position);

        float distance = Mathf.Sqrt(Mathf.Pow(position.x, 2)+ Mathf.Pow(position.z, 2));
        
  
        if (distance > 3)
        {
            this.child.SetActive(false);
            return false;
        }
    
        
        
        
        
        var lignedValue = Player.transform.forward * 10;
        var direction = target.transform.position - Player.transform.position;
        var normeLigned = Mathf.Sqrt(Mathf.Pow(lignedValue.x, 2) + Mathf.Pow(lignedValue.y, 2) +
                                     Mathf.Pow(lignedValue.z, 2));
        Vector3 projectedValue = Player.transform.position + ( Vector3.Dot(lignedValue,direction) / normeLigned )  *
            lignedValue.normalized;

        var directionCamera = target.transform.position - projectedValue;
        var distanceCamera =Mathf.Sqrt(Mathf.Pow(directionCamera.x, 2) + Mathf.Pow(directionCamera.y, 2) +
                                       Mathf.Pow(directionCamera.z, 2));
        
            
        if (distanceCamera < 1)
        {
            this.child.SetActive(true);
            return true;
        }
        
     
        this.child.SetActive(false);
        return false;
    }


    public void DotProduct(Vector3 v1,Vector3 v2)
    {
        
    }
    
    // Update is called once per frame
    void Update()
    {


        if (   DrawElements())
        {

         
           
            if (Input.GetAxisRaw("Mouse ScrollWheel") > 0)
            {
                idx ++;

                DeltaMouse();
                
                Debug.Log(idx);
                this.select(btns[idx]);
            }
            
            if (Input.GetAxisRaw("Mouse ScrollWheel") < 0)
            {
                idx --;

                DeltaMouse();
                
                this.select(btns[idx]);
            }

        }

    }
}
