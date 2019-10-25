using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using Zephyr.DOTSAStar.Runtime.DefineComponent;

namespace Zephyr.DOTSAStar.Editor
{
    public class CreatePFNodeDefine
    {
        [MenuItem("Assets/Create/PF Node Define")]
        public static void CreateDefine()
        {
            var define = ScriptableObject.CreateInstance<Define.Runtime.Define>();
            define.AddComponent<PathFindingNode>();

            var path = GetSelectedPathOrFallback()+"/node_name.asset";
            Assert.IsTrue(path.Contains("Resources/Defines/"),
                "Define files must be in Resources/Defines/ folder");
            
            AssetDatabase.CreateAsset(define, path);
            Debug.Log($"Created A* Path Define at {path}");
        }
        
        public static string GetSelectedPathOrFallback()
        {
            string path = "Assets";
		
            foreach (Object obj in Selection.GetFiltered(typeof(Object), SelectionMode.Assets))
            {
                path = AssetDatabase.GetAssetPath(obj);
                if ( !string.IsNullOrEmpty(path) && File.Exists(path) ) 
                {
                    path = Path.GetDirectoryName(path);
                    break;
                }
            }
            return path;
        }
    }
    
}