using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using Hangfire.InMemory;
using TestImplementation;
using Xunit;

namespace BatchingIssueRepo
{
    public class BatchTests
    {
        [Fact]
        public async Task TestBatch()
        {
            var provider = GenerateServiceProvider();
            
            var backgroundManager = provider.GetService<IBackgroundJobManager>();

            var backgroundQueue = provider.GetService<IHostedService>();
            await backgroundQueue.StartAsync(CancellationToken.None);

            var queuedId = backgroundManager.Queue();
            var jobId = backgroundManager.QueueBatch();
            var continuationId = backgroundManager.ContinueBatch(jobId);

            Assert.NotNull(queuedId);
            Assert.NotNull(jobId);
            Assert.NotNull(continuationId);

            await Task.Delay(2000);

            var storage = provider.GetService<JobStorage>();
            Assert.NotNull(storage);

            Assert.Equal(3, storage.GetMonitoringApi().SucceededListCount());

            await backgroundQueue.StopAsync(CancellationToken.None);           
        }

        private IServiceProvider GenerateServiceProvider()
        {
            JobStorage storage = new InMemoryStorage();
            var services = new ServiceCollection();
            services.AddTransient<IBackgroundJobManager, BackgroundJobManager>();
            services.AddSingleton(x =>
            {
                return storage;
            });
            services.AddHangfire((provider, config) =>
            {
                config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                      .UseSimpleAssemblyNameTypeSerializer()
                      .UseIgnoredAssemblyVersionTypeResolver()
                      .UseRecommendedSerializerSettings()                      
                      .UseStorage(storage)
                      .UseBatches(); //TimeSpan.FromDays(7), filterProvider: new HangfireClientFilterProvider()                
            });
            services.AddHangfireServer(
                options =>
                {
                    //options.FilterProvider = new HangfireClientFilterProvider();
                }
            );
            services.AddTransient<IBatchJobClient, BatchJobClient>();
            return services.BuildServiceProvider();
        }
    }    
}
