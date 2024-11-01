using UnityEditor;
using UnityEngine;

namespace Data
{
    [CustomEditor(typeof(LevelData))]
    public class LevelDataEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var levelData = (LevelData)target;

            // Input fields for rows, columns and goal.
            levelData.goal = EditorGUILayout.IntField("Goal", levelData.goal);
            levelData.rows = EditorGUILayout.IntField("Rows", levelData.rows);
            levelData.columns = EditorGUILayout.IntField("Columns", levelData.columns);

            if (GUILayout.Button("Initialize Grid"))
            {
                levelData.InitializeGrid();
            }

            if (levelData.grid != null && levelData.grid.Count == levelData.rows * levelData.columns)
            {
                // Draw the grid with dropdowns for each cell.
                for (var row = levelData.rows - 1; row >= 0; row--)
                {
                    EditorGUILayout.BeginHorizontal();
                    for (var col = 0; col < levelData.columns; col++)
                    {
                        var index = row * levelData.columns + col;

                        // Dropdown for selecting item types.
                        levelData.grid[index].item =
                            (LevelData.CellItem)EditorGUILayout.EnumPopup(levelData.grid[index].item,
                                GUILayout.Width(70), GUILayout.Height(70));
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Click 'Initialize Grid' to set up the grid.", MessageType.Warning);
            }

            if (GUI.changed)
            {
                EditorUtility.SetDirty(levelData); // Save changes in the editor.
            }
        }
    }
}
