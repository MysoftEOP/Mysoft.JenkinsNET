using Mysoft.JenkinsNET.Exceptions;
using Mysoft.JenkinsNET.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Mysoft.JenkinsNET
{
    /// <summary>
    /// A collection of methods used for interacting with the Jenkins Job-Queue.
    /// </summary>
    /// <remarks>
    /// Used internally by <seealso cref="JenkinsClient"/>
    /// </remarks>
    public sealed class JenkinsClientQueue
    {
        private readonly IJenkinsHttpClient httpClient;

        public JenkinsClientQueue(IJenkinsHttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        /// <summary>
        /// Retrieves all items from the Job-Queue.
        /// </summary>
        /// <exception cref="JenkinsNetException"></exception>
        public JenkinsQueueItem[] GetAllItems(string root = null)
        {
            try
            {
                return httpClient.QueueItemList(root).Result;
            }
            catch (Exception error)
            {
                throw new JenkinsNetException("Failed to retrieve queue item list!", error);
            }
        }

        /// <summary>
        /// Retrieves an item from the Job-Queue.
        /// </summary>
        /// <param name="itemNumber">The ID of the queue-item.</param>
        /// <exception cref="JenkinsJobBuildException"></exception>
        public JenkinsQueueItem GetItem(int itemNumber, string root = null)
        {
            try
            {
                return httpClient.QueueGetItem(itemNumber, root).Result;
            }
            catch (Exception error)
            {
                throw new JenkinsJobBuildException($"Failed to retrieve queue item #{itemNumber}!", error);
            }
        }

        public void CancelItem(int itemNumber)
        {
            try
            {
                httpClient.QueueCancel(itemNumber).Wait();
            }
            catch (Exception error)
            {
                throw new JenkinsJobBuildException($"Failed to cancel queue item #{itemNumber}!", error);
            }
        }
    }
}
