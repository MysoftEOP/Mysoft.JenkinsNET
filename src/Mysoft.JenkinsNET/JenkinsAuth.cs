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
    public sealed class JenkinsAuth
    {
        private readonly HttpClient _httpClient;

        private readonly static string pattern = @"<\?xml[^\>]*\?>";

        private JenkinsCrumb crumb;

        private static readonly object locker = new object();

        public JenkinsAuth(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public JenkinsCrumb TryGetCrumb()
        {
            if (crumb == null)
            {
                lock (locker)
                {
                    if (crumb == null)
                    {
                        crumb = GetCrumb().Result;
                    }
                }
            }

            return crumb;
        }

        private async Task<JenkinsCrumb> GetCrumb()
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
    }
}
