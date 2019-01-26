using System;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityModel
{
    /// <summary>
    /// Extensions for EventHandler<TEventArgs>
    /// </summary>
    internal static class EventExtensions
    {
        public static void FireAndForget<TEventArgs>(this EventHandler<TEventArgs> theEvent, object sender, TEventArgs eventArgs)
        {
            if (theEvent == null) throw new ArgumentNullException(nameof(theEvent));

            var subscribers = theEvent.GetInvocationList();
            if (subscribers.Length > 0)
            {
                Task.Run(() =>
                {
                    foreach (EventHandler<TEventArgs> subscriber in subscribers)
                    {
                        try
                        {
                            subscriber.Invoke(sender, eventArgs);
                        }
                        catch
                        {
                        }
                    }
                }, CancellationToken.None);
            }
        }
    }
}