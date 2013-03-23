#region Copyright (C) 2003 - A X Costa
// 
// Filename: CalendarEntry.cs
// Project: ThreePhaseSharpLib
//
// Begin: 15th August 2003
// Last Modification: 28th August 2003
//
// All rights reserved.
//
#endregion

using System;

namespace ThreePhaseSharpLib
{
	/// <summary>
	/// An entry in the Calendar class
	/// </summary>
	internal class CalendarEntry
	{
		// constant(s)

		// field(s)
		private EntityBase entity;
		private uint timeCell = 0;
		private BEvent nextB;

		// event(s)

		// constructor(s)
		internal CalendarEntry()
		{
			
		}
		internal CalendarEntry (ref EntityBase newEntity, BEvent newNextB, uint newNextTime)
		{
			entity = newEntity;
			timeCell = newNextTime;
			nextB = newNextB;
		}
		// method(s)

		// property(ies)	

		/// <summary>
		/// Entity (reference to an Entity object)
		/// </summary>
		internal EntityBase Entity
		{
			get
			{
				return entity;
			}
			set
			{
				entity = value;
			}
		}
		/// <summary>
		/// Time of Entity change of state
		/// </summary>
		public uint TimeCell
		{
			get
			{
				return timeCell;
			}
			set
			{
				timeCell = value;
			}
		}
		/// <summary>
		/// sets/returns a B Event to be executed
		/// </summary>
		internal BEvent NextB
		{
			get
			{
				return nextB;
			}
			set
			{
				nextB = value;
			}
		}
	}
}
