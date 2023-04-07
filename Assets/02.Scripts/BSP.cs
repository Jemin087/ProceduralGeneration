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

public class BSP : MonoBehaviour
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
    GameObject map;

    [SerializeField]
    GameObject roomLine;

    [SerializeField]
    int maxDepth;   //트리구조의 최대 깊이 크기가 클수록 세밀하게 나누어지고 방이많아짐

    [SerializeField]
    Tilemap tilemap;    //타일맵 테스트중

    [SerializeField]
    Tile tile;          //타일맵 테스트중 


    private void Start()
    {
        Node root = new Node(new RectInt(0, 0, mapSize.x, mapSize.y));
        DrawMap(0, 0);  //1.한 공간을 만들어준다
        Divide(root, 0);    //2. 만들어준 공간을 재귀함수를 이용하여 maxDepth에 도달할때까지 나눈다
        GenerateRoom(root,0); //3.나누어진 공간안에 방을 만드는 함수
        GenerateLoad(root,0); //4.노드를 거슬러올라가면서 길을 이어주는함수
        //FillTesting(); //테스트중
    }

    void DrawMap(int x,int y)
    {
        LineRenderer lineRenderer = Instantiate(map).GetComponent<LineRenderer>();
        lineRenderer.SetPosition(0, new Vector2(x, y) - mapSize / 2);
        lineRenderer.SetPosition(1, new Vector2(x+mapSize.x, y) - mapSize / 2);
        lineRenderer.SetPosition(2, new Vector2(x+mapSize.x, y+mapSize.y) - mapSize / 2);
        lineRenderer.SetPosition(3, new Vector2(x, y+mapSize.y) - mapSize / 2);
    }

    void Divide(Node tree,int n)
    {
        if (n == maxDepth) return;

        int maxLength = Mathf.Max(tree.nodeRect.width, tree.nodeRect.height);   //가로 세로 중 더 긴것으로 나누어주기위해
        int split = Mathf.RoundToInt(Random.Range(maxLength * minDevideRate, maxLength * maxDevideRate)); //나올수 있는 최대길이와 최소길이 중 랜덤으로선택

        if(tree.nodeRect.width>=tree.nodeRect.height)
        {
            tree.leftNode = new Node(new RectInt(tree.nodeRect.x, tree.nodeRect.y, split, tree.nodeRect.height));
            tree.rightNode = new Node(new RectInt(tree.nodeRect.x + split, tree.nodeRect.y, tree.nodeRect.width - split, tree.nodeRect.height));

          // DrawLine(new Vector2(tree.nodeRect.x + split, tree.nodeRect.y), new Vector2(tree.nodeRect.x + split, tree.nodeRect.y + tree.nodeRect.height));
           
        }
        else
        {
            tree.leftNode = new Node(new RectInt(tree.nodeRect.x, tree.nodeRect.y, tree.nodeRect.width,split));
            tree.rightNode = new Node(new RectInt(tree.nodeRect.x, tree.nodeRect.y + split, tree.nodeRect.width, tree.nodeRect.height - split));

          // DrawLine(new Vector2(tree.nodeRect.x, tree.nodeRect.y + split), new Vector2(tree.nodeRect.x + tree.nodeRect.width, tree.nodeRect.y + split));
        }
        tree.leftNode.parNode = tree;
        tree.rightNode.parNode = tree;
        Divide(tree.leftNode, n + 1);
        Divide(tree.rightNode,n + 1);
    }

    void DrawLine(Vector2 from, Vector2 to)
    {
        LineRenderer line = Instantiate(this.line).GetComponent<LineRenderer>();
        line.SetPosition(0, from - mapSize / 2);
        line.SetPosition(1, to - mapSize / 2);
    }


    RectInt GenerateRoom(Node tree,int n)
    {
        RectInt rect;

        if(n==maxDepth)
        {
            rect = tree.nodeRect;
            int width = Random.Range(rect.width / 2, rect.width - 1);
            int height = Random.Range(rect.height / 2, rect.height - 1);


            int x = rect.x + Random.Range(1, rect.width - width);
            int y = rect.y + Random.Range(1, rect.height - height);

            rect = new RectInt(x, y, width, height);
            DrawRectangle(rect);

        }
        else
        {
            tree.leftNode.roomRect = GenerateRoom(tree.leftNode, n + 1);
            tree.rightNode.roomRect = GenerateRoom(tree.rightNode, n + 1);
            rect = tree.leftNode.roomRect;
        }

        return rect;
    }

    void GenerateLoad(Node tree,int n)
    {
        if(n==maxDepth) return;

        Vector2Int leftCenter=tree.leftNode.center;
        Vector2Int rightCenter=tree.rightNode.center;

        DrawLine(new Vector2(leftCenter.x,leftCenter.y),new Vector2(rightCenter.x,leftCenter.y));
        DrawLine(new Vector2(rightCenter.x,leftCenter.y),new Vector2(rightCenter.x,rightCenter.y));

        GenerateLoad(tree.leftNode,n+1);
        GenerateLoad(tree.rightNode,n+1);

    }
    void DrawRectangle(RectInt rect)
    {
        LineRenderer line=Instantiate(roomLine).GetComponent<LineRenderer>();
        line.SetPosition(0,new Vector2(rect.x,rect.y)-mapSize/2);
        line.SetPosition(1,new Vector2(rect.x+rect.width,rect.y)-mapSize/2);
        line.SetPosition(2,new Vector2(rect.x+rect.width,rect.y+rect.height)-mapSize/2);
        line.SetPosition(3,new Vector2(rect.x,rect.y+rect.height)-mapSize/2);
    }


    void FillTesting()
    {
        //lineRenderer.SetPosition(0, new Vector2(x, y) - mapSize / 2);
        //lineRenderer.SetPosition(1, new Vector2(x+mapSize.x, y) - mapSize / 2);
        //lineRenderer.SetPosition(2, new Vector2(x+mapSize.x, y+mapSize.y) - mapSize / 2);
        //lineRenderer.SetPosition(3, new Vector2(x, y+mapSize.y) - mapSize / 2);
        
        for(int x=0-mapSize.x/2; x<mapSize.x; x++)
        {
            for(int y=0-mapSize.y/2; y<mapSize.y; y++)
            {
                tilemap.SetTile(new Vector3Int(x,y,0),tile);
            }
        }


    }
}
