﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Linq;
using System.Timers;

//ref link:https://www.youtube.com/watch?v=F62XC5SOLmI&list=PLRwVmtr-pp07QlmssL4igw1rnrttJXerL&index=30
//ctrl+shift+space --- check target details 
// list -- are dynamic, can grow and shrink
// list -- manage array underneath
// all link function rely on IEnumerator
// IEnumerable -- the container sequence just like LINQ while IEnumerator --- can walk through the sequence of both linq and IEnumrable
// Indexer -- knowledge in operator overloading
// Insert Range --- insert many items at time
// Coverage Testing - testing every lines of codes

//static class noneusinglinqcountmethod
//{
//    public static int Count<T>(this IEnumerable<T> collection)
//    {
//        int ret = 0;
//        foreach (T item in collection)
//            ret++;
//        return ret;
//    }
//}

class MeList<T> : IEnumerable<T>
{
    //T[] items = new T[5];
    T[] items;
    //int count;
    public MeList(int capacity = 5)
    {
        items = new T[capacity];
    }
    public void Add(T item)
    {
        EnsureCapacity();   // MS Implementation method
        if (Count == items.Length)
            Array.Resize(ref items, items.Length * 2);  // resize the underlying containers --- add slots by x2 of previous slot
        items[Count++] = item;
    }

    public IEnumerator<T> GetEnumerator()
    {
        for (int i = 0; i < Count; i++)
            yield return items[i];      // requires yield return knowledge
        //return new MeEnumerator(this);
    }

    IEnumerator System.Collections.IEnumerable.GetEnumerator() { return GetEnumerator(); }
    public T this[int index]    //indexer -- looks like property
    //public T this[int index, string blah, char c]
    {
        get
        {
            //if (index >= count || index < 0)              highlight+ctrl+. + extract method 
            //    throw new IndexOutOfRangeException();
            CheckBoundaries(index);
            return items[index];
        }
        set
        {
            //if (index >= count || index < 0)
            //    throw new IndexOutOfRangeException();
            CheckBoundaries(index);
            items[index] = value;
        }
    }

