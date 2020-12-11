using Hangfire.Client;
using Hangfire.Common;
using Hangfire.States;
using System.Collections.Generic;

namespace BatchingIssueRepo
{
    public sealed class HangfireClientFilterProvider : IJobFilterProvider
    {        
        public IEnumerable<JobFilter> GetFilters(Job job)
        {           
            return new JobFilter[]
                    {
                       new JobFilter(new HangfireTenantClientFilter(), JobFilterScope.Global, null)
                    };
        }
    }

    public sealed class HangfireTenantClientFilter : IClientFilter, IElectStateFilter
    {
        public void OnCreated(CreatedContext filterContext)
        {
            //Intentionally Left Blank
        }

        public void OnCreating(CreatingContext filterContext)
        {
            
        }

        public void OnStateElection(ElectStateContext context)
        {
            
        }
    }
}
