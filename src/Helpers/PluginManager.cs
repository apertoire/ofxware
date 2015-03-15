using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.CSharp;
using Openware.Plugins;

namespace Openware.Helpers
{
    public class PluginManager
    {
        ICore _core;

        public PluginManager(ICore core)
        {
            _core = core;
        }

        public void RunPlugin(string filePath)
        {
            CompilerResults result = LoadPlugin(filePath);

            if (result.Errors.HasErrors)
            {
                StringBuilder errors = new StringBuilder();
                string filename = Path.GetFileName(filePath);
                foreach (CompilerError err in result.Errors)
                {
                    errors.Append(string.Format("\r\n{0}({1},{2}): {3}: {4}", filename, err.Line, err.Column, err.ErrorNumber, err.ErrorText));
                }
                string str = "Error loading plugin\r\n" + errors.ToString();

                throw new ApplicationException(str);
            }

            ProcessPlugin(result.CompiledAssembly);
        }

        private CompilerResults LoadPlugin(string filepath)
        {
            string language = CSharpCodeProvider.GetLanguageFromExtension(Path.GetExtension(filepath));
            CodeDomProvider codeDomProvider = CSharpCodeProvider.CreateProvider(language);
            CompilerParameters compilerParams = new CompilerParameters();
            compilerParams.GenerateExecutable = false;
            compilerParams.GenerateInMemory = true;
            compilerParams.IncludeDebugInformation = false;

            compilerParams.ReferencedAssemblies.Add("System.dll");
            //compilerParams.ReferencedAssemblies.Add("System.Globalization.dll");
            //compilerParams.ReferencedAssemblies.Add("System.IO.dll");
            //compilerParams.ReferencedAssemblies.Add("System.Text.RegularExpressions.dll");
            compilerParams.ReferencedAssemblies.Add("System.Xml.dll");
            compilerParams.ReferencedAssemblies.Add("Openware.Plugins.dll");

            return codeDomProvider.CompileAssemblyFromFile(compilerParams, filepath);
        }

        private void ProcessPlugin(Assembly assembly)
        {
            foreach (Type type in assembly.GetTypes())
            {
                if (!type.IsClass || type.IsNotPublic) continue;
                Type[] interfaces = type.GetInterfaces();
                if (((IList<Type>)interfaces).Contains(typeof(IPlugin)))
                {
                    IPlugin iPlugin = (IPlugin)Activator.CreateInstance(type);
                    iPlugin.Run(_core);
                }
            }
        }
    }
}
