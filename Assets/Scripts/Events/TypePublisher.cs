using System;
using System.Collections.Generic;
using MoreLinq;

namespace Events
{
    // Manages the update dependencies of a type
    public class TypePublisher
    {
        private HashSet<ObjectPublisher> _publishers { get; } = new HashSet<ObjectPublisher>();
        private HashSet<TypePublisher> _dependencies { get; }
        private PublishingState _state { get; set; } = PublishingState.ToDo;
        
        private enum PublishingState { ToDo, WaitingForDependencies, Done }

        public TypePublisher(params TypePublisher[] dependencies)
        {
            _dependencies = dependencies.ToHashSet();
            UpdatePublisher.RegisterTypePublisher(this);
        }

        public void RegisterObjectPublisher(ObjectPublisher objectPublisher) => _publishers.Add(objectPublisher);

        public void Unsubscribe(ObjectPublisher subscriberObject) => _publishers.Remove(subscriberObject);

        public void Publish()
        {
            switch (_state)
            {
                case PublishingState.ToDo:
                {
                    // make sure all dependencies have published first
                    _state = PublishingState.WaitingForDependencies;
                    foreach (var dependency in _dependencies)
                        dependency.Publish();
                    
                    // publish after all dependencies are done
                    foreach (var objectPublisher in _publishers)
                        objectPublisher.Publish();
                    _state = PublishingState.Done;
                    
                    break;
                }
                
                case PublishingState.WaitingForDependencies:
                    throw new Exception("Dependency Cycle in a TypePublisher");

                case PublishingState.Done:
                    break;
            }
        }

        public void ResetState() => _state = PublishingState.ToDo;

        public void ResetObjectPublishers() => _publishers.Clear();
    }
}