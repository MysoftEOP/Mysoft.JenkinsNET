﻿using Mysoft.JenkinsNET.Models;
using System;
using System.Xml.Linq;

namespace Mysoft.JenkinsNET.Internal.Commands
{
    internal class CrumbGetCommand : JenkinsHttpCommand
    {
        public JenkinsCrumb Result {get; private set;}


        public CrumbGetCommand(IJenkinsContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            Url = NetPath.Combine(context.BaseUrl, "crumbIssuer/api/xml");
            UserName = context.UserName;
            Password = context.Password;

            OnWrite = request => {
                request.Method = "GET";
            };

            OnRead = response => {
                using (var stream = response.GetResponseStream()) {
                    if (stream == null) return;

                    var document = XDocument.Load(stream);
                    if (document.Root == null) throw new ApplicationException("An empty response was returned!");

                    Result = new JenkinsCrumb(document.Root);
                }
            };

        #if NET_ASYNC
            OnWriteAsync = async (request, token) => {
                request.Method = "GET";
            };

            OnReadAsync = async (response, token) => {
                using (var stream = response.GetResponseStream()) {
                    if (stream == null) return;

                    var document = XDocument.Load(stream);
                    if (document.Root == null) throw new ApplicationException("An empty response was returned!");

                    Result = new JenkinsCrumb(document.Root);
                }
            };
        #endif
        }
    }
}
