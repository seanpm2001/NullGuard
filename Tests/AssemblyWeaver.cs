using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;
using Fody;

public static class AssemblyWeaver
{
    public static Assembly Assembly;

    static AssemblyWeaver()
    {
        var weavingTask = new ModuleWeaver
        {
            Config = new XElement("NullGuard",
                new XAttribute("IncludeDebugAssert", false),
                new XAttribute("ExcludeRegex", "^ClassToExclude$")),
            DefineConstants = new List<string> {"DEBUG"} // Always testing the debug weaver
        };

        TestResult = weavingTask.ExecuteTestRun("AssemblyToProcess.dll",
            ignoreCodes: new[]
            {
                "0x80131854", // Unexpected type on the stack (related to 0x801318DE)
                "0x801318DE", // Unmanaged pointers are not a verifiable type
                "0x80131869", // Unable to resolve token.
            });
        Assembly = TestResult.Assembly;
        AfterAssemblyPath = TestResult.AssemblyPath;
    }

    public static string AfterAssemblyPath;

    public static TestResult TestResult;
}