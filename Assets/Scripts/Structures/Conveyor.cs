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

    public void AddItem(Item itemToAdd)
    {
        itemToAdd.transform.position = transform.position;
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
                SpriteRenderer sprite = gameObject.GetComponentInChildren<SpriteRenderer>();
                sprite.transform.eulerAngles = new Vector3(0, 0, sprite.transform.eulerAngles.z - 90);

                if ((int)m_Direction + 1 > 3)
                    m_Direction = 0;
                else
                    m_Direction++;

                ResetConnections();
                ClearConnections();

                // Check if we have to connect things now that we've rotated.
                Connect();


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

            Structure northStructure = (Structure)northTile;
            //northStructure.m_ConnectionArray.m_Connections[] = 1;


            // Surrounding thing is a conveyor so we have to hook up its connection.
            m_ConnectionArray.m_Connections[3] = m_ConnectionArray.m_InputOutput[3] + northStructure.m_ConnectionArray.m_InputOutput[1];
            northStructure.m_ConnectionArray.m_Connections[1] = m_ConnectionArray.m_Connections[3];

        }
        if (southTile && southTile.m_Type == TileTypes.CONVEYOR)
        {
            Structure southStructure = (Structure)southTile;
            //southStructure.m_ConnectionArray.m_Connections[0] = 1;

            // Surrounding thing is a conveyor so we have to hook up its connection.
            m_ConnectionArray.m_Connections[1] = m_ConnectionArray.m_InputOutput[1] + southStructure.m_ConnectionArray.m_InputOutput[3];
            southStructure.m_ConnectionArray.m_Connections[3] = m_ConnectionArray.m_Connections[1];

        }
        if (eastTile && eastTile.m_Type == TileTypes.CONVEYOR)
        {
            Structure eastStructure = (Structure)eastTile;
            //eastStructure.m_ConnectionArray.m_Connections[3] = 1;


            // Surrounding thing is a conveyor so we have to hook up its connection.
            m_ConnectionArray.m_Connections[0] = m_ConnectionArray.m_InputOutput[0] + eastStructure.m_ConnectionArray.m_InputOutput[2];
            eastStructure.m_ConnectionArray.m_Connections[2] = m_ConnectionArray.m_Connections[0];

        }
        if (westTile && westTile.m_Type == TileTypes.CONVEYOR)
        {
            Structure westStructure = (Structure)westTile;
            //westStructure.m_ConnectionArray.m_Connections[2] = 1;

            // Surrounding thing is a conveyor so we have to hook up its connection.
            m_ConnectionArray.m_Connections[2] = m_ConnectionArray.m_InputOutput[2] + westStructure.m_ConnectionArray.m_InputOutput[0];
            westStructure.m_ConnectionArray.m_Connections[0] = m_ConnectionArray.m_Connections[2];

        }
    }


    /// <summary>
    /// Sets connection back to what they should be based on direction.
    /// </summary>
    public void ResetConnections()
    {
        // Setting input and output nodes for the conveyor belt.
        if (m_Direction == CONVEYOR_DIRECTION.EAST)
        {
            m_ConnectionArray.m_InputOutput[0] = 1;
            m_ConnectionArray.m_InputOutput[2] = 1;

            // Clearing old connections
            m_ConnectionArray.m_InputOutput[1] = 0;
            m_ConnectionArray.m_InputOutput[3] = 0;
        }
        else if (m_Direction == CONVEYOR_DIRECTION.WEST)
        {
            m_ConnectionArray.m_InputOutput[0] = 1;
            m_ConnectionArray.m_InputOutput[2] = 1;

            // Clearing old connections
            m_ConnectionArray.m_InputOutput[1] = 0;
            m_ConnectionArray.m_InputOutput[3] = 0;
        }
        else if (m_Direction == CONVEYOR_DIRECTION.NORTH)
        {
            m_ConnectionArray.m_InputOutput[1] = 1;
            m_ConnectionArray.m_InputOutput[3] = 1;

            // Clearing old connections
            m_ConnectionArray.m_InputOutput[0] = 0;
            m_ConnectionArray.m_InputOutput[2] = 0;
        }
        else if (m_Direction == CONVEYOR_DIRECTION.SOUTH)
        {
            m_ConnectionArray.m_InputOutput[1] = 1;
            m_ConnectionArray.m_InputOutput[3] = 1;

            // Clearing old connections
            m_ConnectionArray.m_InputOutput[0] = 0;
            m_ConnectionArray.m_InputOutput[2] = 0;
        }
    }

    public void ClearConnections()
    {
        for (int i = 0; i < m_ConnectionArray.m_Connections.Count; i++)
        {
            m_ConnectionArray.m_Connections[i] = 0;
        }
    }
}
