#region Copyright (C) 2003 - A X Costa
// 
// Filename: ResourceBase.cs
// Project: ThreePhaseSharpLib
//
// Begin: 27th August 2003
// Last Modification: 28th August 2003
//
// All rights reserved.
//
#endregion

using System;

namespace ThreePhaseSharpLib
{
	/// <summary>
	/// Base abstract class for resources
	/// </summary>
	public abstract class ResourceBase
	{
		/// constant(s)
		// field(s)
		private string name;
		private uint initialValue = 0;
		private uint count;
		private ulong utilisation = 0;

		// event(s)

		// constructor(s)
		public ResourceBase()
		{
			
		}
		// method(s)

		// property(ies)	

		/// <summary>
		/// Name of Resource
		/// </summary>
		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				if (value.ToString().Length > 0)
				{
					name = value;
				}
			}
		}
		/// <summary>
		/// Initial Value for Resource
		/// </summary>
		public uint InitialValue
		{
			get
			{
				return initialValue;
			}
			set
			{
				initialValue = value;
			}
		}
		/// <summary>
		/// A counter for the number of Resource available
		/// </summary>
		public uint Count
		{
			get
			{
				return count;
			}
			set
			{
				count = value;
			}
		}
		/// <summary>
		/// Utilisation throughout the simulation of the resource
		/// </summary>
		public ulong Utilisation
		{
			get
			{
				return utilisation;
			}
			set
			{
				utilisation = value;
			}
		}
	}
}
