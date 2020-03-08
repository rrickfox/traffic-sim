using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Utility
{
    // Nodes of SortableLinkedList have to implement this interface to allow for the efficiency bonuses mentioned below.
    public interface ISortableListNode
    {
        ISortableListNode previous { get; set; }
        ISortableListNode next { get; set; }
    }
    
    /* A LinkedList that requires Nodes to inherit from ISortableListNode in order to gain:
           - O(1) Insertion (as opposed to O(n) in a regular LinkedList)
           - O(1) Removal (as opposed to O(n) in a regular LinkedList)
       The nodes stay sorted unless they change their values externally in which case one has to call Sort().
       Sort() is efficient if the nodes are mostly sorted already.
     */
    public class SortableLinkedList<TNode> : IEnumerable<TNode>
        where TNode : class, ISortableListNode
    {
        public TNode first { get; set; }
        public TNode last { get; set; }
        private IComparer<TNode> _comparer;

        public SortableLinkedList(IComparer<TNode> comparer)
        {
            _comparer = comparer;
        }

        public IEnumerator<TNode> GetEnumerator()
        {
            // the list is empty
            if (first == null) yield break;
            
            yield return first;
            foreach (var node in LookForward(first))
                yield return node;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        // Runtime: O(1)
        public void AddFirst(TNode node)
        {
            if (first != null)
            {
                if (_comparer.Compare(node, first) > 0)
                    throw new ArgumentException("node has to be less than or equal to first");
                first.previous = node;
                node.next = first;
            }
            else
            {
                last = node;
                node.next = null;
            }

            first = node;
            node.previous = null;
        }

        // Runtime: O(1)
        public void Remove(TNode node)
        {
            if (node.previous != null)
                node.previous.next = node.next;
            else
                first = (TNode) node.next;

            if (node.next != null)
                node.next.previous = node.previous;
            else
                last = (TNode) node.previous;
        }

        // Runtime: O(1)
        private void MoveBefore(TNode toMove, TNode before)
        {
            if (before == toMove) return;
            
            Remove(toMove);
            InsertBefore(toMove, before);
        }

        // Runtime: O(1)
        private void InsertBefore(TNode toInsert, TNode before)
        {
            // update toInsert
            toInsert.previous = before.previous;
            toInsert.next = before;
            
            // update node that will be behind toInsert
            if (before.previous != null)
                before.previous.next = toInsert;
            else
                first = toInsert;
            
            // update node that will be in front of toInsert
            before.previous = toInsert;
        }

        /* Implemented using insertion sort because:
               - other sorting algorithms are inefficient on linked lists (except for merge sort)
               - insertion sort works well on already (almost) sorted lists
           An alternative would be to implement a "natural merge sort" but that algorithm is way more complicated
           for little to no reward.
           Runtime: O(n^2) but O(k*n) if each element is no more than k places away from its sorted position
         */
        public void Sort()
        {
            // list is empty
            if (first == null) return;
            
            var node = (TNode) first.next;
            while (node != null)
            {
                // find the rearmost node which is still larger than the node we want to insert
                var insertBefore = node;
                while (insertBefore.previous != null && _comparer.Compare((TNode) insertBefore.previous, node) > 0)
                    insertBefore = (TNode) insertBefore.previous;
                
                var nextNode = (TNode) node.next;
                // move node into its correct position in the sorted part of the list
                MoveBefore(node, insertBefore);
                node = nextNode;
            }
        }
        
        // Get the nodes after a node in the list.
        // This is implemented as efficiently as possible since the next element can always be accessed immediately.
        private static IEnumerable<TNode> LookForward(TNode node)
        {
            var current = node.next;
            while (current != null)
            {
                yield return (TNode) current;
                current = current.next;
            }
        }

        // Get the nodes before a node in the list.
        // This is implemented as efficiently as possible since the next element can always be accessed immediately.
        private static IEnumerable<TNode> LookBackward(TNode node)
        {
            var current = node.previous;
            while (current != null)
            {
                yield return (TNode) current;
                current = current.previous;
            }
        }
        
        // Get the nodes in the list which are greater than the specified node.
        // This is implemented as efficiently as possible since the next element can always be accessed immediately.
        public IEnumerable<TNode> AllGreater(TNode node)
            => LookForward(node).Where(otherNode => _comparer.Compare(node, otherNode) < 0);

        // Get the nodes in the list which are greater than or equal to the specified node.
        // This is implemented as efficiently as possible since the next element can always be accessed immediately.
        public IEnumerable<TNode> AllGreaterOrEqual(TNode node)
        {
            foreach (var otherNode in LookBackward(node))
            {
                if (_comparer.Compare(node, otherNode) > 0)
                    break;
                yield return otherNode;
            }
            foreach (var otherNode in LookForward(node))
                yield return otherNode;
        }
    }
}