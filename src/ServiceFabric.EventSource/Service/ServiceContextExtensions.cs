using System.Fabric;

namespace DemoService
{
    /// <summary>
    /// Extension methods for <see cref="ServiceContext"/> 
    /// </summary>
    public static class ServiceContextExtensions
    {
        /// <summary>
        /// Convert a <see cref="ServiceContext"/> to a <see cref="LogContext"/>
        /// </summary>
        /// <param name="serviceContext">The see cref="ServiceContext"/> to convert</param>
        /// <returns>An instance of <see cref="LogContext"/></returns>
        public static LogContext ToLogContext(this ServiceContext serviceContext)
        {
            return new LogContext
                {
                    ServiceTypeName = serviceContext.ServiceTypeName,
                    ServiceName = serviceContext.ServiceName.AbsolutePath,
                    ApplicationTypeName = serviceContext.CodePackageActivationContext.ApplicationTypeName,
                    ApplicationName = serviceContext.CodePackageActivationContext.ApplicationName,
                    NodeName = serviceContext.NodeContext.NodeName,
                    CodePackageVersion = serviceContext.CodePackageActivationContext.CodePackageVersion,
                    ReplicaOrInstanceId = serviceContext.ReplicaOrInstanceId,
                    PartitionId = serviceContext.PartitionId
                };
        }
    }
}