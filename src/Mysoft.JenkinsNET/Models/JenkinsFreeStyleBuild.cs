using System.Xml.Linq;

namespace Mysoft.JenkinsNET.Models
{
    /// <summary>
    /// Describes a Jenkins FreeStyle Build.
    /// </summary>
    public class JenkinsFreeStyleBuild : JenkinsBuildBase
    {
        // TODO: Unknown
        //public string BuiltOn => Node?.TryGetValue<string>("builtOn");

        // TODO: Unknown
        //public string[] ChangeSet => Node?.Wrap<string>("changeSet");


        internal JenkinsFreeStyleBuild(XNode node) : base(node) {}
    }
}
