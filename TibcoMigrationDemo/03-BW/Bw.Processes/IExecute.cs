using System;
using System.Threading;
using System.Threading.Tasks;
using Company.Tibco.Amps.Abstractions;

namespace Company.Tibco.Bw.Processes;

public interface IExecute
{
    Task ExecuteAsync(AmpsMessage message, CancellationToken ct);
}
