using System.Collections.ObjectModel;
using System.Management.Automation;

namespace PowershellJSONTest
{
    public class DataService
    {
        //Create CSV from invoking Powershell script
        public Collection<PSObject> ExecuteScript(string scriptPath)
        {
            using PowerShell ps = PowerShell.Create();
            ps.AddScript(scriptPath);
            var pipelineObjects = ps.Invoke();
            return pipelineObjects;
        }
    }
}
