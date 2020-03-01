using System;

namespace Events
{
    public class ObjectPublisher
    {
        private event Action _action;
        private TypePublisher _typePublisher { get; }

        public ObjectPublisher(TypePublisher parent)
        {
            _typePublisher = parent;
            _typePublisher.RegisterObjectPublisher(this);
        }

        public void Subscribe(Action subscriber) => _action += subscriber;
        
        public void UnsubscribeAll() => _typePublisher.Unsubscribe(this);
        
        public void Publish() => _action?.Invoke();
    }
}