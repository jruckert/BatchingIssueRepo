using Hangfire;
using System;

namespace TestImplementation
{
    public interface IBackgroundJobManager
    {
        string Queue();

        public string QueueBatch();

        public string ContinueBatch(string id);
    }

    public class BackgroundJobManager : IBackgroundJobManager
    {
        private readonly IBackgroundJobClient _client;

        private readonly IBatchJobClient _batchClient;

        public BackgroundJobManager(IBackgroundJobClient client, IBatchJobClient batchClient)
        {
            _client = client;
            _batchClient = batchClient;
        }

        public string Queue()
        {
            return _client.Enqueue<TestService>(a => a.Queued());
        }

        public string ContinueBatch(string id)
        {
            return _batchClient.ContinueBatchWith(id, x => x.Enqueue<TestService>(a => a.TestFinish()));
        }

        public string QueueBatch()
        {
            return _batchClient.StartNew(x => x.Enqueue<TestService>(a => a.TestStart()));
        }
    }

    public class TestService
    {
        public void Queued()
        {
            Console.WriteLine("Test");
        }

        public void TestStart()
        {
            Console.WriteLine("Test");
        }

        public void TestFinish()
        {
            Console.WriteLine("Test");
        }
    }
}
