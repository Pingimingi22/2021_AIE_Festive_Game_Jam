using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum CONVEYOR_DIRECTION
{ 
    EAST,
    SOUTH,
    WEST,
    NORTH
}

public class Conveyor : Structure
{

    public float m_TransportSpeed;

    // connection struct todo

    public Vector2 m_StartPoint;
    public Vector2 m_EndPoint;
    public int m_TotalSegments;

    public bool m_CanDepositItem;

    public CONVEYOR_DIRECTION m_Direction;


	// Start is called before the first frame update
	protected override void Start()
	{
        base.Start();
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

                if ((int)m_Direction + 1 > 3)
                    m_Direction = 0;
                else
                    m_Direction++;


                Debug.Log("ROTATED TILE.");
            }

            //else
            //{
            //    Debug.Log("PLACED TILE.");
            //    PlaceTile();
            //}
        }
    }

	public override void Connect()
	{
        GameManager manager = GameManager.s_Instance;
        Clickable northTile = manager.GetTile((int)m_WorldIndex.x, (int)m_WorldIndex.y + 1);
        Clickable southTile = manager.GetTile((int)m_WorldIndex.x, (int)m_WorldIndex.y - 1);
        Clickable eastTile = manager.GetTile((int)m_WorldIndex.x + 1, (int)m_WorldIndex.y);
        Clickable westTile = manager.GetTile((int)m_WorldIndex.x - 1, (int)m_WorldIndex.y);


        if (northTile && northTile.m_Type == TileTypes.CONVEYOR)
        {
            // Surrounding thing is a conveyor so we have to hook up its connection.
            m_ConnectionArray.m_Connections[0] = 1;

            Structure northStructure = (Structure)northTile;
            northStructure.m_ConnectionArray.m_Connections[1] = 1;

        }
        if (southTile && southTile.m_Type == TileTypes.CONVEYOR)
        {
            // Surrounding thing is a conveyor so we have to hook up its connection.
            m_ConnectionArray.m_Connections[1] = 1;

            Structure southStructure = (Structure)southTile;
            southStructure.m_ConnectionArray.m_Connections[0] = 1;
        }
        if (eastTile && eastTile.m_Type == TileTypes.CONVEYOR)
        {
            // Surrounding thing is a conveyor so we have to hook up its connection.
            m_ConnectionArray.m_Connections[2] = 1;

            Structure eastStructure = (Structure)eastTile;
            eastStructure.m_ConnectionArray.m_Connections[3] = 1;
        }
        if (westTile && westTile.m_Type == TileTypes.CONVEYOR)
        {
            // Surrounding thing is a conveyor so we have to hook up its connection.
            m_ConnectionArray.m_Connections[3] = 1;

            Structure westStructure = (Structure)westTile;
            westStructure.m_ConnectionArray.m_Connections[2] = 1;
        }
    }
}
