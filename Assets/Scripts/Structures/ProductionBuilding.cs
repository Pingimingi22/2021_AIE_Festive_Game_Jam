using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductionBuilding : Structure
{

    public int m_CurrentElves;
    public bool m_IsProducing;

    // connections struct todo.

    public float m_CurrentProgress;
    public Item m_ProductionItem; // Item that is produced by this building.

    public float m_ProductionTime;
    public float m_ProductionRate;
    public float m_ProductionTime_Elves;


    public int m_CurrentConveyorBelt = 0;
    List<Conveyor> m_AttachedConveyors = new List<Conveyor>();



    // Start is called before the first frame update
    void Start()
    {
        //m_ProductionRate = 60 / m_ProductionTime + 60/(2*m_ProductionTime)* m_CurrentElves;  prodcution rate in fps??
        //m_ProductionTime_Elves = 60/m_ProductionRate  production time after applying elves
        if (m_IsProducing)
        {
            m_CurrentProgress = m_CurrentProgress - 1;
        }

        if (m_CurrentProgress == 0)
        {
            m_CurrentProgress = m_ProductionTime_Elves;
            //instantiate m_ProductionItem at next conveyer in cycle (4 possible outputs)  %%daniel
        }

    }

    // Update is called once per frame
    protected override void Update()
    {
        //base.Update();

        if (m_IsProducing)
        {
            m_CurrentProgress += Time.deltaTime;
            if (m_CurrentProgress >= m_ProductionTime)
            {
                ProduceItem();
                m_CurrentProgress = 0;
            }
        }

        // 
    }

    public void AddConveyor(Conveyor conveyor)
    {
        m_AttachedConveyors.Add(conveyor);
    }

    public void ProduceItem()
    {
        // We pick a conveyor belt.
        if (m_AttachedConveyors.Count > 0)
        {
            Conveyor conveyorChosen = m_AttachedConveyors[m_CurrentConveyorBelt];

            // Create the item.
            Item newItem = GameObject.Instantiate(m_ProductionItem);
            newItem.m_CurrentConveyor = conveyorChosen;
            conveyorChosen.AddItem(newItem);

            if (m_CurrentConveyorBelt + 1 > m_AttachedConveyors.Count - 1)
            {
                m_CurrentConveyorBelt = 0;
            }
            else
                m_CurrentConveyorBelt++;
        }
        else
        {
            Debug.Log("Factory does not have any conveyor belts!");
        }
    }
}
