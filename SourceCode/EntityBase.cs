#region Copyright (C) 2003 - A X Costa
// 
// Filename: EntityBase.cs
// Project: ThreePhaseSharpLib
//
// Begin: 27th August 2003
// Last Modification: 14th September 2003
//
// All rights reserved.
//
#endregion

using System;

namespace ThreePhaseSharpLib
{
	/// <summary>
	/// Base abstract class for entities
	/// </summary>
	public abstract class EntityBase
	{
		// constant(s)

		// field(s)
		private string name = "untitled";
		private bool available = false;
		//private uint timeCell = 0;
		private ulong utilisation = 0;

		// event(s)

		// constructor(s)
		public EntityBase()
		{
		}
		// method(s)

		// property(ies)	

		/// <summary>
		/// Name of Entity
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
		/// If Entity is Available for Scheduling
		/// </summary>
		public bool Available
		{
			get
			{
				return available;
			}
			set
			{
				available = value;
			}
		}
		/// <summary>
		/// Utilisation throughout the simulation of the entity
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
