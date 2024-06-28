using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//You should consider moving cursor and character sprites to their own layers.

public class Cursor : MonoBehaviour
{
    private Camera cam;
    //public GameObject cursor;
    private Vector2 direction; //Declaring here (and not updating in script) means can be changed from game window (as opposed to opening code and changing)
    public GameObject hitOverlayTile;
    public Vector3 hitOverlayTilePos;
    public GameObject player;
    
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        Debug.Log("Camera height: " + cam.pixelHeight + ", width: " + cam.pixelWidth);
    }

    // Update is called once per frame
    void Update()
    {
        //RaycastHit2D hit = Physics2D.Raycast(this.gameObject.transform.position, direction, 10.0f);
        
        Vector3 tempMousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(tempMousePos, Vector2.zero);

        if (hit.collider != null)
        {
            //Transform objectHit = hit.transform;
            hitOverlayTile = hit.collider.gameObject;
            hitOverlayTilePos = hit.transform.position;
            //Debug.Log("hit.transform: (" + hitOverlayTilePos + ")");
        }
    }

    void LateUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //Debug.Log("OBJECT COLOUR CURSOR: " + hitOverlayTile.GetComponent<SpriteRenderer>().color);
            Debug.Log("Was clicked. " + hitOverlayTile);
            if (hitOverlayTile.GetComponent<SpriteRenderer>().color != new Color(1,1,1,1))
            {
                hitOverlayTile.GetComponent<IndividualTiles>().ShowTile();
                //IndividualTiles always sets the colour back to invisible before LateUpdate starts. Have to find a away around that or try a different system.

                //cam.transform.position = new Vector3(hitOverlayTilePos.x, hitOverlayTilePos.y, hitOverlayTilePos.z -10);
                //^use for beginning of player phase, moveing, selecting someone to attack

                playerMovement(hitOverlayTilePos);
                //^this is a temp function call, find a way to do the same thing in player script instead
            }
        }
    }

    void OnGUI()
    {
        //Transforms a point from screen space to world space
        //World space coordinates can be calculated even when off the screen
        //Screen space is defines in pixels: Bottom left is (0,0), top right is (pixelWidth, pixelHeight)
        //and the z position is in world units from the camera

        Event currentEvent = Event.current;
        Vector2 mousePos = new Vector2();

        //Get the mouse position from Event
        //The y position from Event is inverted
        mousePos.x = currentEvent.mousePosition.x;
        mousePos.y = cam.pixelHeight - currentEvent.mousePosition.y;

        Vector3 worldMousePos = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, cam.nearClipPlane));
        //transform.position = worldMousePos; //just follows mouse exactly
        transform.position = new Vector3(hitOverlayTilePos.x, hitOverlayTilePos.y, hitOverlayTilePos.z - 0.5f);
        
        //A way to display info directly on the game window (as opposed to on console)
        GUILayout.BeginArea(new Rect(20,20,250,120));
        GUILayout.Label("Screen pixels: " + cam.pixelWidth + ":" + cam.pixelHeight);
        GUILayout.Label("Mouse position: " + mousePos);
        GUILayout.Label("World position: " + worldMousePos.ToString("F3"));
        GUILayout.EndArea();

    }

    //a temp fcn, need to find a different way to do this (in player script)
    void playerMovement(Vector3 _posToMoveTo)
    {
        player.transform.position = _posToMoveTo + new Vector3Int(0,0,-3);
        //^ -3 is an arbitrary number that only works if there aren't any tiles closer than -3.
    }
}
