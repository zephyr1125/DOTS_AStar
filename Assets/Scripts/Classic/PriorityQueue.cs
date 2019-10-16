using System;
using System.Collections.Generic;

namespace Classic
{
    public class PriorityQueue<T>
    {
        private List<Tuple<T, double>> _elements = new List<Tuple<T,double>>();
        
        public int Count => _elements.Count;

        public void Enqueue(T element, double priority)
        {
            _elements.Add(Tuple.Create(element, priority));
        }

        public T Dequeue()
        {
            int bestIndex = 0;

            for (int i = 0; i < _elements.Count; i++) {
                if (_elements[i].Item2 < _elements[bestIndex].Item2) {
                    bestIndex = i;
                }
            }

            T bestItem = _elements[bestIndex].Item1;
            _elements.RemoveAt(bestIndex);
            return bestItem;
        }
    }
}