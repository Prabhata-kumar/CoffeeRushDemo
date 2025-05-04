using UnityEngine;
using System.Collections.Generic;
using System.Threading;

public class GridManager : MonoBehaviour
{
    public int  _wight, _hight;
    public GameObject _tiles;
    public Camera _camera;
    public Color _white,_brown;
    public void Start()
    {
        GenerateTheGride();
    }

    private void GenerateTheGride()
    {
        for (int i = 0; i < _wight; i++)
        {
            for (int j = 0; j < _hight; j++)
            {
                var spawnedTile = Instantiate(_tiles, new Vector3(i, j), Quaternion.identity);
                spawnedTile.name = $"tiles {i} {j}";
                SpriteRenderer spr = spawnedTile.GetComponent<SpriteRenderer>();
                if((i % 2 == 0 &&  j % 2 != 0) || (i % 2 != 0 && j % 2 == 0))
                {
                    spr.color = _white;
                }
                else
                {
                    spr.color = _brown;
                }
            }
        }

        _camera.transform.position = new Vector3((float)_wight / 2 - 0.5f, (float)_hight / 2 - 0.5f, -10);
    }
}
