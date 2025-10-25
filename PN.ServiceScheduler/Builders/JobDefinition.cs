namespace PN.ServiceScheduler.Builders
{
    public record JobDefinition(string Name, Type JobType, bool IsScoped);
}
