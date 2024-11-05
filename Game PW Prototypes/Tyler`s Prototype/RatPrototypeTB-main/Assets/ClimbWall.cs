using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class ClimbWall : MonoBehaviour
{
    [SerializeField]
    private GameObject PlayerRat;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == PlayerRat)
        {
            PlayerRat.GetComponent<movement>().ChangeToClimbing();
        }
    } 
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject == PlayerRat)
        {
            PlayerRat.GetComponent<movement>().ChangeToWalking();

        }
    }
}
