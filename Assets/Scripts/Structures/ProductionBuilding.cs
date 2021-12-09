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





    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        // 
    }
}
