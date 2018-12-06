using Mysoft.JenkinsNET.Internal;
using Mysoft.JenkinsNET.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Linq;
using Mysoft.JenkinsNET.Exceptions;
using System.Web;
using System.Xml;

namespace Mysoft.JenkinsNET
{
    internal class JenkinsHttpClient
    {
        private readonly HttpClient _httpClient;

        private readonly static string pattern = @"<\?xml[^\>]*\?>";

        internal JenkinsHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jobName"></param>
        /// <param name="buildNumber"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        async Task<MemoryStream> ArtifactGetCommand(string jobName, string buildNumber, string filename)
        {
            if (string.IsNullOrEmpty(jobName))
                throw new ArgumentException("'jobName' cannot be empty!");

            if (string.IsNullOrEmpty(buildNumber))
                throw new ArgumentException("'buildNumber' cannot be empty!");

            if (string.IsNullOrEmpty(filename))
                throw new ArgumentException("'filename' cannot be empty!");

            var urlFilename = filename.Replace('\\', '/');
            var url = NetPath.Combine("job", jobName, buildNumber, "artifact", urlFilename);

            using (var response = await _httpClient.PostAsync(url, null))
            {
                using (var stream = await response.Content.ReadAsStreamAsync())
                {
                    using (var result = new MemoryStream())
                    {
                        stream.CopyTo(result);
                        result.Seek(0, SeekOrigin.Begin);
                        return result;
                    }
                }
            }
        }

        /// <summary>
        /// 获取构建任务
        /// </summary>
        /// <param name="jobName"></param>
        /// <param name="buildNumber"></param>
        /// <returns></returns>
        async Task<JenkinsBuildBase> BuildGetCommand(string jobName, string buildNumber)
        {
            if (string.IsNullOrEmpty(jobName))
                throw new ArgumentException("'jobName' cannot be empty!");

            if (string.IsNullOrEmpty(buildNumber))
                throw new ArgumentException("'buildNumber' cannot be empty!");

            var url = NetPath.Combine("job", jobName, buildNumber, "api/xml");

            using (var response = await _httpClient.PostAsync(url, null))
            {
                var document = await ReadXml(response);
                return new JenkinsBuildBase(document.Root);
            }
        }

