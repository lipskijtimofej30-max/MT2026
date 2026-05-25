using System.IO;
using UnityEditor;
using UnityEngine;

namespace Game.Editor
{
    public class EnumGenerator : AssetPostprocessor
    {
        private const string TEXT_FILE_PATH = "Assets/Game/Data/KeyDialogue.txt";
        private const string ENUM_SCRIPT_PATH = "Assets/Game/Scripts/KeyDialogueType.cs";

        // Метод срабатывает, когда ты меняешь любой ассет в проекте
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach (string str in importedAssets)
            {
                if (str == TEXT_FILE_PATH)
                {
                    GenerateEnum();
                    break;
                }
            }
        }

        public static void GenerateEnum()
        {
            string[] lines = File.ReadAllLines(TEXT_FILE_PATH);
        
            string code = "public enum KeyDialogueType\n{\n    None = 0,\n";
        
            for (int i = 0; i < lines.Length; i++)
            {
                string id = lines[i].Trim();
                if (string.IsNullOrEmpty(id)) continue;
                code += $"    {id} = {i + 1},\n";
            }
        
            code += "}";

            // Записываем файл на диск
            File.WriteAllText(ENUM_SCRIPT_PATH, code);
            // Заставляем Unity перекомпилировать код
            AssetDatabase.Refresh();
            Debug.Log("GameEvent Enum успешно обновлен!");
        }
    }
}