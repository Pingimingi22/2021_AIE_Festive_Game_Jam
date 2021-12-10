using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingConveyorCollider : MonoBehaviour
{
    public ProductionBuilding m_Building;
    public CONVEYOR_DIRECTION m_Direction;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


	private void OnCollisionEnter2D(Collision2D other)
	{
        if (other.gameObject && other.gameObject.tag == "Conveyor")
        {
            Conveyor conveyor = other.gameObject.GetComponent<Conveyor>();
            if (m_Direction == CONVEYOR_DIRECTION.EAST)
            {
                if (conveyor.m_Direction == CONVEYOR_DIRECTION.WEST) // Facing away so it's an output.
                {
                    m_Building.AddInputConveyor(conveyor);
                }
                else if (conveyor.m_Direction == CONVEYOR_DIRECTION.EAST) // Facing same way so it's an input.
                {
                    m_Building.AddOutputConveyor(conveyor);
                }
            }

            else if (m_Direction == CONVEYOR_DIRECTION.WEST)
            {
                if (conveyor.m_Direction == CONVEYOR_DIRECTION.WEST) // Facing away so it's an output.
                {
                    m_Building.AddOutputConveyor(conveyor);
                }
                else if (conveyor.m_Direction == CONVEYOR_DIRECTION.EAST) // Facing same way so it's an input.
                {
                    m_Building.AddInputConveyor(conveyor);
                }
            }

            else if (m_Direction == CONVEYOR_DIRECTION.NORTH)
            {
                if (conveyor.m_Direction == CONVEYOR_DIRECTION.SOUTH) // Facing away so it's an output.
                {
                    m_Building.AddInputConveyor(conveyor);
                }
                else if (conveyor.m_Direction == CONVEYOR_DIRECTION.NORTH) // Facing same way so it's an input.
                {
                    m_Building.AddOutputConveyor(conveyor);
                }
            }

            else if (m_Direction == CONVEYOR_DIRECTION.SOUTH)
            {
                if (conveyor.m_Direction == CONVEYOR_DIRECTION.NORTH) // Facing away so it's an output.
                {
                    m_Building.AddInputConveyor(conveyor);
                }
                else if (conveyor.m_Direction == CONVEYOR_DIRECTION.SOUTH) // Facing same way so it's an input.
                {
                    m_Building.AddOutputConveyor(conveyor);
                }
            }
        }
	}
}
