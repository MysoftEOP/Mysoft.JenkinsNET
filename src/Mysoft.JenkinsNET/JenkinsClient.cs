﻿using Mysoft.JenkinsNET.Exceptions;
using Mysoft.JenkinsNET.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Mysoft.JenkinsNET
{
    /// <summary>
    /// HTTP-Client for interacting with Jenkins API.
    /// </summary>
    public class JenkinsClient
    {
        /// <summary>
        /// Group of methods for interacting with Jenkins Jobs.
        /// </summary>
        public JenkinsClientJobs Jobs { get; }

        /// <summary>
        /// Group of methods for interacting with Jenkins Builds.
        /// </summary>
        public JenkinsClientBuilds Builds { get; }

        /// <summary>
        /// Group of methods for interacting with the Jenkins Job-Queue.
        /// </summary>
        public JenkinsClientQueue Queue { get; }

        private readonly IJenkinsHttpClient httpClient;

        /// <summary>
        /// Creates a new Jenkins Client.
        /// </summary>
        public JenkinsClient(IJenkinsHttpClient httpClient, JenkinsClientJobs jobs, JenkinsClientBuilds builds, JenkinsClientQueue queue, JenkinsAuth authHttpClient)
        {
            Jobs = jobs;
            Builds = builds;
            Queue = queue;
            this.httpClient = httpClient;
        }

        /// <summary>
        /// Gets the root description of the Jenkins node.
        /// </summary>
        /// <exception cref="JenkinsNetException"></exception>
        public Jenkins Get(string root = null)
        {
            try
            {
                return httpClient.JenkinsGet(root).Result;
            }
            catch (Exception error)
            {
                throw new JenkinsNetException("Failed to retrieve Jenkins description!", error);
            }
        }
    }
}
