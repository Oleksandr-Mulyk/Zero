using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Zero.Data;

namespace Zero
{
    public class Worker(IServiceProvider serviceProvider, IHostApplicationLifetime hostApplicationLifetime)
        : BackgroundService
    {
        public const string ActivitySourceName = "Migrations";
        private static readonly ActivitySource _activitySource = new(ActivitySourceName);

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            using var activity = _activitySource.StartActivity("Migrating database", ActivityKind.Client);

            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var strategy = dbContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () => { await dbContext.Database.MigrateAsync(cancellationToken); });
        }
    }
}
