using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Overlay : MonoBehaviour
{
    public GameObject overlayTilePrefab;
    public GameObject overlayContainer;

    public static Overlay instance;
    public List<Vector3Int> tiles; //contains all overlay tile positions (in tilemap coords) which only exist on very top of map
    public List<Vector2Int> tiles2D; //all overlay tile positions without z coordinate (in tilemap coords)
    public Dictionary<Vector3Int, Vector3> tilesGridandWorldPos;
    private bool preTileExists;

    //NODEGRID:
    //public Transform player;
    public LayerMask unwalkableMask;
    Node[,] grid;
    //^these are set in the inspector
    int gridSizeX, gridSizeY; //can eventually switch to using tilemap.boundsint


    //I'm not certain what this does, I'll keep it in until it starts causing errors \_(-^-)_/
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
        } //deletes overlay tiles if script is/has already been run
    }
    
    // Start is called before the first frame update
    void Start()
    {   
        //why do you need to instantiate dictionaries but not lists?
        tilesGridandWorldPos = new Dictionary<Vector3Int, Vector3>();

        if (overlayTilePrefab != null)
        {
            preTileExists = true;
        }

        //Tilemap tilemap = GetComponent<Tilemap>(); //Tilemap isn't a component of Grid, but rather a child, so causes errors
        var tileMap = gameObject.GetComponentInChildren<Tilemap>();
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

            Vector3 tileWorldPos = cellPosition + tileOfset;
            tilesGridandWorldPos.Add(location, tileWorldPos);

            if (preTileExists == true)
            {
                overlayTilePrefab.transform.position = tileWorldPos;
                preTileExists = false;
            } 
            else 
            {
                if (tileOfset != new Vector3(0,0,0))
                {
                    Instantiate(overlayTilePrefab, tileWorldPos, Quaternion.identity, overlayContainer.transform);
                }
            }
        }

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

    //NODEGRID:
    void CreateGrid()
    {
		foreach (var overlayTilePos in tilesGridandWorldPos)
        {
            bool walkable = true;
			//bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadiusX, unwalkableMask));
			//^THIS MAY NOT BE THE WAY YOU WANT TO CHECK FOR COLLISIONS!
			
			grid[overlayTilePos.Key.x, overlayTilePos.Key.y] = new Node(walkable, overlayTilePos.Value, overlayTilePos.Key.x, overlayTilePos.Key.y);
		}
    }

    public List<Node> GetNeighbours(Node node)
    {
	    List<Node> neighbours = new List<Node>();
	    for (int x = -1; x <= 1; x++)
	    {
		    for (int y = -1; y <= 1; y++)
		    {
			    if (x == 0 && y == 0)
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

    public Node NodeFromWorldPoint(Vector3 targetWorldPosition) //can get world point from raycast in from Cursor script
    {
		Debug.Log("NodeFromWorldPoint was called");
        
        //THIS IS BORROWED FROM CURSOR SCRIPT! (You need to turn it into a function that you can reuse)
        RaycastHit2D hit = Physics2D.Raycast(targetWorldPosition, Vector2.zero);

        Debug.Log("Raycast hit: " + hit);

        if (hit.collider != null)
        {
            Debug.Log("hit.collider != null");
            GameObject hitOverlayTile = hit.collider.gameObject;
            Vector3 hitOverlayTilePos = hit.transform.position;

			foreach (var overlayTilePos in tilesGridandWorldPos)
			{
				if (hitOverlayTilePos == overlayTilePos.Value)
				{
					Debug.Log("hitOverlayTilePos is in overlayWorldGrid");
                    int x = overlayTilePos.Key.x;
			        int y = overlayTilePos.Key.y;
	
            	    return grid[x,y];
				}
			}
			
			//int index = overlayWorldGrid.IndexOf(hitOverlayTilePos); //returns -1
			//Debug.Log("Index of hitOverlayTilePos in overlayWorldGrid: " + index);
		
			/*int x = overlayGrid[index].x;
			int y = overlayGrid[index].y;
	
        	return grid[x,y];*/
        }

        Debug.Log("hit.collider = null");
		return null;
    }

    public List<Node> path;

    void OnDrawGizmos()
    {
	    if (grid != null)
        {
            //Node playerNode = NodeFromWorldPoint(player.position);
		
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
			
                /*if (playerNode == n) 
			    {
				    Gizmos.color = Color.cyan;
			    }*/

			    Gizmos.DrawCube(n.worldPosition, new Vector3(0.25f, 0.25f, 0.25f));
		    }
        }
    }
}
