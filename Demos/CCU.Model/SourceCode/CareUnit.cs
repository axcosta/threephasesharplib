#region Copyright (C) 2003 - A X Costa
// 
// Filename: CareUnit.cs
// Project: ThreePhaseSharpLib.Demos.CCUModel
//
// Begin: 09th September 2003
// Last Modification: 13th September 2003
//
// All rights reserved.
//
#endregion

using System;

namespace ThreePhaseSharpLib.Demos.CCUModel
{
	/// <summary>
	/// A Care Unit class derived from the ResourceBase in the library
	/// </summary>
	public class CareUnit : ThreePhaseSharpLib.ResourceBase
	{
		// constant(s)
		// field(s)
		private uint numberOfEmergencyOnlyBeds;
		// event(s)
		// constructor(s)
		public CareUnit(string name)
		{
			this.Name = name;
		}		
		// method(s)
		// property(ies)
		/// <summary>
		/// Number of Beds reserved for Emergency Patients.
		/// </summary>
		public uint NumberOfEmergencyOnlyBeds
		{
			get
			{
				return numberOfEmergencyOnlyBeds;
			}
			set
			{
				numberOfEmergencyOnlyBeds = value;
			}
		}
	}
}
