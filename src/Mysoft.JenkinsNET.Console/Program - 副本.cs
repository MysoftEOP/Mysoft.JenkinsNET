using Microsoft.Extensions.DependencyInjection;
using Mysoft.JenkinsNET.Models;
using Mysoft.JenkinsNET.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Mysoft.JenkinsNET.Console
{
    class Program
    {
        static IServiceProvider serviceProvider;

        static void Main(string[] args)
        {
            System.Console.WriteLine("Hello World!");

            var services = new ServiceCollection();
            services.AddJenkinsNET("http://jenkins.mingyuanyun.com", "123", "123");
            services.AddTransient<JenkinsJobRunner>();

            serviceProvider = services.BuildServiceProvider();

            CreateJob();

            System.Console.ReadKey();
        }

        static void GetCrumb()
        {
            var httpClient = serviceProvider.GetService<IJenkinsHttpClient>();
            var crumb = httpClient.CrumbGet().Result;
        }

        static void TestRunner()
        {
            var runner = serviceProvider.GetService<JenkinsJobRunner>();

            var model = runner.RunWithParameters("RDC/job/RdcTest_wh", new Dictionary<string, string>
            {
                {"BRANCH_NAME","feature/wuh06-sprint_1210_1221"}
            });

            System.Console.WriteLine($"QueueId:{model.QueueId}");
            System.Console.WriteLine($"Number:{model.Number}");
            System.Console.WriteLine($"Url:{model.Url}");
        }

        static void CreateJob()
        {
            var jobClient = serviceProvider.GetService<JenkinsClientJobs>();

            var str = File.ReadAllText($"{AppContext.BaseDirectory}\\jekins-job-scm-git-template.xml");
            XDocument xmlDoc = XDocument.Parse(str);
            XNode folderConfig = xmlDoc.Root;

            var project = new JenkinsProject(folderConfig);
            //jobClient.Create("/job/kf-wuh06-assign-project-1", "wuh06-test", project);

            jobClient.Create("wuh06-test", project, "/job/kf-wuh06-assign-project-1");
        }

        static void Build()
        {
            var jobClient = serviceProvider.GetService<JenkinsClientJobs>();

            //jobClient.BuildWithParameters("");
        }
    }
}
