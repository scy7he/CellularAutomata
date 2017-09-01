using UnityEngine;

namespace CellularAutomata.Assets.Scripts
{
    public class Utils
    {
      public static Vector3 GetPositionAtIndex(Transform t, int x, int y, Vector2 max, float scale) {
        Vector3 pos = new Vector3(x * scale, 0f, y * scale);
        Vector3 extents = new Vector3(max.x * scale, 0f, max.y * scale);
        return t.position + pos - (extents / 4);
      }

      Vector3 GetCenterOffset(float scale) {
        return (Vector3.one * scale) / 2;
      }

      public static int[] GetXYFromIndex(int index, Vector2 dimensions) {
        int x = (int)(index % dimensions.x),
            y = (int)((index - x) / dimensions.x);
        return new int[2] { x, y };
      }

      public static int GetGridSize(Vector2 xy) {
        return (int)(xy.x * xy.y);
      }

      public static bool GetRandomBool(float chance) {
        return (Random.value > chance) ? true : false;
      }
    }
}
