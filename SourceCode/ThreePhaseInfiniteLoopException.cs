#region Copyright (C) 2003 - A X Costa
// 
// Filename: ThreePhaseInfiniteLoopException.cs
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
	/// Exception class derived from ApplicationException that will throw a ThreePhaseInfiniteLoopException Exception
	/// </summary>
	public class ThreePhaseInfiniteLoopException : System.ApplicationException
	{
		public ThreePhaseInfiniteLoopException()
			:base()	{}
		public ThreePhaseInfiniteLoopException(string message)
			:base(message)	{}
		public ThreePhaseInfiniteLoopException(string message, Exception inner)
			:base(message, inner)	{}
	}
}