    void CheckBoundaries(int index)
    {
        if (index >= Count || index < 0)
            throw new IndexOutOfRangeException();
    }
    public int Capacity
    {
        get { return items.Length; }
    }
    //public int Count { get { return count; } }
    public int Count { get; private set; }
    public void Clear()     // for data waste not cleaned
    {
        /*      Optimization
        Count = 0;
        if (typeof(T).BaseType.Equals(typeof(ValueType)))   // no worry garbage collection
            return;*/

        // nullifying all items(value/reference) types
        for (int i = 0; i < Count; i++)
            items[i] = default(T);
        Count = 0;
    }
    public void TrimExcess()    // for MeList<int>
    {
        T[] newArray = new T[Count];
        Array.Copy(items, newArray, Count);
        items = newArray;
    }
    public void Insert(int index, T item)
    {
        // resize the underlying containers --- add slots by x2 of previous slot
        //if (Count == items.Length)
        //    Array.Resize(ref items, items.Length * 2);
        EnsureCapacity();   // for dupplication of code

        // Shuffle everyone down the existing array
        //Array.Copy(sourceArray, sourceIndex, destinationArray, destinationIndex, length);
        Array.Copy(items, index, items, index + 1, Count - index);
        items[index] = item;    // output : 32, 83, 25, 99, 42, 31
        Count++;

    }
    public void InsertRange(int index, IEnumerable<T> newItems)
    {
        T[] newItemsAsArray = newItems.ToArray();   // 4xfaster - trade memory for speed
        //EnsureCapacity(Count + newItemsAsArray.Length); // faster optimization 
        //int newItemsCount = newItems.Count();   // for optimization
        //EnsureCapacity(Count + newItems.Count());
        //EnsureCapacity(Count + newItemsCount);  // 1st copy

        if (Count + newItemsAsArray.Length > Capacity) // premature optimization 
        {
            T[] newUnderlyingArray = new T[Count = newItemsAsArray.Length];
            Array.Copy(items, newUnderlyingArray, index);
            Array.Copy(items, index, newUnderlyingArray, index + newItemsAsArray.Length, Count - index);
            items = newUnderlyingArray; // remove the old array
        }
        else
        {
            //Shuffle
            Array.Copy(items, index, items, index + newItemsAsArray.Length, Count - index);
        }
        // Shuffle
        //Array.Copy(sourceArray, sourceIndex, destinationArray, destinationIndex, length);
        //Array.Copy(items, index, items, index + newItems.Count(), Count - index);
        //Array.Copy(items, index, items, index + newItemsCount, Count - index);  // 2nd copy
        Array.Copy(newItemsAsArray, 0, items, index, newItemsAsArray.Length);
        //foreach (T newItem in newItems)
        //    items[index++] = newItem;
        //Count += newItems.Count();
        Count += newItemsAsArray.Length;
    }
    public MeList<T> GetRange(int index, int amount)
    {
        MeList<T> ret = new MeList<T>(amount);
        Array.Copy(items, index, ret.items, 0, amount);
        ret.Count = amount;
        return ret;
    }
    public void RemoveAt(int index)     // ideally must require BOUNCE CHECKING
    {
        Array.Copy(items, index + 1, items, index, Count - (index + 1));
        Count--;
    }
    void EnsureCapacity()
    {
        EnsureCapacity(Count + 1);
    }
    //void EnsureCapacity()
    void EnsureCapacity(int neededCapacity)
    //void EnsureCapacity(int neededCapacity = Count + 1) // error: Runtime value(int neededCapacity = Count + 1) must be default args implementation and constant compile time value(int neededCapacity = 8)
    {
        //if (Count == items.Length)
        //    Array.Resize(ref items, items.Length * 2);
        if (neededCapacity > Capacity)
        {
            int targetSize = items.Length * 2;
            if (targetSize < neededCapacity)
                targetSize = neededCapacity;
            Array.Resize(ref items, targetSize);
            //Console.WriteLine("Resizing...");
        }
    }

}
class MainClass
{
    static void Main()
    {
        // Remove, RemoveAll
        List<int> myPartyAges = new List<int> { 35, 39, 42, 88, 42, 99 };
        myPartyAges.Remove(42);     // 35,39,88,42,99   -- only removes the 1st 
        foreach(int i in myPartyAges)
            Console.WriteLine(i);   





        // RemoveAt, RemoveRange, Remove, RemoveAll
        //List<int> myPartyAges = new List<int> { 35, 39, 42, 88, 99 };   // 8 - Default Capacity representation of List 
        //MeList<int> myPartyAges = new MeList<int> { 35, 39, 42, 88, 99 };
        //Console.WriteLine(myPartyAges.Count);
        //Console.WriteLine(myPartyAges.Capacity);

        //foreach (int i in myPartyAges)
        //    Console.Write(i + " ");
        //myPartyAges.RemoveAt(1);    // do shuffles 
        //Console.WriteLine();
        //foreach (int i in myPartyAges)
        //    Console.Write(i + " ");




        // Insert, InsertRange, GetRange

        // GetRange
        //MeList<int> myPartyAges = new MeList<int> { 35, 39, 42, 88, 41, 31 };
        //MeList<int> theRange = myPartyAges.GetRange(2, 3);
        //List<int> myPartyAges = new List<int> { 35, 39, 42, 88, 41, 31 };
        //List<int> theRange = myPartyAges.GetRange(2, 3);    // 42, 88, 41
        //foreach (int i in theRange)
        //    Console.WriteLine(i);



        //Stopwatch timer = new Stopwatch();
        //int[] aBunchOfItems = Enumerable.Range(0, 10000000).ToArray();
        //IEnumerable<int> aBunchOfItems = Enumerable.Range(0, 100000); // Enumerable.Range--yield Knowledge
        //List<int> aBunchOfItems = Enumerable.Range(0, 10000000).ToList();

        // None Built-in List
        //MeList<int> myPartyAges = new MeList<int>();
        //myPartyAges.AddRange(aBunchOfItems);       // missing reference for vs2022-pending      
        //timer.Restart();
        //myPartyAges.InsertRange(5, aBunchOfItems);
        //myPartyAges.InsertRange(5, myPartyAges);
        //timer.Stop();
        //Console.WriteLine("MeList:\t" + (timer.ElapsedTicks / (float)Stopwatch.Frequency));

        // Built -in Microsoft List
        //List<int> myPartyAges2 = new List<int>();
        //myPartyAges2.AddRange(aBunchOfItems);
        //timer.Restart();
        //myPartyAges2.InsertRange(5, aBunchOfItems);     // slower builtin  -- cause of built in code for IEnumerable-- reflector video time-- 9:50
        //myPartyAges2.InsertRange(5, myPartyAges2);    // faster builtin
        //timer.Stop();
        //Console.WriteLine("List:\t" + (timer.ElapsedTicks / (float)Stopwatch.Frequency));



        //foreach(int i in myPartyAges)
        //    Console.WriteLine(i + " ");
        //myPartyAges.InsertRange(5, aBunchOfItems);
        //Console.WriteLine(myPartyAges.Count);
        //foreach (int i in myPartyAges)
        //   Console.WriteLine(i + " ");


        // Remove, RemoveAll, RemoveAt, RemoveRange
        // Contains, Exist, Find, FindAll, FindIndex, FindIndex, FindLast, FindLastIndex, LastIndexOf, index
        // TrueForAll, ForEach, GetEnumerator
        // BinarySearch, Sort, Reverse
        // ConvertAll
        // CopyTo, ToArray
        // AsReadOnly

        //List<int> myPartyAges = new List<int>(5) { 32, 83, 99, 42, 31 }; // Built-In implementation
        //MeList<int> myPartyAges = new MeList<int>(9) { 32, 83, 99, 42, 31 };
        //MeList<int> myPartyAges = new MeList<int>(5) { 32, 83, 99, 42, 31 }; // error: initial capacity (5) not enough for insert .... requires resize (EnsureCapacity Method)
        //MeList<int> myPartyAges = new MeList<int>(4) { 32, 83, 99, 42 };
        //myPartyAges.Insert(2, 25);
        //myPartyAges.InsertRange(2, new[] { 55, 65, 75 });   // Built-In implementation
        //myPartyAges.InsertRange(2, new[] { 55, 65, 75 }); 
        //foreach (int age in myPartyAges)

        //Console.WriteLine(age);



        // Capacity, Count, TrimExcess, Clear
        //List<int> myPartyAges = new List<int>() { 25, 34, 32 };
        //List<int> myPartyAges = new List<int>(10) { 25, 34, 32 };
        //Console.WriteLine(myPartyAges.Count);
        //Console.WriteLine(myPartyAges.Capacity);
        //myPartyAges.Add(99);
        //myPartyAges.Add(101);
        //Console.WriteLine(myPartyAges.Count);
        //Console.WriteLine(myPartyAges.Capacity);
        //List<int> myPartyAges = new List<int>() { };
        //List<int> myPartyAges = new List<int>(6000) { };
        //MeList<int> myPartyAges = new MeList<int>(6000) { };
        //MeList<int> myPartyAges = new MeList<int>() { };
        //int currentCapacity = myPartyAges.Capacity;
        //Console.WriteLine(currentCapacity);
        //for (int i = 0; i < 500; i++)
        //{
        //    myPartyAges.Add(i);
        //if(currentCapacity != myPartyAges.Capacity)
        //{
        //    Console.WriteLine("Resized to " + myPartyAges.Capacity);
        //    currentCapacity = myPartyAges.Capacity;
        //}
        //}
        //Console.WriteLine(myPartyAges.Capacity);
        //myPartyAges.TrimExcess();   // remove excess byte(int)
        //Console.WriteLine(myPartyAges.Capacity);
        //myPartyAges.Clear();    // remove all items on the array
        //Console.WriteLine(myPartyAges.Capacity);
    }

}
