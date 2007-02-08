//
// SortedDictionaryTest.cs
//
// Author:
//	Atsushi Enomoto <atsushi@ximian.com>
//
// Copyright (C) 2006 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

#if NET_2_0

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

using NUnit.Framework;

namespace MonoTests.System.Collections.Generic
{
	[TestFixture]
        public class SortedDictionaryTest
	{
		[Test]
		public void CtorNullComparer ()
		{
			SortedDictionary<int,string> sd =
				new SortedDictionary<int,string> ((IComparer<int>) null);
			Assert.AreEqual (Comparer<int>.Default, sd.Comparer);
		}

		[Test]
		[ExpectedException (typeof (ArgumentNullException))]
		public void CtorNullDictionary ()
		{
			new SortedDictionary<int,string> (default (IDictionary<int,string>));
		}

		[Test]
		[ExpectedException (typeof (ArgumentNullException))]
		public void CtorComparerDictionaryNullComparer ()
		{
			new SortedDictionary<int,string> (default (IDictionary<int,string>), null);
		}

		[Test]
		[ExpectedException (typeof (ArgumentNullException))]
		public void CtorComparerDictionaryNullDictionary ()
		{
			new SortedDictionary<int,string> (null, default (IComparer<int>));
		}

		[Test]
		public void CtorDefault ()
		{
			SortedDictionary<int,string> d =
				new SortedDictionary<int,string> ();
			Assert.IsNotNull (d.Comparer);
		}

		[Test]
		public void CtorDictionary ()
		{
			Dictionary<int,string> src = new Dictionary<int,string> ();
			src.Add (0, "Foo");
			src.Add (4, "Bar");
			src.Add (2, "Baz");
			SortedDictionary<int,string> d =
				new SortedDictionary<int,string> (src);
			Assert.AreEqual (3, d.Count, "#1");
			Assert.AreEqual ("Bar", d [4], "#2");
			IDictionaryEnumerator e = d.GetEnumerator ();
			Assert.IsTrue (e.MoveNext (), "#3");
			Assert.AreEqual ("Foo", e.Value, "#4");
			Assert.IsTrue (e.MoveNext (), "#5");
			Assert.AreEqual ("Baz", e.Value, "#6");
			Assert.IsTrue (e.MoveNext (), "#7");
			Assert.AreEqual ("Bar", e.Value, "#8");

			src.Add (3, "Hoge"); // it does not affect.
			Assert.AreEqual (3, d.Count, "#9");
		}

		[Test]
		[ExpectedException (typeof (ArgumentException))]
		public void AddDuplicate ()
		{
			SortedDictionary<int,string> d =
				new SortedDictionary<int,string> ();
			d.Add (0, "A");
			d.Add (0, "A");
		}

		[Test]
		public void AddNullValue ()
		{
			SortedDictionary<int,string> d =
				new SortedDictionary<int,string> ();
			d.Add (0, null);
			d.Add (1, "A");
			d.Add (2, null);
			Assert.IsNull (d [0], "#0");
			Assert.AreEqual ("A", d [1], "#1");
			Assert.IsNull (d [2], "#2");
		}

		[Test]
		[ExpectedException (typeof (ArgumentNullException))]
		public void AddNullKey ()
		{
			SortedDictionary<string,string> d =
				new SortedDictionary<string,string> ();
			d.Add (null, null);
		}

		[Test]
		[ExpectedException (typeof (ArgumentNullException))]
		[Category ("NotWorking")] // see bug #78019
		public void AddNullKeyNullable ()
		{
			SortedDictionary<Nullable<int>,string> d =
				new SortedDictionary<Nullable<int>,string> ();
			d.Add (null, "TEST");
		}

		[Test]
		[ExpectedException (typeof (KeyNotFoundException))]
		public void GetItemNonexistent ()
		{
			SortedDictionary<int,int> d =
				new SortedDictionary<int,int> ();
			Assert.AreEqual (0, d [0]); // does not exist.
		}

		[Test]
		public void SetItemNonexistent ()
		{
			SortedDictionary<int,int> d =
				new SortedDictionary<int,int> ();
			d [0] = 1;
			Assert.AreEqual (1, d.Count);
		}

