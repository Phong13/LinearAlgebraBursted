using UnityEditor;
using UnityEngine;

namespace LinearAlgebraEditorBatch
{
    public static class BatchCodeGen
    {
        public static void Run()
        {
            Debug.Log("[BatchCodeGen] Triggering Tools/UnityCodeGen/Generate ...");
            bool ok = EditorApplication.ExecuteMenuItem("Tools/UnityCodeGen/Generate");
            Debug.Log("[BatchCodeGen] ExecuteMenuItem returned: " + ok);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        }
    }
}
