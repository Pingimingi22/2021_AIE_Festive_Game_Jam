using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingBuilding : Structure
{
    public int m_CurrentElves;
    public bool m_IsProducing;
    public int m_ResourceCost;
    public int m_CurrentResource;


    // connections struct todo.

    public float m_CurrentProgress;
    public Item m_ProductionItem; // Item that is produced by this building.
    public float m_ProductionTime;
    public float m_ProductionRate;
    public float m_ProductionTime_Elves;
    public List<GameObject> ItemQueue = new List<GameObject>();


    public List<Conveyor> m_AttachedConveyors = new List<Conveyor>();
    public int m_CurrentConveyorBelt;
    

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        BoxCollider2D ItemSort = gameObject.GetComponent<BoxCollider2D>();
        if (m_Level == 1 && ItemSort.tag == "Wood" ) //check if right resource for level
        {
            m_CurrentResource = m_CurrentResource + 1; //adding 1 to the inventory
            ItemQueue.Add(collision.gameObject);
        }
        else
        {
            GameObject.Destroy(collision.gameObject);
        }
        if (m_Level == 2 && ItemSort.tag == "Plastic") //check if right resource for level
        {
            m_CurrentResource = m_CurrentResource + 1; //adding 1 to the inventory
        }
        else
        {
            GameObject.Destroy(collision.gameObject);
        }
        if (m_Level == 3 && ItemSort.tag == "Metal") //check if right resource for level
        {
            m_CurrentResource = m_CurrentResource + 1; //adding 1 to the inventory
        }
        else
        {
            GameObject.Destroy(collision.gameObject);
        }
    }



    // Start is called before the first frame update
    void Start()
    {
        //m_ProductionRate = 60 / m_ProductionTime + 60/(2*m_ProductionTime)* m_CurrentElves;  prodcution rate in fps?? calculates items per minute
        //m_ProductionTime_Elves = 60/m_ProductionRate  production time in seconds after applying elves
                                 

        

        if (m_IsProducing)
        {
            m_CurrentProgress = m_CurrentProgress - 1;
        }

        if (m_CurrentProgress == 0)
        {
            m_CurrentProgress = m_ProductionTime_Elves;
            m_IsProducing = false;
            //instantiate m_ProductionItem at next conveyer in cycle (3 possible outputs)  %%daniel
        }

    }

    // Update is called once per frame
    protected override void Update()
    {
        //base.Update();

        // Checking if we can produce.
        if (m_CurrentResource >= m_ResourceCost)
        {
            m_IsProducing = true;
            m_CurrentResource = m_CurrentResource - m_ResourceCost;

            for (int i = 0; i < ItemQueue.Count; i++)
            {
                GameObject.Destroy(ItemQueue[i]);
            }
            ItemQueue.Clear();
        }


        if (m_IsProducing)
        {
            m_CurrentProgress += Time.deltaTime;
            if (m_CurrentProgress >= m_ProductionTime)
            {
                ProduceItem();
                m_CurrentProgress = 0;
            }
        }

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
            //Debug.Log("Factory does not have any conveyor belts!");
        }
    }

    public void AddConveyor(Conveyor conveyor)
    {
        m_AttachedConveyors.Add(conveyor);
    }

}
