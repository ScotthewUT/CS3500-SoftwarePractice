using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;

namespace TestDependencyGraph
{
    /// <summary>
    ///This is a test class for DependencyGraphTest and is intended
    ///to contain all DependencyGraphTest Unit Tests
    ///</summary>
    [TestClass()]
    public class DependencyGraphTest
    {
        /// <summary>
        ///Empty graph should contain nothing
        ///</summary>
        [TestMethod()]
        public void SimpleEmptyTest()
        {
            DependencyGraph t = new DependencyGraph();
            Assert.AreEqual(0, t.Size);
        }


        /// <summary>
        ///Empty graph should contain nothing
        ///</summary>
        [TestMethod()]
        public void SimpleEmptyRemoveTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("x", "y");
            Assert.AreEqual(1, t.Size);
            t.RemoveDependency("x", "y");
            Assert.AreEqual(0, t.Size);
        }


        /// <summary>
        ///Empty graph should contain nothing
        ///</summary>
        [TestMethod()]
        public void EmptyEnumeratorTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("x", "y");
            IEnumerator<string> e1 = t.GetDependees("y").GetEnumerator();
            Assert.IsTrue(e1.MoveNext());
            Assert.AreEqual("x", e1.Current);
            IEnumerator<string> e2 = t.GetDependents("x").GetEnumerator();
            Assert.IsTrue(e2.MoveNext());
            Assert.AreEqual("y", e2.Current);
            t.RemoveDependency("x", "y");
            Assert.IsFalse(t.GetDependees("y").GetEnumerator().MoveNext());
            Assert.IsFalse(t.GetDependents("x").GetEnumerator().MoveNext());
        }


        /// <summary>
        ///Replace on an empty DG shouldn't fail
        ///</summary>
        [TestMethod()]
        public void SimpleReplaceTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("x", "y");
            Assert.AreEqual(t.Size, 1);
            t.RemoveDependency("x", "y");
            t.ReplaceDependents("x", new HashSet<string>());
            t.ReplaceDependees("y", new HashSet<string>());
        }


        ///<summary>
        ///It should be possibe to have more than one DG at a time.
        ///</summary>
        [TestMethod()]
        public void StaticTest()
        {
            DependencyGraph t1 = new DependencyGraph();
            DependencyGraph t2 = new DependencyGraph();
            t1.AddDependency("x", "y");
            Assert.AreEqual(1, t1.Size);
            Assert.AreEqual(0, t2.Size);
        }


        /// <summary>
        ///Non-empty graph contains something
        ///</summary>
        [TestMethod()]
        public void SizeTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a", "b");
            t.AddDependency("a", "c");
            t.AddDependency("c", "b");
            t.AddDependency("b", "d");
            Assert.AreEqual(4, t.Size);
        }


        /// <summary>
        ///Non-empty graph contains something
        ///</summary>
        [TestMethod()]
        public void EnumeratorTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a", "b");
            t.AddDependency("a", "c");
            t.AddDependency("c", "b");
            t.AddDependency("b", "d");

            IEnumerator<string> e = t.GetDependees("a").GetEnumerator();
            Assert.IsFalse(e.MoveNext());

            e = t.GetDependees("b").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            String s1 = e.Current;
            Assert.IsTrue(e.MoveNext());
            String s2 = e.Current;
            Assert.IsFalse(e.MoveNext());
            Assert.IsTrue(((s1 == "a") && (s2 == "c")) || ((s1 == "c") && (s2 == "a")));

            e = t.GetDependees("c").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("a", e.Current);
            Assert.IsFalse(e.MoveNext());

            e = t.GetDependees("d").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("b", e.Current);
            Assert.IsFalse(e.MoveNext());
        }

        
        /// <summary>
        ///Non-empty graph contains something
        ///</summary>
        [TestMethod()]
        public void ReplaceThenEnumerate()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("x", "b");
            t.AddDependency("a", "z");
            t.ReplaceDependents("b", new HashSet<string>());
            t.AddDependency("y", "b");
            t.ReplaceDependents("a", new HashSet<string>() { "c" });
            t.AddDependency("w", "d");
            t.ReplaceDependees("b", new HashSet<string>() { "a", "c" });
            t.ReplaceDependees("d", new HashSet<string>() { "b" });

            IEnumerator<string> e = t.GetDependees("a").GetEnumerator();
            Assert.IsFalse(e.MoveNext());

            e = t.GetDependees("b").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            String s1 = e.Current;
            Assert.IsTrue(e.MoveNext());
            String s2 = e.Current;
            Assert.IsFalse(e.MoveNext());
            Assert.IsTrue(((s1 == "a") && (s2 == "c")) || ((s1 == "c") && (s2 == "a")));

            e = t.GetDependees("c").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("a", e.Current);
            Assert.IsFalse(e.MoveNext());

            e = t.GetDependees("d").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("b", e.Current);
            Assert.IsFalse(e.MoveNext());
        }


        /// <summary>
        /// Test the dependees count indexer on an empty graph.
        /// </summary>
        [TestMethod()]
        public void CountDependeesEmptyTest()
        {
            DependencyGraph emptyDG = new DependencyGraph();
            Assert.IsTrue(emptyDG["A1"] == 0);

            emptyDG.AddDependency("A1", "B2");
            emptyDG.AddDependency("C3", "D4");
            emptyDG.AddDependency("E5", "F6");
            emptyDG.RemoveDependency("A1", "B2");
            emptyDG.RemoveDependency("C3", "D4");
            emptyDG.RemoveDependency("E5", "F6");

            Assert.IsTrue(emptyDG["B2"] == 0);
            Assert.IsTrue(emptyDG["D4"] == 0);
            Assert.IsTrue(emptyDG["F6"] == 0);
        }


        /// <summary>
        /// Test the dependees count indexer while adding items.
        /// </summary>
        [TestMethod()]
        public void CountDependeesIncTest()
        {
            DependencyGraph testDG = new DependencyGraph();
            string dent = "A0";
            string[] dees = new string[] {"D0", "D1", "D2", "D3", "D4",
                                               "D5", "D6", "D7", "D8", "D9" };
            int count = 0;
            Assert.IsTrue(testDG[dent] == count);
            foreach (string dee in dees)
            {
                testDG.AddDependency(dee, dent);
                count++;
                Assert.IsTrue(testDG[dent] == count);
            }
        }


        /// <summary>
        /// Test the dependees count indexer while removing items.
        /// </summary>
        [TestMethod()]
        public void CountDependeesDecTest()
        {
            DependencyGraph testDG = new DependencyGraph();
            string dent = "A0";
            string[] dees = new string[] {"D0", "D1", "D2", "D3", "D4",
                                               "D5", "D6", "D7", "D8", "D9" };
            int count = dees.Length;
            testDG.ReplaceDependees(dent, dees);
            Assert.IsTrue(testDG[dent] == count);
            foreach (string dee in dees)
            {
                testDG.RemoveDependency(dee, dent);
                count--;
                Assert.IsTrue(testDG[dent] == count);
            }
        }


        /// <summary>
        /// Tests HasDependents method.
        /// </summary>
        [TestMethod()]
        public void HasDependentsTest()
        {
            DependencyGraph testDG = new DependencyGraph();
            string[] dents = new string[] {"T0", "T1", "T2", "T3"};
            testDG.ReplaceDependents("D0", dents);
            testDG.AddDependency("D1", "T4");

            // A node not in the graph has no dependents.
            Assert.IsFalse(testDG.HasDependents("T5"));
            // A node in the graph, but with no dependents, has no dependents.
            Assert.IsFalse(testDG.HasDependents("T0"));
            // A node with several dependents, has dependents.
            Assert.IsTrue(testDG.HasDependents("D0"));
            // A node with a single dependent, has dependents.
            Assert.IsTrue(testDG.HasDependents("D1"));

            testDG.RemoveDependency("D1", "T4");
            // A node that used to have a dependent, has no dependents.  Aww, that's kind of sad. :(
            Assert.IsFalse(testDG.HasDependents("D1"));
        }


        /// <summary>
        /// Tests HasDependees method.
        /// </summary>
        [TestMethod()]
        public void HasDependeesTest()
        {
            DependencyGraph testDG = new DependencyGraph();
            string[] dees = new string[] { "D0", "D1", "D2", "D3" };
            testDG.ReplaceDependees("T0", dees);
            testDG.AddDependency("D4", "T1");

            // A node not in the graph has no dependees.
            Assert.IsFalse(testDG.HasDependees("D5"));
            // A node in the graph, but with no dependees, has no dependees.
            Assert.IsFalse(testDG.HasDependees("D0"));
            // A node with several dependees, has dependees.
            Assert.IsTrue(testDG.HasDependees("T0"));
            // A node with a single dependee, has dependees.
            Assert.IsTrue(testDG.HasDependees("T1"));

            testDG.RemoveDependency("D4", "T1");
            // A node that used to have a dependee, has no dependees.
            Assert.IsFalse(testDG.HasDependees("T1"));
        }


        /// <summary>
        /// Test RemoveDependency when the ordered pair doesn't exist.
        /// </summary>
        [TestMethod()]
        public void RemoveNonDependency()
        {
            DependencyGraph testDG = new DependencyGraph();
            Assert.IsTrue(testDG.Size == 0);
            testDG.AddDependency("D0", "T0");
            Assert.IsTrue(testDG.Size == 1);
            testDG.RemoveDependency("T0", "D0");
            Assert.IsTrue(testDG.Size == 1);
            testDG.RemoveDependency("D1", "T1");
        }

        /// <summary>
        ///Using lots of data
        ///</summary>
        [TestMethod()]
        public void StressTest()
        {
            // Dependency graph
            DependencyGraph t = new DependencyGraph();

            // A bunch of strings to use
            const int SIZE = 200;
            string[] letters = new string[SIZE];
            for (int i = 0; i < SIZE; i++)
            {
                letters[i] = ("" + (char)('a' + i));
            }

            // The correct answers
            HashSet<string>[] dents = new HashSet<string>[SIZE];
            HashSet<string>[] dees = new HashSet<string>[SIZE];
            for (int i = 0; i < SIZE; i++)
            {
                dents[i] = new HashSet<string>();
                dees[i] = new HashSet<string>();
            }

            // Add a bunch of dependencies
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = i + 1; j < SIZE; j++)
                {
                    t.AddDependency(letters[i], letters[j]);
                    dents[i].Add(letters[j]);
                    dees[j].Add(letters[i]);
                }
            }

            // Remove a bunch of dependencies
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = i + 4; j < SIZE; j += 4)
                {
                    t.RemoveDependency(letters[i], letters[j]);
                    dents[i].Remove(letters[j]);
                    dees[j].Remove(letters[i]);
                }
            }

            // Add some back
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = i + 1; j < SIZE; j += 2)
                {
                    t.AddDependency(letters[i], letters[j]);
                    dents[i].Add(letters[j]);
                    dees[j].Add(letters[i]);
                }
            }

            // Remove some more
            for (int i = 0; i < SIZE; i += 2)
            {
                for (int j = i + 3; j < SIZE; j += 3)
                {
                    t.RemoveDependency(letters[i], letters[j]);
                    dents[i].Remove(letters[j]);
                    dees[j].Remove(letters[i]);
                }
            }

            // Make sure everything is right
            for (int i = 0; i < SIZE; i++)
            {
                Assert.IsTrue(dents[i].SetEquals(new HashSet<string>(t.GetDependents(letters[i]))));
                Assert.IsTrue(dees[i].SetEquals(new HashSet<string>(t.GetDependees(letters[i]))));
            }
        }

    }
}
