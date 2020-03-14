using System;

namespace Events
{
    // The publisher for a unique object
    public class ObjectPublisher : IDisposable
    {
        private event Action _action;
        private TypePublisher _typePublisher { get; }

        public ObjectPublisher(TypePublisher parent)
        {
            _typePublisher = parent;
            _typePublisher.RegisterObjectPublisher(this);
        }

        public bool Subscribe(Action subscriber)
        {
            _action += subscriber;
            return true;
        }

        public void Dispose() => _typePublisher.Unsubscribe(this);
        
        public void Publish() => _action?.Invoke();
    }
}