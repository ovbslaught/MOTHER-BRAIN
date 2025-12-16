using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;
using System.CodeDom.Compiler;
using System.CodeDom;
using Microsoft.CSharp;
using System.IO;
using System.Web.Razor;
using System.Web.WebPages;

namespace runnerDotNet
{
    public class RunnerRazor
    {
		private static object lockObject = new object();
		static Dictionary<string, DynamicContentGeneratorBase> _assemblyCache = new Dictionary<string, DynamicContentGeneratorBase>();
	
        internal static string RenderRazorString<T>(T model, string templateString, dynamic viewBag = null)
        {
			lock( RunnerRazor.lockObject ) {
				DynamicContentGeneratorBase templateInstance;
				if(_assemblyCache.TryGetValue(templateString, out templateInstance))
				{
					templateInstance.Model = model;

					if (viewBag != null)
					{
						templateInstance.ViewBag = viewBag;
						templateInstance.ViewBag.xt = model;
					}
					return _assemblyCache[templateString].GetContent();
				}
				
				const string dynamicallyGeneratedClassName = "DynamicContentTemplate";
				const string namespaceForDynamicClasses = "runnerDotNet";
				const string dynamicClassFullName = "runnerDotNet.DynamicContentTemplate";

				var host = new RazorEngineHost(new CSharpRazorCodeLanguage())
				{
					DefaultBaseClass = typeof(DynamicContentGeneratorBase).Name,
					DefaultClassName = dynamicallyGeneratedClassName,
					DefaultNamespace = namespaceForDynamicClasses,
				};
				host.NamespaceImports.Add("System");
				host.NamespaceImports.Add("System.Dynamic");
				host.NamespaceImports.Add("System.Text");
				host.NamespaceImports.Add("System.Web");
				host.NamespaceImports.Add("System.Web.Mvc");
				host.NamespaceImports.Add("System.Web.Mvc.Html");
				var engine = new RazorTemplateEngine(host);

				var tr = new StringReader(templateString); // here is where the string come in place
				GeneratorResults razorTemplate = engine.GenerateCode(tr);

				var compiledAssembly = CreateCompiledAssemblyFor(razorTemplate.GeneratedCode);

				templateInstance = (DynamicContentGeneratorBase)compiledAssembly.CreateInstance(dynamicClassFullName);
				_assemblyCache[templateString] = templateInstance;

				templateInstance.Model = model;

				if (viewBag != null)
				{
					templateInstance.ViewBag = viewBag;
					templateInstance.ViewBag.xt = model;
				}

				return templateInstance.GetContent();
			}
        }

        private static Assembly CreateCompiledAssemblyFor(CodeCompileUnit unitToCompile)
        {
            var compilerParameters = new CompilerParameters();
            compilerParameters.ReferencedAssemblies.Add(typeof(DynamicContentGeneratorBase).Assembly.Location);
            compilerParameters.GenerateInMemory = true;
			compilerParameters.TempFiles = new System.CodeDom.Compiler.TempFileCollection(MVCFunctions.getabspath("temp"));

            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                            .Where(a => !a.IsDynamic).Select(a => a.Location);
            compilerParameters.ReferencedAssemblies.AddRange(assemblies.ToArray());

			// debugging section
			//compilerParameters.IncludeDebugInformation = true; 
			// System.Text.StringBuilder sb = new System.Text.StringBuilder();
			// StringWriter sw = new StringWriter(sb);
			// new CSharpCodeProvider().GenerateCodeFromCompileUnit(unitToCompile, sw, new CodeGeneratorOptions());
			// var code = sb.ToString();
			// File.WriteAllText("d:\\temp.txt", code);  // set breakpoint here and edit temp.txt if needed
			// code = File.ReadAllText("d:\\temp.txt");
			// CompilerResults compilerResults = new CSharpCodeProvider().CompileAssemblyFromSource(compilerParameters, code);
			
            CompilerResults compilerResults = new CSharpCodeProvider().CompileAssemblyFromDom(compilerParameters, unitToCompile);
			
			if (compilerResults.Errors.Count > 0)
            {
                string errorText = "";
                foreach (CompilerError compilerError in compilerResults.Errors)
                    errorText += compilerError + "\n";
                throw new ApplicationException(errorText);
            }
			
            Assembly compiledAssembly = compilerResults.CompiledAssembly;
            return compiledAssembly;
        }
    }
}