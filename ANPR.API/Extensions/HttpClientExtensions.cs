using Microsoft.Extensions.Primitives;
using Microsoft.Lonsum.Services.ANPR.API.ViewModels;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;

namespace Microsoft.Lonsum.Services.ANPR.API.Extensions
{
    public static class HttpClientExtensions
    {
        public static async Task<MultipartResponse> GetMultipartAsync(this HttpClient httpClient, string url)
        {
            var response = await SendRequestAsync(httpClient, url, HttpMethod.Get);
            return await HandleMultipartResponseAsync(response);
        }

        private static async Task<HttpResponseMessage> SendRequestAsync(HttpClient httpClient, string url, HttpMethod method)
        {
            var request = new HttpRequestMessage(method, url);

            return await httpClient.SendAsync(request);
        }

        private static async Task<MultipartResponse> HandleMultipartResponseAsync(HttpResponseMessage response)
        {
            MultipartResponse multipartResponse = new MultipartResponse();
            if (response.IsSuccessStatusCode)
            {
                string contentType = response.Content.Headers.ContentType?.ToString();
                if (contentType != null && contentType.StartsWith("multipart/mixed"))
                {
                    string boundary = GetBoundary(contentType);
                    var boundaryBytes = Encoding.UTF8.GetBytes(boundary);

                    var contentBytes = await response.Content.ReadAsByteArrayAsync();
                    try
                    {
                        // Split the content using the boundary bytes
                        var contentParts = SplitByteArray(contentBytes, boundaryBytes);
                        foreach (var partBytes in contentParts)
                        {
                            string partString = Encoding.UTF8.GetString(partBytes);
                            if (String.IsNullOrEmpty(partString)) continue;
                            string contentTypeHeader = GetHeaderContent(partString, "Content-Type");
                            if (!String.IsNullOrEmpty(contentTypeHeader) && ContainsBytes(partBytes, Encoding.UTF8.GetBytes("\r\n\r\n")))
                            {
                                // Skip headers and process the content
                                int headerEnd = IndexOf(partBytes, Encoding.UTF8.GetBytes("\r\n\r\n"), 0);
                                if (headerEnd != -1)
                                {
                                    byte[] contentData = new byte[partBytes.Length - headerEnd - 4];
                                    Array.Copy(partBytes, headerEnd + 4, contentData, 0, contentData.Length);
                                    // Process the content data based on its content type
                                    if (contentTypeHeader.Contains("text/xml"))
                                    {
                                        string xmlData = Encoding.UTF8.GetString(contentData);
                                        multipartResponse.XmlData = xmlData;
                                    }
                                    else if (contentTypeHeader.Contains("image/jpeg"))
                                    {
                                        //string filename = GetHeaderValue(partString, "Content-Disposition", "filename");
                                        string filename = GetFilenameFromContentDisposition(partString);
                                        if (String.IsNullOrEmpty(filename))
                                            filename = "detectionPicture.jpeg";

                                        var imgData = new Image
                                        {
                                            FileName = filename,
                                            Extension = contentTypeHeader,
                                            Data = contentData
                                        };
                                        multipartResponse.Images = imgData;
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new HttpRequestException($"Request failed with status code {response.StatusCode}");
                    }

                }
                // Process the multipart content.
                return multipartResponse;
            }

            throw new HttpRequestException($"Request failed with status code {response.StatusCode}");
        }

        private static string GetBoundary(string headers)
        {
            var contentMediaType = Microsoft.Net.Http.Headers.MediaTypeHeaderValue.Parse(headers);
            var boundary = Microsoft.Net.Http.Headers.HeaderUtilities.RemoveQuotes(contentMediaType.Boundary);
            if (StringSegment.IsNullOrEmpty(boundary))
                throw new InvalidDataException("Missing content-type boundary.");

            return boundary.Value;
        }

        // Helper method to extract header values
        private static string GetHeaderContent(string text, string headerName)
        {
            string[] lines = text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                if (line.StartsWith(headerName))
                {
                    return line.Substring(headerName.Length + 2); // +2 to skip ": "
                }
            }
            return null;
        }

        // Helper method to extract filename from Content-Disposition header
        static string GetFilenameFromContentDisposition(string text)
        {
            // Use regular expressions to extract the filename
            Match match = Regex.Match(text, @"filename=""([^""]+)""");
            if (match.Success)
            {
                return match.Groups[1].Value;
            }
            return null;
        }

        private static string GetHeaderValue(string text, string headerName, string parameterName = null)
        {
            string[] lines = text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                if (line.StartsWith(headerName))
                {
                    var header = new ContentDispositionHeaderValue(line);
                    if (parameterName != null && header.Parameters.Any(p => p.Name == parameterName))
                    {
                        return header.Parameters.First(p => p.Name == parameterName).Value;
                    }
                    else if (parameterName == null && header.Parameters.Count == 0)
                    {
                        return header.Name;
                    }
                }
            }

            return null;
        }

        private static byte[][] SplitByteArray(byte[] source, byte[] delimiter)
        {
            var parts = new System.Collections.Generic.List<byte[]>();
            int offset = 0;

            while (offset < source.Length)
            {
                int index = IndexOf(source, delimiter, offset);

                if (index == -1)
                    index = source.Length;

                var part = new byte[index - offset];
                Array.Copy(source, offset, part, 0, part.Length);
                parts.Add(part);

                offset = index + delimiter.Length;
            }

            return parts.ToArray();
        }

        private static int IndexOf(byte[] source, byte[] pattern, int start)
        {
            int index = -1;

            for (int i = start; i <= source.Length - pattern.Length; i++)
            {
                bool found = true;
                for (int j = 0; j < pattern.Length; j++)
                {
                    if (source[i + j] != pattern[j])
                    {
                        found = false;
                        break;
                    }
                }
                if (found)
                {
                    index = i;
                    break;
                }
            }

            return index;
        }

        private static bool ContainsBytes(byte[] source, byte[] pattern)
        {
            int limit = source.Length - pattern.Length + 1;
            for (int i = 0; i < limit; ++i)
            {
                bool found = true;
                for (int j = 0; j < pattern.Length; ++j)
                {
                    if (source[i + j] != pattern[j])
                    {
                        found = false;
                        break;
                    }
                }
                if (found) return true;
            }
            return false;
        }

        private static async Task<string> HandleXmlResponseAsync(MultipartContent multipartContent)
        {
            // Find and process the XML part.
            var xmlPart = multipartContent.FirstOrDefault(c => c.Headers.ContentType.MediaType.Equals("application/xml"));
            if (xmlPart != null)
            {
                return await xmlPart.ReadAsStringAsync();
            }

            throw new Exception("XML part not found in multipart response.");
        }

        //private static async Task<byte[]> HandleImageResponsesAsync(MultipartContent multipartContent)
        //{
        //    // Find and process the image parts.
        //    var imageParts = multipartContent.Where(c => c.Headers.ContentType.MediaType.StartsWith("image/")).ToList();
        //    if (imageParts.Count > 0)
        //    {
        //        // Combine image parts into a single byte array or a list.
        //        var images = new List<byte[]>();
        //        foreach (var imagePart in imageParts)
        //        {
        //            var imageBytes = await imagePart.ReadAsByteArrayAsync();
        //            images.Add(imageBytes);
        //        }
        //        return images.ToArray();
        //    }

        //    throw new Exception("Image parts not found in multipart response.");
        //}
    }
}
