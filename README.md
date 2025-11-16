# Service scheduler

A lightweight, dependency-injection--friendly background job scheduler
for .NET applications.

This library allows you to register background jobs ("services") that
run on custom schedules. The schedule definition (a **trigger**) is
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
conf.AddScopedJob<ScopedJob>("Report").EveryHour();
conf.AddTransientJob<SyncJob>("Synchronization").EveryDayAt(new TimeOnly(3, 0), TimeZoneInfo.FindSystemTimeZoneById("Europe/Prague")); // at 03:00
```

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
    bool ShouldRun(DateTime nowUtc);
}
```

------------------------------------------------------------------------

## ✔️ Job Execution Guarantees

-   Jobs run in their own tasks
-   No overlapping runs (if a job is still running when the next trigger
    occurs, the run is skipped)
-   Cancellation is respected through `CancellationToken`
