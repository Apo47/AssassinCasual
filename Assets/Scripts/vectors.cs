using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class vectors : MonoBehaviour
{
    public Transform left;
    public Transform right;



    public void LateUpdate()
    {
        Vector3 view = transform.position;


        view.x = Mathf.Clamp(view.x, left.transform.position.x, right.transform.position.x);
        transform.position = view;

    }


}
