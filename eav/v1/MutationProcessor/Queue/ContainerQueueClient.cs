using System;
using Azure.Storage.Queues;

namespace MutationProcessor.Queue
{
    public class ContainerQueueClient : QueueClient
    {
        public ContainerQueueClient(string connectionString, string queue) : base(connectionString, queue)
        {
        }

        public override Uri Uri
        {
            get
            {
                if (InContainer)
                {
                    var u = new UriBuilder
                    {
                        Port = base.Uri.Port,
                        Path = base.Uri.PathAndQuery,
                        Scheme = base.Uri.Scheme,
                        Host = "host.docker.internal"
                    };
                    return u.Uri;
                }

                return base.Uri;
            }
        }

        protected override Uri MessagesUri { 
            get
            {
                if (InContainer)
                {
                    var u = new UriBuilder
                    {
                        Port = base.MessagesUri.Port,
                        Path = base.MessagesUri.PathAndQuery,
                        Scheme = base.MessagesUri.Scheme,
                        Host = "host.docker.internal"
                    };
                    return u.Uri;
                }

                return base.MessagesUri;
            } 
        }
        
        private bool InContainer => Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "1";
    }
}