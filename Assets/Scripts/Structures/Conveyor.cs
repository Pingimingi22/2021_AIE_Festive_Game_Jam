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


    public int m_NextConnectionToSend = 0; // Tracks the next connecting conveyor to send to.

    public int m_TotalConveyorConnections = 0;


    // Hardcoding connections please work
    Conveyor m_LeftConnection = null;
    Conveyor m_RightConnection = null;
    Conveyor m_UpConnection = null;
    Conveyor m_DownConnection = null;

    List<Conveyor> m_HardConnections = new List<Conveyor>();



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

                ClearConnections();
                ResetInputs();

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

            Conveyor northConveyor = (Conveyor)northStructure;
            northConveyor.ClearConnections();
            northConveyor.ResetInputs();
            
            ClearConnections();
            ResetInputs();
            //northConveyor.Connect();
            //northConveyor.GetTotalConnections();

            //ClearConnections();
            //ResetInputs();


            

            // Surrounding thing is a conveyor so we have to hook up its connection.
            m_ConnectionArray.m_Connections[3] += m_ConnectionArray.m_InputOutput[3] + northStructure.m_ConnectionArray.m_InputOutput[1];
            northStructure.m_ConnectionArray.m_Connections[1] += m_ConnectionArray.m_InputOutput[3] + northStructure.m_ConnectionArray.m_InputOutput[1];

            

        }
        if (southTile && southTile.m_Type == TileTypes.CONVEYOR)
        {
            Structure southStructure = (Structure)southTile;
            //southStructure.m_ConnectionArray.m_Connections[0] = 1;

            Conveyor southConveyor = (Conveyor)southStructure;
            southConveyor.ClearConnections();
            southConveyor.ResetInputs();
            ClearConnections();
            ResetInputs();

            //southConveyor.Connect();
            //southConveyor.GetTotalConnections();


            //ClearConnections();
            //ResetInputs();

            // Surrounding thing is a conveyor so we have to hook up its connection.
            m_ConnectionArray.m_Connections[1] += m_ConnectionArray.m_InputOutput[1] + southStructure.m_ConnectionArray.m_InputOutput[3];

            


            southStructure.m_ConnectionArray.m_Connections[3] += m_ConnectionArray.m_InputOutput[1] + southStructure.m_ConnectionArray.m_InputOutput[3];

        }
        if (eastTile && eastTile.m_Type == TileTypes.CONVEYOR)
        {
            Structure eastStructure = (Structure)eastTile;
            //eastStructure.m_ConnectionArray.m_Connections[3] = 1;

            Conveyor eastConveyor = (Conveyor)eastStructure;
            eastConveyor.ClearConnections();
            eastConveyor.ResetInputs();
            ResetInputs();
            ClearConnections();
            //eastConveyor.Connect();
            //eastConveyor.GetTotalConnections();

            //ClearConnections();
            //ResetInputs();

            // Surrounding thing is a conveyor so we have to hook up its connection.
            m_ConnectionArray.m_Connections[0] += m_ConnectionArray.m_InputOutput[0] + eastStructure.m_ConnectionArray.m_InputOutput[2];
            eastStructure.m_ConnectionArray.m_Connections[2] += m_ConnectionArray.m_InputOutput[0] + eastStructure.m_ConnectionArray.m_InputOutput[2];

            

        }
        if (westTile && westTile.m_Type == TileTypes.CONVEYOR)
        {
            Structure westStructure = (Structure)westTile;
            //westStructure.m_ConnectionArray.m_Connections[2] = 1;

            Conveyor westConveyor = (Conveyor)westStructure;
            //westConveyor.GetTotalConnections();

            westConveyor.ClearConnections();
            westConveyor.ResetInputs();
            ResetInputs();
            ClearConnections();
            //
            //westConveyor.Connect();
            //westConveyor.GetTotalConnections();

            //ClearConnections();
            //ResetInputs();


            // Surrounding thing is a conveyor so we have to hook up its connection.
            m_ConnectionArray.m_Connections[2] += m_ConnectionArray.m_InputOutput[2] + westStructure.m_ConnectionArray.m_InputOutput[0];
            westStructure.m_ConnectionArray.m_Connections[0] += m_ConnectionArray.m_InputOutput[2] + westStructure.m_ConnectionArray.m_InputOutput[0];

            

        }

        GetTotalConnections();
    }

    //just reset connecting node not all nodes on connecting conveyor


    /// <summary>
    /// Sets connection back to what they should be based on direction.
    /// </summary>
    public void ResetInputs()
    {
        // Setting input and output nodes for the conveyor belt.
        if (m_Direction == CONVEYOR_DIRECTION.EAST)
        {
            m_ConnectionArray.m_InputOutput[0] = 1;
            m_ConnectionArray.m_InputOutput[2] = 1;

            // Clearing old connections
            m_ConnectionArray.m_InputOutput[1] = 0;
            m_ConnectionArray.m_InputOutput[3] = 0;

            // Setting output for connections to -1 so they never add to 1.
            m_ConnectionArray.m_Connections[2] = -2;
            m_ConnectionArray.m_Connections[1] = -1;
            m_ConnectionArray.m_Connections[3] = -1;


        }
        else if (m_Direction == CONVEYOR_DIRECTION.WEST)
        {
            m_ConnectionArray.m_InputOutput[0] = 1;
            m_ConnectionArray.m_InputOutput[2] = 1;

            // Clearing old connections
            m_ConnectionArray.m_InputOutput[1] = 0;
            m_ConnectionArray.m_InputOutput[3] = 0;

            // Setting output for connections to -1 so they never add to 1.
            m_ConnectionArray.m_Connections[0] = -2;
            m_ConnectionArray.m_Connections[1] = -1;
            m_ConnectionArray.m_Connections[3] = -1;

        }
        else if (m_Direction == CONVEYOR_DIRECTION.NORTH)
        {
            m_ConnectionArray.m_InputOutput[1] = 1;
            m_ConnectionArray.m_InputOutput[3] = 1;

            // Clearing old connections
            m_ConnectionArray.m_InputOutput[0] = 0;
            m_ConnectionArray.m_InputOutput[2] = 0;

            // Setting output for connections to -1 so they never add to 1.
            m_ConnectionArray.m_Connections[1] = -2;
            m_ConnectionArray.m_Connections[0] = -1;
            m_ConnectionArray.m_Connections[2] = -1;
        }
        else if (m_Direction == CONVEYOR_DIRECTION.SOUTH)
        {
            m_ConnectionArray.m_InputOutput[1] = 1;
            m_ConnectionArray.m_InputOutput[3] = 1;

            // Clearing old connections
            m_ConnectionArray.m_InputOutput[0] = 0;
            m_ConnectionArray.m_InputOutput[2] = 0;

            // Setting output for connections to -1 so they never add to 1.
            m_ConnectionArray.m_Connections[3] = -2;
            m_ConnectionArray.m_Connections[0] = -1;
            m_ConnectionArray.m_Connections[2] = -1;

        }
    }

    public void ClearConnections()
    {
        for (int i = 0; i < m_ConnectionArray.m_Connections.Count; i++)
        {
            //if(m_ConnectionArray.m_Connections[i] > 0)
                m_ConnectionArray.m_Connections[i] = 0;
        }
    }

    public void GetTotalConnections()
    {
        int totalConnections = 0;
        m_HardConnections.Clear();
        for (int i = 0; i < m_ConnectionArray.m_Connections.Count; i++)
        {
            if (m_ConnectionArray.m_Connections[i] > 0)
            {
                totalConnections++;

                if (i == 0)
                {
                    // east connection
                    m_RightConnection = (Conveyor)GetConnection(CONVEYOR_DIRECTION.EAST);
                    m_HardConnections.Add(m_RightConnection);
                }
                else if (i == 2)
                {
                    // west connection
                    m_LeftConnection = (Conveyor)GetConnection(CONVEYOR_DIRECTION.WEST);
                    m_HardConnections.Add(m_LeftConnection);
                }
                else if (i == 3)
                {
                    // west connection
                    m_UpConnection = (Conveyor)GetConnection(CONVEYOR_DIRECTION.NORTH);
                    m_HardConnections.Add(m_UpConnection);
                }
                else if (i == 1)
                {
                    // west connection
                    m_DownConnection = (Conveyor)GetConnection(CONVEYOR_DIRECTION.SOUTH);
                    m_HardConnections.Add(m_DownConnection);
                }
            }
        }
        m_TotalConveyorConnections = totalConnections;
    }

    public Conveyor GetNextConveyor(Conveyor blacklistedConveyor)
    {
        Conveyor nextConveyor = null;
        if (m_TotalConveyorConnections > 1)
        {
            //CONVEYOR_DIRECTION nextConveyorDir = GetDirectionFromNum(m_NextConnectionToSend);
            //Clickable nextClickable = GetConnection(nextConveyorDir);

            //Conveyor nextConveyor = GetDirectionFromNum(m_NextConnectionToSend);

            nextConveyor = GetConveyorFromHardCoded(m_NextConnectionToSend);

            // Increment the progress on which conveyor belt to send to next.
            if (m_NextConnectionToSend + 1 > m_TotalConveyorConnections - 1)
                m_NextConnectionToSend = 0;
            else
                m_NextConnectionToSend++;
     
        }
        else
        {
            Clickable conveyorClickable = GetFirstConnection();
            nextConveyor = (Conveyor)conveyorClickable;
        }

        return nextConveyor;
    }

    public Clickable GetConnection(CONVEYOR_DIRECTION direction)
    {
        switch (direction)
        {
            case CONVEYOR_DIRECTION.EAST:
                return GameManager.s_Instance.GetTile((int)m_WorldIndex.x + 1, (int)m_WorldIndex.y);
            case CONVEYOR_DIRECTION.NORTH:
                return GameManager.s_Instance.GetTile((int)m_WorldIndex.x, (int)m_WorldIndex.y + 1);
            case CONVEYOR_DIRECTION.SOUTH:
                return GameManager.s_Instance.GetTile((int)m_WorldIndex.x, (int)m_WorldIndex.y - 1);
            case CONVEYOR_DIRECTION.WEST:
                return GameManager.s_Instance.GetTile((int)m_WorldIndex.x - 1, (int)m_WorldIndex.y);
        }

        return null;
    }
    public CONVEYOR_DIRECTION GetDirectionFromNum(int num)
    {
        CONVEYOR_DIRECTION dirAttempt = CONVEYOR_DIRECTION.NORTH;
        switch (num)
        {
            case 0:
                dirAttempt = CONVEYOR_DIRECTION.EAST;
                break;
            case 1:
                dirAttempt = CONVEYOR_DIRECTION.SOUTH;
                break;
            case 2:
                dirAttempt = CONVEYOR_DIRECTION.WEST;
                break;
            case 3:
                dirAttempt = CONVEYOR_DIRECTION.NORTH;
                break;
        }

        //Debug.LogError("Error in GetDirectionFromNum!");
        return dirAttempt;
            
    }

    public Clickable GetFirstConnection()
    {
        Clickable firstConnection = null;

        for (int i = 0; i < m_ConnectionArray.m_Connections.Count; i++)
        {
            if (m_ConnectionArray.m_Connections[i] > 0)
            {
                if (IsOppositeDirection(i))
                {
                    // Skip this connection if it is in the opposite direction.
                    continue;
                }
                else
                { 
                    CONVEYOR_DIRECTION dir = GetDirectionFromNum(i);
                    return GetConnection(dir);
                }
            }
        }
        return firstConnection;
    }

    public bool IsOppositeDirection(int numInArray)
    {
        if (m_Direction == CONVEYOR_DIRECTION.EAST && numInArray == 2)
            return true;
        else if (m_Direction == CONVEYOR_DIRECTION.SOUTH && numInArray == 3)
            return true;
        else if (m_Direction == CONVEYOR_DIRECTION.WEST && numInArray == 0)
            return true;
        else if (m_Direction == CONVEYOR_DIRECTION.NORTH && numInArray == 1)
            return true;

        return false;
    }

    public void ResetConveyorConnections()
    {
        m_LeftConnection = null;
        m_RightConnection = null;
        m_UpConnection = null;
        m_DownConnection = null;
    }

    public Conveyor GetConveyorFromHardCoded(int index)
    {
        if (index > m_TotalConveyorConnections)
        {
            Debug.LogError("GetConveyorFromHardCoded index is greater than the amount of total conveyor connections!");
        }
        else
        {
            return m_HardConnections[index];
        }

        return null;
    }
}
