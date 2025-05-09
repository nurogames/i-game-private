using UnityEngine;
using UnityEditor;

namespace iGame.Editor
{

    public static class IGameUtils
    {
        public static string GetTextFromFile(string filename)
        {
            TextAsset asset = AssetDatabase.LoadAssetAtPath<TextAsset>($"Packages/com.nuro.i-game.core/Editor/Data/{filename}.txt");
            return asset != null ? asset.text : null;
        }

    }
}