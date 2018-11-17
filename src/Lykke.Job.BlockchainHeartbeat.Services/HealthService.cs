using System.Collections.Generic;
using JetBrains.Annotations;
using Lykke.Job.BlockchainHeartbeat.Core.Domain.Health;
using Lykke.Job.BlockchainHeartbeat.Core.Services;

namespace Lykke.Job.BlockchainHeartbeat.Services
{
    // NOTE: See https://lykkex.atlassian.net/wiki/spaces/LKEWALLET/pages/35755585/Add+your+app+to+Monitoring
    [UsedImplicitly]
    public class HealthService : IHealthService
    {
        public string GetHealthViolationMessage()
        {
            return null;
        }

        public IEnumerable<HealthIssue> GetHealthIssues()
        {
            var issues = new HealthIssuesCollection();

            return issues;
        }
    }
}
