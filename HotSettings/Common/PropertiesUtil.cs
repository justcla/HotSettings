using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using System;
using System.Diagnostics;

namespace HotSettings.Common
{
    class PropertiesUtil
    {

        public void PrintProperties()
        {
            Debug.WriteLine("===== General =========");
            PrintItems("TextEditor", "General");
            Debug.WriteLine("===== CSharp =========");
            PrintItems("TextEditor", "CSharp");
            Debug.WriteLine("===== CSharp-Specific =========");
            PrintItems("TextEditor", "CSharp-Specific");
            Debug.WriteLine("===== All Languages =========");
            PrintItems("TextEditor", "AllLanguages");
        }

        private void PrintItems(string category, string page)
        {
            DTE2 _DTE2 = (DTE2)ServiceProvider.GlobalProvider.GetService(typeof(DTE));
            Properties properties = _DTE2.Properties[category, page];

            foreach (Property prop in properties)
            {
                try
                {
                    Debug.WriteLine(prop.Name);
                }
                catch (Exception ex)
                {
                    // Do nothing
                }
            }
        }

    }
}
