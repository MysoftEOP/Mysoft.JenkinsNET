using Mysoft.JenkinsNET.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Mysoft.JenkinsNET
{
    public interface IJenkinsHttpClient
    {
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="jobName"></param>
        ///// <param name="buildNumber"></param>
        ///// <param name="filename"></param>
        ///// <returns></returns>
        //Task<MemoryStream> ArtifactGet(string jobName, string buildNumber, string filename, string root = null);

        /// <summary>
        /// 获取构建任务
        /// </summary>
        /// <param name="jobName"></param>
        /// <param name="buildNumber"></param>
        /// <returns></returns>
        Task<JenkinsBuildBase> BuildGet(string jobName, string buildNumber, string root = null);

        /// <summary>
        /// 停止构建任务
        /// </summary>
        /// <param name="jobName"></param>
        /// <param name="buildNumber"></param>
        /// <param name="root"></param>
        /// <returns></returns>
        Task BuildStop(string jobName, int buildNumber, string root = null);

        /// <summary>
        /// 获取构建输出
        /// </summary>
        /// <param name="jobName"></param>
        /// <param name="buildNumber"></param>
        /// <returns></returns>
        Task<string> BuildOutput(string jobName, string buildNumber, string root = null);

        /// <summary>
        /// 获取部署过程的Html
        /// </summary>
        /// <param name="jobName"></param>
        /// <param name="buildNumber"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        Task<JenkinsProgressiveHtmlResponse> BuildProgressiveHtml(string jobName, string buildNumber, int start, string root = null);

        /// <summary>
        /// 获取部署过程的Text
        /// </summary>
        /// <param name="jobName"></param>
        /// <param name="buildNumber"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        Task<JenkinsProgressiveTextResponse> BuildProgressiveText(string jobName, string buildNumber, int start, string root = null);

        Task<JenkinsCrumb> CrumbGet();

        Task<Jenkins> JenkinsGet(string root = null);

        Task<JenkinsBuildResult> JobBuild(string jobName, string root = null);

        Task<JenkinsBuildResult> JobBuildWithParameters(string jobName, IDictionary<string, string> jobParameters, string root = null);

        Task JobCreate(string jobName, JenkinsProject job, string root = null);

        Task JobUpdate(string jobName, JenkinsProject job, string root = null);

        Task JobDelete(string jobName, string root = null);

        Task<JenkinsQueueItem> QueueGetItem(int itemNumber, string root = null);

        Task<JenkinsQueueItem[]> QueueItemList(string root = null);

        Task<T> JobGet<T>(string jobName, string root = null) where T : class, IJenkinsJob;

        Task QueueCancel(int itemNumber);
    }
}
