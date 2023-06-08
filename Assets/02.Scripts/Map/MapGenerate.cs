using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Node
{
    public Node leftNode;
    public Node rightNode;
    public Node parNode;
    public RectInt nodeRect;
    public RectInt roomRect;

    public Vector2Int center
    {
        get
        {
            return new Vector2Int(roomRect.x + roomRect.width / 2, roomRect.y + roomRect.height / 2);
        }
    }
    public Node(RectInt rect)
    {
        this.nodeRect = rect;
    }
}

public class MapGenerate : MonoBehaviour
{
    [SerializeField]
    Vector2Int mapSize;    //맵 크기

    [SerializeField]
    float minDevideRate;    //최소로 나눌 크기 현재 0.4

    [SerializeField]
    float maxDevideRate;    //최대로 나눌 크기 현재 0.6

    [SerializeField]        //line,map,roomLine 현재 어떤식으로 방이 나누어져있고 길이 이어져있는지 알기 위해 만든 lineRender용 prefab
    GameObject line;

    [SerializeField]
    GameObject line2;

    [SerializeField]
    GameObject map;

    [SerializeField]
    GameObject roomLine;

    [SerializeField]
    int maxDepth;   //트리구조의 최대 깊이 크기가 클수록 세밀하게 나누어지고 방이많아짐

    [SerializeField]
    Tilemap tilemap;    //타일맵 테스트중

    [SerializeField]
    Tile backgroundTile;          //타일맵 테스트중 

    [SerializeField]
    Tile[] groundTiles;

    [SerializeField]
    Tile[] roomTiles;

    [SerializeField]
    Tile[] wallTiles;

    [SerializeField]
    Tile[] objectTiles;

   
    public bool debugflag = false;
    Node root;


    private void Start()
    {
        root = new Node(new RectInt(0, 0, mapSize.x, mapSize.y));
        if (!debugflag)
        {
            DrawTileBackGround();
            Divide(root, 0);
            GenerateRoom(root, 0);
            GenerateLoad(root, 0);
        }

     

   
    }


