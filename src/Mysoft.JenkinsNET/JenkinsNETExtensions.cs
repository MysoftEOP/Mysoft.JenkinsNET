using Microsoft.Extensions.DependencyInjection;
using Mysoft.JenkinsNET.Models;
using Mysoft.JenkinsNET.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mysoft.JenkinsNET
{
    public static class JenkinsNETExtensions
    {
        public static IServiceCollection AddJenkinsNET(this IServiceCollection serviceCollection, string root, string userName, string passwordOrToken)
        {
            return serviceCollection
                .AddTransient<JenkinsClientJobs>()
                .AddTransient<JenkinsClientBuilds>()
                .AddTransient<JenkinsClientQueue>()
                .AddTransient<IJenkinsClient, JenkinsClient>()
                .AddJenkinsHttpClient(root, userName, passwordOrToken)
                ;
        }
    }
}
