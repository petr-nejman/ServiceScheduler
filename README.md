 # Service scheduler

 A lightweight, dependency-injection friendly background job scheduler for .NET applications. It separates job logic from scheduling (triggers) so your code remains clean and testable.

 ------------------------------------------------------------------------

 ## ✨ Features

 - Simple and expressive job registration API
 - Clean separation between `job` (what to run) and `trigger` (when to run)
 - Built on top of `IHostedService` and `Microsoft.Extensions.DependencyInjection`
 - Supports multiple jobs, each with its own trigger

 ------------------------------------------------------------------------

 ## 🚀 Installation

 ```powershell
 Install-Package PN.ServiceScheduler
 ```

 ------------------------------------------------------------------------

 ## 🧠 How It Works

 Implement a job by creating a class that implements `IJob` and its single method `ExecuteAsync`.

 Register jobs and triggers in `Program.cs` via `services.AddServiceScheduler(...)`.

 `AddServiceScheduler`:

 - Builds the configured registrations and registers them as a singleton `IReadOnlyList<Registration>` in the DI container.
 - Registers a default `IJobFactory` (`DefaultJobFactory`) if no other `IJobFactory` is registered.
 - Adds the `Scheduler` as a hosted service so it runs with the application lifetime.

 Note about `TimeProvider`: `AddServiceScheduler` accepts an optional `timeProvider` parameter (default `null`) and forwards it to `SchedulerBuilder`. If `null`, `TimeProvider.System` is used.

 If you provide a custom `IJobFactory` (for example resolving jobs from an external container), call `conf.UseExternalJobFactory()` — the builder will then record job registrations for scheduling but will NOT auto-register job implementation types into the provided `IServiceCollection`.

  ## 📌 Example: Using an external job factory

 ```csharp
 services.AddServiceScheduler(conf =>
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

 ```csharp
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

 ```csharp
 services.AddServiceScheduler(conf =>
 {
     conf.AddSingletonJob<MyCustomJob>("Job name").Every(TimeSpan.FromSeconds(10));
 });
 ```

 You can add as many jobs as you want:

 ```csharp
 conf.AddScopedJob<ScopedJob>("Report").Every(TimeSpan.FromHours(1));  // every hour
 conf.AddTransientJob<SyncJob>("Synchronization").EveryDayAt(new TimeOnly(3, 0), TimeZoneInfo.FindSystemTimeZoneById("Europe/Prague")); // every day at 03:00
 ```

 Job lifetimes and disposal

 - `AddSingletonJob<T>` registers `T` as a singleton in the DI container.
 - `AddScopedJob<T>` registers `T` with a scoped lifetime. The default `DefaultJobFactory` creates an async scope for scoped jobs and disposes it after the job finishes.
 - `AddTransientJob<T>` registers `T` as transient and resolves it from the root provider when executed.

 ------------------------------------------------------------------------

 ## ⏱ Built-in Triggers

 The library ships with several ready-to-use triggers:

 - `Every(TimeSpan interval)`
 - `OnceAfter(TimeSpan after)`
 - `EveryDayAt(TimeOnly time, TimeZoneInfo timeZone)`
 - `EveryDayAt(IEnumerable<TimeOnly> time, TimeZoneInfo timeZone)`
 - `UsingTrigger(ITrigger trigger)`

 You can also create your own custom trigger by implementing:

 ```csharp
 public interface ITrigger
 {
     DateTime? GetNextRunUtc();
     void SetLastRunUtc(DateTime lastRunUtc);
 }
 ```

 ------------------------------------------------------------------------

 ## ✔️ Job Execution Guarantees

 - Jobs run in their own tasks
 - No overlapping runs (if a job is still running when the next trigger occurs, the run is skipped)
 - Cancellation is respected through `CancellationToken`

 Additional notes

 - The scheduler picks the next run time from all registered triggers and waits until that time. If no next run times remain the scheduler stops.
 - The library uses `TimeProvider` internally (and exposes time-provider aware triggers) which makes it easier to unit-test scheduling logic by providing a custom `TimeProvider`.
