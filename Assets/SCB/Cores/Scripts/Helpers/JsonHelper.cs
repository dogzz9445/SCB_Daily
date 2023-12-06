using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace SCB.Cores
{
    public static class JsonHelper
    {
        public static T DeserializeOrDefault<T>(string json) where T : class
        {
            if (string.IsNullOrEmpty(json))
                return null;

            return JsonConvert.DeserializeObject<T>(json,
                new JsonSerializerSettings()
                {
                    DefaultValueHandling = DefaultValueHandling.Populate,
                    NullValueHandling = NullValueHandling.Ignore,
                });
        }

        public static async Task<string> ReadTextAsync(string filename)
        {
            if (string.IsNullOrEmpty(filename))
                return null;

            if (!File.Exists(filename))
                return null;

            return await File.ReadAllTextAsync(filename, Encoding.UTF8);
        }

        public static async Task<JToken> ReadJTokenAsync(string filename)
        {
            var json = await ReadTextAsync(filename);
            var reader = new JsonTextReader(new StringReader(json));
            return await JToken.ReadFromAsync(reader);
        }

        public static async Task<T> ReadFileOrDefaultAsync<T>(string filename) where T : class
        {
#if UNITY_EDITOR
            Debug.Log($"JsonHelper try to read {filename}");
#endif
            if (!File.Exists(filename))
            {
                return null;
            }
            var json = await ReadTextAsync(filename);
            return DeserializeOrDefault<T>(json);
        }

        public static async Task WriteFileAsync<T>(string fullFileName, T jsonObject)
        {
            var directoryName = Path.GetDirectoryName(fullFileName);
            if (!string.IsNullOrEmpty(directoryName))
            {
                if (!Directory.Exists(directoryName))
                {
                    Directory.CreateDirectory(directoryName);
                }
            }
            using var file = File.CreateText(fullFileName);
            using var writer = new JsonTextWriter(file);
            string json = JsonConvert.SerializeObject(jsonObject, Formatting.Indented,
                new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
            await writer.WriteRawAsync(json);
        }

        public static async Task WriteStringAsync(string fullFileName, string json)
        {
            var directoryName = Path.GetDirectoryName(fullFileName);
            if (!string.IsNullOrEmpty(directoryName))
            {
                if (!Directory.Exists(directoryName))
                {
                    Directory.CreateDirectory(directoryName);
                }
            }
            using var file = File.CreateText(fullFileName);
            using var writer = new JsonTextWriter(file);
            await writer.WriteRawAsync(json);
        }


        public static JToken ReadJToken(string filename)
        {
            var json = File.ReadAllText(filename);
            var reader = new JsonTextReader(new StringReader(json));
            return JToken.ReadFrom(reader);
        }

        public static T ReadFileOrDefault<T>(string filename) where T : new()
        {
            if (File.Exists(filename))
            {
                var json = File.ReadAllText(filename);
                if (!string.IsNullOrEmpty(json))
                {
                    return JsonConvert.DeserializeObject<T>(json,
                        new JsonSerializerSettings()
                        {
                            DefaultValueHandling = DefaultValueHandling.Populate,
                            NullValueHandling = NullValueHandling.Ignore
                        });
                }
            }
            return new T();
        }
    }
}
