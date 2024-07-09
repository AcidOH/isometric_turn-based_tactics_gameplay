using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IHeapItem<Node>
{
	public bool walkable;
	public Vector3 worldPosition; //world position of the node
	public int gridX;
	public int gridY;
	public GameObject overlayTile;

	public int gCost;
	public int hCost;
	public Node parent;
	int heapIndex;
	public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY, GameObject _overlayTile) //assigns values when node is created
	{
		walkable = _walkable;
		worldPosition = _worldPos;
		gridX = _gridX;
		gridY = _gridY;
		overlayTile = _overlayTile;
	}
	
	public int fCost
	{
		get 
		{
			return gCost + hCost;
		}
	}

	public int HeapIndex
	{
		get
		{
			return heapIndex;
		}
		set
		{
			heapIndex = value;
		}
	}
	public int CompareTo(Node nodeToCompare)
	{
		//int CompareTo returns 1 is int is higher, 0 if same, and -1 if lower
		int compare = fCost.CompareTo(nodeToCompare.fCost);
		if (compare == 0)
		{
			compare = hCost.CompareTo(nodeToCompare.hCost);
		}
		return -compare;
	}
}
