using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InboundWorker
{

    /// <summary>
    /// Defines a contract for handling table-agnostic database change events.
    /// </summary>
    public interface IDynamicTableHandler
    {
        Task HandleDynamicEventAsync(string tableName, string operationType, string payloadJson);
    }

    /// <summary>
    /// A universal metadata-driven handler that processes database mutations 
    /// for *any* arbitrary table without requiring hardcoded entity models.
    /// </summary>
    public class GenericDynamicHandler : IDynamicTableHandler
    {
        private readonly ILogger<GenericDynamicHandler> _logger;

        public GenericDynamicHandler(ILogger<GenericDynamicHandler> logger)
        {
            _logger = logger;
        }

        public Task HandleDynamicEventAsync(string tableName, string operationType, string payloadJson)
        {
            // Metadata-driven processing: Safely executes DML for ANY arbitrary table name
            // In production, execute dynamic Entity Framework Core command builders or SQL scripts here based on `tableName`.
            _logger.LogInformation("[TARGET DB SYNC] Executing dynamic [{Op}] on Table -> '{Table}' | Payload: {Payload}",
                operationType, tableName, payloadJson);

            return Task.CompletedTask;
        }
    }
}
