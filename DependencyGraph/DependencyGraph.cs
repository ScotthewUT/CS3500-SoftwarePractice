// Skeleton implementation written by Joe Zachary for CS 3500, September 2013.
// Version 1.1 (Fixed error in comment for RemoveDependency.)
// Version 1.2 - Daniel Kopta 
//               (Clarified meaning of dependent and dependee.)
//               (Clarified names in solution/project structure.)
// Version 1.3 - Scott Crowley (u1178178)
//               (Implemented the methods previously defined.)

using System.Collections.Generic;

namespace SpreadsheetUtilities
{

    /// <summary>
    /// (s1,t1) is an ordered pair of strings
    /// t1 depends on s1; s1 must be evaluated before t1
    /// 
    /// A DependencyGraph can be modeled as a set of ordered pairs of strings.  Two ordered pairs
    /// (s1,t1) and (s2,t2) are considered equal if and only if s1 equals s2 and t1 equals t2.
    /// Recall that sets never contain duplicates.  If an attempt is made to add an element to a 
    /// set, and the element is already in the set, the set remains unchanged.
    /// 
    /// Given a DependencyGraph DG:
    /// 
    ///    (1) If s is a string, the set of all strings t such that (s,t) is in DG is called dependents(s).
    ///        (The set of things that depend on s)    
    ///        
    ///    (2) If s is a string, the set of all strings t such that (t,s) is in DG is called dependees(s).
    ///        (The set of things that s depends on) 
    //
    // For example, suppose DG = {("a", "b"), ("a", "c"), ("b", "d"), ("d", "d")}
    //     dependents("a") = {"b", "c"}
    //     dependents("b") = {"d"}
    //     dependents("c") = {}
    //     dependents("d") = {"d"}
    //     dependees("a") = {}
    //     dependees("b") = {"a"}
    //     dependees("c") = {"a"}
    //     dependees("d") = {"b", "d"}
    /// </summary>
    public class DependencyGraph
    {
        // The graph's private members:
        // _dependees is a Collection of key-value pairs where the key is a "node" & its value is a
        // list of all the node's "parents" (i.e. a list of lists of dependees).
        private Dictionary<string, HashSet<string>> _dependees;

        // _dependents is a Collection of key-value pairs where the key is a "node" & its value is a
        // list of all the node's "children" (i.e. a list of lists of dependents). NOTE: These two
        // Collections are redundant; if an ordered pair exists in _dependents, it should also exist
        // in _dependees. Both are maintained to assist traversing edges in either direction.
        private Dictionary<string, HashSet<string>> _dependents;

        // The number of dependent-dependee relationships in the graph.
        private int _size;


        /// <summary>
        /// Creates an empty DependencyGraph.
        /// </summary>
        public DependencyGraph()
        {
            _dependees  = new Dictionary<string, HashSet<string>>();
            _dependents = new Dictionary<string, HashSet<string>>();
            _size = 0;
        }


        /// <summary>
        /// The number of ordered pairs in the DependencyGraph.
        /// </summary>
        public int Size
        {
            get { return _size; }
        }


        /// <summary>
        /// The size of dependees(s) (i.e. the number of elements that s depends upon).
        /// This property is an example of an indexer.  If dg is a DependencyGraph, you would
        /// invoke it like this:
        /// dg["a"]
        /// It should return the size of dependees("a").
        /// </summary>
        public int this[string s]
        {
            get
            {
                HashSet<string> dees;
                if (_dependees.TryGetValue(s, out dees))
                {
                    return dees.Count;
                }
                return 0;
            }
        }


        /// <summary>
        /// Reports whether dependents(s) is non-empty.
        /// </summary>
        public bool HasDependents(string s)
        {
            return _dependents.ContainsKey(s);
        }


        /// <summary>
        /// Reports whether dependees(s) is non-empty.
        /// </summary>
        public bool HasDependees(string s)
        {
            return _dependees.ContainsKey(s);
        }


        /// <summary>
        /// Enumerates dependents(s).
        /// </summary>
        public IEnumerable<string> GetDependents(string s)
        {
            HashSet<string> dents;
            // Check first than the string has any dependents.
            if (_dependents.TryGetValue(s, out dents))
            {
                // Copy the string's list of dependents to an array and return it.
                string[] list_of_dependents = new string[dents.Count];
                dents.CopyTo(list_of_dependents);
                return list_of_dependents;
            }
            // If the string is dependentless, return an empty list.
            string[] empty = new string[0];
            return empty;
                
            /* NOTE: I feel there's a more efficient method to let the caller enumerate the set.
             * The HashSet currently backing this has a GetEnumerator method that should be useful.
             * Not sure how to make this work for now, but copying each string to an array is O(n). */
        }


