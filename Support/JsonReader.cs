using System;
using System.IO;
using System.Text.Json;

namespace CapstoneProject.Support
{
    public static class JsonReader
    {
        public static T ReadData<T>(string fileName)
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = Path.Combine(baseDir, "TestData", fileName);

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Test data file not found at path: {filePath}");
            }

            string jsonContent = File.ReadAllText(filePath);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var data = JsonSerializer.Deserialize<T>(jsonContent, options);
            if (data == null)
            {
                throw new InvalidOperationException($"Failed to deserialize test data from: {fileName}");
            }

            return data;
        }
    }
}