#region Copyright (C) 2003 - A X Costa
// 
// Filename: ValueOutOfRangeException.cs
// Project: ThreePhaseSharpLib
//
// Begin: 07th September 2003
// Last Modification: 07th September 2003
//
// All rights reserved.
//
#endregion

using System;

namespace ThreePhaseSharpLib
{
	/// <summary>
	/// Exception class derived from ApplicationException that will throw a ValueOutOfRange Exception
	/// </summary>
	public class ValueOutOfRangeException : System.ApplicationException
	{
		public ValueOutOfRangeException()
			:base()	{}
		public ValueOutOfRangeException(string message)
			:base(message)	{}
		public ValueOutOfRangeException(string message, Exception inner)
			:base(message, inner)	{}
	}
}
