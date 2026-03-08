#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Tilemaps;

public class GrassGenerator : MonoBehaviour
{
    public Tilemap tilemap;
    public TileBase[] grassTiles;
    public int width = 20;
    public int height = 20;

#if UNITY_EDITOR
    [ContextMenu("Generate And Save Prefab")]
    public void GenerateAndSave()
    {
        tilemap.ClearAllTiles();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                TileBase tile = grassTiles[Random.Range(0, grassTiles.Length)];
                tilemap.SetTile(new Vector3Int(x, y, 0), tile);
            }
        }

        string path = "Assets/GeneratedGrass.prefab";
        PrefabUtility.SaveAsPrefabAsset(gameObject, path);

        Debug.Log("Prefab saved at " + path);
    }
#endif
}