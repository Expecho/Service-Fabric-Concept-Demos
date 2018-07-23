using System;
using System.Diagnostics.Tracing;

namespace DemoService
{
    [EventData]
    public class LogContext
    {
        public string ServiceTypeName { get; set; }
        public string ServiceName { get; set; }
        public string ApplicationTypeName { get; set; }
        public string ApplicationName { get; set; }
        public string NodeName { get; set; }
        public string CodePackageVersion { get; set; }
        public long ReplicaOrInstanceId { get; set; }
        public Guid PartitionId { get; set; }
    }
}