using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PathIndicator : MonoBehaviour
{
    public List<Node> path;

    void Start()
    {
        path = GetComponent<Overlay>().path;
    }

    public enum ArrowDirection
    {
        None = 0,
        UpOrDown = 1,
        LeftOrRight = 2,
        TopRight = 3,
        BottomRight = 4,
        TopLeft = 5,
        BottomLeft = 6,
        UpEnd = 7,
        DownEnd = 8,
        LeftEnd = 9,
        RightEnd = 10
    }

    public ArrowDirection TranslateDirection(Vector2Int prev, Vector2Int next)
    {
        //FINDING THE DIRECTION
        //(you don't need to use else if because of the return)
        //remember that prev and next are vectors that point in the direction of movement (rather than the tile pos w/r to curr)

        if (prev == Vector2Int.up) //(below curr, pointing up)
        {
            if (next == Vector2Int.up) return ArrowDirection.UpOrDown;
            if (next == Vector2Int.left) return ArrowDirection.BottomLeft;
            if (next == Vector2Int.right) return ArrowDirection.BottomRight;
            if (next == Vector2Int.zero) return ArrowDirection.UpEnd;
        }
        if (prev == Vector2Int.down)
        {
            if (next == Vector2Int.down) return ArrowDirection.UpOrDown;
            if (next == Vector2Int.left) return ArrowDirection.TopLeft;
            if (next == Vector2Int.right) return ArrowDirection.TopRight;
            if (next == Vector2Int.zero) return ArrowDirection.DownEnd;
        }
        if (prev == Vector2Int.left)
        {
            if (next == Vector2Int.up) return ArrowDirection.TopRight;
            if (next == Vector2Int.down) return ArrowDirection.BottomRight;
            if (next == Vector2Int.left) return ArrowDirection.LeftOrRight;
            if (next == Vector2Int.zero) return ArrowDirection.LeftEnd;
        }
        if (prev == Vector2Int.right)
        {
            if (next == Vector2Int.up) return ArrowDirection.TopLeft;
            if (next == Vector2Int.down) return ArrowDirection.BottomLeft;
            if (next == Vector2Int.right) return ArrowDirection.LeftOrRight;
            if (next == Vector2Int.zero) return ArrowDirection.RightEnd;
        }
        if (prev == Vector2Int.zero)
        {
            if (next == Vector2Int.up || next == Vector2Int.down) return ArrowDirection.UpOrDown;
            if (next == Vector2Int.left || next == Vector2Int.right) return ArrowDirection.LeftOrRight;
        }
        return ArrowDirection.None;
    }
}
