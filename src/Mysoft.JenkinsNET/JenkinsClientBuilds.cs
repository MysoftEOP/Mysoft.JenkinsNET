using Mysoft.JenkinsNET.Exceptions;
using Mysoft.JenkinsNET.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Mysoft.JenkinsNET
{
    /// <summary>
    /// A collection of methods used for interacting with Jenkins Builds.
    /// </summary>
    /// <remarks>
    /// Used internally by <seealso cref="JenkinsClient"/>
    /// </remarks>
    public sealed class JenkinsClientBuilds
    {
        private readonly IJenkinsHttpClient httpClient;

        public JenkinsClientBuilds(IJenkinsHttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        /// <summary>
        /// Gets information describing a Jenkins Job Build.
        /// </summary>
        /// <param name="jobName">The name of the Job.</param>
        /// <param name="buildNumber">The number of the build.</param>
        /// <exception cref="JenkinsJobGetBuildException"></exception>
        public T Get<T>(string jobName, string buildNumber, string root = null) where T : class, IJenkinsBuild
        {
            try
            {
                return httpClient.BuildGet(jobName, buildNumber, root).Result as T;
            }
            catch (Exception error)
            {
                throw new JenkinsJobGetBuildException($"Failed to retrieve build #{buildNumber} of Jenkins Job '{jobName}'!", error);
            }
        }

        public void Stop(string jobName, int buildNumber, string root = null)
        {
            try
            {
                httpClient.BuildStop(jobName, buildNumber, root).Wait();
            }
            catch (Exception error)
            {
                throw new JenkinsJobGetBuildException($"Failed to stop build #{buildNumber} of Jenkins Job '{jobName}'!", error);
            }
        }

        /// <summary>
        /// Gets the progressive text output from a Jenkins Job Build.
        /// </summary>
        /// <param name="jobName">The name of the Job.</param>
        /// <param name="buildNumber">The number of the build.</param>
        /// <param name="start">The character position to begin reading from.</param>
        /// <exception cref="JenkinsNetException"></exception>
        public JenkinsProgressiveTextResponse GetProgressiveText(string jobName, string buildNumber, int start, string root = null)
        {
            try
            {
                return httpClient.BuildProgressiveText(jobName, buildNumber, start, root).Result;
            }
            catch (Exception error)
            {
                throw new JenkinsNetException($"Failed to retrieve progressive text from build #{buildNumber} of Jenkins Job '{jobName}'!", error);
            }
        }

        /// <summary>
        /// Gets the console output from a Jenkins Job Build.
        /// </summary>
        /// <param name="jobName">The name of the Job.</param>
        /// <param name="buildNumber">The number of the build.</param>
        /// <param name="start">The character position to begin reading from.</param>
        /// <exception cref="JenkinsNetException"></exception>
        public JenkinsProgressiveHtmlResponse GetProgressiveHtml(string jobName, string buildNumber, int start, string root = null)
        {
            try
            {
                return httpClient.BuildProgressiveHtml(jobName, buildNumber, start, root).Result;
            }
            catch (Exception error)
            {
                throw new JenkinsNetException($"Failed to retrieve progressive HTML from build #{buildNumber} of Jenkins Job '{jobName}'!", error);
            }
        }
    }
}
