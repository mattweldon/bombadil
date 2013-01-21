using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using Newtonsoft.Json;
using System.Reflection;
using Bombadil.Core.Managers;

namespace Bombadil.Core.Models
{
    public static class Utilities
    {

        public static T DeserializeJson<T>(string rawData)
        {

            // Convert the data to an Object.
            var archive = JsonConvert.DeserializeObject<T>(rawData, new JsonSerializerSettings() { MissingMemberHandling = MissingMemberHandling.Ignore, NullValueHandling = NullValueHandling.Ignore });

            return archive;
        }

        public static string RawFileContents(string reference)
        {
            string fileContents = "";

            if (!string.IsNullOrEmpty(reference))
            {
                fileContents = File.ReadAllText(reference);
            }

            return fileContents;
        }

        public static string Left(this string input, int length)
        {
            string result = input;
            if (input != null && input.Length > length)
            {
                result = input.Substring(0, length);
            }
            return result;
        }

        public static string Right(this string input, int length)
        {
            string result = input;
            if (input != null && input.Length > length)
            {
                result = input.Substring(input.Length - length);
            }
            return result;
        }
    }
}