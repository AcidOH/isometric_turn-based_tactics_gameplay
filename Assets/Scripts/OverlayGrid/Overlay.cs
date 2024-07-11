using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Tilemaps;
//using UnityEngine.Timeline;

public class Overlay : MonoBehaviour
{
    public GameObject overlayTilePrefab;
    public GameObject overlayContainer;

    //public static Overlay instance;
    public List<Vector3Int> tiles; //contains all overlay tile positions (in tilemap coords) which only exist on very top of map
    public List<Vector2Int> tiles2D; //all overlay tile positions without z coordinate (in tilemap coords)
    public List<(Vector3Int, Vector3, GameObject)> nodeAndTileInfo;
    //public Dictionary<Vector3Int, Vector3> tilesGridandWorldPos;
    private bool preTileExists;

    //NODEGRID:
    //public bool onlyDisplayPathGizmos;
    public LayerMask unwalkableMask;
    public int gridSizeX, gridSizeY; //can eventually switch to using tilemap.boundsint

    [HideInInspector]
    public Node[,] grid;

    [HideInInspector]
    public List<Node> path;

    PathIndicator pathIndicator;
    Cursor cursor;

    //What the code deletes depends on what Overlay is attached to
    //deletes Grid and Tilemap when not attached to Grid...
    //also sometimes deletes A* game object when attached to A*

    /*private void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(this.gameObject);
            Debug.Log("This game object was destroyed");
        } else
        {
            instance = this;
            Debug.Log("instance was set to this");
        } //deletes overlay tiles if script is/has already been run
    }*/


    void Start()
    {
        pathIndicator = GetComponent<PathIndicator>();
        cursor = GameObject.Find("Cursor").GetComponentInChildren<Cursor>();

        //tilesGridandWorldPos = new Dictionary<Vector3Int, Vector3>();
        nodeAndTileInfo = new List<(Vector3Int, Vector3, GameObject)> ();

        if (overlayTilePrefab != null)
        {
            preTileExists = true;
        }

        var tileMap = GameObject.Find("Grid").GetComponentInChildren<Tilemap>();
        Debug.Log("Tilemap: " + tileMap);
        
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
            }
        }

        foreach (var location in tiles)
        {
            Vector3 cellPosition = tileMap.GetCellCenterWorld(location);
            TileBase tileBase = tileMap.GetTile(location);
            string tileName = tileBase.name.ToString();
            Vector3 tileOfset = tileTypeIdentifier(tileName);

            Vector3 tileWorldPos = cellPosition + tileOfset;

            GameObject tile = overlayTilePrefab;

            if (preTileExists == true)
            {
                overlayTilePrefab.transform.position = tileWorldPos;
                preTileExists = false;
            } 
            else 
            {
                if (tileOfset != Vector3.zero)
                {
                    tile = Instantiate(overlayTilePrefab, tileWorldPos, Quaternion.identity, overlayContainer.transform);
                }
            }

            nodeAndTileInfo.Add((location - origin, tileWorldPos, tile));
        }

        CreateGrid();
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

        //THE BUG WHERE CERTAIN OVERLAYTILES DON'T SORT PROPERLY BEHIDE TILEMAP TILES IS STILL PRESENT!
        
        return _tileOfset;
    }

    //NODEGRID:
    public int maxHeapSize
    {
        get
        {
            return gridSizeX*gridSizeY;
        }
    }
    
    void CreateGrid()
    {	
        grid = new Node[gridSizeX, gridSizeY];
        
        foreach (var overlayTilePos in nodeAndTileInfo)
        {
            bool walkable = true;
            //bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadiusX, unwalkableMask));
            //^THIS MAY NOT BE THE WAY YOU WANT TO CHECK FOR COLLISIONS!

            grid[overlayTilePos.Item1.x, overlayTilePos.Item1.y] = new Node(walkable, overlayTilePos.Item2, overlayTilePos.Item1.x, overlayTilePos.Item1.y, overlayTilePos.Item3);
        }
    }

    public List<Node> GetNeighbours(Node node)
    {
	    List<Node> neighbours = new List<Node>();
	    for (int x = -1; x <= 1; x++)
	    {
		    for (int y = -1; y <= 1; y++)
		    {
			    if ((x == y) || (x == -1 && y == 1) || (x == 1 && y == -1)) //ignores of corners and center
			    //if (x == 0 && y == 0)
                {
				    continue;
			    }
			    
                int checkX = node.gridX + x; //gridX and gridY are properties of Node class
			    int checkY = node.gridY + y;
			
                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbours.Add(grid[checkX,checkY]);
                }
		    }
	    }
	    return neighbours;
    }

    public Node NodeFromWorldPoint(Vector3 targetWorldPosition)
    {
        //(you previously used raycast the same way as in Cursor script to find the node)
        //(only works if targetWorldPosition is snapped into the grid (which should be the case))
        
        foreach (var overlayTilePos in nodeAndTileInfo)
        {
            if (overlayTilePos.Item2 == targetWorldPosition)
            {
                int x = overlayTilePos.Item1.x;
                int y = overlayTilePos.Item1.y;

                return grid[x, y];
            }
        }
        return null;
    }


    /*void OnDrawGizmos()
    {
        if (grid != null)
        {
	        foreach (Node n in grid)
            {
                Gizmos.color = (n.walkable)?Color.white:Color.red;
			
                if (path != null)
			    {  
	                if (path.Contains(n))
			        {
			            Gizmos.color = Color.black;
		            }
	            }
                
                Gizmos.DrawCube(n.worldPosition, new Vector3(0.25f, 0.25f, 0.25f));
		    }
        }
    }*/
}
