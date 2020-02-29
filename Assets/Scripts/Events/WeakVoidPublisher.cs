using System;
using System.Collections.Generic;

namespace Events
{
    // Publisher that does not keep subscribers alive if they are only referenced by this publisher
    // IMPORTANT: See comment for Publish()
    // TODO: Currently there is a mild memory leak since the WeakReference objects themselves never get purged.
    // TODO: This may also impact performance if the simulation runs for a very long time
    public class WeakVoidPublisher
    {
        // save weak references to actions
        private HashSet<WeakReference<Action>> _subscribers { get; } = new HashSet<WeakReference<Action>>();

        public void Subscribe(Action action) => _subscribers.Add(new WeakReference<Action>(action));

        // TODO: check if this actually works (not sure whether the new object will match the existing one from the set)
        public bool Unsubscribe(Action action) => _subscribers.Remove(new WeakReference<Action>(action));
        
        // IMPORTANT: Don't subscribe Actions that may unreference other subscribed actions.
        // Otherwise its weak reference and therefore the set will be mutated during iteration.
        public void Publish()
        {
            try
            {
                foreach (var weakReference in _subscribers)
                    if (weakReference.TryGetTarget(out var action))
                        action.Invoke();
            }
            catch (InvalidOperationException e)
            {
                throw new Exception("Subscribers to WeakVoidPublisher must never modify" +
                                    " other subscribers to the same WeakVoidPublisher", e);
            }
        }
    }
}
