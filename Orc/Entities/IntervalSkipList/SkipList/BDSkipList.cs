
// BDSkipList - a thread-safe bi-directional skiplist
//
// authored by David W. Jeske (2008-2010)
//
// KNOWN BUGS:
//   Keys.CopyTo([]) unimplemented
//  
// This code is provided without warranty to the public domain. You may use it for any
// purpose without restriction.
//
// originally inspired by the public source code:
// http://www.codersource.net/csharp_skip_list.aspx
//
// wikipedia explanation
// http://en.wikipedia.org/wiki/Skip_list
//
// A skip list is built in layers. The bottom layer is an ordinary ordered linked list. 
// Each higher layer acts as an "express lane" for the lists below, where an element in 
// layer i appears in layer i+1 with some fixed probability p (two commonly-used values 
// for p are 1/2 or 1/4). On average, each element appears in 1/(1-p) lists, and the tallest 
// element (usually a special head element at the front of the skip list) in \log_{1/p} n\, lists.
//
// head[3] S->1------------------------------>S
// head[2] S->1------->4---->6--------------->S
// head[1] S->1---->3->4---->6------->9------>S
// head[0] S->1->2->3->4->5->6->7->8->9->10-->S
//
// NOTE: this is space inefficient for small K,V because we allocate a linked list node 
// for EVERY K,V, plus every skiplist jump point.

// FURTHERMORE, this skiplist is extended to a doubly-linked skiplist, so we can traverse
// efficiently in either direction.

namespace Orc.Entities.IntervalSkipList.SkipList
{
    using System;
    using System.Text;
    using System.Collections.Generic;
    using System.Diagnostics;

    // For some notes about Debugger Visualizers.... 
    // http://msdn.microsoft.com/en-us/magazine/cc163974.aspx
    // http://www.4guysfromrolla.com/articles/011806-1.aspx
    [DebuggerDisplay(" SkipList- {ToString()}")]