		[Test]
		public void SetItemExistent ()
		{
			SortedDictionary<int,int> d =
				new SortedDictionary<int,int> ();
			d.Add (0, 0);
			Assert.AreEqual (1, d.Count, "#1");
			d [0] = 1;
			Assert.AreEqual (1, d.Count, "#2");
			Assert.AreEqual (1, d [0], "#3");
		}

		[Test]
		public void GetEnumerator1 ()
		{
			SortedDictionary<int,string> d =
				new SortedDictionary<int,string> ();
			d.Add (1, "A");
			d.Add (3, "B");
			d.Add (2, "C");
			SortedDictionary<int,string>.Enumerator e = d.GetEnumerator ();
			Assert.IsTrue (e.MoveNext (), "#1");
			Assert.AreEqual ("A", e.Current.Value, "#2");
			Assert.IsTrue (e.MoveNext (), "#3");
			Assert.AreEqual ("C", e.Current.Value, "#4");
			Assert.IsTrue (e.MoveNext (), "#5");
			Assert.AreEqual ("B", e.Current.Value, "#6");
			Assert.IsFalse (e.MoveNext (), "#7");
		}

		[Test]
		[ExpectedException (typeof (InvalidOperationException))]
		public void GetEnumerator2 ()
		{
			SortedDictionary<int,string> d =
				new SortedDictionary<int,string> ();
			d.Add (1, "A");
			d.Add (3, "B");
			d.Add (2, "C");
			IEnumerator e = d.GetEnumerator ();
			d.Add (4, "D");
			e.MoveNext ();
		}

		[Test]
		public void CustomComparer ()
		{
			SortedDictionary<int,string> d =
				new SortedDictionary<int,string> (
					ReverseComparer<int>.Instance);

			d.Add (1, "A");
			d.Add (3, "B");
			d.Add (2, "C");
			SortedDictionary<int,string>.Enumerator e = d.GetEnumerator ();
			Assert.IsTrue (e.MoveNext (), "#1");
			Assert.AreEqual ("B", e.Current.Value, "#2");
			Assert.IsTrue (e.MoveNext (), "#3");
			Assert.AreEqual ("C", e.Current.Value, "#4");
			Assert.IsTrue (e.MoveNext (), "#5");
			Assert.AreEqual ("A", e.Current.Value, "#6");
			Assert.IsFalse (e.MoveNext (), "#7");
		}

		[Test]
		public void Remove ()
		{
			SortedDictionary<int,string> d =
				new SortedDictionary<int,string> ();
			Assert.IsFalse (d.Remove (0), "#1");
			d.Add (0, "Foo");
			Assert.IsTrue (d.Remove (0), "#2");
			Assert.IsFalse (d.Remove (0), "#3");
		}

		[Test]
		public void TryGetValue ()
		{
			SortedDictionary<int,string> d =
				new SortedDictionary<int,string> ();
			string s;
			Assert.IsFalse (d.TryGetValue (0, out s), "#1");
			Assert.IsNull (s, "#2");
			d.Add (0, "Test");
			Assert.IsTrue (d.TryGetValue (0, out s), "#3");
			Assert.AreEqual ("Test", s, "#4");
			Assert.IsFalse (d.TryGetValue (1, out s), "#5");
			Assert.IsNull (s, "#6");
		}

		[Test]
		[ExpectedException (typeof (ArgumentNullException))]
		public void IDictionaryAddKeyNull ()
		{
			IDictionary d = new SortedDictionary<string,string> ();
			d.Add (null, "A");
		}

		[Test]
		[ExpectedException (typeof (ArgumentNullException))]
		public void IDictionaryAddKeyNullValueType ()
		{
			IDictionary d = new SortedDictionary<int,string> ();
			d.Add (null, "A");
		}

		[Test]
		public void IDictionaryAddValueNull ()
		{
			IDictionary d = new SortedDictionary<string,string> ();
			// If we simply check "if (value is TValue)" it won't pass.
			d.Add ("A", null);
		}

		[Test]
#if TARGET_JVM
		[Ignore ("NMA")]
#endif		
		[ExpectedException (typeof (ArgumentException))]
		public void IDictionaryAddValueNullValueType ()
		{
			IDictionary d = new SortedDictionary<string,int> ();
			// If we simply allow null it won't result in ArgumentException.
			d.Add ("A", null);
		}

		[Test]
		[ExpectedException (typeof (NotSupportedException))]
		public void KeysICollectionAdd ()
		{
			SortedDictionary<int,string> d = new SortedDictionary<int,string> ();
			d.Add (1, "A");
			ICollection<int> col = d.Keys;
			col.Add (2);
		}

