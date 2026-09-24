# Solution 1 - AMPS

This solution wraps the official `AMPS.Client` NuGet package from 60East behind `IAmpsMessaging`. It exposes connect, publish, subscribe, SOW query, and SOW delete operations as the reusable `Company.Tibco.Amps` package.

The official AMPS C# client is available as the `AMPS.Client` NuGet package. The sample pins 5.3.5.1; align this with the AMPS server/client version approved by your project. See 60East documentation for HAClient, durable subscriptions, replay, and failover before enabling those features in production.
