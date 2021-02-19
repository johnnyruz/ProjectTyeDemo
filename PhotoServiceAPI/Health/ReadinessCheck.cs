using MassTransit;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using PhotoServiceAPI.Services;
using System.Threading;
using System.Threading.Tasks;

namespace PhotoServiceAPI.Health
{
    public class ReadinessCheck : IHealthCheck
    {
        readonly PhotoService _photoService;

        public ReadinessCheck(PhotoService photoService)
        {
            _photoService = photoService;
        }

        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default(CancellationToken))
        {

            if (_photoService.CheckDbConnection())
            {
                return Task.FromResult(
                    HealthCheckResult.Healthy());
            }

            return Task.FromResult(
                HealthCheckResult.Unhealthy());
        }
    }
}