        /// <summary>
        /// 获取构建输出
        /// </summary>
        /// <param name="jobName"></param>
        /// <param name="buildNumber"></param>
        /// <returns></returns>
        async Task<string> BuildOutputCommand(string jobName, string buildNumber)
        {
            if (string.IsNullOrEmpty(jobName))
                throw new ArgumentException("'jobName' cannot be empty!");

            if (string.IsNullOrEmpty(buildNumber))
                throw new ArgumentException("'buildNumber' cannot be empty!");

            var url = NetPath.Combine("job", jobName, buildNumber, "consoleText");

            using (var response = await _httpClient.GetAsync(url))
            {
                using (var content = response.Content)
                {
                    using (var stream = await content.ReadAsStreamAsync())
                    {
                        var encoding = TryGetEncoding(content, Encoding.UTF8);
                        using (var reader = new StreamReader(stream, encoding))
                        {
                            return reader.ReadToEnd();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 获取部署过程的Html
        /// </summary>
        /// <param name="jobName"></param>
        /// <param name="buildNumber"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        async Task<JenkinsProgressiveHtmlResponse> BuildProgressiveHtmlCommand(string jobName, string buildNumber, int start)
        {
            if (string.IsNullOrEmpty(jobName))
                throw new ArgumentException("'jobName' cannot be empty!");

            if (string.IsNullOrEmpty(buildNumber))
                throw new ArgumentException("'buildNumber' cannot be empty!");

            var url = NetPath.Combine("job", jobName, buildNumber, "logText/progressiveHtml");

            var paras = new Dictionary<string, string>
            {
                 { "start",start.ToString()}
            };

            using (var response = await _httpClient.PostAsync(url, new FormUrlEncodedContent(paras)))
            {
                var hSize = response.Headers.GetValues("X-Text-Size").FirstOrDefault();
                var hMoreData = response.Headers.GetValues("X-More-Data").FirstOrDefault();

                if (!int.TryParse(hSize, out var _size))
                    throw new ApplicationException($"Unable to parse x-text-size header value '{hSize}'!");

                using (var content = response.Content)
                {
                    using (var stream = await response.Content.ReadAsStreamAsync())
                    {
                        var encoding = TryGetEncoding(content, Encoding.UTF8);
                        using (var reader = new StreamReader(stream, encoding))
                        {
                            return new JenkinsProgressiveHtmlResponse
                            {
                                Size = _size,
                                MoreData = string.Equals(hMoreData, bool.TrueString, StringComparison.OrdinalIgnoreCase),
                                Html = reader.ReadToEnd()
                            };
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 获取部署过程的Text
        /// </summary>
        /// <param name="jobName"></param>
        /// <param name="buildNumber"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        async Task<JenkinsProgressiveTextResponse> BuildProgressiveTextCommand(string jobName, string buildNumber, int start)
        {
            if (string.IsNullOrEmpty(jobName))
                throw new ArgumentException("'jobName' cannot be empty!");

            if (string.IsNullOrEmpty(buildNumber))
                throw new ArgumentException("'buildNumber' cannot be empty!");

            var url = NetPath.Combine("job", jobName, buildNumber, "logText/progressiveText");

            var paras = new Dictionary<string, string>
            {
                 { "start",start.ToString()}
            };

            using (var response = await _httpClient.PostAsync(url, new FormUrlEncodedContent(paras)))
            {
                var hSize = response.Headers.GetValues("X-Text-Size").FirstOrDefault();
                var hMoreData = response.Headers.GetValues("X-More-Data").FirstOrDefault();

                if (!int.TryParse(hSize, out var _size))
                    throw new ApplicationException($"Unable to parse x-text-size header value '{hSize}'!");

                using (var content = response.Content)
                {
                    using (var stream = await response.Content.ReadAsStreamAsync())
                    {
                        var encoding = TryGetEncoding(content, Encoding.UTF8);
                        using (var reader = new StreamReader(stream, encoding))
                        {
                            return new JenkinsProgressiveTextResponse
                            {
                                Size = _size,
                                MoreData = string.Equals(hMoreData, bool.TrueString, StringComparison.OrdinalIgnoreCase),
                                Text = reader.ReadToEnd()
                            };
                        }
                    }
                }
            }
        }

        async Task<JenkinsCrumb> CrumbGetCommand()
        {
            using (var response = await _httpClient.GetAsync("crumbIssuer/api/xml"))
            {
                using (var stream = await response.Content.ReadAsStreamAsync())
                {
                    var document = XDocument.Load(stream);
                    if (document.Root == null)
                    {
                        throw new ApplicationException("An empty response was returned!");
                    }

                    return new JenkinsCrumb(document.Root);
                }
            }
        }

        async Task<Jenkins> JenkinsGetCommand()
        {
            using (var response = await _httpClient.GetAsync("api/xml"))
            {
                var document = await ReadXml(response);
                return new Jenkins(document.Root);
            }
        }

        async Task<JenkinsBuildResult> JobBuildCommand(string jobName)
        {
            if (string.IsNullOrEmpty(jobName))
                throw new ArgumentException("'jobName' cannot be empty!");

            var url = NetPath.Combine("job", jobName, "build?delay=0sec");

            using (var response = await _httpClient.PostAsync(url, null))
            {
                if (response.StatusCode != System.Net.HttpStatusCode.Created)
                    throw new JenkinsJobBuildException($"Expected HTTP status code 201 but found {(int)response.StatusCode}!");

                return new JenkinsBuildResult
                {
                    QueueItemUrl = response.Headers.Location.AbsoluteUri
                };
            }
        }

        async Task<JenkinsBuildResult> JobBuildWithParametersCommand(string jobName, IDictionary<string, string> jobParameters)
        {
            if (string.IsNullOrEmpty(jobName))
                throw new ArgumentException("'jobName' cannot be empty!");

            if (jobParameters == null)
                throw new ArgumentNullException(nameof(jobParameters));

            var _params = new Dictionary<string, string>(jobParameters)
            {
                ["delay"] = "0sec",
            };

            var query = new StringWriter();
            WriteJobParameters(query, _params);

            var url = NetPath.Combine("job", jobName, $"buildWithParameters?{query}");

            using (var response = await _httpClient.PostAsync(url, null))
            {
                if (response.StatusCode != System.Net.HttpStatusCode.Created)
                    throw new JenkinsJobBuildException($"Expected HTTP status code 201 but found {(int)response.StatusCode}!");

                return new JenkinsBuildResult
                {
                    QueueItemUrl = response.Headers.Location.AbsoluteUri
                };
            }
        }

        async Task JobCreateCommand(string jobName, JenkinsProject job)
        {
            if (string.IsNullOrEmpty(jobName))
                throw new ArgumentException("Value cannot be empty!", nameof(jobName));

            if (job == null)
                throw new ArgumentNullException(nameof(job));

            var url = NetPath.Combine("createItem") + NetPath.Query(new { name = jobName });

            var xmlSettings = new XmlWriterSettings
            {
                ConformanceLevel = ConformanceLevel.Fragment,
                Indent = false,
            };

            var contentData = "";

            using (var sw = new StringWriter())
            {
                using (var writer = XmlWriter.Create(sw, xmlSettings))
                {
                    job.Node.WriteTo(writer);
                }

                contentData = sw.ToString();
            }

            using (var response = await _httpClient.PostAsync(url, new StringContent(contentData, Encoding.UTF8, "application/xml")))
            {

            }
        }

        async Task JobDeleteCommand(string jobName)
        {
            if (string.IsNullOrEmpty(jobName))
                throw new ArgumentException("'jobName' cannot be empty!");

            var url = NetPath.Combine("job", jobName, "doDelete");

            using (var response = await _httpClient.PostAsync(url, null))
            {

            }
        }

        async Task<JenkinsQueueItem> QueueGetItemCommand(int itemNumber)
        {
            var url = NetPath.Combine("queue/item", itemNumber.ToString(), "api/xml");

            using (var response = await _httpClient.PostAsync(url, null))
            {
                using (var stream = await response.Content.ReadAsStreamAsync())
                {
                    var document = XDocument.Load(stream);
                    if (document.Root == null) throw new ApplicationException("An empty response was returned!");

                    return new JenkinsQueueItem(document.Root);
                }
            }
        }

        async Task<JenkinsQueueItem[]> QueueItemListCommand()
        {
            using (var response = await _httpClient.GetAsync("queue/api/xml"))
            {
                using (var stream = await response.Content.ReadAsStreamAsync())
                {
                    var document = XDocument.Load(stream);
                    if (document.Root == null) throw new ApplicationException("An empty response was returned!");

                    return System.Xml.XPath.Extensions.XPathSelectElements(document, "/queue/item")
                        .Select(node => new JenkinsQueueItem(node)).ToArray();
                }
            }
        }

        /// <summary>
        /// 返回数据读取到xml对象
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        private async Task<XDocument> ReadXml(HttpResponseMessage response)
        {
            using (var stream = await response.Content.ReadAsStreamAsync())
            {
                using (var reader = new StreamReader(stream))
                {
                    var xml = reader.ReadToEnd();
                    xml = RemoveXmlDeclaration(xml);
                    return XDocument.Parse(xml);
                }
            }
        }

        private static string RemoveXmlDeclaration(string xml)
        {
            return Regex.Replace(xml, pattern, string.Empty);
        }

        private static Encoding TryGetEncoding(string name, Encoding fallback)
        {
            try
            {
                return Encoding.GetEncoding(name);
            }
            catch
            {
                return fallback;
            }
        }

        private static Encoding TryGetEncoding(HttpContent content, Encoding fallback)
        {
            var defaultEncoding = "utf-8";

            if (content?.Headers?.ContentEncoding?.Count > 1)
            {
                defaultEncoding = content.Headers.ContentEncoding.FirstOrDefault();
            }

            return TryGetEncoding(defaultEncoding, fallback);
        }

        private void WriteJobParameters(TextWriter writer, IDictionary<string, string> jobParameters)
        {
            var isFirst = true;
            foreach (var pair in jobParameters)
            {
                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    writer.Write('&');
                }

                var encodedName = HttpUtility.UrlEncode(pair.Key);
                var encodedValue = HttpUtility.UrlEncode(pair.Value);

                writer.Write(encodedName);
                writer.Write('=');
                writer.Write(encodedValue);
            }
        }
    }
}
