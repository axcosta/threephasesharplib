#region Copyright (C) 2003 - A X Costa
// 
// Filename: ConfigurationCannotBeChangedException.cs
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
	/// Exception class derived from ApplicationException that will throw a ConfigurationCannotBeChangedException Exception
	/// </summary>
	public class ConfigurationCannotBeChangedException : System.ApplicationException
	{
		public ConfigurationCannotBeChangedException()
			:base()	{}
		public ConfigurationCannotBeChangedException(string message)
			:base(message)	{}
		public ConfigurationCannotBeChangedException(string message, Exception inner)
			:base(message, inner)	{}
	}
}
