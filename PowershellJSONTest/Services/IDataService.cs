namespace PowershellJSONTest
{
    public interface IDataService
    {
        void CreateCsv(string scriptPath);
        string GetJsonFromCsv<T>(string csvPath);
    }
}
