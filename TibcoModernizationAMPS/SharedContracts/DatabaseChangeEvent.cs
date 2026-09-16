namespace SharedContracts;
/// <summary>
/// Represents a universal metadata envelope used to transport database change events 
/// across the message broker without needing hardcoded schemas for every individual table.
/// </summary>
public class DatabaseChangeEvent
{
    /// <summary>
    /// Unique identifier for tracking the transaction or change event across services.
    /// Defaults to an 8-character truncated GUID string for clean logging.
    /// </summary>
    public string TransactionId { get; set; } = Guid.NewGuid().ToString("N")[..8];
    
    /// <summary>
    /// The database operation type being performed (e.g., INSERT, UPDATE, DELETE).
    /// </summary>
    public string OperationType { get; set; } = "INSERT"; // INSERT, UPDATE, DELETE
    
    /// <summary>
    /// The name of the source database table where the mutation occurred (e.g., Orders, Customers).
    /// Enables dynamic routing on the consumer side.
    /// </summary>
    public string TableName { get; set; } = string.Empty;   // E.g., Orders, Customers, Inventory, etc.

    /// <summary>
    /// UTC timestamp indicating precisely when the database mutation event was captured.
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// The actual row data payload serialized as a JSON string. 
    /// This allows arbitrary schemas to fit inside the standard event envelope.
    /// </summary>
    public string PayloadJson { get; set; } = string.Empty; // Serialized row values
}