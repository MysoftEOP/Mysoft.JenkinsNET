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
    public sealed class JenkinsHttpClient : IJenkinsHttpClient
    {
        private readonly HttpClient _httpClient;

        private readonly static string pattern = @"<\?xml[^\>]*\?>";

        public JenkinsHttpClient(HttpClient httpClient)
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
        public async Task<MemoryStream> ArtifactGet(string jobName, string buildNumber, string filename)
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
        public async Task<JenkinsBuildBase> BuildGet(string jobName, string buildNumber)
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
        public async Task<string> BuildOutput(string jobName, string buildNumber)
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
        public async Task<JenkinsProgressiveHtmlResponse> BuildProgressiveHtml(string jobName, string buildNumber, int start)
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
        public async Task<JenkinsProgressiveTextResponse> BuildProgressiveText(string jobName, string buildNumber, int start)
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

        public async Task<JenkinsCrumb> CrumbGet()
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

                    var crumb = new JenkinsCrumb(document.Root);

                    if (crumb != null)
                    {
                        _httpClient.DefaultRequestHeaders.Add(crumb.CrumbRequestField, crumb.Crumb);
                    }

                    return crumb;
                }
            }
        }

        public async Task<Jenkins> JenkinsGet()
        {
            using (var response = await _httpClient.GetAsync("api/xml"))
            {
                var document = await ReadXml(response);
                return new Jenkins(document.Root);
            }
        }

        public async Task<JenkinsBuildResult> JobBuild(string jobName)
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

        public async Task<JenkinsBuildResult> JobBuildWithParameters(string jobName, IDictionary<string, string> jobParameters)
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

        public async Task JobCreate(string jobName, JenkinsProject job)
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

        public async Task JobDelete(string jobName)
        {
            if (string.IsNullOrEmpty(jobName))
                throw new ArgumentException("'jobName' cannot be empty!");

            var url = NetPath.Combine("job", jobName, "doDelete");

            using (var response = await _httpClient.PostAsync(url, null))
            {

            }
        }

        public async Task<JenkinsQueueItem> QueueGetItem(int itemNumber)
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

        public async Task<JenkinsQueueItem[]> QueueItemList()
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

    public interface IJenkinsHttpClient
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="jobName"></param>
        /// <param name="buildNumber"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        Task<MemoryStream> ArtifactGet(string jobName, string buildNumber, string filename);

        /// <summary>
        /// 获取构建任务
        /// </summary>
        /// <param name="jobName"></param>
        /// <param name="buildNumber"></param>
        /// <returns></returns>
        Task<JenkinsBuildBase> BuildGet(string jobName, string buildNumber);

        /// <summary>
        /// 获取构建输出
        /// </summary>
        /// <param name="jobName"></param>
        /// <param name="buildNumber"></param>
        /// <returns></returns>
        Task<string> BuildOutput(string jobName, string buildNumber);

        /// <summary>
        /// 获取部署过程的Html
        /// </summary>
        /// <param name="jobName"></param>
        /// <param name="buildNumber"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        Task<JenkinsProgressiveHtmlResponse> BuildProgressiveHtml(string jobName, string buildNumber, int start);

        /// <summary>
        /// 获取部署过程的Text
        /// </summary>
        /// <param name="jobName"></param>
        /// <param name="buildNumber"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        Task<JenkinsProgressiveTextResponse> BuildProgressiveText(string jobName, string buildNumber, int start);

        Task<JenkinsCrumb> CrumbGet();

        Task<Jenkins> JenkinsGet();

        Task<JenkinsBuildResult> JobBuild(string jobName);

        Task<JenkinsBuildResult> JobBuildWithParameters(string jobName, IDictionary<string, string> jobParameters);

        Task JobCreate(string jobName, JenkinsProject job);

        Task JobDelete(string jobName);

        Task<JenkinsQueueItem> QueueGetItem(int itemNumber);

        Task<JenkinsQueueItem[]> QueueItemList();
    }
}
