using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public GameObject target;

    public float follow_speed = 4.0f;
    public float z = -10.0f;

    Transform targetTransform;


    public bool debugflag = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!debugflag)
        {
            this.transform.position = Vector2.Lerp(this.transform.position, target.transform.position, follow_speed * Time.deltaTime);
            this.transform.Translate(0, 0, z);
        }
    }
}