    public class BDSkipList<K, V> :
        IScannableDictionary<K,V>
        //where K : IComparable
        where K : IComparable<K>
        // where V: IEquatable<V>
    {
        /* This "Node" class models a node in the Skip List structure. It holds a pair of (key, value)
         * and also three pointers towards Node situated to the right, upwards and downwards. */
        private class Node<K, V> where K : IComparable<K>
        {

            private Node()
            {  // only we are allowed to call this
                this.left = this.right = this.up = this.down = null;
            }
            public Node(K key, V value)
                : this()
            {
                this.key = key;
                this.value = value;
                this.is_sentinel = false;
            }
            internal static Node<K, V> newSentinel()
            {
                Node<K, V> node = new Node<K, V>();
                node.is_sentinel = true;
                return node;
            }

            public K key;
            public V value;
            public bool is_sentinel;
            public Node<K, V> left, right, up, down;
        }

        /* Public constructor of the class. Receives as parameters the maximum number
         * of paralel lists and the probability. */
        public BDSkipList(int maxListsCount, double probability)
        {
            /* Store the maximum number of lists and the probability. */
            this.maxLevelHeight = maxListsCount;
            this.probability = probability;
            this.Clear();
        }

        public IEnumerable<KeyValuePair<K, V>> scanForward(IScanner<K> scanner)
        {
            IComparable<K> lowestKeyTest = null;
            IComparable<K> highestKeyTest = null;
            if (scanner != null)
            {
                lowestKeyTest = scanner.genLowestKeyTest();
                highestKeyTest = scanner.genHighestKeyTest();
            }

            Node<K, V> node = (lowestKeyTest != null) ? this._findNextNode(lowestKeyTest, true) : this.head[0].right;

            while ((node != null) && (!node.is_sentinel) &&
                    ((highestKeyTest == null) || (highestKeyTest.CompareTo(node.key) >= 0)))
            {
                if (scanner == null || scanner.MatchTo(node.key))
                {
                    yield return new KeyValuePair<K, V>(node.key, node.value);
                }
                node = this._findNext(node);
            }
        }

        public IEnumerable<KeyValuePair<K, V>> scanBackward(IScanner<K> scanner)
        {
            IComparable<K> highestKeyTest = null;
            IComparable<K> lowestKeyTest = null;

            if (scanner != null)
            {
                lowestKeyTest = scanner.genLowestKeyTest();
                highestKeyTest = scanner.genHighestKeyTest();
            }

            Node<K, V> node = (highestKeyTest != null) ? this._findPrevNode(highestKeyTest, true) : this.tail[0].left;

            while ((node != null) && (!node.is_sentinel) &&
                    ((lowestKeyTest == null) || (lowestKeyTest.CompareTo(node.key) <= 0)))
            {
                if (scanner == null || scanner.MatchTo(node.key))
                {
                    yield return new KeyValuePair<K, V>(node.key, node.value);
                }
                node = this._findPrev(node);
            }
        }

        /* This is another public constructor. It creates a skip list with 11 aditional lists
         * and with a probability of 0.5. */
        public BDSkipList() : this(11, 0.5) { }

        #region Add_Remove_Clear_Implementations
        // -------------------------[ Add  / Remove / Clear ]--------------------------------

        public void Clear()
        {
            // Initialize the sentinel nodes. We have a list of nodes on both end of the
            // skiplist, so we can start at either end and traverse
            lock (this)
            {
                this.head = new Node<K, V>[this.maxLevelHeight + 1];
                this.tail = new Node<K, V>[this.maxLevelHeight + 1];

                for (int i = 0; i <= this.maxLevelHeight; i++)
                {
                    this.head[i] = Node<K, V>.newSentinel();
                    this.tail[i] = Node<K, V>.newSentinel();
                }

                /* Link the head sentinels of the lists one to another. Also link all head
                 * sentinels to the Singleton tail second sentinel. */
                for (int i = 0; i <= this.maxLevelHeight; i++)
                {
                    this.head[i].right = this.tail[i];
                    this.tail[i].left = this.head[i];
                    if (i > 0)
                    {
                        this.head[i].down = this.head[i - 1];
                        this.head[i - 1].up = this.head[i];

                        this.tail[i].down = this.tail[i - 1];
                        this.tail[i - 1].up = this.tail[i];
                    }
                }

                /* For the beginning we have no additional list, only the bottom list. */
                this.currentListsCount = 0;

                this.nodeCount = 0;
            }
        }

        /* Adds a pair of (key, value) into the Skip List structure. */
        public void Add(K key, V value)
        {
            lock (this)
            {
                /* When inserting a key into the list, we will start from the top list and 
                 * move right until we find a node that has a greater key than the one we want to insert.
                 * At this moment we move down to the next list and then again right. 
                 * 
                 * In this array we store the rightmost node that we reach on each level. We need to
                 * store them because when we will insert the new key in the lists, it will be inserted
                 * after these rightmost nodes. 
                 *
                 * Because we are a BIDIRECTIONAL skip list, we do this once forwards, and once backwards
                 */

                // Forward from head
                Node<K, V>[] next = new Node<K, V>[this.maxLevelHeight + 1];

                /* Now we will parse the skip list structure, from the top list, going right and then down
                 * and then right again and then down again and so on. We use a "cursor" variable that will
                 * represent the current node that we reached in the skip list structure. */
                {
                    Node<K, V> cursor = this.head[this.currentListsCount];
                    for (int i = this.currentListsCount; i >= 0; i--)
                    {
                        /* If we are not at the topmost list, then we move down with one level. */
                        if (i < this.currentListsCount)
                            cursor = cursor.down;

                        /* While we do not reach the tail sentinel and we do not find a greater 
                         * numeric value than the one we want to insert, keep moving right. */
                        while ((!cursor.right.is_sentinel) && (cursor.right.key.CompareTo(key) < 0))
                            cursor = cursor.right;

                        /* Store this rightmost reached node on this level. */
                        next[i] = cursor;
                    }
                }

                // Backward from tail
                Node<K, V>[] prev = new Node<K, V>[this.maxLevelHeight + 1];
                /* Now we will parse the skip list structure, from the top list, going left and then down
                 * and then left again and then down again and so on. We use a "cursor" variable that will
                 * represent the current node that we reached in the skip list structure. */
                {
                    Node<K, V> pcursor = this.tail[this.currentListsCount];
                    for (int i = this.currentListsCount; i >= 0; i--)
                    {
                        /* If we are not at the topmost list, then we move down with one level. */
                        if (i < this.currentListsCount)
                            pcursor = pcursor.down;

                        /* While we do not reach the head sentinel and we do not find a lessor
                         * numeric value than the one we want to insert, keep moving left. */
                        while ((!pcursor.left.is_sentinel) && (pcursor.left.key.CompareTo(key) > 0))
                            pcursor = pcursor.left;

                        /* Store this rightmost reached node on this level. */
                        prev[i] = pcursor;
                    }
                }

                /* Here we are on the bottom list, and we test to see if the new value to add
                 * is not already in the skip list. If it already exists, then we throw the
                 * proper exception. */
                if ((!next[0].right.is_sentinel) && (next[0].right.key.CompareTo(key) == 0))
                {

                    // we used to just set the value
                    // next[0].right.value = value;
                    throw new ArgumentException("BDSkipList collision: Add(" + key.ToString() + "," + value.ToString() +
                                        ") collides with (" + next[0].right.key.ToString() + "," + next[0].right.value.ToString() + ")");
                }

                    /* If the number to insert is not in the list, then we flip the coin and insert
                     * it in the lists. */
                else
                {
                    /* We find a new level number which will tell us in how many lists to add the new number. 
                     * This new random level number is generated by flipping the coin (see below). */
                    int newLevel = this.NewRandomLevel();

                    /* If the new level is greater than the current number of lists, then we extend our
                     * "next/prev nodes" array to include more head/tail pointers. 
                     * At the same time we increase the number of current lists. 
                     */
                    if (newLevel > this.currentListsCount)
                    {
                        for (int i = this.currentListsCount + 1; i <= newLevel; i++)
                        {
                            next[i] = this.head[i];
                            prev[i] = this.tail[i];
                        }
                        this.currentListsCount = newLevel;
                    }

                    /* Now we add the node to the lists and adjust the pointers accordingly. 
                     * We add the node starting from the bottom list and moving up to the next lists.
                     * When we get above the bottom list, we start updating the "up" and "down" pointer of the 
                     * nodes. */

                    // handle updating the right/left links at the same time
                    {
                        Node<K, V> prevNode = null;
                        Node<K, V> n = null;
                        for (int i = 0; i <= newLevel; i++)
                        {
                            prevNode = n;
                            n = new Node<K, V>(key, value);
                            n.right = next[i].right;
                            n.left = prev[i].left;
                            next[i].right = n;
                            prev[i].left = n;
                            if (i > 0)
                            {
                                n.down = prevNode;
                                prevNode.up = n;
                            }
                        }
                    }

                    this.nodeCount++; // increment our count of nods
                }
            }
        }

        /* This method computes a random value, smaller than the maximum admitted lists count. This random
         * value will tell us into how many lists to insert a new key. */
        private int NewRandomLevel()
        {
            int newLevel = 0;
            while ((newLevel < this.maxLevelHeight) && this.FlipCoin())
                newLevel++;
            return newLevel;
        }

        /* This method simulates the flipping of a coin. It returns true, or false, similar to a coint which
         * falls on one face or the other. */
        private bool FlipCoin()
        {
            double d = this.r.NextDouble();
            return (d < this.probability);
        }

        /* This method removes a fully matching key/value combo from the Skip List structure. 
         * If valuetest is null, only the key is considered
         */
        private bool _Remove(IComparable<K> keytest, IEquatable<V> valuetest)
        {
            lock (this)
            {
                /* To remove in this bidirectional skiplist, we find the  
                 * matching node, and then unlink it from both sides.
                 */

                Node<K, V> target = this._findNextNode(keytest, true);

                if (target == null)
                {
                    // nothing to remove
                    return false;
                }

                if (target.down != null)
                {
                    throw new Exception("BDSkipList internal ERROR, _findNextNode() returned node not on the bottom!");
                }

                if (keytest.CompareTo(target.key) != 0)
                {
                    // not equal
                    return false;
                }

                /* If we are doing valuematch, check the value of the node we found*/
                if (valuetest != null)
                {
                    if (!valuetest.Equals(target.value))
                    {
                        // values don't match, so we won't remove
                        return false;
                    }
                }

                // ---------------------------------------
                // REMOVE! - Now remove it from both sides.

                while (target != null)
                {
                    Node<K, V> left = target.left;
                    Node<K, V> right = target.right;
                    left.right = right;
                    right.left = left;

                    target = target.up;
                }

                this.nodeCount--;  // decrement our node counter
                return true;
            }
        }
        #endregion

        #region Internal_Find_Walk_GetEnumerator_Implementations
        // -----------------------[ internal find/walk implementations ] -------------------------


        Node<K, V> _findNextNode(IComparable<K> keytest, bool equal_ok)
        {
            lock (this)
            {
                /* We parse the skip list structure starting from the topmost list, from the head sentinel
                 * node. As long as we have keys smaller than the key we search for, we keep moving right.
                 * When we find a key that is greater or equal that the key we search, then we go down one
                 * level and there we try to go again right. When we reach the bottom list, we stop. */
                Node<K, V> cursor = this.head[this.currentListsCount];
                for (int i = this.currentListsCount; i >= 0; i--)
                {
                    if (i < this.currentListsCount)
                    {
                        cursor = cursor.down;
                    }
                    while ((!cursor.right.is_sentinel) && (keytest != null) && (keytest.CompareTo(cursor.right.key) >= 0))
                    {
                        if (equal_ok && keytest.CompareTo(cursor.right.key) == 0)
                        {
                            break;
                        }
                        cursor = cursor.right;
                    }

                }

                /* Here we are on the bottom list. Now we see if the next node is valid, if it is
                 * we return the node, if not, we return null.
                 */
                if (!cursor.right.is_sentinel)
                {
                    return cursor.right;
                }
                else
                {
                    return null;
                }
            }
        }
        Node<K, V> _findPrevNode(IComparable<K> keytest, bool equal_ok)
        {
            lock (this)
            {
                /* We parse the skip list structure starting from the topmost list, from the tail sentinel
                 * node. As long as we have keys bigger than the key we search for, we keep moving left.
                 * When we find a key that is less or equal to the key we're searching for, then we go down one
                 * level and there we try to go again left. When we reach the bottom list, we stop. */
                Node<K, V> pcursor = this.tail[this.currentListsCount];
                for (int i = this.currentListsCount; i >= 0; i--)
                {
                    if (i < this.currentListsCount)
                    {
                        pcursor = pcursor.down;
                    }
                    while ((!pcursor.left.is_sentinel) && (keytest != null) && (keytest.CompareTo(pcursor.left.key) <= 0))
                    {
                        if (equal_ok && keytest.CompareTo(pcursor.left.key) == 0)
                        {
                            break;
                        }
                        pcursor = pcursor.left;
                    }
                }


                /* Here we are on the bottom list. Now we see if the next node is valid, if it is
                 * we return the node, if not, we return null.
                 */
                if (!pcursor.left.is_sentinel)
                {
                    return pcursor.left;
                }
                else
                {
                    return null;
                }
            }
        }

        Node<K, V> _findNext(Node<K, V> node)
        {
            lock (this)
            {
                // double check that we are in the bottom level "real" linked list
                while ((node != null) && (node.down != null))
                {
                    node = node.down;
                }
                Node<K, V> next_node = node.right;
                // if it is the sentinel value, return null instead
                if (next_node.is_sentinel)
                {
                    return null;
                }
                else
                {
                    return next_node;
                }
            }
        }

        Node<K, V> _findPrev(Node<K, V> node)
        {
            lock (this)
            {
                // double check that we are in the bottom level "real" linked list
                while ((node != null) && (node.down != null))
                {
                    node = node.down;
                }
                Node<K, V> prev_node = node.left;
                // if it is the sentinel value, return null instead
                if (prev_node.is_sentinel)
                {
                    return null;
                }
                else
                {
                    return prev_node;
                }
            }
        }


        Node<K, V> findNode(IComparable<K> keytest)
        {
            Node<K, V> next_node = this._findNextNode(keytest, true);
            if (next_node != null)
            {
                if (keytest.CompareTo(next_node.key) == 0)
                {
                    return next_node;  // node match
                }
            }
            return null;
        }

        public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
        {
            foreach (Node<K, V> cursor in this._internalEnumerator())
            {
                yield return new KeyValuePair<K, V>(cursor.key, cursor.value);
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        private IEnumerable<Node<K, V>> _internalEnumerator()
        {
            Node<K, V> cursor = this.head[0];

            // check that we are on the full-linked-list layer (i.e. bottom)
            if (cursor.down != null)
            {
                throw new Exception("Skiplist head[0] didn't bring us to the bottom, full-linked-list layer");
            }
            // move past the starting sentinel value
            if (cursor.is_sentinel)
            {
                cursor = cursor.right;
            }
            else
            {
                throw new Exception("Invalid head sentinel in SkipList");
            }
            // now iterate across the linked list
            while (!cursor.is_sentinel)
            {
                yield return cursor;

                if (cursor.right == cursor)
                {
                    throw new Exception("Invalid cyclic right pointer in SkipList, node: (" +
                        cursor.key.ToString() + "," + cursor.value.ToString() + ")");
                }
                cursor = cursor.right;

            }
            if (this.tail[0] != cursor)
            {
                throw new Exception("Invalid tail sentinel in SkipList");
            }
        }

        #endregion

        public bool Remove(IComparable<K> keytest)
        {
            return this._Remove(keytest, null);
        }

        /* This method prints the content of the Skip List structure. It can be useful for debugging. */
        override public string ToString()
        {
            int values_included = 0;

            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<K, V> kvp in this)
            {
                if (values_included > 0) { sb.Append(", "); }
                sb.Append(kvp.Key.ToString());
                sb.Append("=>");
                sb.Append(kvp.Value.ToString());

                values_included++;
                if (values_included > 10)
                {
                    sb.Append("...");  // only include 10 values, then elide
                    break;
                }
            }
            return sb.ToString();
        }

        #region SkipList_InstanceData

        /* This array will hold the first sentinel node from each list. */
        private Node<K, V>[] head;

        /* This node will represent the second sentinel for the lists. It is enough
         * to store only one sentinel node and link all lists to it, instead of creating
         * sentinel nodes for each list separately. */
        private Node<K, V>[] tail;

        /* This number represents the maximum number of lists that can be created. 
         * However it is possible that not all lists are created, depending on how
         * the coin flips. In order to optimize the operations we will not create all
         * lists from the beginning, but will create them only as necessary. */
        private int maxLevelHeight;

        /* This number represents the number of currently created lists. It can be smaller
         * than the number of maximum accepted lists. */
        private int currentListsCount;

        /* This number represents the probability of adding a new element to another list above
         * the bottom list. Usually this probability is 0.5 and is equivalent to the probability
         * of flipping a coin (that is why we say that we flip a coin and then decide if we
         * add the element to another list). However it is better to leave this probability
         * easy to change, because in some situations a smaller or a greater probability can
         * be better suited. */
        private double probability;

        private int nodeCount;  // the number of nodes we currently have

        /* This is a random number generator that is used for simulating the flipping of the coin. */
        private Random r = new Random();

        #endregion

        #region SkipList_FindNextFindPrev
        // ------------------------[ FindNext / FindPrev ]---------------------------

        public KeyValuePair<K, V> FindNext(IComparable<K> keytest, bool equal_ok)
        {
            Node<K, V> node = this._findNextNode(keytest, equal_ok);
            if (node != null)
            {
                return new KeyValuePair<K, V>(node.key, node.value);
            }
            else
            {
                throw new KeyNotFoundException(
                  String.Format("FindNext(IComparable<{0}>({1})",
                    typeof(K).ToString(),
                    keytest.ToString()));

            }

        }

        public KeyValuePair<K, V> FindPrev(IComparable<K> keytest, bool equal_ok)
        {
            Node<K, V> node = this._findPrevNode(keytest, false);
            if (node != null)
            {
                return new KeyValuePair<K, V>(node.key, node.value);
            }
            else
            {
                throw new KeyNotFoundException(
                  String.Format("FindNext(IComparable<{0}>({1})",
                    typeof(K).ToString(),
                    keytest.ToString()));

            }

        }

        #endregion


        public V this[K key]
        {
            get
            {
                Node<K, V> node = this.findNode(key);
                if (node != null)
                {
                    return node.value;
                }
                else
                {
                    throw new KeyNotFoundException("key not found: " + key.ToString());
                }
            }
            set
            {
                Node<K, V> node = this.findNode(key);
                if (node != null)
                {
                    node.value = value;
                }
                else
                {
                    this.Add(key, value);
                }

            }
        }


        #region IDictionary_mapping
        // ---------------------[ IDictionary ]-------------------------------
        public bool ContainsKey(K key)
        {
            Node<K, V> node = this.findNode(key);
            return (node != null);
        }
        public bool Remove(K key)
        {
            return this.Remove((IComparable<K>)key);
        }
        public bool TryGetValue(K key, out V value)
        {
            Node<K, V> node = this.findNode(key);
            if (node != null)
            {
                value = node.value;
                return true;
            }
            else
            {
                value = default(V);
                return false;
            }
        }
        public int Count
        {
            get { return this.nodeCount; }
        }

        // ------------ class KeysInterface --------------
        public class KeysInterface : ICollection<K>, IEnumerable<K>
        {
            BDSkipList<K, V> target;
            internal KeysInterface(BDSkipList<K, V> target)
            {
                this.target = target;
            }
            public bool IsReadOnly { get { return true; } }
            public int Count
            {
                get
                {
                    return this.target.Count;
                }
            }

            public bool Contains(K key)
            {
                Node<K, V> node = this.target.findNode(key);
                return (node != null);
            }
            public void CopyTo(K[] keys, int start_index)
            {
                // TODO: implement this!
                throw new Exception("NOT IMPLEMENTED: BDSkipList.KeysInterface.CopyTo()");
            }


            #region ICollection_readonly_stubs
            public void Add(K key)
            {
                throw new Exception("ICollection.Add(..) called on read-only SkipList.KeysInterface");
            }
            public void Clear()
            {
                throw new Exception("ICollection.Clear() called on read-only SkipList.KeysInterface");
            }
            public bool Remove(K key)
            {
                throw new Exception("ICollection.Remove() called on read-only SkipList.KeysInterface");
            }
            #endregion

            public IEnumerator<K> GetEnumerator()
            {
                foreach (KeyValuePair<K, V> kvp in this.target)
                {
                    yield return kvp.Key;
                }
            }
            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }

        }

        public ICollection<K> Keys
        {
            get { return new KeysInterface(this); }
        }
        public ICollection<V> Values
        {
            get
            {
                Dictionary<V, bool> values = new Dictionary<V, bool>();
                foreach (KeyValuePair<K, V> kvp in this)
                {
                    values[kvp.Value] = true;
                }
                return values.Keys;
            }

        }

        public void Add(KeyValuePair<K, V> kvp)
        {
            this.Add(kvp.Key, kvp.Value);
        }
        public bool Contains(KeyValuePair<K, V> kvp)
        {
            V value;
            bool found = this.TryGetValue(kvp.Key, out value);
            if (found && kvp.Value.Equals(value))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public void CopyTo(KeyValuePair<K, V>[] dest_array, int start_index)
        {
            int cur_index = start_index;
            foreach (Node<K, V> skip_node in this._internalEnumerator())
            {
                dest_array[cur_index++] = new KeyValuePair<K, V>(skip_node.key, skip_node.value);
            }
        }
        public bool Remove(KeyValuePair<K, V> matchingItem)
        {
            // TODO is it valid that we are only removing by key
            return this.Remove(matchingItem.Key);
        }
        public bool IsReadOnly { get { return false; } }

        #endregion
    }
}
