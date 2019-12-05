using System;
using System.Collections.Generic;

namespace Confighub.Microsoft.Extensions.Configuration
{
    public class Options
    {
        public string Url { get; set; }
        public string Token { get; set; }
        public string ApplicationName { get; set; }
        public string PushPath { get; set; }
        public string PullPath { get; set; }
        public bool EnableSync { get; set; }
        public bool EnableKeyCreation { get; set; }
        public bool UpdateAppSettings { get; set; }
        public bool ThrowExceptions { get; set; }
        public List<string> PullContexts { get; set; }
        public List<string> PushContexts { get; set; }

        public void Validate()
        {
            if (this.Url == null)
                throw new ArgumentException("ConfigHub url can not be null");
            if (this.Token == null)
                throw new ArgumentException("ConfigHub token can not be null");
            if (this.ApplicationName == null)
                throw new ArgumentException("ConfigHub applicationName can not be null");
            if (this.PushPath == null)
                throw new ArgumentException("ConfigHub PushPath can not be null");
            if (this.PullPath == null)
                throw new ArgumentException("ConfigHub PullPath can not be null");
            if (this.PullContexts == null || this.PullContexts.Count == 0)
                throw new ArgumentException("ConfigHub PullContexts can not be null or empty");
            if (this.PushContexts == null || this.PullContexts.Count == 0)
                throw new ArgumentException("ConfigHub PushContexts can not be null or empty");
        }
    }
}
