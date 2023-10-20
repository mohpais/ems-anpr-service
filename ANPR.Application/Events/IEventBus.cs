namespace Microsoft.Lonsum.Services.ANPR.Application.Events
{
    public interface IEventBus
    {
        void Publish(string message);
        void Exchange(string message);
    }
}
