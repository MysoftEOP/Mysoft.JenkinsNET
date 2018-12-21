using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Mysoft.JenkinsNET.Internal
{
    internal static class XNodeExtensions
    {
        public static T GetValue<T>(this XNode parentNode, string expression)
        {
            var node = parentNode.XPathEvaluate(expression);
            if (node == null)
            {
                throw new ApplicationException($"Unable to locate node '{expression}'!");
            }

            var value = GetNodeValue(node);
            if (value == null)
            {
                throw new ApplicationException("Unable to get node value!");
            }

            return value.To<T>();
        }

        public static T TryGetValue<T>(this XNode parentNode, string expression, T defaultValue = default(T))
        {
            var node = parentNode.XPathEvaluate(expression);
            var value = GetNodeValue(node);
            if (string.IsNullOrEmpty(value))
            {
                return defaultValue;
            }

            return value.To<T>();
        }

        public static T Wrap<T>(this XNode parentNode, string expression, Func<XElement, T> wrapFunc)
        {
            var node = parentNode.XPathSelectElement(expression);
            if (node == null)
            {
                return default(T);
            }

            return wrapFunc(node);
        }

        public static IEnumerable<T> WrapGroup<T>(this XNode parentNode, string expression, Func<XElement, T> wrapFunc)
        {
            return parentNode
                .XPathSelectElements(expression)
                .Select(wrapFunc);
        }

        private static string GetNodeValue(object node)
        {
            object tmpNode = null;

            if (node is IEnumerable)
            {
                tmpNode = (node as IEnumerable)
                    .Cast<object>()
                    .FirstOrDefault();
            }

            switch (tmpNode)
            {
                case null:
                    return null;
                case XElement _e:
                    return _e.Value;
                case XAttribute _a:
                    return _a.Value;
                default:
                    throw new ApplicationException($"Unable to retrieve value from node of type '{node.GetType().Name}'!");
            }
        }
    }
}
