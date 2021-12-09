using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conveyor : Structure
{

    public float m_TransportSpeed;

    // connection struct todo

    public Vector2 m_StartPoint;
    public Vector2 m_EndPoint;
    public int m_TotalSegments;

    public bool m_CanDepositItem;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    protected override void Update()
    {
        HandleInput();
    }

    protected override void HandleInput()
    {
        if (Input.GetMouseButtonDown(0) && GameManager.s_Instance.m_CurrentSelection == this)
        {

            gameObject.name = "clicked";
            //if (m_Type == TileTypes.CONVEYOR)
            //{
            if (GameManager.s_Instance.m_CurrentSelection.m_Type == TileTypes.CONVEYOR)
            {
                transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z + 90);
                Debug.Log("ROTATED TILE.");
            }

            //else
            //{
            //    Debug.Log("PLACED TILE.");
            //    PlaceTile();
            //}
        }
    }
}
