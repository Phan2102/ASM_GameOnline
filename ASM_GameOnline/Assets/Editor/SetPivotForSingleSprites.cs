using UnityEngine;
using UnityEditor;
using System.IO;

public class SetPivotForSingleSprites : MonoBehaviour
{
    [MenuItem("Tools/Sprite/Set Pivot Bottom Center (Single Sprites)")]
    static void SetPivotForAllSingleSprites()
    {
        string folderPath = "Assets/Nghi/Character/A Type"; // 🔧 Đường dẫn tới thư mục chứa sprites

        string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { folderPath });
        int count = 0;

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;

            if (importer != null && importer.textureType == TextureImporterType.Sprite)
            {
                if (importer.spriteImportMode == SpriteImportMode.Single)
                {
                    // Sử dụng SerializedObject để chỉnh pivot và alignment
                    SerializedObject so = new SerializedObject(importer);
                    so.FindProperty("m_Alignment").intValue = (int)SpriteAlignment.Custom;
                    so.FindProperty("m_SpritePivot").vector2Value = new Vector2(0.5f, 0f); // Bottom Center

                    so.ApplyModifiedProperties();
                    importer.SaveAndReimport();

                    count++;
                    Debug.Log($"✔️ Set pivot cho {path}");
                }
            }
        }

        Debug.Log($"✅ Đã chỉnh pivot Bottom Center cho {count} sprite đơn.");
    }
}
