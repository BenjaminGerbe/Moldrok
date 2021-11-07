using UnityEditor;
using UnityEngine;


[CustomEditor (typeof(MapGenerator))]
public class MapGeneratorEditor : Editor
{
    /// <summary>
    /// this class allow us to change the aspect of editor
    /// </summary>

    public override void OnInspectorGUI()
    {
        MapGenerator mapGen = (MapGenerator)target;

     
        if (DrawDefaultInspector() && mapGen.autoUpdate)
        {
            mapGen.DrawMapInEditer();
        }


        if (GUILayout.Button("Generate"))
        {
            mapGen.DrawMapInEditer();
        }
    }

}
