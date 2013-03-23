#region Copyright (C) 2003 - A X Costa
// 
// Filename: PatientGroup.cs
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
	/// Patient Group class derived from the EntityBase in the simulation library
	/// </summary>
	public class PatientGroup : ThreePhaseSharpLib.EntityBase
	{
		// constant(s)
		// field(s)
		private System.Random randomNumberGenerator = new System.Random();
		private uint populationByYear;
		private float losLognormalMean;
		private float losLognormalStandardDeviation;
		private uint losMinimum;
		private uint losMaximum;
		private uint numberOfArrivals;
		private uint numberOfPatientsInQueue;
		private uint numberOfPatientsDeferred;
		// event(s)
		// constructor(s)
		public PatientGroup(string name)
		{
			this.Name = name;
		}
		//method(s)
		/// <summary>
		/// This method will return the LOS (length of stay), in days, of a patient sampled from the Lognormal distribution
		/// </summary>
		public uint PatientLOS()
		{
			uint patientLOSInDays;
			uint patientLOSInHours;
			do
			{
				patientLOSInDays = (uint)(this.LogNormalDev (losLognormalMean, losLognormalStandardDeviation));
			} while ((patientLOSInDays < losMinimum) || (patientLOSInDays > losMaximum));
			patientLOSInHours = patientLOSInDays * 24;
			return (patientLOSInHours);
		}
		/// <summary>
		/// This method will return the next arrival (in days) for a patient in this group
		/// </summary>
		public uint NextArrival()
		{
			double arrivalRate;
			double exponentialResult;
			uint nextArrivalTime;

			
			if (populationByYear > 0)
			{
				arrivalRate =  (365.0 * 24.0) / (double) populationByYear;
			}
			else
			{
				arrivalRate = 100000;
			}
			exponentialResult = Exponential (arrivalRate);
			nextArrivalTime = (uint) (Math.Round (exponentialResult, 0));
			if (nextArrivalTime == 0)
			{
				nextArrivalTime = 1;
			}
			return (nextArrivalTime);
		}
		/// <summary>
		/// Generates a random number from the Exponential distribution
		/// </summary>
		/// <param name="mean">Mean</param>
		public double Exponential(double mean)
		{
			double dummy;
			do
			{
				dummy = randomNumberGenerator.NextDouble();
			} while (dummy == 0.0);

			return (- mean * Math.Log(dummy));
		}
		/// <summary>
		/// Generates a random number from the Normal distribution
		/// </summary>
		/// <param name="mean">Mean</param>
		/// <param name="sigma">Sigma</param>
		private double NormalDev(double mean, double sigma)
		{
			double fac, rsq, v1, v2;
			do 
			{
				v1 = 2.0 * randomNumberGenerator.NextDouble() - 1.0;
				v2 = 2.0 * randomNumberGenerator.NextDouble() - 1.0;
				rsq = v1* v1 + v2 * v2;
			} while ((rsq >= 1.0) || (rsq == 0.0));
			fac = Math.Sqrt(-2.0 * Math.Log(rsq)/rsq);

			//Box-Muller transformation
			return ((v1 * fac * sigma) + mean);
		}

		/// <summary>
		/// Generates a random number from the LogNormal distribution with
		/// Mean and StDev values greater than 0
		/// </summary>
		/// <param name="mean">Mean</param>
		/// <param name="stDev">Standard Deviation</param>
		private double LogNormalDev(double mean, double stDev)
		{
			double mu, sd, sdSq;
			if ((mean <= 0) || (stDev <= 0))
				throw (new System.ApplicationException ("[Log Normal Distribution]\n\nINVALID PARAMETER!\nThe Mean and StDev values must be greater than 0."));
			else 
			{
				mu = Math.Log ((mean * mean) / Math.Sqrt ((stDev * stDev) + (mean * mean)));
				sdSq = Math.Log (((stDev * stDev) + (mean * mean)) / (mean * mean));
				sd = Math.Sqrt (sdSq);
				return Math.Exp (this.NormalDev(mu,sd));
			}
		}
		// property(ies)
		/// <summary>
		/// Population of this group in a year
		/// </summary>
		public uint PopulationByYear
		{
			get
			{
				return populationByYear;
			}
			set
			{
				populationByYear = value;
			}
		}
		/// <summary>
		/// The first parameter of the Lognormal (Mean)
		/// </summary>
		public float LOSLognormalMean
		{
			get
			{
				return losLognormalMean;
			}
			set
			{
				if (value > 0) 
				{
					losLognormalMean = value;
				}
				else
				{
					//throw an exception or send a message.
				}
			}
		}
		/// <summary>
		/// The second parameter of the Lognormal distribution (Standard Deviation)
		/// </summary>
		public float LOSLognormalStandardDeviation
		{
			get
			{
				return losLognormalStandardDeviation;
			}
			set
			{
				if (value > 0) 
				{
					losLognormalStandardDeviation = value;
				}
				else
				{
					//throw an exception or send a message.
				}
			}
		}
		/// <summary>
		/// The minimum acceptable LOS (length of stay)
		/// </summary>
		public uint LOSMinimum
		{
			get
			{
				return losMinimum;
			}
			set
			{
				losMinimum = value;
			}
		}
		/// <summary>
		/// The maximum acceptable LOS (length of stay)
		/// </summary>
		public uint LOSMaximum
		{
			get
			{
				return losMaximum;
			}
			set
			{
				losMaximum = value;
			}
		}
		/// <summary>
		/// The number of patient arrivals for this group
		/// </summary>
		public uint NumberOfArrivals
		{
			get
			{
				return numberOfArrivals;
			}
			set
			{
				numberOfArrivals = value;
			}
		}
		/// <summary>
		/// The number of patient deferrals for this group
		/// </summary>
		public uint NumberOfPatientsDeferred
		{
			get
			{
				return numberOfPatientsDeferred;
			}
			set
			{
				numberOfPatientsDeferred = value;
			}
		}
		/// <summary>
		/// The number of patients in queue of this group
		/// </summary>
		public uint NumberOfPatientsInQueue
		{
			get
			{
				return numberOfPatientsInQueue;
			}
			set
			{
				numberOfPatientsInQueue = value;
			}
		}
	}
}
