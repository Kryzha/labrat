using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class control_cam : MonoBehaviour
{
    [SerializeField] private float xsens, ysens;
    [SerializeField] private float radius;
    [SerializeField] private Transform player;
    float s, t;

    private void Update()
    {  
        float hInput = Input.GetAxisRaw("Mouse X")* xsens;
        float vInput = Input.GetAxisRaw("Mouse Y") * ysens;

        s += hInput; 
        t -= vInput;

        t = Mathf.Clamp(t, -1.57f, 0);
        if (Input.GetKeyDown(KeyCode.K))
        {
            Debug.Log(s);
            Debug.Log(t);
        }
        float x = Mathf.Cos(s)* radius;
        float y = Mathf.Cos(t)* radius;
        float z = Mathf.Sin(s)* radius;

        transform.position = new Vector3(player.position.x + x, player.position.y + y,player.position.z + z);
        transform.LookAt(player);
    }
}