		[Test]
		[ExpectedException (typeof (NotSupportedException))]
		public void KeysICollectionClear ()
		{
			SortedDictionary<int,string> d = new SortedDictionary<int,string> ();
			d.Add (1, "A");
			ICollection<int> col = d.Keys;
			col.Clear ();
		}

		[Test]
		[ExpectedException (typeof (NotSupportedException))]
		public void KeysICollectionRemove ()
		{
			SortedDictionary<int,string> d = new SortedDictionary<int,string> ();
			d.Add (1, "A");
			ICollection<int> col = d.Keys;
			col.Remove (1);
		}

		[Test]
		[ExpectedException (typeof (NotSupportedException))]
		public void ValuesICollectionAdd ()
		{
			SortedDictionary<int,string> d = new SortedDictionary<int,string> ();
			d.Add (1, "A");
			ICollection<string> col = d.Values;
			col.Add ("B");
		}

		[Test]
		[ExpectedException (typeof (NotSupportedException))]
		public void ValuesICollectionClear ()
		{
			SortedDictionary<int,string> d = new SortedDictionary<int,string> ();
			d.Add (1, "A");
			ICollection<string> col = d.Values;
			col.Clear ();
		}

		[Test]
		[ExpectedException (typeof (NotSupportedException))]
		public void ValuesICollectionRemove ()
		{
			SortedDictionary<int,string> d = new SortedDictionary<int,string> ();
			d.Add (1, "A");
			ICollection<string> col = d.Values;
			col.Remove ("A");
		}

		[Test]
		public void KeysGetEnumerator1 ()
		{
			SortedDictionary<int,string> d =
				new SortedDictionary<int,string> ();
			d.Add (1, "A");
			d.Add (3, "B");
			d.Add (2, "C");
			IEnumerator e = d.Keys.GetEnumerator ();
			Assert.IsTrue (e.MoveNext (), "#1");
			Assert.AreEqual (1, e.Current, "#2");
			Assert.IsTrue (e.MoveNext (), "#3");
			Assert.AreEqual (2, e.Current, "#4");
			Assert.IsTrue (e.MoveNext (), "#5");
			Assert.AreEqual (3, e.Current, "#6");
			Assert.IsFalse (e.MoveNext (), "#7");
		}

		[Test]
		[ExpectedException (typeof (InvalidOperationException))]
		public void KeysGetEnumerator2 ()
		{
			SortedDictionary<int,string> d =
				new SortedDictionary<int,string> ();
			d.Add (1, "A");
			d.Add (3, "B");
			d.Add (2, "C");
			IEnumerator e = d.Keys.GetEnumerator ();
			d.Add (4, "D");
			e.MoveNext ();
		}

		[Test]
		public void ValuesGetEnumerator1 ()
		{
			SortedDictionary<int,string> d =
				new SortedDictionary<int,string> ();
			d.Add (1, "A");
			d.Add (3, "B");
			d.Add (2, "C");
			IEnumerator e = d.Values.GetEnumerator ();
			Assert.IsTrue (e.MoveNext (), "#1");
			Assert.AreEqual ("A", e.Current, "#2");
			Assert.IsTrue (e.MoveNext (), "#3");
			Assert.AreEqual ("C", e.Current, "#4");
			Assert.IsTrue (e.MoveNext (), "#5");
			Assert.AreEqual ("B", e.Current, "#6");
			Assert.IsFalse (e.MoveNext (), "#7");
		}

		[Test]
		[ExpectedException (typeof (InvalidOperationException))]
		public void ValuesGetEnumerator2 ()
		{
			SortedDictionary<int,string> d =
				new SortedDictionary<int,string> ();
			d.Add (1, "A");
			d.Add (3, "B");
			d.Add (2, "C");
			IEnumerator e = d.Values.GetEnumerator ();
			d.Add (4, "D");
			e.MoveNext ();
		}
	}

	class ReverseComparer<T> : IComparer<T>
	{
		static ReverseComparer<T> instance = new ReverseComparer<T> ();
		public static ReverseComparer<T> Instance {
			get { return instance; }
		}

		ReverseComparer ()
		{
		}

		public int Compare (T t1, T t2)
		{
			return Comparer<T>.Default.Compare (t2, t1);
		}
	}
}

#endif
