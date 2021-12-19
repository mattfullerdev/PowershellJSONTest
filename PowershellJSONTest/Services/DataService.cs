using CsvHelper;
using System.Globalization;
using System.Management.Automation;
using System.Text.Json;

namespace PowershellJSONTest
{
    public class DataService : IDataService
    {
        //Create CSV from invoking Powershell script
        public void CreateCsv(string scriptPath)
        {
            using PowerShell ps = PowerShell.Create();
            ps.AddScript(scriptPath);
            var pipelineObjects = ps.Invoke();
        }

        //Serialize data from generated CSV file to JSON
        public string GetJsonFromCsv<T>(string csvPath)
        {
            using var reader = new StreamReader(csvPath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            var records = csv.GetRecords<T>();
            return JsonSerializer.Serialize(records, new JsonSerializerOptions { WriteIndented = true });
        }
    }
}
