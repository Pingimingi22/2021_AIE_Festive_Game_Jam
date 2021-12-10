using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchPresent : MonoBehaviour
{
    public Rigidbody2D m_Rigidbody;
    // Start is called before the first frame update
    void Start()
    {
        m_Rigidbody.AddForce(new Vector3(0.707f, 0.707f, 0) * 25, ForceMode2D.Impulse);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
