using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


[RequireComponent(typeof(Image))]
public class Slot : MonoBehaviour
{
    public Color baseColor;
    public Color hoverColor;
    private Image img;

    private bool hover;

    public void hovered()
    {
        Selected();
        hover = true;
    }

    public void deHovered()
    {
        hover = false;

        Deselected();
    }

    public void Selected()
    {
        img.color = hoverColor;
      
    }

    public void Deselected()
    {
        img.color = baseColor;
    }
    
    public bool GetHovered()
    {
        return hover;
    }


    void OnDisable()
    {
        hover = false;
        img.color = baseColor;
    }
    // Start is called before the first frame update
    void Start()
    {
        img = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (hover)
        {
            img.color = hoverColor;
        }   
    }
}
