﻿using Mysoft.JenkinsNET.Exceptions;
using Mysoft.JenkinsNET.Internal.Commands;
using Mysoft.JenkinsNET.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Mysoft.JenkinsNET
{
    /// <summary>
    /// HTTP-Client for interacting with Jenkins API.
    /// </summary>
    public class JenkinsClient : IJenkinsContext
    {
        /// <summary>
        /// The address of the Jenkins instance.
        /// ie: http://localhost:8080
        /// </summary>
        public string BaseUrl {get; set;}

        /// <summary>
        /// [optional] Jenkins Username.
        /// </summary>
        public string UserName {get; set;}

        /// <summary>
        /// [optional] Jenkins ApiToken for the <see cref="UserName"/>.
        /// </summary>
        public string ApiToken {get; set;}

        /// <summary>
        /// Gets or sets the security Crumb to use on API requests.
        /// </summary>
        public JenkinsCrumb Crumb {get; set;}

        /// <summary>
        /// [optional] Jenkins Password.
        /// </summary>
        [Obsolete("This property will be removed in future versions; please use 'JenkinsClient.ApiToken' instead.")]
        public string Password {
            get => ApiToken;
            set => ApiToken = value;
        }

        /// <summary>
        /// Group of methods for interacting with Jenkins Jobs.
        /// </summary>
        public JenkinsClientJobs Jobs {get;}

        /// <summary>
        /// Group of methods for interacting with Jenkins Builds.
        /// </summary>
        public JenkinsClientBuilds Builds {get;}

        /// <summary>
        /// Group of methods for interacting with the Jenkins Job-Queue.
        /// </summary>
        public JenkinsClientQueue Queue {get;}

        /// <summary>
        /// Group of methods for interacting with Jenkins Artifacts.
        /// </summary>
        public JenkinsClientArtifacts Artifacts {get;}


        /// <summary>
        /// Creates a new Jenkins Client.
        /// </summary>
        public JenkinsClient()
        {
            Jobs = new JenkinsClientJobs(this);
            Builds = new JenkinsClientBuilds(this);
            Queue = new JenkinsClientQueue(this);
            Artifacts = new JenkinsClientArtifacts(this);
        }

        /// <summary>
        /// Creates a new Jenkins Client using the provided BaseUrl.
        /// </summary>
        public JenkinsClient(string baseUrl) : this()
        {
            this.BaseUrl = baseUrl;
        }

        /// <summary>
        /// Updates the security Crumb attached to this client.
        /// </summary>
        /// <exception cref="JenkinsNetException"></exception>
        public void UpdateSecurityCrumb()
        {
            try {
                var cmd = new CrumbGetCommand(this);
                cmd.Run();
                Crumb = cmd.Result;
            }
            catch (Exception error) {
                throw new JenkinsNetException("Failed to retrieve crumb!", error);
            }
        }

    #if NET_ASYNC
        /// <summary>
        /// Updates the security Crumb attached to this client asynchronously.
        /// </summary>
        /// <param name="token">An optional token for aborting the request.</param>
        /// <exception cref="JenkinsNetException"></exception>
        public async Task UpdateSecurityCrumbAsync(CancellationToken token = default(CancellationToken))
        {
            try {
                var cmd = new CrumbGetCommand(this);
                await cmd.RunAsync(token);
                Crumb = cmd.Result;
            }
            catch (Exception error) {
                throw new JenkinsNetException("Failed to retrieve crumb!", error);
            }
        }
    #endif

        /// <summary>
        /// Gets the root description of the Jenkins node.
        /// </summary>
        /// <exception cref="JenkinsNetException"></exception>
        public Jenkins Get()
        {
            try {
                var cmd = new JenkinsGetCommand(this);
                cmd.Run();
                return cmd.Result;
            }
            catch (Exception error) {
                throw new JenkinsNetException("Failed to retrieve Jenkins description!", error);
            }
        }

    #if NET_ASYNC
        /// <summary>
        /// Gets the root description of the Jenkins node asynchronously.
        /// </summary>
        /// <param name="token">An optional token for aborting the request.</param>
        /// <exception cref="JenkinsNetException"></exception>
        public async Task<Jenkins> GetAsync(CancellationToken token = default(CancellationToken))
        {
            try {
                var cmd = new JenkinsGetCommand(this);
                await cmd.RunAsync(token);
                return cmd.Result;
            }
            catch (Exception error) {
                throw new JenkinsNetException("Failed to retrieve Jenkins description!", error);
            }
        }
    #endif
    }
}