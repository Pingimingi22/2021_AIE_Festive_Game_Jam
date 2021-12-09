﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clickable : MonoBehaviour
{
    public bool m_IsClicked;
    public TileTypes m_Type;
    public int m_ID = 0;

    public Vector2 m_WorldIndex;

    public void Click()
    { }

    private void Select()
    { }

    private void Deselect()
    { }


	// Start is called before the first frame update
	protected virtual void Start()
    {
        
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        Debug.Log("Parent update.");
        HandleInput();
    }

    protected virtual void HandleInput()
    {
        if (Input.GetMouseButtonDown(0) && GameManager.s_Instance.m_CurrentSelection == this)
        {

            gameObject.name = "clicked";
            //if (m_Type == TileTypes.CONVEYOR)
            //{
            if (GameManager.s_Instance.m_CurrentSelection.m_Type == TileTypes.CONVEYOR)
            {
                transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z + 90);
                Debug.Log("ROTATED TILE.");
            }

            else
            {
                Debug.Log("PLACED TILE.");
                PlaceTile();
            }
        }
    }

	private void OnMouseOver()
	{
        Debug.Log("Testing mouse over function.");
        DrawSelectionOverlay();
        GameManager.s_Instance.SelectTile(this);
        Debug.Log("Selected tile is: " + this.gameObject.name);
	}

    private void DrawSelectionOverlay()
    {
        GameManager.s_Instance.PlaceOverlay(this);
    }


    /// <summary>
    /// When we place a tile, we're going to do it the lazy way. Instead of converting an existing one to a new tile, we will
    /// instantiate the new tile and remove the old one. This is less efficient but it's a game jam so whatevs.
    /// </summary>
    private void PlaceTile()
    {
        if (GameManager.s_Instance.m_CurrentPlaceable) // If we have a placeable to place.
        {
            // When we place a tile, we have to remove the collider from the tile beneath.
            this.GetComponent<BoxCollider2D>().enabled = false;


            GameObject newTile = GameObject.Instantiate(GameManager.s_Instance.m_CurrentPlaceable.gameObject);

            Clickable newClickable = newTile.GetComponent<Clickable>();
            newClickable.m_WorldIndex.x = m_WorldIndex.x;
            newClickable.m_WorldIndex.y = m_WorldIndex.y;
            GameManager.s_Instance.UpdateTile((int)m_WorldIndex.x, (int)m_WorldIndex.y, newClickable);


            if (GameManager.s_Instance.m_CurrentPlaceable is Structure || GameManager.s_Instance.m_CurrentPlaceable is Conveyor)
            {
                // The placed item is a structure, we have to connect it to the world grid.
                Structure newTileStructure = newTile.GetComponent<Structure>();
                newTileStructure.Connect();
            }



            newTile.transform.position = this.transform.position;

            

        }
    }



    /// <summary>
    /// Finds all surrounding tiles. 
    /// </summary>
    protected List<Clickable> GetSurroundingTiles()
    {
        GameManager manager = GameManager.s_Instance;

        List<Clickable> surroundingTiles = new List<Clickable>();

        // ========================== RIGHT CONNECTION ========================== //
        if (manager.GetTile((int)m_WorldIndex.x + 1, (int)m_WorldIndex.y))
        {
            Clickable rightTile = manager.GetTile((int)m_WorldIndex.x + 1, (int)m_WorldIndex.y);
   
            surroundingTiles.Add(rightTile);    
            
        }
        // ========================== LEFT CONNECTION ========================== //
        if (manager.GetTile((int)m_WorldIndex.x - 1, (int)m_WorldIndex.y))
        {
            Clickable leftTile = manager.GetTile((int)m_WorldIndex.x - 1, (int)m_WorldIndex.y);

            surroundingTiles.Add(leftTile);
            
        }
        // ========================== UP CONNECTION ========================== //
        if (manager.GetTile((int)m_WorldIndex.x, (int)m_WorldIndex.y + 1))
        {
            Clickable upTile = manager.GetTile((int)m_WorldIndex.x, (int)m_WorldIndex.y + 1);

                surroundingTiles.Add(upTile);
            
        }
        // ========================== DOWN CONNECTION ========================== //
        if (manager.GetTile((int)m_WorldIndex.x, (int)m_WorldIndex.y - 1))
        {
            Clickable downTile = manager.GetTile((int)m_WorldIndex.x, (int)m_WorldIndex.y - 1);

                surroundingTiles.Add(downTile);
            
        }

        return surroundingTiles;
        
    }


}
