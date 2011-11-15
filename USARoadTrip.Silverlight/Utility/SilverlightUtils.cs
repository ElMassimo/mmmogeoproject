using System;
using System.Collections.Generic;
using System.Text;

namespace USARoadTrip.Silverlight.Utility
{
    public static class SilverlightUtils
    {
        public static Uri GetViewUri(string viewName, Dictionary<string, object> parameters = null)
        {
            StringBuilder uriString = new StringBuilder();
            uriString.AppendFormat("/Views/{0}.xaml", viewName);

            if(parameters != null)
            {
                uriString.Append("?");
                foreach (var entry in parameters)
                    uriString.AppendFormat("{0}={1}&", entry.Key, entry.Value);
                uriString.Remove(uriString.Length - 1, 1);
            }

            return new Uri(uriString.ToString(), UriKind.Relative);
        }
    }
}
