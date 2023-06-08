//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.Tilemaps;
//public class PlayerMovement : MonoBehaviour
//{

//    public Tilemap tilemap;

//    public Tile[] walkableTiles;

//    List<Vector3Int> path;
//    int currentPathIndex;

//    Vector3Int currentTilePosition;

//    bool isMoving;

//    public Astar astar;


//    // Update is called once per frame
//    void Update()
//    {
//        if(Input.GetMouseButtonDown(0))
//        {
//            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
//            Debug.Log("world : "+worldPos);
//            Vector3Int clickedTilePos = tilemap.WorldToCell(worldPos); //Vector3 Pos -> Vector3Int∑Œ πŸ≤„¡ÿ¥Ÿ

//            if(CanMoveToTile(clickedTilePos))
//            {

//                currentTilePosition = tilemap.WorldToCell(transform.position);
                           
//                path = CalculatePath(currentTilePosition, clickedTilePos);
//                currentPathIndex = 0;
//                isMoving = true;
//            }

//            if(isMoving)
//            {
//                if(currentPathIndex<path.Count)
//                {
//                    Vector3 targetPos = tilemap.GetCellCenterWorld(path[currentPathIndex]);
//                    transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime);

//                    if(transform.position==targetPos)
//                    {
//                        currentTilePosition = path[currentPathIndex];
//                        currentPathIndex++;
//                    }
//                }
//                else
//                {
//                    isMoving = false;
//                }
//            }

//        }
//    }


//    bool CanMoveToTile(Vector3Int tilePos)
//    {
//        TileBase tile = tilemap.GetTile(tilePos);
//        if(tile!=null)
//        {
//            foreach(Tile walkable in walkableTiles)
//            {
//                if(tile==walkable)
//                {
//                    return true;
//                }
//            }
//        }

//        return false;
//    }


//    List<Vector3Int> CalculatePath(Vector3Int startPos,Vector3Int endPos)
//    {
//        List<Vector3Int> calculatedPath = new List<Vector3Int>();

//        if(astar!=null)
//        {
//            calculatedPath = astar.CalculatePath(startPos, endPos,maxOpenListSize: 1000);
//        }

//        return calculatedPath;
//    }
//}
