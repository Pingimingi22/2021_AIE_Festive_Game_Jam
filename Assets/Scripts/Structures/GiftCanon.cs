using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiftCanon : Structure
{
    public bool m_IsProducing;
    public int m_ResourceCost;
    public int m_CurrentResource;
    public Item m_ProductionItem;


    // connections struct todo.

    public float m_CurrentProgress;
    public float m_ProductionTime;
    public float m_ProductionRate;


    public List<GameObject> ItemQueue = new List<GameObject>();

    public Sprite m_Fuse1;
    public Sprite m_Fuse2;
    public Sprite m_Fuse3;
    public Sprite m_Fuse4;
    public Sprite m_Fuse5;

    public SpriteRenderer m_OurRenderer;

    public GameObject m_PresentLaunchPrefab;
    private void OnCollisionEnter2D(Collision2D collision)
    {

        BoxCollider2D ItemSort = gameObject.GetComponent<BoxCollider2D>();
        if (collision.gameObject.tag == "Present") //check if right resource for level
        {
            m_CurrentResource = m_CurrentResource + 1; //adding 1 to the inventory
            ItemQueue.Add(collision.gameObject);



            if (m_CurrentResource >= m_ResourceCost && !m_IsProducing)
            {
                m_IsProducing = true;

                for (int i = ItemQueue.Count - 1; i >= ItemQueue.Count - m_CurrentResource; i--)
                {
                    GameObject.Destroy(ItemQueue[i]);
                    ItemQueue.RemoveAt(i);
                    m_CurrentResource--;
                }

                //m_CurrentResource = m_CurrentResource - m_ResourceCost;

                //for (int i = 0; i < ItemQueue.Count; i++)
                //{
                //    if (ItemQueue[i].tag == "Wood1")
                //    {
                //        //add 10 to cash
                //    }
                //    else if (ItemQueue[i].tag == "Wood2")
                //    {
                //        //add 20 cash
                //    }
                //    else if (ItemQueue[i].tag == "Wood3")
                //    {
                //        //add 40 cash
                //    }
                //    else if (ItemQueue[i].tag == "Plastic1")
                //    {
                //        //add 25 cash
                //    }
                //    else if (ItemQueue[i].tag == "Plastic2")
                //    {
                //        //add 50 cash
                //    }
                //    else if (ItemQueue[i].tag == "Plastic3")
                //    {
                //        //add 100 cash
                //    }
                //    else if (ItemQueue[i].tag == "Metal1")
                //    {
                //        //add 45 cash
                //    }
                //    else if (ItemQueue[i].tag == "Metal2")
                //    {
                //        //add 125 cash
                //    }
                //    else if (ItemQueue[i].tag == "Metal3")
                //    {
                //        //add 250 cash
                //    }
                //    GameObject.Destroy(ItemQueue[i]);
                //}

            }

        }
        //else
        //{
        //    GameObject.Destroy(collision.gameObject);
        //}
    }
    


    // Start is called before the first frame update
    void Start()
    {
        //m_ProductionRate = 60 / m_ProductionTime + 60/(2*m_ProductionTime)* m_CurrentElves;  prodcution rate in fps?? calculates items per minute
        m_IsProducing = false;
    }
    void Update()
    {
        



        if (m_IsProducing)
        {
            m_CurrentProgress += Time.deltaTime;
            if (m_CurrentProgress <= 0.8 * m_ProductionTime)
            {
                //change to fuse 2
                //m_Renderer.sprite = m_Fuse2;
            }
            else if (m_CurrentProgress <= 0.6 * m_ProductionTime)
            {
                //change to fuse 3
                //m_Renderer.sprite = m_Fuse3;
            }
            else if (m_CurrentProgress <= 0.4 * m_ProductionTime)
            {
                //change to fuse 4
                //m_Renderer.sprite = m_Fuse4;
            }
            else if (m_CurrentProgress <= 0.2 * m_ProductionTime)
            {
                //change to fuse 5
                //m_Renderer.sprite = m_Fuse5;
            }

            // We are done producing.
            if (m_CurrentProgress >= m_ProductionTime)
            {
                m_IsProducing = false;
                m_CurrentProgress = 0;

                //instantiate present shooting through the sky
                //change to fuse 1

                GameManager.s_Instance.m_Cash += 100;

                GameObject newLaunchedPresent = GameObject.Instantiate(m_PresentLaunchPrefab);
                newLaunchedPresent.transform.position = transform.position;
                
            }
        }
    }
}
