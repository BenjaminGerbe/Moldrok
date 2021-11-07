using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructurePlaceHolder : MonoBehaviour
{
    void Update()
    {

        //Ray ray = new Ray(this.transform.position,Vector3.down * 1000 );
        //RaycastHit hit;

       

        //if (Physics.Raycast(ray,out hit) && hit.transform.gameObject.layer != LayerMask.NameToLayer("Default"))
        //{
        //    this.transform.position = hit.point;

        //    Debug.DrawRay(this.transform.position, Vector3.down * 1000,Color.red);
        //    Debug.Log(hit.point + this.transform.gameObject.ToString());

        //}
        //else
        //{
        //    Debug.DrawRay(this.transform.position, Vector3.down * 1000, Color.white);
        //}


    }

    private void OnDrawGizmos()
    {
        //Ray ray = new Ray(this.transform.position, Vector3.down * 1000);
        //Gizmos.DrawRay(ray);        
    }

}
