using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager s_Instance = null;

    public Camera m_Camera;


    [Header("World Settings")]
    public int m_Width = 64;
    public int m_Height = 64;

    public GameObject m_BackgroundTilePrefab;
    public GameObject m_SelectionOverlayTilePrefab;

    [HideInInspector]
    public GameObject m_SelectionOverlayObj = null;


    public float m_Cash;
    public float m_TimeLeft;


    public Clickable m_CurrentSelection;
    public Clickable m_CurrentPlaceable;

    public float m_GiftsPerSecond;
    public int m_TotalGiftsSent;


    public Vector3 m_HalfExtentBG;

    // Storing information for the world map.
    public List<List<Clickable>> m_WorldGrid;

    public GameObject m_TestItemPrefab;

    public Transform m_StartConveyor = null;
    public bool m_IsPlacingConveyor = false;
    public Vector2 testpoint = Vector2.zero;
    public Vector2 testpoint2 = Vector2.zero;
    public HANDLE_TYPE m_HeldHandleDirection;

    public List<GameObject> m_PlannedConveyors = new List<GameObject>();


    public Sprite m_ConveyorSprite;

    public int m_TotalPlacedSegments = 0;

    public float m_CurrentTempSpriteRotation = 0;



    private void Awake()
    {
        if (s_Instance == null)
        {
            s_Instance = this;
        }
        else
        {
            Debug.Log("You have more than 1 GameManager in the scene!");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        BoxCollider2D collider = m_BackgroundTilePrefab.GetComponent<BoxCollider2D>();

        m_HalfExtentBG.x = 0.32f;
        m_HalfExtentBG.y = 0.32f;

        m_SelectionOverlayObj = GameObject.Instantiate(m_SelectionOverlayTilePrefab);
        m_SelectionOverlayObj.transform.position = Vector3.zero;


        GenerateBackground();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            // Place a present on the conveyer belt that the mouse is currently over.
            if (m_CurrentSelection is Conveyor)
            {
                Conveyor conveyorSelected = (Conveyor)m_CurrentSelection;

                GameObject newItem = GameObject.Instantiate(m_TestItemPrefab);
                conveyorSelected.AddItem(newItem.GetComponent<Item>());

                Item newItemItem = newItem.GetComponent<Item>();
                newItemItem.m_CurrentConveyor = conveyorSelected;
            }
        }

        if (m_IsPlacingConveyor)
        {
            Vector2 mousePos = Input.mousePosition;

            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(mousePos), Vector2.zero);

            testpoint = hit.point;

            // If X is greater, draw horizontally, otherwise draw vertically.
            float xDistance = hit.point.x - m_StartConveyor.position.x;
            float yDistance = hit.point.y - m_StartConveyor.position.y;

            Vector2 toHit = (Vector2)hit.point - (Vector2)m_StartConveyor.position;
            testpoint = hit.point;

            Debug.Log("xDistance: " + Mathf.Abs(xDistance) + ", yDistance: " + Mathf.Abs(yDistance));



            if (Mathf.Abs(xDistance) > Mathf.Abs(yDistance))
            {
                int requiredSegments = (int)(Mathf.Abs(xDistance) / 0.32f);
                testpoint2.y = m_StartConveyor.position.y;

                // Set conveyor plans based on handle type.
                if (m_HeldHandleDirection == HANDLE_TYPE.RIGHT)
                {
                    testpoint2.x = /*m_StartConveyor.position.x + 0.32f / 2) + */ m_StartConveyor.position.x + requiredSegments * 0.32f;
                }

                if (xDistance < 0)
                {
                    testpoint2.x = /*m_StartConveyor.position.x + 0.32f / 2) + */ m_StartConveyor.position.x + -requiredSegments * 0.32f;
                }
                else
                    testpoint2.x = /*m_StartConveyor.position.x + 0.32f / 2) + */ m_StartConveyor.position.x + requiredSegments * 0.32f;

                if (m_TotalPlacedSegments > requiredSegments)
                {
                    // We need to delete some of the planned conveyor belts.
                    //int amountOfExtra = m_PlannedConveyors.Count - requiredSegments;
                    //for (int i = amountOfExtra; i > 0; i--)
                    //{
                    //    GameObject.Destroy(m_PlannedConveyors[amountOfExtra + m_PlannedConveyors.Count]);
                    //    m_PlannedConveyors.RemoveAt(amountOfExtra + m_PlannedConveyors.Count);
                    //}

                    int amountOfExtra = m_TotalPlacedSegments - requiredSegments;
                    for (int i = m_PlannedConveyors.Count - 1; i > requiredSegments - 1; i--)
                    {
                        m_PlannedConveyors[i].GetComponent<Clickable>().ClearTempSprite();
                        m_PlannedConveyors.RemoveAt(i);
                        m_TotalPlacedSegments--;
                    }
                    //m_PlannedConveyors.Clear();

                    int hi = 5;
                }

                for (int i = 0; i < requiredSegments; i++)
                {
                    if (xDistance < 0)
                    {
                        if (m_HeldHandleDirection == HANDLE_TYPE.LEFT)
                        {
                            for (int j = 0; j < requiredSegments; j++)
                            {
                                Clickable conveyorTile = m_StartConveyor.GetComponent<Clickable>();
                                Clickable newTile = GetTile((int)conveyorTile.m_WorldIndex.x - requiredSegments, (int)conveyorTile.m_WorldIndex.y);
                                newTile.SetTempSprite(m_ConveyorSprite, 180);
                                m_CurrentTempSpriteRotation = 180;

                                if (m_TotalPlacedSegments < requiredSegments)
                                {
                                    m_PlannedConveyors.Add(newTile.gameObject);
                                    Debug.Log("==================================== Added conveyor to list =========================================");
                                    m_TotalPlacedSegments++;
                                }

                            }

                            //GameObject newPlannedConveyor = GameObject.Instantiate(m_CurrentPlaceable).gameObject;
                            //newPlannedConveyor.transform.position = new Vector3(m_StartConveyor.position.x + -requiredSegments * 0.32f, m_StartConveyor.position.y, 0);
                            //m_PlannedConveyors.Add(newPlannedConveyor);
                        }
                    }
                    else
                    {
                        if (m_HeldHandleDirection == HANDLE_TYPE.RIGHT)
                        {
                            for (int j = 0; j < requiredSegments; j++)
                            {
                                Clickable conveyorTile = m_StartConveyor.GetComponent<Clickable>();
                                Clickable newTile = GetTile((int)conveyorTile.m_WorldIndex.x + requiredSegments, (int)conveyorTile.m_WorldIndex.y);
                                newTile.SetTempSprite(m_ConveyorSprite, 0);
                                m_CurrentTempSpriteRotation = 0;

                                if (m_TotalPlacedSegments < requiredSegments)
                                {
                                    m_PlannedConveyors.Add(newTile.gameObject);
                                    Debug.Log("==================================== Added conveyor to list =========================================");
                                    m_TotalPlacedSegments++;
                                }

                            }

                            //GameObject newPlannedConveyor = GameObject.Instantiate(m_CurrentPlaceable).gameObject;
                            //newPlannedConveyor.transform.position = new Vector3(m_StartConveyor.position.x + requiredSegments * 0.32f, m_StartConveyor.position.y, 0);
                            //m_PlannedConveyors.Add(newPlannedConveyor);
                        }

                    }
                }
            }
            else
            {
                int requiredSegments = (int)(Mathf.Abs(yDistance) / 0.32f);
                testpoint2.x = m_StartConveyor.position.x;

                if (yDistance < 0)
                {
                    testpoint2.y = /*m_StartConveyor.position.x + 0.32f / 2) + */ m_StartConveyor.position.y + -requiredSegments * 0.32f;
                }
                else
                    testpoint2.y = /*m_StartConveyor.position.x + 0.32f / 2) + */ m_StartConveyor.position.y + requiredSegments * 0.32f;



                if (m_TotalPlacedSegments > requiredSegments)
                {
                    // We need to delete some of the planned conveyor belts.
                    //int amountOfExtra = m_PlannedConveyors.Count - requiredSegments;
                    //for (int i = amountOfExtra; i > 0; i--)
                    //{
                    //    GameObject.Destroy(m_PlannedConveyors[amountOfExtra + m_PlannedConveyors.Count]);
                    //    m_PlannedConveyors.RemoveAt(amountOfExtra + m_PlannedConveyors.Count);
                    //}

                    int amountOfExtra = m_TotalPlacedSegments - requiredSegments;
                    for (int i = m_PlannedConveyors.Count - 1; i > requiredSegments - 1; i--)
                    {
                        m_PlannedConveyors[i].GetComponent<Clickable>().ClearTempSprite();
                        m_PlannedConveyors.RemoveAt(i);
                        m_TotalPlacedSegments--;
                    }
                    //m_PlannedConveyors.Clear();

                    int hi = 5;
                }


                for (int i = 0; i < requiredSegments; i++)
                {


                    if (yDistance < 0)
                    {
                        if (m_HeldHandleDirection == HANDLE_TYPE.DOWN)
                        {
                            for (int j = 0; j < requiredSegments; j++)
                            {
                                Clickable conveyorTile = m_StartConveyor.GetComponent<Clickable>();
                                Clickable newTile = GetTile((int)conveyorTile.m_WorldIndex.x, (int)conveyorTile.m_WorldIndex.y - requiredSegments);
                                newTile.SetTempSprite(m_ConveyorSprite, -90);
                                m_CurrentTempSpriteRotation = -90;

                                if (m_TotalPlacedSegments < requiredSegments)
                                {
                                    m_PlannedConveyors.Add(newTile.gameObject);
                                    Debug.Log("==================================== Added conveyor to list =========================================");
                                    m_TotalPlacedSegments++;
                                }
                            }


                            
                            //GameObject newPlannedConveyor = GameObject.Instantiate(m_CurrentPlaceable).gameObject;
                            //newPlannedConveyor.transform.position = new Vector3(m_StartConveyor.position.x + -requiredSegments * 0.32f, m_StartConveyor.position.y, 0);
                            //m_PlannedConveyors.Add(newPlannedConveyor);
                        }

                    }
                    else
                    {

                        if (m_HeldHandleDirection == HANDLE_TYPE.UP)
                        {
                            for (int j = 0; j < requiredSegments; j++)
                            {
                                Clickable conveyorTile = m_StartConveyor.GetComponent<Clickable>();
                                Clickable newTile = GetTile((int)conveyorTile.m_WorldIndex.x, (int)conveyorTile.m_WorldIndex.y + requiredSegments);
                                newTile.SetTempSprite(m_ConveyorSprite, 90);

                                m_CurrentTempSpriteRotation = 90;

                                if (m_TotalPlacedSegments < requiredSegments)
                                {
                                    m_PlannedConveyors.Add(newTile.gameObject);
                                    Debug.Log("==================================== Added conveyor to list =========================================");
                                    m_TotalPlacedSegments++;
                                }
                            }



                            //GameObject newPlannedConveyor = GameObject.Instantiate(m_CurrentPlaceable).gameObject;
                            //newPlannedConveyor.transform.position = new Vector3(m_StartConveyor.position.x + requiredSegments * 0.32f, m_StartConveyor.position.y, 0);
                            //m_PlannedConveyors.Add(newPlannedConveyor);
                        }
                    }
                }


            }
            //else
            //{
            //    int requiredSegments = (int)(yDistance / 0.32f);
            //    testpoint2.x = m_StartConveyor.position.x;
            //    testpoint2.y = requiredSegments * 0.32f;
            //}


        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(testpoint, 0.1f);

        if (m_StartConveyor)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(m_StartConveyor.position, testpoint2);
            Gizmos.DrawSphere(m_StartConveyor.position, 0.05f);
        }

    }


    public void GenerateBackground()
    {
        m_WorldGrid = new List<List<Clickable>>();
        for (int i = 0; i < m_Width; i++)
        {
            m_WorldGrid.Add(new List<Clickable>());
            for (int j = 0; j < m_Height; j++)
            {
                GameObject newBg = GameObject.Instantiate(m_BackgroundTilePrefab);
                Vector2 newPos;
                newPos.x = i * m_HalfExtentBG.x;
                newPos.y = j * m_HalfExtentBG.y;
                newBg.transform.position = newPos;

                Clickable bgClickable = newBg.GetComponent<Clickable>();

                // Initialising world grid.
                m_WorldGrid[i].Add(bgClickable);

                bgClickable.m_WorldIndex.x = i;
                bgClickable.m_WorldIndex.y = j;
            }
        }
    }

    /// <summary>
    /// Places selection overlay over clickable object.
    /// </summary>
    /// <param name="clickable"></param>
    public void PlaceOverlay(Vector3 position)
    {
        m_SelectionOverlayObj.transform.position = position;
    }

    public void SelectTile(Clickable tileToSelect)
    {
        m_CurrentSelection = (Clickable)tileToSelect;

        if (tileToSelect.m_Type == TileTypes.CONVEYOR)
        {
            Debug.Log("test");
        }
    }

    public Clickable GetTile(int x, int y)
    {
        if (x < m_Width && x > -1)
        {
            if (y < m_Height && y > -1)
            {
                // Passed in a valid location on the grid.
                return m_WorldGrid[x][y];

            }
        }

        return null;

    }

    public void UpdateTile(int xIndex, int yIndex, Clickable newTile)
    {
        m_WorldGrid[xIndex][yIndex] = newTile;
    }

    public void ResizeSelectionOverlay(float width, float height)
    {
        SpriteRenderer selectionRenderer = m_SelectionOverlayObj.GetComponent<SpriteRenderer>();
        selectionRenderer.transform.localScale = new Vector3(width, height, 1);
    }

    public void ClearAllPlannedConveyor()
    {
        for (int i = m_PlannedConveyors.Count - 1; i > -1; i--)
        {
            m_PlannedConveyors[i].GetComponent<Clickable>().ClearTempSprite();
            m_PlannedConveyors.RemoveAt(i);
            m_TotalPlacedSegments--;
        }

        if (m_PlannedConveyors.Count > 0)
        {
            Debug.LogError("ClearAllPlannedConveyor() did not clear all!");
        }
    }

    public void StopPlanning()
    {
        m_IsPlacingConveyor = false;
        ClearAllPlannedConveyor();
    }

    public void ConfirmConveyorPlan()
    {
        for (int i = 0; i < m_PlannedConveyors.Count; i++)
        {
            Clickable planClickable = m_PlannedConveyors[i].gameObject.GetComponent<Clickable>();

            Clickable newRealConveyor = GameObject.Instantiate(m_CurrentPlaceable);
            newRealConveyor.transform.position = planClickable.transform.position;
            newRealConveyor.ClearTempSprite(); // Newly instantiated conveyors have their temporary renderered enabled so I hide it like this.
            newRealConveyor.m_WorldIndex = m_PlannedConveyors[i].GetComponent<Clickable>().m_WorldIndex; // Assign world index to new conveyor.

            // Have to tell the world that this new conveyor belt should replace the current world index reference.
            m_WorldGrid[(int)newRealConveyor.m_WorldIndex.x][(int)newRealConveyor.m_WorldIndex.y] = newRealConveyor;

            SpriteRenderer newConveyorRenderer = newRealConveyor.gameObject.GetComponentInChildren<SpriteRenderer>();
            newConveyorRenderer.transform.eulerAngles = new Vector3(0, 0, m_CurrentTempSpriteRotation);

            

            // Setting inputs/outputs and connections for this confirmed choice of conveyor belts.
            Conveyor newConveyorConveyor = (Conveyor)newRealConveyor;
            int connectionNum = GetNumDirFromHandleDir(m_HeldHandleDirection);
            newConveyorConveyor.m_ConnectionArray.m_Connections[connectionNum] = 1;

        }
        StopPlanning();        
    }

    public int GetNumDirFromHandleDir(HANDLE_TYPE handleDir)
    {
        switch (handleDir)
        {
            case HANDLE_TYPE.RIGHT:
                return 0;
            case HANDLE_TYPE.DOWN:
                return 1;
            case HANDLE_TYPE.LEFT:
                return 2;
            case HANDLE_TYPE.UP:
                return 3;

            default:
                Debug.LogError("GetNumDirFromHandleDir has broken!");
                return -1;
        }
    }
    
}
