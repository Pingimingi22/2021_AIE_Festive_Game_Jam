using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clickable : MonoBehaviour
{
    public bool m_IsClicked;
    public TileTypes m_Type;
    public int m_ID = 0;

    public Vector2 m_WorldIndex;


    public SpriteRenderer m_Renderer;
    public SpriteRenderer m_TemporaryRenderer;
    public Sprite m_CacheSprite;
    public Sprite m_TemporarySprite;

    public void Click()
    { }

    private void Select()
    { }

    private void Deselect()
    { }


	// Start is called before the first frame update
	protected virtual void Start()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = transform.GetComponentInChildren<SpriteRenderer>();
        }

        m_Renderer = spriteRenderer;
        m_CacheSprite = m_Renderer.sprite;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        //Debug.Log("Parent update.");
        HandleInput();
    }

    protected virtual void HandleInput()
    {
        if (Input.GetMouseButtonDown(1) && GameManager.s_Instance.m_CurrentSelection == this)
        {
            if (GameManager.s_Instance.m_IsPlacingConveyor)
            {
                GameManager.s_Instance.StopPlanning();
            }
        }
        if (Input.GetMouseButtonDown(0) && GameManager.s_Instance.m_CurrentSelection == this)
        {


            if (GameManager.s_Instance.m_IsPlacingConveyor)
            {
                GameManager.s_Instance.ConfirmConveyorPlan();
            }

            //gameObject.name = "clicked";
            //if (m_Type == TileTypes.CONVEYOR)
            //{
            else if (GameManager.s_Instance.m_CurrentSelection.m_Type == TileTypes.CONVEYOR)
            {
                transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z + 90);
                //Debug.Log("ROTATED TILE.");
            }

            else if (GameManager.s_Instance.m_CurrentPlaceable != null)
            {
                //Debug.Log("PLACED TILE.");
                PlaceTile();
            }
            else
            {
                GameManager.s_Instance.Click(this);
            }
        }
        
        
    }

	protected virtual void OnMouseOver()
	{
        //Debug.Log("Testing mouse over function.");
        if (GameManager.s_Instance.m_CurrentClicked == null || GameManager.s_Instance.m_CurrentClicked == this)
        { 
            DrawSelectionOverlay(1, 1, transform.position);
        }
        GameManager.s_Instance.SelectTile(this);
        //Debug.Log("Selected tile is: " + this.gameObject.name);
	}

    protected virtual void DrawSelectionOverlay(float width, float height, Vector3 position)
    {

        GameManager.s_Instance.PlaceOverlay(position);
        if (width != 1)
        {
            int hi = 5;
        }
        GameManager.s_Instance.ResizeSelectionOverlay(width, height);

    }


    /// <summary>
    /// When we place a tile, we're going to do it the lazy way. Instead of converting an existing one to a new tile, we will
    /// instantiate the new tile and remove the old one. This is less efficient but it's a game jam so whatevs.
    /// </summary>
    private void PlaceTile()
    {
        if (GameManager.s_Instance.m_CurrentPlaceable) // If we have a placeable to place.
        {
            GameObject newTile = null;
            if (GameManager.s_Instance.m_CurrentPlaceable == GameManager.s_Instance.m_TestFactoryPrefab)
            {
                // Because this building is bigger than 1x1 we have to get all 10 of the tiles it goes over.
                int startX = (int)m_WorldIndex.x - 2;
                int startY = (int)m_WorldIndex.y - 2;

                int endX = (int)m_WorldIndex.x + 2;
                int endY = (int)m_WorldIndex.y + 2;

                // First we check if we can even start in the top left corner, if we can't, abort.
                if (startX < 0 || startX > GameManager.s_Instance.m_Width - 1)
                {
                    Debug.Log("========================== COULD NOT PLACE FACTORY ==========================");
                    return;
                }
                if (startY < 0 || startY > GameManager.s_Instance.m_Height - 1)
                {
                    Debug.Log("========================== COULD NOT PLACE FACTORY ==========================");
                    return;
                }

                // Okay, if we've got to here, we know the index is valid but now we have to check if the bottom right corner is valid.
                if (endX < 0 || endX > GameManager.s_Instance.m_Width - 1)
                {
                    Debug.Log("========================== COULD NOT PLACE FACTORY ==========================");
                    return;
                }
                if (endY < 0 || endY > GameManager.s_Instance.m_Height - 1)
                {
                    Debug.Log("========================== COULD NOT PLACE FACTORY ==========================");
                    return;
                }

                // Okay, now we know that the factory is within the grids limits.

                bool success = false;
                while (!success)
                {
                    success = true;
                    for (int i = startX; i < endX; i++)
                    {
                        for (int j = startY; j < endY; j++)
                        {
                            if (GameManager.s_Instance.m_WorldGrid[i][j] == null)
                            {
                                success = false; // fail.
                                return;
                            }
                            else
                            {
                                // Can only place on BG tiles.
                                Clickable tileCheck = GameManager.s_Instance.m_WorldGrid[i][j].GetComponent<Clickable>();
                                if (tileCheck.m_Type != TileTypes.BG)
                                {
                                    success = false;
                                    return;
                                }
                            }
                        }
                    }
                }


                
                if (GameManager.s_Instance.m_CurrentPlaceable == GameManager.s_Instance.m_TestFactoryPrefab)
                {
                    newTile = GameObject.Instantiate(GameManager.s_Instance.m_CurrentPlaceable.gameObject);
                }

                ProductionBuilding productionBuilding = newTile.GetComponent<ProductionBuilding>();
                // Okay after all of that, we now know that we can place the factory down.
                for (int i = startX; i < endX + 1; i++)
                {
                    for (int j = startY; j < endY + 1; j++)
                    {
                        GameManager.s_Instance.m_WorldGrid[i][j].m_Type = TileTypes.BUILDING;
                        GameManager.s_Instance.m_WorldGrid[i][j].GetComponent<BoxCollider2D>().enabled = false;

                        

                        // Have to manually place real conveyors on the conveyor tiles.
                        if (i == m_WorldIndex.x + 2 && j == m_WorldIndex.y)
                        {
                            Conveyor newConveyor = ManuallyPlaceConveyor(i, j, 0);
                            productionBuilding.AddConveyor(newConveyor);
                        }
                        if (i == m_WorldIndex.x - 2 && j == m_WorldIndex.y)
                        {
                            Conveyor newConveyor = ManuallyPlaceConveyor(i, j, 180);
                            productionBuilding.AddConveyor(newConveyor);
                        }
                        if (i == m_WorldIndex.x && j == m_WorldIndex.y + 2)
                        {
                            Conveyor newConveyor = ManuallyPlaceConveyor(i, j, 90);
                            productionBuilding.AddConveyor(newConveyor);
                        }
                        if (i == m_WorldIndex.x && j == m_WorldIndex.y - 2)
                        {
                            Conveyor newConveyor = ManuallyPlaceConveyor(i, j, -90);
                            productionBuilding.AddConveyor(newConveyor);
                        }

                    }
                }
            }
            else if (GameManager.s_Instance.m_CurrentPlaceable == GameManager.s_Instance.m_TestWorkshopPrefab)
            {
                // Because this building is bigger than 1x1 we have to get all 10 of the tiles it goes over.
                int startX = (int)m_WorldIndex.x - 1;
                int startY = (int)m_WorldIndex.y + 1;

                int endX = (int)m_WorldIndex.x + 1;
                int endY = (int)m_WorldIndex.y - 1;

                // First we check if we can even start in the top left corner, if we can't, abort.
                if (startX < 0 || startX > GameManager.s_Instance.m_Width - 1)
                {
                    Debug.Log("========================== COULD NOT PLACE FACTORY ==========================");
                    return;
                }
                if (startY < 0 || startY > GameManager.s_Instance.m_Height - 1)
                {
                    Debug.Log("========================== COULD NOT PLACE FACTORY ==========================");
                    return;
                }

                // Okay, if we've got to here, we know the index is valid but now we have to check if the bottom right corner is valid.
                if (endX < 0 || endX > GameManager.s_Instance.m_Width - 1)
                {
                    Debug.Log("========================== COULD NOT PLACE FACTORY ==========================");
                    return;
                }
                if (endY < 0 || endY > GameManager.s_Instance.m_Height - 1)
                {
                    Debug.Log("========================== COULD NOT PLACE FACTORY ==========================");
                    return;
                }

                // Okay, now we know that the factory is within the grids limits.

                bool success = false;
                while (!success)
                {
                    success = true;
                    for (int i = startX; i < endX + 1; i++)
                    {
                        for (int j = startY; j > endY - 1; j--)
                        {
                            if (GameManager.s_Instance.m_WorldGrid[i][j] == null)
                            {
                                success = false; // fail.
                                return;
                            }
                            else
                            {
                                // Can only place on BG tiles.
                                Clickable tileCheck = GameManager.s_Instance.m_WorldGrid[i][j].GetComponent<Clickable>();
                                if (tileCheck.m_Type != TileTypes.BG)
                                {
                                    success = false;
                                    return;
                                }
                            }
                        }
                    }
                }



                if (GameManager.s_Instance.m_CurrentPlaceable == GameManager.s_Instance.m_TestFactoryPrefab || GameManager.s_Instance.m_CurrentPlaceable == GameManager.s_Instance.m_TestWorkshopPrefab)
                {
                    newTile = GameObject.Instantiate(GameManager.s_Instance.m_CurrentPlaceable.gameObject);
                }

                ProductionBuilding productionBuilding = newTile.GetComponent<ProductionBuilding>();
                // Okay after all of that, we now know that we can place the factory down.
                for (int i = startX; i < endX + 1; i++)
                {
                    for (int j = startY; j > endY - 1; j--)
                    {
                        GameManager.s_Instance.m_WorldGrid[i][j].m_Type = TileTypes.BUILDING;
                        GameManager.s_Instance.m_WorldGrid[i][j].GetComponent<BoxCollider2D>().enabled = false;



                        // Have to manually place real conveyors on the conveyor tiles.
                        //if (i == m_WorldIndex.x + 2 && j == m_WorldIndex.y)
                        //{
                        //    Conveyor newConveyor = ManuallyPlaceConveyor(i, j, 0);
                        //    productionBuilding.AddConveyor(newConveyor);
                        //}
                        //if (i == m_WorldIndex.x - 2 && j == m_WorldIndex.y)
                        //{
                        //    Conveyor newConveyor = ManuallyPlaceConveyor(i, j, 180);
                        //    productionBuilding.AddConveyor(newConveyor);
                        //}
                        //if (i == m_WorldIndex.x && j == m_WorldIndex.y + 2)
                        //{
                        //    Conveyor newConveyor = ManuallyPlaceConveyor(i, j, 90);
                        //    productionBuilding.AddConveyor(newConveyor);
                        //}
                        //if (i == m_WorldIndex.x && j == m_WorldIndex.y - 2)
                        //{
                        //    Conveyor newConveyor = ManuallyPlaceConveyor(i, j, -90);
                        //    productionBuilding.AddConveyor(newConveyor);
                        //}

                    }
                }
            }


            // When we place a tile, we have to remove the collider from the tile beneath.
            this.GetComponent<BoxCollider2D>().enabled = false;

            if (GameManager.s_Instance.m_CurrentPlaceable != GameManager.s_Instance.m_TestFactoryPrefab)
            {
                newTile = GameObject.Instantiate(GameManager.s_Instance.m_CurrentPlaceable.gameObject);     
            }


            Clickable newClickable = newTile.GetComponent<Clickable>();
            newClickable.m_WorldIndex.x = m_WorldIndex.x;
            newClickable.m_WorldIndex.y = m_WorldIndex.y;
            GameManager.s_Instance.UpdateTile((int)m_WorldIndex.x, (int)m_WorldIndex.y, newClickable);


            if (GameManager.s_Instance.m_CurrentPlaceable is Structure || GameManager.s_Instance.m_CurrentPlaceable is Conveyor)
            {
                // The placed item is a structure, we have to connect it to the world grid.
                Structure newTileStructure = newTile.GetComponent<Structure>();
                //newTileStructure.Connect();

                
                if (GameManager.s_Instance.m_CurrentPlaceable is Conveyor)
                {
                    Conveyor newTileConveyor = newTile.GetComponent<Conveyor>();
                    

                    //newTileConveyor.ClearConnections();
                    //newTileConveyor.ResetInputs();

                    // Check if we have to connect things now that we've rotated.
                    //newTileConveyor.Connect();

                    
                }
            }



            newTile.transform.position = this.transform.position;

            

        }
    }



    /// <summary>
    /// Finds all surrounding tiles. 
    /// </summary>
    protected List<Clickable> GetSurroundingTiles(int dir, int excludeDirNum)
    {
        GameManager manager = GameManager.s_Instance;

        List<Clickable> surroundingTiles = new List<Clickable>();

        // ========================== RIGHT CONNECTION ========================== //
        if (manager.GetTile((int)m_WorldIndex.x + 1, (int)m_WorldIndex.y) && dir == 0)
        {
            Clickable rightTile = manager.GetTile((int)m_WorldIndex.x + 1, (int)m_WorldIndex.y);
   
            surroundingTiles.Add(rightTile);    
            
        }
        // ========================== LEFT CONNECTION ========================== //
        if (manager.GetTile((int)m_WorldIndex.x - 1, (int)m_WorldIndex.y) && dir == 2)
        {
            Clickable leftTile = manager.GetTile((int)m_WorldIndex.x - 1, (int)m_WorldIndex.y);

            surroundingTiles.Add(leftTile);
            
        }
        // ========================== UP CONNECTION ========================== //
        if (manager.GetTile((int)m_WorldIndex.x, (int)m_WorldIndex.y + 1) && dir == 3)
        {
            Clickable upTile = manager.GetTile((int)m_WorldIndex.x, (int)m_WorldIndex.y + 1);

                surroundingTiles.Add(upTile);
            
        }
        // ========================== DOWN CONNECTION ========================== //
        if (manager.GetTile((int)m_WorldIndex.x, (int)m_WorldIndex.y - 1) && dir == 1)
        {
            Clickable downTile = manager.GetTile((int)m_WorldIndex.x, (int)m_WorldIndex.y - 1);

                surroundingTiles.Add(downTile);
            
        }

        return surroundingTiles;
        
    }

    public void SetTempSprite(Sprite sprite, float rotation)
    {
        m_TemporarySprite = sprite;
        m_TemporaryRenderer.sprite = m_TemporarySprite;
        m_TemporaryRenderer.color = Color.yellow;

        m_TemporaryRenderer.transform.eulerAngles = new Vector3(0, 0, rotation);
        m_TemporaryRenderer.enabled = true;
    }

    public void ClearTempSprite()
    {
        m_TemporaryRenderer.sprite = null;
        m_TemporaryRenderer.enabled = false;
    }

    public Conveyor ManuallyPlaceConveyor(int worldXIndex, int worldYIndex, float rotation)
    {
        Clickable currentTile = GameManager.s_Instance.GetTile(worldXIndex, worldYIndex);

        Clickable newRealConveyor = GameObject.Instantiate(GameManager.s_Instance.m_ConveyorPrefab);
        newRealConveyor.transform.position = currentTile.transform.position;
        newRealConveyor.ClearTempSprite(); // Newly instantiated conveyors have their temporary renderered enabled so I hide it like this.
        newRealConveyor.m_WorldIndex.x = worldXIndex; // Assign world index to new conveyor.
        newRealConveyor.m_WorldIndex.y = worldYIndex;

        // Have to disable collider of background tile since we've placed something on top of it.
        GameManager.s_Instance.m_WorldGrid[(int)newRealConveyor.m_WorldIndex.x][(int)newRealConveyor.m_WorldIndex.y].GetComponent<BoxCollider2D>().enabled = false;

        // Have to tell the world that this new conveyor belt should replace the current world index reference.
        GameManager.s_Instance.m_WorldGrid[(int)newRealConveyor.m_WorldIndex.x][(int)newRealConveyor.m_WorldIndex.y] = newRealConveyor;

        SpriteRenderer newConveyorRenderer = newRealConveyor.gameObject.GetComponentInChildren<SpriteRenderer>();
        newConveyorRenderer.transform.eulerAngles = new Vector3(0, 0, rotation);

        return (Conveyor)newRealConveyor;
    }

}
