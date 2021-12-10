using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum HANDLE_TYPE
{ 
    LEFT,
    RIGHT,
    UP,
    DOWN
}
public class ConveyorHandle : Clickable
{
    public HANDLE_TYPE m_HandleType;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
    }

    protected override void HandleInput()
    {
        if (Input.GetMouseButtonDown(0) && GameManager.s_Instance.m_CurrentSelection == this)
        {

            if (GameManager.s_Instance.m_CurrentSelection.m_Type == TileTypes.CONVEYOR_HANDLE)
            {
                Debug.Log(" ============================ Clicked on a HANDLE ============================");
                GameManager.s_Instance.m_IsPlacingConveyor = true;
                GameManager.s_Instance.m_StartConveyor = transform.parent.transform;
                GameManager.s_Instance.m_HeldHandleDirection = m_HandleType;

                // Remove current placeable so we don't accidentally place while planning conveyor belts.
                GameManager.s_Instance.ClearPlaceable();

                GameManager.s_Instance.Unclick();
                
            }
        }
    }

	protected override void OnMouseOver()
	{
        //Debug.Log("Testing mouse over function.");
        if (GameManager.s_Instance.m_CurrentClicked == null || GameManager.s_Instance.m_CurrentClicked == this)
        { 
            DrawSelectionOverlay(0.25f, 1, gameObject.transform.position);
        }
        GameManager.s_Instance.SelectTile(this);
        //Debug.Log("Selected tile is: " + this.gameObject.name);
    }

    protected override void DrawSelectionOverlay(float width, float height, Vector3 position)
    {

        base.DrawSelectionOverlay(width, height, position);

    }

}
