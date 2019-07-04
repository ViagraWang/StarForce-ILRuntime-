using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class NamespaceBuilder
{
    public static string AddNameSpace(string contents, string namespaceName)
    {

        string result = "";
        bool havsNS = contents.Contains("namespace ");
        string t = havsNS ? "" : "\t";

        using (TextReader reader = new StringReader(contents))
        {
            int index = 0;
            bool addedNS = false;
            while (reader.Peek() != -1)
            {
                string line = reader.ReadLine();

                if (line.IndexOf("using") > -1 || line.Contains("#"))
                {
                    result += line + "\n";
                }
                else if (!addedNS && !havsNS)
                {
                    result += "\nnamespace " + namespaceName + "\n{";
                    addedNS = true;
                    result += t + line + "\n";
                }
                else
                {
                    if (havsNS && line.Contains("namespace "))
                    {
                        if (line.Contains("{"))
                        {
                            result += "namespace " + namespaceName + " \n{\n";
                        }
                        else
                        {
                            result += "namespace " + namespaceName + "\n";
                        }
                    }
                    else
                    {
                        result += t + line + "\n";
                    }
                }
                ++index;
            }
            reader.Close();
        }
        if (!havsNS)
        {
            result += "}";
        }

        return result;
    }

}