        /// <summary>
        /// Enumerates dependees(s).
        /// </summary>
        public IEnumerable<string> GetDependees(string s)
        {
            HashSet<string> dees;
            // Check first than the string has any dependees.
            if (_dependees.TryGetValue(s, out dees))
            {
                // Copy the string's list of dependees to an array and return it.
                string[] list_of_dependees = new string[dees.Count];
                dees.CopyTo(list_of_dependees);
                return list_of_dependees;
            }
            // If the string is independent, return an empty list.
            string[] empty = new string[0];
            return empty;
        }


        /// <summary>
        /// <para>Adds the ordered pair (s,t), if it doesn't exist</para>
        /// 
        /// <para>This should be thought of as:</para>   
        /// 
        ///   t depends on s
        ///
        /// </summary>
        /// <param name="s"> s must be evaluated first. T depends on S</param>
        /// <param name="t"> t cannot be evaluated until s is</param>        /// 
        public void AddDependency(string s, string t)
        {
            // A set of s's dependents.
            HashSet<string> sdents;
            // Check first if s has any dependents.
            if (_dependents.TryGetValue(s, out sdents))
            {   
                // If one of s's dependents is t, nothing more to do.
                if (sdents.Contains(t))
                    return;
                // Otherwise, add t to s's dependent list and increment the graph's size.
                sdents.Add(t);
                _size++;
            }
            else
            {   // If s didn't exist in _dependents, it didn't have any assigned yet. Create the
                // list of dependents, add t to it, add the list to _dependents, and increment the
                // graph's size.
                sdents = new HashSet<string>();
                sdents.Add(t);
                _dependents.Add(s, sdents);
                _size++;
            }
            // A set of t's dependees.
            HashSet<string> tdees;
            // Check first if t has any dependees.
            if (_dependees.TryGetValue(t, out tdees))
                // Add s to t's dependees list.
                tdees.Add(s);
            else
            {   // If t didn't exist in _dependees, it was previously independent or empty. Create
                // the list of dependees, add s to it, then add the list to _dependees.
                tdees = new HashSet<string>();
                tdees.Add(s);
                _dependees.Add(t, tdees);
            }
        }


        /// <summary>
        /// Removes the ordered pair (s,t), if it exists
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        public void RemoveDependency(string s, string t)
        {
            // A set of s's dependents.
            HashSet<string> sdents;
            // Check first if s has any dependents.
            if (_dependents.TryGetValue(s, out sdents))
            {
                // If t isn't a dependent of s, there's nothing to remove.
                if (!sdents.Contains(t))
                    return;
                // Otherwise, remove t from s's dependent list and decrement the size of the graph.
                sdents.Remove(t);
                _size--;
                // If s's dependent list is now empty, remove it from _dependents.
                if (sdents.Count < 1)
                    _dependents.Remove(s);
            }
            else
            {   // If s didn't exist in _dependents, there is no ordered pair to remove.
                return;
            }
            // A set of t's dependees.
            HashSet<string> tdees;
            // Check first that t has dependees. It should.
            if (_dependees.TryGetValue(t, out tdees))
            {   
                // Remove s from t's dependee list.
                tdees.Remove(s);
                // If t's dependee list is now empty, remove it from _dependees.
                if (tdees.Count < 1)
                    _dependees.Remove(t);
            }
        }


        /// <summary>
        /// Removes all existing ordered pairs of the form (s,r).  Then, for each
        /// t in newDependents, adds the ordered pair (s,t).
        /// </summary>
        public void ReplaceDependents(string s, IEnumerable<string> newDependents)
        {
            // A set of s's dependents.
            HashSet<string> sdents;
            // Check first if s has any dependents.
            if (_dependents.TryGetValue(s, out sdents))
            {
                IEnumerable<string> oldDependents = GetDependents(s);
                foreach (string r in oldDependents)
                    RemoveDependency(s, r);
            }
            foreach (string t in newDependents)
                AddDependency(s, t);
        }


        /// <summary>
        /// Removes all existing ordered pairs of the form (r,s).  Then, for each 
        /// t in newDependees, adds the ordered pair (t,s).
        /// </summary>
        public void ReplaceDependees(string s, IEnumerable<string> newDependees)
        {
            // A set of s's dependees.
            HashSet<string> sdees;
            // Check first if t has any dependees.
            if (_dependees.TryGetValue(s, out sdees))
            {
                IEnumerable<string> oldDependees = GetDependees(s);
                foreach (string r in oldDependees)
                    RemoveDependency(r, s);
            }
            foreach (string t in newDependees)
                AddDependency(t, s);
        }

    }

}

