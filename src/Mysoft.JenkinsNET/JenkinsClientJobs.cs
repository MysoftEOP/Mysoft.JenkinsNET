using Mysoft.JenkinsNET.Exceptions;
using Mysoft.JenkinsNET.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Mysoft.JenkinsNET
{
    /// <summary>
    /// A collection of methods used for interacting with Jenkins Jobs API.
    /// </summary>
    /// <remarks>
    /// Used internally by <seealso cref="JenkinsClient"/>
    /// </remarks>
    public sealed class JenkinsClientJobs
    {
        private readonly IJenkinsHttpClient httpClient;

        public JenkinsClientJobs(IJenkinsHttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        /// <summary>
        /// Enqueues a Job to be built.
        /// </summary>
        /// <param name="jobName">The name of the Job.</param>
        /// <exception cref="JenkinsJobBuildException"></exception>
        public JenkinsBuildResult Build(string jobName)
        {
            try
            {
                return httpClient.JobBuild(jobName).Result;
            }
            catch (Exception error)
            {
                throw new JenkinsJobBuildException($"Failed to build Jenkins Job '{jobName}'!", error);
            }
        }

        /// <summary>
        /// Enqueues a Job with parameters to be built.
        /// </summary>
        /// <param name="jobName">The name of the Job.</param>
        /// <param name="jobParameters">The collection of parameters for building the job.</param>
        /// <exception cref="JenkinsJobBuildException"></exception>
        public JenkinsBuildResult BuildWithParameters(string jobName, IDictionary<string, string> jobParameters)
        {
            try
            {
                return httpClient.JobBuildWithParameters(jobName, jobParameters).Result;
            }
            catch (Exception error)
            {
                throw new JenkinsJobBuildException($"Failed to build Jenkins Job '{jobName}'!", error);
            }
        }
    }
}
