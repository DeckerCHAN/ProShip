/* part of Pyrolite, by Irmen de Jong (irmen@razorvine.net) */
/*
 * Declaration: This file been copied to project ProShip explicitly from Pyrolite
 * rather than use nuget package is the concerning about the size of assembly.
 * ProShip does not own any part of the code in this file.
 */

using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace LibProShip.Infrastructure.Unpickling
{
    /// <summary>
    /// Helper type that represents the unpickler working stack. 
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class UnpickleStack
    {
        private readonly ArrayList _stack;
        public readonly object MARKER;

        public UnpickleStack()
        {
            _stack = new ArrayList();
            MARKER = new object(); // any new unique object
        }

        public void add(dynamic o)
        {
            _stack.Add(o);
        }

        public void add_mark()
        {
            _stack.Add(MARKER);
        }

        public dynamic pop()
        {
            int size = _stack.Count;
            var result = _stack[size - 1];
            _stack.RemoveAt(size - 1);
            return result;
        }

        public ArrayList pop_all_since_marker()
        {
            ArrayList result = new ArrayList();
            dynamic o = pop();
            while (!o.Equals(MARKER))
            {
                result.Add(o);
                o = pop();
            }

            result.TrimToSize();
            result.Reverse();
            return result;
        }

        public dynamic peek()
        {
            return _stack[_stack.Count - 1];
        }

        public void trim()
        {
            _stack.TrimToSize();
        }

        public int size()
        {
            return _stack.Count;
        }

        public void clear()
        {
            _stack.Clear();
            _stack.TrimToSize();
        }
    }
}