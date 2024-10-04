using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{

    static public bool goalMet = false;
    private void OnTriggerEnter(Collider other)
    {
        //when the trigger is hit by something
        //check tto see if its a proj
        Projectile proj = other.GetComponent<Projectile>();

        if (proj != null)
        {
            //if so , set goalmet to tru
            Goal.goalMet = true;
            //also se the alphaa of the color to higher opacit
            Material mat =GetComponent<Renderer>().material;
            Color c = mat.color;
            c.a = .75f;
            mat.color = c;
        }
    }




}
