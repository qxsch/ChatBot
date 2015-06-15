using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QXS.ChatBot
{
    public class BotRuleCodeCompiler
    {
        public const string DefaultNamspace = "QXS.ChatBot.CompiledBotRuleCode";
        public readonly string Code;
        public readonly string ClassName;

        protected CompilerResults results;
        protected MethodInfo method;

        public BotRuleCodeCompiler(string code, List<string> AssemblyReferences = null, List<string> NameSpaceUsings = null)
        {
            ClassName = "Program" + (Guid.NewGuid()).ToString("N"); // just set the pure ClassName

            this.Code =
                "using System;\n" +
                "using System.Text;\n" +
                "using System.Text.RegularExpressions;\n" +
                "using QXS.ChatBot;\n"

            ;
            if (NameSpaceUsings == null)
            {
                this.Code +=
                    "using System.Collections;\n" +
                    "using System.Collections.Generic;\n" +
                    "using System.Linq;\n" +
                    "using System.Reflection;\n"
                ;
            }
            else
            {
                Regex namspaceValidator = new Regex(@"^([a-z]+[a-z0-9]*)(\.[a-z]+[a-z0-9]*)*$", RegexOptions.IgnoreCase);
                foreach (string nspace in NameSpaceUsings)
                {
                    if (!namspaceValidator.IsMatch(nspace))
                    {
                        throw new ArgumentException("Invalid namespace using for value \"" + nspace + "\"!");
                    }
                    this.Code += "using " + nspace + ";\n";
                }
            }

            this.Code += @"
namespace " + DefaultNamspace + @"
{
    public class " + ClassName + @"
    {
        public static string Process(Match match, ChatSessionInterface session)
        {
            " + code.Replace("\n", "\n            ").TrimEnd() + @"
            return null;
        }
    }
}
";
            CSharpCodeProvider provider = new CSharpCodeProvider();

            CompilerParameters parameters = new CompilerParameters();
            parameters.ReferencedAssemblies.Add("System.dll");
            parameters.ReferencedAssemblies.Add("mscorlib.dll");
            parameters.ReferencedAssemblies.Add("ChatBot.dll");
            if (AssemblyReferences == null)
            {
                parameters.ReferencedAssemblies.Add("System.Core.dll");
                parameters.ReferencedAssemblies.Add("System.Data.dll");
                parameters.ReferencedAssemblies.Add("System.Data.DataSetExtensions.dll");
                parameters.ReferencedAssemblies.Add("System.Xml.dll");
                parameters.ReferencedAssemblies.Add("System.Xml.Linq.dll");
            }
            else
            {
                foreach (string aref in AssemblyReferences)
                {
                    parameters.ReferencedAssemblies.Add(aref);
                }
            }
            // True - memory generation, false - external file generation
            parameters.GenerateInMemory = true;
            // True - exe file generation, false - dll file generation
            parameters.GenerateExecutable = false;

            results = provider.CompileAssemblyFromSource(parameters, this.Code);

            if (results.Errors.HasErrors)
            {
                StringBuilder sb = new StringBuilder();

                foreach (CompilerError error in results.Errors)
                {
                    sb.AppendLine(String.Format("Error ({0}): {1}", error.ErrorNumber, error.ErrorText));
                }

                throw new InvalidOperationException(sb.ToString());
            }

            ClassName = DefaultNamspace + "." + ClassName; // set the full ClassName
            method = results.CompiledAssembly.GetType(ClassName).GetMethod("Process");
        }


        public string Execute(Match match, ChatSessionInterface session)
        {
            object result = method.Invoke(null, new object[] { match, session });
            if (result == null)
            {
                return null;
            }
            return (string)result;
        }

    }
}

