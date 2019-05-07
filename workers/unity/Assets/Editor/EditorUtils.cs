using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class EditorUtils
{
    [MenuItem("SpatialOS/Tools/Generate Component ID")]
    public static int GenerateComponentId()
    {
        while (true)
        {
            var candidate = Random.Range(100, 536_870_910);
            if (candidate < 190_000 || candidate > 199_999)
            {
                Debug.Log("Component ID: " + candidate);
                return candidate;
            }
        }
    }
}
