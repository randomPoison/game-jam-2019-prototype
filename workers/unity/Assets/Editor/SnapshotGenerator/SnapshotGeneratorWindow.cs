using System.IO;
using UnityEditor;
using UnityEngine;

namespace BetaApartUranus.Editor
{
    internal class SnapshotEditorWindow : EditorWindow
    {
        private SnapshotGenerator.Arguments _arguments;

        [MenuItem("SpatialOS/Generate snapshot")]
        public static void GenerateMenuItem()
        {
            GetWindow<SnapshotEditorWindow>().Show();
        }

        public void Awake()
        {
            minSize = new Vector2(200, 120);
            titleContent = new GUIContent("Generate snapshot");

            SetDefaults();
        }

        private void SetDefaults()
        {
            _arguments = new SnapshotGenerator.Arguments
            {
                OutputPath = Path.GetFullPath(
                    Path.Combine(
                        Application.dataPath,
                        "..",
                        "..",
                        "..",
                        "snapshots",
                        "default.snapshot")),
                NumResourceNodes = 1,
                WorldDimensions = new Vector2(5000f, 5000f),
            };
        }

        public void OnGUI()
        {
            using (new EditorGUILayout.VerticalScope())
            {
                if (GUILayout.Button("Defaults"))
                {
                    SetDefaults();
                    Repaint();
                }

                _arguments.OutputPath = EditorGUILayout.TextField("Snapshot path", _arguments.OutputPath);

                _arguments.NumResourceNodes = EditorGUILayout.IntField("Resource Nodes", _arguments.NumResourceNodes);
                _arguments.WorldDimensions.x = EditorGUILayout.FloatField("World Width", _arguments.WorldDimensions.x);
                _arguments.WorldDimensions.y = EditorGUILayout.FloatField("World Height", _arguments.WorldDimensions.y);

                var shouldDisable = string.IsNullOrEmpty(_arguments.OutputPath);
                using (new EditorGUI.DisabledScope(shouldDisable))
                {
                    if (GUILayout.Button("Generate snapshot"))
                    {
                        SnapshotGenerator.Generate(_arguments);
                    }
                }
            }
        }
    }
}
