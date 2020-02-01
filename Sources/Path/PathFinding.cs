using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;

public class PathFinding : MonoBehaviour
{
    public int actualBlock;
    public Vector3Int pos;
    public List<Tilemap> tileMaps = new List<Tilemap>();
    public BoundsInt area;

    [Space]

    public GameObject debugPath;

    Camera cam;
    Player player;
    // Use this for initialization
    void Start()
    {
        //FAIRE autre chose 
        TilemapCollider2D[] tilemapCollider2D = FindObjectOfType<Grid>().transform.GetComponentsInChildren<TilemapCollider2D>();

        foreach (TilemapCollider2D tile in tilemapCollider2D)
        {
            tileMaps.Add(tile.GetComponent<Tilemap>());
        }

        print(tileMaps[1].size.x);
        print(tileMaps[0].size.x);
        print("origin : " + tileMaps[1].origin);
        print("size : " + tileMaps[1].size);
        area.position = tileMaps[1].origin;
        area.size = tileMaps[1].size;

        if (debugPath != null)
        {
            foreach (Tilemap item in tileMaps)
            {
                TileBase[] tileArray = item.GetTilesBlock(area);
                for (int index = 0; index < tileArray.Length; index++)
                {
                    if (tileArray[index])
                    {
                        //print(tileArray[index]);
                        string resultString = Regex.Match(tileArray[index].name, @"\d+").Value;
                        print(tileArray[index].name);

                        Vector3 pos;
                        pos.x = index % area.size.x + area.position.x;
                        pos.y = index / area.size.x + area.position.y;
                        pos.z = 0;

                        debugPath.GetComponentInChildren<DebugPath>().AddNbMap(resultString, pos);
                    }
                }
            }   
        }




        cam = Camera.main;
        player = GameManager.Singleton.Player.GetComponent<Player>();


        //for (int i = 0; i < 3; i++)
        //{
        //    for (int j = 0; j < 3; j++)
        //    {
        //        Vector3 pos;
        //        pos.x = i ;
        //        pos.y = j ;
        //        pos.z = 0;
        //        debugPath.GetComponentInChildren<DebugPath>().AddNbMap(12, pos);
        //    }
        //}
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
           // GetPosition();
        }
    }

    void GetPosition()
    {

        pos = new Vector3Int((int)player.transform.position.x, (int)player.transform.position.y, 0);


        actualBlock = tileMaps[1].GetTile(pos).GetInstanceID();
    }
}
