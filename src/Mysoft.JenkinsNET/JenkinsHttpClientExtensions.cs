using Microsoft.Extensions.DependencyInjection;
using Mysoft.JenkinsNET.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Mysoft.JenkinsNET
{
    public static class JenkinsHttpClientExtensions
    {
        public static IServiceCollection AddJenkinsHttpClient(this IServiceCollection serviceCollection, string root, string userName, string passwordOrToken)
        {
            return serviceCollection
                .AddHttpClient("JenkinsNET", (provider, c) =>
                {
                    c.BaseAddress = new Uri(root);
                    c.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(new ProductHeaderValue("Mysoft.JenkinsNET")));
                    c.DefaultRequestHeaders.Connection.Add("keep-alive");

                    if (string.IsNullOrWhiteSpace(userName) == false && string.IsNullOrWhiteSpace(passwordOrToken) == false)
                    {
                        var data = Encoding.UTF8.GetBytes($"{userName}:{passwordOrToken}");
                        var basicAuthToken = Convert.ToBase64String(data);
                        c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", basicAuthToken);
                    }

                    var crumb = provider.GetService<JenkinsCrumb>();
                    if (crumb != null)
                    {
                        c.DefaultRequestHeaders.Add(crumb.CrumbRequestField, crumb.Crumb);
                    }
                })
                .AddTypedClient<IJenkinsHttpClient, JenkinsHttpClient>()
                .Services
                .AddHttpClient<JenkinsAuthHttpClient>("JenkinsNET_Auth", c =>
                {
                    c.BaseAddress = new Uri(root);
                    c.DefaultRequestHeaders.Connection.Add("keep-alive");
                })
                .Services
                .AddSingleton(provider =>
                {
                    var httpClient = provider.GetService<JenkinsAuthHttpClient>();
                    return httpClient.CrumbGet().Result;
                })
                ;
        }
    }
}
