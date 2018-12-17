using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;

namespace Mysoft.JenkinsNET
{
    public static class JenkinsNETExtensions
    {
        public static IServiceCollection UseJenkinsNET(this IServiceCollection serviceCollection, string root, string token)
        {
            return serviceCollection
                .AddHttpClient<JenkinsHttpClient>("JenkinsNET", c =>
                {
                    c.BaseAddress = new Uri(root);
                    c.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(new ProductHeaderValue("Mysoft JenkinsNET")));
                    c.DefaultRequestHeaders.Connection.Add("keep-alive");
                    c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                })
                .Services;
        }
    }
}
