using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Overlay : MonoBehaviour
{
    public GameObject overlayTilePrefab;
    public GameObject overlayContainer;

    public static Overlay instance;
    public List<Vector3Int> tiles; //contains all overlay tile positions (which only exist on very top of map)
    public List<Vector2Int> tiles2D; //all overlay tile positions without z coordinate
    private bool preTileExists;

    //I'm not sure what this does, I'll keep it in until it starts causing errors \_(-^-)_/
    private void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(this.gameObject);
            Debug.Log("This game object was destroyed");
        } else
        {
            instance = this;
            Debug.Log("instance was set to this");
        } //I think this just prevents the script from running if its already being run/from generating tiles if the ties are already there
    }
    
    // Start is called before the first frame update
    void Start()
    {   
        if (overlayTilePrefab != null)
        {
            preTileExists = true;
        }

        //Tilemap tilemap = GetComponent<Tilemap>(); //Tilemap isn't a component of Grid, but rather a child, so causes errors
        var tileMap = gameObject.GetComponentInChildren<Tilemap>(); //Acknowledges Grid parent, so works
        tileMap.CompressBounds();

        Vector3Int origin = tileMap.origin;
        BoundsInt size = tileMap.cellBounds;

        Debug.Log("Tilemap origin: (" + origin.x + ", " + origin.y + ", " + origin.z + ")");
        Debug.Log("Boundsint size: (" + size.x + ", " + size.y + ", " + size.z + ")");

        foreach (var tileLocation in size.allPositionsWithin) //something goes wrong when z = -2. Why?
        {
            if (tileMap.HasTile(tileLocation) && !tiles2D.Contains(new Vector2Int(tileLocation.x +2, tileLocation.y+2)))
            {
                tiles.Add(tileLocation);
                tiles2D.Add(new Vector2Int(tileLocation.x, tileLocation.y));
                //Debug.Log("Tilemap tile location:" + tileLocation);
            }
        }

        foreach (var location in tiles)
        {
            Vector3 cellPosition = tileMap.GetCellCenterWorld(location);
            TileBase tileBase = tileMap.GetTile(location);
            string tileName = tileBase.name.ToString();
            Vector3 tileOfset = tileTypeIdentifier(tileName);

            if (preTileExists == true)
            {
                overlayTilePrefab.transform.position = cellPosition + tileOfset;
                preTileExists = false;
            } 
            else 
            {
                if (tileOfset != new Vector3(0,0,0))
                {
                    overlayRender(overlayTilePrefab, cellPosition, tileOfset);
                }
            }
        }

    }

    void overlayRender (GameObject overlayTilePrefab, Vector3 position, Vector3 transformation)
    {
        Vector3 worldTileLocation = new Vector3(position.x + transformation.x, position.y + transformation.y, position.z + transformation.z);
        Instantiate(overlayTilePrefab, worldTileLocation, Quaternion.identity, overlayContainer.transform); //Quaternion is the rotation (I think)
        Debug.Log("TileLocation: (" + worldTileLocation.x + ", " + worldTileLocation.y + ", " + worldTileLocation.z + ")");
    }

    private Vector3 tileTypeIdentifier(string _tileName)
    {
        Vector3 _tileOfset = new Vector3(0,0,0);

        if (_tileName == "Dirt Path Full Height" || _tileName == "Grass and Flower Full Height" /*"isometric-sandbox-sheet_0"*/)
        {
            _tileOfset = new Vector3(0, 0.5f, -1);
        }
        if (_tileName == "Dirt Path Half Height" || _tileName == "Grass and Flower Half Height" /*"isometric-sandbox-sheet_1"*/)
        {
            _tileOfset = new Vector3(0, 0.25f, -1);
        }
        //You have to do this for every new type of tile you add to the tilemap.

        //THE BUG WHERE CERTAIN OVERLAYTILES DONT SORT PROPERLY BEHIDE TILEMAP TILES IS STILL PRESENT!
        
        return _tileOfset;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
