# Service scheduler

A lightweight, dependency-injection--friendly background job scheduler
for .NET applications.

This library allows you to register background jobs ("services") that
run on custom schedules. The schedule definition (a "trigger") is
separated from the job implementation itself, keeping your code clean,
testable, and flexible.

------------------------------------------------------------------------

## ✨ Features

-   Simple and expressive job registration API
-   Clean separation between **job logic** and **job triggers**
-   Built on top of **IHostedService** and
    **Microsoft.Extensions.DependencyInjection**
-   Supports multiple jobs, each with its own trigger
-   Triggers define *when* to run; jobs define *what* to run

------------------------------------------------------------------------

## 🚀 Installation

``` powershell
Install-Package PN.ServiceScheduler
```

------------------------------------------------------------------------

## 🧠 How It Works

You implement a job by creating a class that implements `IJob`.
Each job has exactly one method: `ExecuteAsync`.

You then register the job and its trigger inside `Program.cs` using the
scheduler builder.

Registration details

The package exposes an extension method `AddServiceScheduler` for
`IServiceCollection` that wires everything up for you:

- Builds the configured registrations and registers them as a singleton
  `IReadOnlyList<Registration>` in the DI container.
- Registers a default `IJobFactory` (`DefaultJobFactory`) if no other
  `IJobFactory` is registered.
- Adds the `Scheduler` as a hosted service so it runs with the
  application lifetime.

By default the scheduler builder will also attempt to register the job
implementation types you add (via `AddSingletonJob<T>`,
`AddScopedJob<T>`, `AddTransientJob<T>`) into the same `IServiceCollection`.
This is convenient for the common case where the default `IJobFactory`
resolves jobs from the application's DI container.

However, if you provide a custom `IJobFactory` that resolves job
instances from an external or separate container, automatic job type
registration into the default `IServiceCollection` can be undesirable
or incorrect. To support this scenario the `SchedulerBuilder` exposes
`UseExternalJobFactory()` which disables auto-registration of job types.
When external job factory mode is enabled the builder will still record
the job registrations for the scheduler, but it will not add the
implementation types to the `IServiceCollection`.

Example: using an external job factory

```csharp
builder.Services.AddServiceScheduler(conf =>
{
    // Tell the builder that job instances will come from a custom factory
    conf.UseExternalJobFactory();

    // Job registrations are recorded for scheduling, but TJob won't be
    // automatically registered in the default IServiceCollection.
    conf.AddSingletonJob<MyJob>("my-job").Every(TimeSpan.FromSeconds(10));
});

// Register your custom factory (must implement IJobFactory)
services.TryAddSingleton<IJobFactory, MyExternalJobFactory>();
```

------------------------------------------------------------------------

## 📌 Example: Creating a Job

``` csharp
public class MyCustomJob : IJob
{
    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await Task.Delay(500, cancellationToken); // Example work
    }
}
```

------------------------------------------------------------------------

## 📌 Example: Scheduling the Job

Add the scheduler and your job inside `Program.cs`:

``` csharp
builder.Services.AddServiceScheduler(conf =>
{
    conf.AddSingletonJob<MyCustomJob>("Job name").Every(TimeSpan.FromSeconds(10));
});
```

You can add as many jobs as you want:

``` csharp
conf.AddScopedJob<ScopedJob>("Report").Every(TimeSpan.FromHours(1));  //every hour
conf.AddTransientJob<SyncJob>("Synchronization").EveryDayAt(new TimeOnly(3, 0), TimeZoneInfo.FindSystemTimeZoneById("Europe/Prague")); // every day at 03:00
```

Job lifetimes and disposal

- `AddSingletonJob<T>` registers `T` as a singleton in the DI container.
- `AddScopedJob<T>` registers `T` with a scoped lifetime. The default
  `DefaultJobFactory` creates an async scope for scoped jobs and disposes
  it after the job finishes.
- `AddTransientJob<T>` registers `T` as transient and resolves it from
  the root provider when executed.

------------------------------------------------------------------------

## ⏱ Built-in Triggers

The library ships with several ready-to-use triggers:

-   `Every(TimeSpan interval)`
-   `OnceAfter(TimeSpan after)`
-   `EveryDayAt(TimeOnly time, TimeZoneInfo timeZone)`
-   `EveryDayAt(IEnumerable<TimeOnly> time, TimeZoneInfo timeZone)`
-   `UsingTrigger(ITrigger trigger)`

You can also create your own custom trigger by implementing:

``` csharp
public interface ITrigger
{
    DateTime? GetNextRunUtc();
    void SetLastRunUtc(DateTime lastRunUtc);
}
```

------------------------------------------------------------------------

## ✔️ Job Execution Guarantees

-   Jobs run in their own tasks
-   No overlapping runs (if a job is still running when the next trigger
    occurs, the run is skipped)
-   Cancellation is respected through `CancellationToken`

Additional notes

- The scheduler picks the next run time from all registered triggers and
  waits until that time. If no next run times remain the scheduler stops.
- The library uses `TimeProvider` internally (and exposes time-provider
  aware triggers) which makes it easier to unit-test scheduling logic by
  providing a custom `TimeProvider`.
