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
        public JenkinsBuildResult Build(string jobName, string root = null)
        {
            try
            {
                return httpClient.JobBuild(jobName, root).Result;
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
        public JenkinsBuildResult BuildWithParameters(string jobName, IDictionary<string, string> jobParameters, string root = null)
        {
            try
            {
                return httpClient.JobBuildWithParameters(jobName, jobParameters, root).Result;
            }
            catch (Exception error)
            {
                throw new JenkinsJobBuildException($"Failed to build Jenkins Job '{jobName}'!", error);
            }
        }

        /// <summary>
        /// Gets a Job description from Jenkins.
        /// </summary>
        /// <param name="jobName">The Name of the Job to retrieve.</param>
        /// <exception cref="JenkinsNetException"></exception>
        public T Get<T>(string jobName, string root = null) where T : class, IJenkinsJob
        {
            try
            {
                return httpClient.JobGet<T>(jobName, root).Result;
            }
            catch (Exception error)
            {
                throw new JenkinsNetException($"Failed to get Jenkins Job '{jobName}'!", error);
            }
        }

        /// <summary>
        /// Creates a new Job in Jenkins.
        /// </summary>
        /// <param name="jobName">The name of the Job to create.</param>
        /// <param name="job">The description of the Job to create.</param>
        /// <exception cref="JenkinsNetException"></exception>
        public void Create(string jobName, JenkinsProject job, string root = null)
        {
            try
            {
                httpClient.JobCreate(jobName, job, root).Wait();
            }
            catch (Exception error)
            {
                throw new JenkinsNetException($"Failed to create Jenkins Job '{jobName}'!", error);
            }
        }

        public void Delete(string jobName, string root = null)
        {
            try
            {
                httpClient.JobDelete(jobName, root).Wait();
            }
            catch (Exception error)
            {
                throw new JenkinsNetException($"Failed to delete Jenkins Job '{jobName}'!", error);
            }
        }
    }
}
