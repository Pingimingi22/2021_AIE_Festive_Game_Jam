using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Structure : Clickable
{

    public int m_Level;
    public float m_Cost;
    public Connection m_ConnectionArray;

    // Start is called before the first frame update
    protected override void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Connects structure and surrounding structure to the grid.
    /// </summary>
	public virtual void Connect()
	{
        GameManager manager = GameManager.s_Instance;
        Clickable northTile = manager.GetTile((int)m_WorldIndex.x, (int)m_WorldIndex.y + 1);
        Clickable southTile = manager.GetTile((int)m_WorldIndex.x, (int)m_WorldIndex.y - 1);
        Clickable eastTile = manager.GetTile((int)m_WorldIndex.x + 1, (int)m_WorldIndex.y);
        Clickable westTile = manager.GetTile((int)m_WorldIndex.x - 1, (int)m_WorldIndex.y);




        if (northTile && northTile.m_Type == TileTypes.CONVEYOR)
        {
            // Surrounding thing is a conveyor so we have to hook up its connection.
            m_ConnectionArray.m_InputOutput[0] = 1;

            Structure northStructure = (Structure)northTile;
            northStructure.m_ConnectionArray.m_InputOutput[1] = 1;

        }
        if (southTile && southTile.m_Type == TileTypes.CONVEYOR)
        {
            // Surrounding thing is a conveyor so we have to hook up its connection.
            m_ConnectionArray.m_InputOutput[1] = 1;

            Structure southStructure = (Structure)southTile;
            southStructure.m_ConnectionArray.m_InputOutput[0] = 1;
        }
        if (eastTile && eastTile.m_Type == TileTypes.CONVEYOR)
        {
            // Surrounding thing is a conveyor so we have to hook up its connection.
            m_ConnectionArray.m_InputOutput[2] = 1;

            Structure eastStructure = (Structure)eastTile;
            eastStructure.m_ConnectionArray.m_InputOutput[3] = 1;
        }
        if (westTile && westTile.m_Type == TileTypes.CONVEYOR)
        {
            // Surrounding thing is a conveyor so we have to hook up its connection.
            m_ConnectionArray.m_InputOutput[3] = 1;

            Structure westStructure = (Structure)westTile;
            westStructure.m_ConnectionArray.m_InputOutput[2] = 1;
        }
    }
}
