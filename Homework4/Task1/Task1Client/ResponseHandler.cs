using System;
using System.Collections.Generic;
using System.IO;

namespace Task1Client
{
    /// <summary>
    /// Handles responses from the server.
    /// </summary>
    public static class ResponseHandler
    {
        /// <summary>
        /// Transforms a string responce to a list of pairs
        /// </summary>
        public static (int, List<(string, bool)>) HandleListResponse(string response)
        {
            var splitResponse = response.Split(' ');

            if (!int.TryParse(splitResponse[0], out var resultLength))
            {
                throw new ArgumentException(response);
            }

            if (resultLength == -1)
            {
                throw new DirectoryNotFoundException(response);
            }

            var result = new List<(string, bool)>();

            for (var i = 1; i < resultLength * 2; i += 2)
            {
                result.Add((splitResponse[i], bool.Parse(splitResponse[i + 1])));
            }

            return (result.Count, result);
        }
    }
}
