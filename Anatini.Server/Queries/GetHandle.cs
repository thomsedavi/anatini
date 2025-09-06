using Anatini.Server.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Anatini.Server.Queries
{
    public class GetHandle(string handleValue) : IQuery<Handle?>
    {
        public async Task<Handle?> ExecuteAsync()
        {
            using var context = new AnatiniContext();

            return await context.Handles.WithPartitionKey(handleValue).FirstOrDefaultAsync(handle => handle.Value == handleValue);
        }
    }
}
