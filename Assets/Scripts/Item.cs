using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public float m_Value;
    public int m_WoodCost;
    public int m_PlasticCost;
    public int m_ElectronicsCost;

    public Conveyor m_CurrentConveyor;

    public Conveyor m_PrevConveyor = null;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (m_CurrentConveyor)
        {
            UpdatePosition();
        }
    }

	private void OnCollisionEnter2D(Collision2D collision)
	{
        if (collision.transform.tag == "Conveyor")
        {
            m_CurrentConveyor = collision.gameObject.GetComponent<Conveyor>();
        }
	}

    private void UpdatePosition()
    {
        // We have to move the item to the centrepoint of the current conveyor
        // We also have to find out which conveyor belt to send the item too next.

        Conveyor nextConveyor;

        Vector2 movDir = Vector3.zero;
        if (m_CurrentConveyor != null)
        { 
            movDir = m_CurrentConveyor.transform.position - transform.position;
            movDir.Normalize();
        }

        Vector2 newPos;
        newPos.x = transform.position.x;
        newPos.y = transform.position.y;

        newPos += 0.01f * movDir;

        transform.position = newPos;


        if (Vector3.Distance(transform.position, m_CurrentConveyor.transform.position) < 0.05f)
        {
            //Debug.Log("Item reached conveyor midpoint!... Moving to next conveyor now.");
            m_PrevConveyor = m_CurrentConveyor;

            if (m_CurrentConveyor != null)
            { 
                m_CurrentConveyor.gameObject.name = "item passed";
            }
            m_CurrentConveyor = m_CurrentConveyor.GetNextConveyor(m_PrevConveyor);

            
        }
    }
}