    private void Update()
    {
      
        if (debugflag)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                DrawTileBackGround();
                //DrawMap(0, 0);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                Divide(root, 0);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                GenerateRoom(root, 0);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                GenerateLoad(root, 0);
            }
        }
   
        
    }


    void Divide(Node tree, int n) //사각형을 위에서 설정한 비율로 나누어줍니다 0.4/0.6 비율 (재귀적으로)
    {
        if (n == maxDepth) return;

        int maxLength = Mathf.Max(tree.nodeRect.width, tree.nodeRect.height);   //가로 세로 중 더 긴것으로 나누어주기위해
        int split = Mathf.RoundToInt(Random.Range(maxLength * minDevideRate, maxLength * maxDevideRate)); //나올수 있는 최대길이와 최소길이 중 랜덤으로선택

        if (tree.nodeRect.width >= tree.nodeRect.height)   //가로가 더 길경우 좌/우로 
        {
            tree.leftNode = new Node(new RectInt(tree.nodeRect.x, tree.nodeRect.y, split, tree.nodeRect.height));
            tree.rightNode = new Node(new RectInt(tree.nodeRect.x + split, tree.nodeRect.y, tree.nodeRect.width - split, tree.nodeRect.height));

            //DrawLine(new Vector2(tree.nodeRect.x + split, tree.nodeRect.y), new Vector2(tree.nodeRect.x + split, tree.nodeRect.y + tree.nodeRect.height));
            DrawTileWall(new Vector2Int(tree.nodeRect.x + split, tree.nodeRect.y), new Vector2Int(tree.nodeRect.x + split, tree.nodeRect.y + tree.nodeRect.height));
        }
        else        //세로 
        {
            tree.leftNode = new Node(new RectInt(tree.nodeRect.x, tree.nodeRect.y, tree.nodeRect.width, split));
            tree.rightNode = new Node(new RectInt(tree.nodeRect.x, tree.nodeRect.y + split, tree.nodeRect.width, tree.nodeRect.height - split));

            //DrawLine(new Vector2(tree.nodeRect.x, tree.nodeRect.y + split), new Vector2(tree.nodeRect.x + tree.nodeRect.width, tree.nodeRect.y + split));
            DrawTileWall(new Vector2Int(tree.nodeRect.x, tree.nodeRect.y + split), new Vector2Int(tree.nodeRect.x + tree.nodeRect.width, tree.nodeRect.y + split));
        }
        tree.leftNode.parNode = tree;
        tree.rightNode.parNode = tree;
        Divide(tree.leftNode, n + 1);
        Divide(tree.rightNode, n + 1);
    }


    RectInt GenerateRoom(Node tree, int n) //나누어준 사각형안에 방을 생성해줍니다.
    {
        RectInt rect;

        if (n == maxDepth)
        {
            rect = tree.nodeRect;
            int width = Random.Range(rect.width / 2, rect.width - 1);
            int height = Random.Range(rect.height / 2, rect.height - 1);


            int x = rect.x + Random.Range(1, rect.width - width);
            int y = rect.y + Random.Range(1, rect.height - height);

            rect = new RectInt(x, y, width, height);
            //DrawRectangle(rect);
           
            DrawTileRoom(rect);

     
        }
        else
        {
            tree.leftNode.roomRect = GenerateRoom(tree.leftNode, n + 1);
            tree.rightNode.roomRect = GenerateRoom(tree.rightNode, n + 1);
            rect = tree.leftNode.roomRect;
        }

        return rect;
    }

    void GenerateLoad(Node tree, int n) //방생성 후 노드를 거슬러올라가 길연결을해줍니다
    {
        if (n == maxDepth) return;

        Vector2Int leftCenter = tree.leftNode.center;
        Vector2Int rightCenter = tree.rightNode.center;

        //DrawLine_2(new Vector2(leftCenter.x,leftCenter.y),new Vector2(rightCenter.x,leftCenter.y));
        //DrawLine_2(new Vector2(rightCenter.x,leftCenter.y),new Vector2(rightCenter.x,rightCenter.y));
        DrawTileLine(new Vector2Int(leftCenter.x, leftCenter.y), new Vector2Int(rightCenter.x, leftCenter.y));
        DrawTileLine(new Vector2Int(rightCenter.x, leftCenter.y), new Vector2Int(rightCenter.x, rightCenter.y));

        GenerateLoad(tree.leftNode, n + 1);
        GenerateLoad(tree.rightNode, n + 1);
        //if(n==0)
        //{
            
        //}
      

    }
    void DrawTileLine(Vector2Int from, Vector2Int to)
    {
        for (int x = from.x; x < to.x; x++)
        {
            tilemap.SetTile(new Vector3Int(x - mapSize.x / 2, to.y - mapSize.y / 2, 0), groundTiles[0]);
        }
        for (int y = from.y; y < to.y; y++)
        {
            tilemap.SetTile(new Vector3Int(to.x - mapSize.x / 2, y - mapSize.y / 2, 0), groundTiles[0]);
        }

    }


    void DrawTileWall(Vector2Int from, Vector2Int to)
    {
        for (int x = from.x; x < to.x; x++)
        {
            tilemap.SetTile(new Vector3Int(x - mapSize.x / 2, to.y - mapSize.y / 2, 0), wallTiles[0]);
        }
        for (int y = from.y; y < to.y; y++)
        {
            tilemap.SetTile(new Vector3Int(to.x - mapSize.x / 2, y - mapSize.y / 2, 0), wallTiles[1]);
        }
    }

    void DrawTileRoom(RectInt rect)
    {
        int grassTileCount = 0;
        int waterTileCount = 1;
        for (int x = rect.x; x < rect.width + rect.x; x++)
        {
            for (int y = rect.y; y < rect.height + rect.y; y++)
            {
                if (rect.x == x || rect.y == y || rect.width + rect.x - 1 == x || rect.height + rect.y - 1 == y)
                {
                    tilemap.SetTile(new Vector3Int(x - mapSize.x / 2, y - mapSize.y / 2, 0), roomTiles[0]);
                }
                else
                {
                    tilemap.SetTile(new Vector3Int(x - mapSize.x / 2, y - mapSize.y / 2, 0), roomTiles[1]);
                }
            }
        }

        for (int x = rect.xMin + 1; x < rect.xMax; x++)
        {
            for (int y = rect.yMin + 1; y < rect.yMax; y++)
            {

                if ((x == rect.xMin + 1 || x == rect.xMax - 2) && y == rect.yMax - 1) //사각형의 위쪽벽부분
                {
                    float chance = Random.Range(0f, 1f);

                    if (chance < 0.3f)
                    {
                        tilemap.SetTile(new Vector3Int(x - mapSize.x / 2, y - mapSize.y / 2, 0), objectTiles[1]);

                        if(waterTileCount<3)
                        {
                            for (int i = 1; i < 4; i++)
                            {
                                tilemap.SetTile(new Vector3Int(x - mapSize.x / 2, y - mapSize.y / 2 - i, 0), objectTiles[2]);
                            }
                            waterTileCount++;
                        }
                       
                       

                    }
                }
                else
                {
                    if(x == rect.xMin + 1 || x == rect.xMax - 1 || y == rect.yMin + 1 || y == rect.yMax - 1)
                    {
                        continue;
                    }
                    float chance = Random.Range(0f, 1f);
                    if (chance < 0.08f&&grassTileCount<10)
                    {
                        grassTileCount++;
                        tilemap.SetTile(new Vector3Int(x - mapSize.x / 2, y - mapSize.y / 2, 0), objectTiles[0]);
                    }

                    if(grassTileCount==9)
                    {
                        grassTileCount = 0;
                    }
                }
            }
        }

    }

    void DrawTileBackGround()
    {
        for (int x = -mapSize.x / 2; x < mapSize.x / 2; x++)
        {
            for (int y = -mapSize.y / 2; y < mapSize.y / 2; y++)
            {
                tilemap.SetTile(new Vector3Int(x, y, 0), backgroundTile);
            }
        }

    }



   
}
