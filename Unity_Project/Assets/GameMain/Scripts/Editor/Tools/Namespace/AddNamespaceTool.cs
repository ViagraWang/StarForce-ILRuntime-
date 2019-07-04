using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Game.Editor
{
    public class AddNamespaceTool : ScriptableWizard
    {
        public string folder = "Assets/";
        public string namespaceName;

        void OnEnable()
        {
            if (Selection.activeObject != null)
            {
                string dirPath = AssetDatabase.GetAssetOrScenePath(Selection.activeObject);
                if (File.Exists(dirPath))
                {
                    dirPath = dirPath.Substring(0, dirPath.LastIndexOf("/"));
                }
                folder = dirPath;
            }
        }

        public static void CreateWizard()
        {
            AddNamespaceTool editor = ScriptableWizard.DisplayWizard<AddNamespaceTool>("Add Namespace", "Add");
            editor.minSize = new Vector2(300, 200);
        }
        public void OnWizardCreate()
        {
            //save settting

            if (!string.IsNullOrEmpty(folder) && !string.IsNullOrEmpty(namespaceName))
            {

                List<string> filesPaths = new List<string>();
                filesPaths.AddRange(
                    Directory.GetFiles(Path.GetFullPath(".") + Path.DirectorySeparatorChar + folder, "*.cs", SearchOption.AllDirectories)
                );
                Dictionary<string, bool> scripts = new Dictionary<string, bool>();

                int counter = -1;
                foreach (string filePath in filesPaths)
                {

                    scripts[filePath] = true;

                    EditorUtility.DisplayProgressBar("Add Namespace", filePath, counter / (float)filesPaths.Count);
                    counter++;

                    string contents = File.ReadAllText(filePath);
                    string res = NamespaceBuilder.AddNameSpace(contents, namespaceName);
                    File.WriteAllText(filePath, res);
                }


                //处理加了命名空间后出现方法miss
                filesPaths.AddRange(
                    Directory.GetFiles(Path.GetFullPath(".") + Path.DirectorySeparatorChar + folder, "*.unity", SearchOption.AllDirectories)
                );
                filesPaths.AddRange(
                    Directory.GetFiles(Path.GetFullPath(".") + Path.DirectorySeparatorChar + folder, "*.prefab", SearchOption.AllDirectories)
                );


                counter = -1;
                foreach (string filePath in filesPaths)
                {
                    EditorUtility.DisplayProgressBar("Modify Script Ref", filePath, counter / (float)filesPaths.Count);
                    counter++;

                    string contents = File.ReadAllText(filePath);

                    string result = "";
                    using (TextReader reader = new StringReader(contents))
                    {
                        int index = 0;
                        while (reader.Peek() != -1)
                        {
                            string line = reader.ReadLine();

                            if (line.IndexOf("m_ObjectArgumentAssemblyTypeName:") > -1 && !line.Contains(namespaceName))
                            {

                                string scriptName = line.Split(':')[1].Split(',')[0].Trim();
                                if (scripts.ContainsKey(scriptName))
                                {
                                    line = line.Replace(scriptName, "namespaceName." + scriptName);
                                }

                                result += line + "\n";
                            }
                            else
                            {
                                result += line + "\n";
                            }
                            ++index;
                        }
                        reader.Close();
                    }

                    File.WriteAllText(filePath, result);
                }


                EditorUtility.ClearProgressBar();
                AssetDatabase.Refresh();
            }
        }

    }
}
