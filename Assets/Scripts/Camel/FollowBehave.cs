using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowBehave : MonoBehaviour
{


    /// <summary>
    /// this script allow the camel to follow the player
    /// </summary>

    public Transform Player;
    


    private bool detect = false;
    private Vector3 dir;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            detect = true;
        }
    }


    void FixedUpdate()
    {

   

        if (detect && Vector3.Distance(this.transform.position,Player.position) > 2)
        {

            Debug.Log("je passe");

            Vector3 p = new Vector3(Player.position.x, this.transform.position.y, Player.position.z);

            dir = p - this.transform.position;

            Debug.DrawRay(this.transform.position, dir * 10, Color.white);
       

            dir = dir.normalized;
           

            this.GetComponent<Rigidbody>().velocity = dir * 3 ;

            Quaternion rotation = Quaternion.LookRotation(dir, Vector3.up);
            transform.rotation = rotation;

        }

    }
}
