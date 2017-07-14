#region Copyright (C) 2003 - A X Costa
// 
// Filename: Model.cs
// Project: ThreePhaseSharpLib.Demos.CCUModel
//
// Begin: 09th September 2003
// Last Modification: 15th September 2003
//
// All rights reserved.
//
#endregion

using System;
using System.Diagnostics;
using static ThreePhaseSharpLib.SimulationController;
using static ThreePhaseSharpLib.SimulationEventAndActivityManager;

namespace ThreePhaseSharpLib.Demos.CCUModel
{
    /// <summary>
    /// This is a demonstration model for the three-phase simulation library.
    /// </summary>
    public class Model
	{
        // constant(s)
        // field(s)     
        public SimulationController theSimulation = new SimulationController();
        public EntityBase emergencyArrivalMachine = new PatientGroup("Emergency Arrival Machine");
		public EntityBase electiveArrivalMachine = new PatientGroup("Elective Arrival Machine");
		public ResourceBase beds = new CareUnit("CCU Beds");
		public Event emergencyArrive = new Event (EmergencyPatientArrive);
		public Event electiveArrive = new Event (ElectivePatientArrive);
		public Event emergencyEndOccupyBed = new Event (EmergencyEndOccupyBed);
		public Event electiveEndOccupyBed = new Event (ElectiveEndOccupyBed);
		public Activity emergencyBeginOccupyBed = new Activity (EmergencyBeginOccupyBed);
		public Activity electiveBeginOccupyBed = new Activity (ElectiveBeginOccupyBed);
		public EntityBase emergencyPatient = new Patient();

		private uint totalEmergencyArrivals;
		private uint totalElectiveArrivals;
		private uint totalEmergencyTransfers;
		private uint totalElectiveDeferrals;
		private ulong totalBedsUtilisation;

        private uint NumberOfRuns;
        private uint RunDuration;

        // Initialize the trace source.
        private static readonly TraceSource trace = new TraceSource("CCUModel");

        // event(s)
        // constructor(s)
        // method(s)        

        //method that is invoked whenever the OnStartSimulation event occur
        public void SimulationStart(object theSimulationObject, SimulationInfoEventArgs si)
		{
			totalEmergencyArrivals = 0;
			totalElectiveArrivals = 0;
			totalEmergencyTransfers = 0;
			totalElectiveDeferrals = 0;
			totalBedsUtilisation = 0;
		}
		//method that is invoked whenever the OnStartRun event occur
		public void RunStart(object theSimulationObject, SimulationInfoEventArgs si)
		{
			//defining model temporary variables
			CareUnit icuCareUnit;
			icuCareUnit = (CareUnit) beds;
			PatientGroup electivePatientGroup;
			electivePatientGroup = (PatientGroup) electiveArrivalMachine;
			PatientGroup emergencyPatientGroup;
			emergencyPatientGroup = (PatientGroup) emergencyArrivalMachine;
			// reinitialising model variables
			theSimulation.Technique.Schedule(ref emergencyArrivalMachine, emergencyArrive,  0);
			theSimulation.Technique.Schedule(ref electiveArrivalMachine, electiveArrive,  0);
			emergencyPatientGroup.NumberOfArrivals = 0;
			emergencyPatientGroup.NumberOfPatientsDeferred = 0;
			emergencyPatientGroup.NumberOfPatientsInQueue = 0;
			electivePatientGroup.NumberOfArrivals = 0;
			electivePatientGroup.NumberOfPatientsDeferred = 0;
			electivePatientGroup.NumberOfPatientsInQueue = 0;
			icuCareUnit.Count = icuCareUnit.InitialValue;
			icuCareUnit.Utilisation = 0;
		}
		//method that is invoked whenever the OnFinishRun event occur
		public void RunFinished(object theSimulationObject, SimulationInfoEventArgs si)
		{
			//defining model temporary variables
			CareUnit icuCareUnit;
			icuCareUnit = (CareUnit) beds;
			PatientGroup electivePatientGroup;
			electivePatientGroup = (PatientGroup) electiveArrivalMachine;
			PatientGroup emergencyPatientGroup;
			emergencyPatientGroup = (PatientGroup) emergencyArrivalMachine;
			float bedOccupancyRate;
			//results for run
			bedOccupancyRate = icuCareUnit.Utilisation;
			bedOccupancyRate = bedOccupancyRate / (RunDuration * icuCareUnit.InitialValue);
			if (bedOccupancyRate > 1)
			{
				bedOccupancyRate = 1;
			}
			Console.WriteLine ("-------------------------------------------------------");
			Console.WriteLine ("Results for Run {0} of {1}", si.currentRun, NumberOfRuns);
			Console.WriteLine ("-------------------------------------------------------");
			Console.WriteLine ("Number of Emergency Arrivals: \t\t{0}", emergencyPatientGroup.NumberOfArrivals);
			Console.WriteLine ("Number of Elective Arrivals: \t\t{0}", electivePatientGroup.NumberOfArrivals);
			Console.WriteLine ("Number of (Emergency) Transfers: \t{0}", 
				emergencyPatientGroup.NumberOfPatientsDeferred);
			Console.WriteLine ("Number of (Elective) Deferrals: \t{0}", 
				electivePatientGroup.NumberOfPatientsDeferred);
			Console.WriteLine ("Utilisation (of Beds): \t\t\t{0} (hours)", icuCareUnit.Utilisation);
			Console.WriteLine ("Availability (of Beds): \t\t{0} (hours)", 
				(RunDuration * icuCareUnit.InitialValue));

			Console.WriteLine ("Bed Occupancy Rate: \t\t\t{0:P2}", bedOccupancyRate);

			totalEmergencyArrivals += emergencyPatientGroup.NumberOfArrivals;
			totalElectiveArrivals += electivePatientGroup.NumberOfArrivals;
			totalEmergencyTransfers += emergencyPatientGroup.NumberOfPatientsDeferred;
			totalElectiveDeferrals += electivePatientGroup.NumberOfPatientsDeferred;
			totalBedsUtilisation += icuCareUnit.Utilisation;
		}
		//method that is invoked whenever the OnFinishSimulation event occur
		public void SimulationFinish(object theSimulationObject, SimulationInfoEventArgs si)
		{
			//defining model temporary variables
			CareUnit icuCareUnit;
			icuCareUnit = (CareUnit) beds;
			float bedOccupancyRate;
			//results for run
			bedOccupancyRate = totalBedsUtilisation;
			bedOccupancyRate = bedOccupancyRate / (RunDuration * icuCareUnit.InitialValue * NumberOfRuns);
			if (bedOccupancyRate > 1)
			{
				bedOccupancyRate = 1;
			}
			Console.WriteLine ();
			Console.WriteLine ("=======================================================");
			Console.WriteLine ("Summary Results (average) for {0} run(s)", NumberOfRuns);
			Console.WriteLine ("=======================================================");
			Console.WriteLine ("Emergency Arrivals: \t\t\t{0}", 
				totalEmergencyArrivals / NumberOfRuns);
			Console.WriteLine ("Elective Arrivals: \t\t\t{0}", 
				totalElectiveArrivals / NumberOfRuns);
			Console.WriteLine ("Emergency Transfers: \t\t\t{0}", 
				totalEmergencyTransfers / NumberOfRuns);
			Console.WriteLine ("Elective Deferrals: \t\t\t{0}", 
				totalElectiveDeferrals / NumberOfRuns);
			Console.WriteLine ("Bed Occupancy Rate: \t\t\t{0:P2}", bedOccupancyRate);
			Console.WriteLine ("=======================================================");
			Console.WriteLine ("=======================================================");
			Console.WriteLine ();
			Console.WriteLine ();
		}
		//method that is invoked whenever the CompletionThreePhases event occur
		public void CompletionThreePhases(object theSimulationObject, SimulationInfoEventArgs si)
		{
			//Console.WriteLine ("ST {0}", si.time);
		}
		public void EmergencyPatientArrive() 
		{
			PatientGroup emergencyPatientGroup;
			emergencyPatientGroup = (PatientGroup) emergencyArrivalMachine;
			// add arrival to entity number of arrivals
			emergencyPatientGroup.NumberOfArrivals += 1;
			// add arrival to entity queue
			emergencyPatientGroup.NumberOfPatientsInQueue += 1; 
			// next arrival will occur in patient group inter-arrival time
			theSimulation.Technique.Schedule (ref emergencyArrivalMachine, emergencyArrive, emergencyPatientGroup.NextArrival()); 
		}
		public void ElectivePatientArrive() 
		{
			PatientGroup electivePatientGroup;
			electivePatientGroup = (PatientGroup) electiveArrivalMachine;
			// add arrival to entity number of arrivals
			electivePatientGroup.NumberOfArrivals += 1;
			// add arrival to entity queue
			electivePatientGroup.NumberOfPatientsInQueue += 1; 
			// next arrival will occur in patient group inter-arrival time
			theSimulation.Technique.Schedule(ref electiveArrivalMachine, electiveArrive, electivePatientGroup.NextArrival()); 
		}
		public bool EmergencyBeginOccupyBed()
		{
			PatientGroup emergencyPatientGroup;
			emergencyPatientGroup = (PatientGroup) emergencyArrivalMachine;
			CareUnit icuCareUnit;
			icuCareUnit = (CareUnit) beds;
			//defines a variable to flag if C Activitiy has started
			bool activityStarted = false;
			//loops while there are patients in queue and number of available beds is greater than zero.
			while ((emergencyPatientGroup.NumberOfPatientsInQueue > 0) && (icuCareUnit.Count > 0))
			{
					//decrement the number of beds available
					icuCareUnit.Count -=1;
					//decrement the number of patients in queue
					emergencyPatientGroup.NumberOfPatientsInQueue -= 1;
					//create a new Patient entity
					EntityBase newPatient = new Patient("Emergency Patient");
					//schedule a B Event (EndOccupyBed) to occur within patient group LOS (in days)
					uint patientLOS = emergencyPatientGroup.PatientLOS();
					icuCareUnit.Utilisation += patientLOS;
					theSimulation.Technique.Schedule(ref newPatient, emergencyEndOccupyBed, patientLOS);
					//set flag to true (activity has started)
					activityStarted = true;
			}
			// if there are still emergency patients in queue, they are transferred.
			if (emergencyPatientGroup.NumberOfPatientsInQueue > 0)
			{
				emergencyPatientGroup.NumberOfPatientsDeferred += emergencyPatientGroup.NumberOfPatientsInQueue;
				emergencyPatientGroup.NumberOfPatientsInQueue = 0;
			}
			//return value of flag variable
			return (activityStarted);
		}
		public bool ElectiveBeginOccupyBed()
		{
			PatientGroup electivePatientGroup;
			electivePatientGroup = (PatientGroup) electiveArrivalMachine;
			CareUnit icuCareUnit;
			icuCareUnit = (CareUnit) beds;
			//defines a variable to flag if C Activitiy has started
			bool activityStarted = false;
			//loops while there are patients in queue and number of available beds is greater than zero.
			while ((electivePatientGroup.NumberOfPatientsInQueue > 0) && 
				((icuCareUnit.Count - icuCareUnit.NumberOfEmergencyOnlyBeds) > 0))
			{
				//decrement the number of beds available
				icuCareUnit.Count -=1;
				//decrement the number of patients in queue
				electivePatientGroup.NumberOfPatientsInQueue -= 1;
				//create a new Patient entity
				EntityBase newPatient = new Patient("Elective Patient");
				//schedule a B Event (EndOccupyBed) to occur within patient group LOS (in days)
				uint patientLOS = electivePatientGroup.PatientLOS();
				icuCareUnit.Utilisation += patientLOS;
				theSimulation.Technique.Schedule(ref newPatient, electiveEndOccupyBed, patientLOS);                
                //theSimulation.Schedule (ref electivePatient, electiveEndOccupyBed, patientLOS);
                //set flag to true (activity has started)
                activityStarted = true;
			}
			// if there are still elective patients in queue, they are deferred.
			if (electivePatientGroup.NumberOfPatientsInQueue > 0)
			{
				electivePatientGroup.NumberOfPatientsDeferred += electivePatientGroup.NumberOfPatientsInQueue;
				electivePatientGroup.NumberOfPatientsInQueue = 0;
			}
			//return value of flag variable
			return (activityStarted);
		}
		public void EmergencyEndOccupyBed() 
		{
			CareUnit icuCareUnit;
			icuCareUnit = (CareUnit) beds;
			icuCareUnit.Count += 1;
		}
		public void ElectiveEndOccupyBed() 
		{
			CareUnit icuCareUnit;
			icuCareUnit = (CareUnit) beds;
			icuCareUnit.Count += 1;
		}
		public void SettingSimulationParameters()
		{
            RunDuration = 365 * 24; // 365 days times 24 hours = 8760 hours	
            NumberOfRuns = 10;
            theSimulation.Configurator.Duration = RunDuration;
            theSimulation.Configurator.NumberOfRuns = NumberOfRuns;
        }
		public void SettingEmergencyPatientGroup()
		{
			PatientGroup emergencyPatientGroup;
			emergencyPatientGroup = (PatientGroup) emergencyArrivalMachine;
			emergencyPatientGroup.PopulationByYear = 400;
			emergencyPatientGroup.LOSLognormalMean = 4.79F;
			emergencyPatientGroup.LOSLognormalStandardDeviation = 10.26F;
			emergencyPatientGroup.LOSMinimum = 1;
			emergencyPatientGroup.LOSMaximum = 102;
		}
		public void SettingElectivePatientGroup()
		{
			PatientGroup electivePatientGroup;
			electivePatientGroup = (PatientGroup) electiveArrivalMachine;
			electivePatientGroup.PopulationByYear = 200;
			electivePatientGroup.LOSLognormalMean = 3.49F;
			electivePatientGroup.LOSLognormalStandardDeviation = 8.69F;
			electivePatientGroup.LOSMinimum = 1;
			electivePatientGroup.LOSMaximum = 54;
		}
		public void SettingCareUnit()
		{
			CareUnit icuCareUnit;
			icuCareUnit = (CareUnit) beds;
			icuCareUnit.InitialValue = 10;
			icuCareUnit.Count = icuCareUnit.InitialValue;
			icuCareUnit.NumberOfEmergencyOnlyBeds = 1;
		}
		// property(ies)
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		void Main(string[] args)
		{
           
			Model theModel = new Model();

            //assigning OnStartRun event/handler from simulation to a method in the model class
            theSimulation.OnStartSimulation += new StartSimulationHandler(SimulationStart);
            //assigning OnStartRun event/handler from simulation to a method in the model class
            theSimulation.OnStartRun += new StartRunHandler(RunStart);
            //assigning OnFinishRun event/handler from simulation to a method in the model class
            theSimulation.OnFinishRun += new FinishRunHandler(RunFinished);
			//assigning OnFinishSimulation event/handler from simulation to a method in the model class
			theSimulation.OnFinishSimulation += new FinishSimulationHandler(SimulationFinish);
			//assigning OnCompletionThreePhases event/handler from simulation to a method in the model class            
			//theSimulationOnCompleteThreePhases += new CompleteThreePhasesHandler (CompletionThreePhases);            
            
			theSimulation.EntityAndResourceManager.AddEntity (emergencyArrivalMachine);
			theSimulation.EntityAndResourceManager.AddEntity (electiveArrivalMachine);
			theSimulation.EntityAndResourceManager.AddResource (beds);
			theSimulation.EventAndActivityManager.AddEvent (emergencyArrive);
			theSimulation.EventAndActivityManager.AddEvent(electiveArrive);
			theSimulation.EventAndActivityManager.AddEvent(emergencyEndOccupyBed);
			theSimulation.EventAndActivityManager.AddEvent(electiveEndOccupyBed);
			theSimulation.EventAndActivityManager.AddActivity(emergencyBeginOccupyBed);
			theSimulation.EventAndActivityManager.AddActivity(electiveBeginOccupyBed);

			theModel.SettingSimulationParameters();
			theModel.SettingEmergencyPatientGroup();
			theModel.SettingElectivePatientGroup();
			theModel.SettingCareUnit();

			Console.WriteLine("******** Critical Care Unit Model version 1.0 *********");
			Console.WriteLine("*                                                     *");
			Console.WriteLine("*       (using ThreePhaseSharpLib version 2.0)        *");
			Console.WriteLine("*                                                     *");
			Console.WriteLine("*                                  by Andre X. Costa  *");
			Console.WriteLine("*                                          July 2017  *");
			Console.WriteLine("*******************************************************");
			Console.WriteLine();
			string reply = "";
			bool exit = false;
			do
			{
				Console.WriteLine("*******************************************************");
				Console.WriteLine(" Options:                                              ");
				Console.WriteLine("                                                       ");
				Console.WriteLine(" R (or r): Run Simulation and View Results on screen   ");
				Console.WriteLine(" X (or x): Exit program                                ");
				Console.WriteLine("                                                       ");
				Console.Write(" Enter your option and press <Enter>: ");
				reply = Console.ReadLine();
				
				
				if (reply.Length > 0)
				{
					reply = reply.Substring (0,1).ToUpper();
				}
				if (reply == "R")
				{
					try
					{
						Console.WriteLine("*******************************************************");
						Console.WriteLine();
						Console.WriteLine ("=======================================================");
						Console.WriteLine ("Simulation Results");
						Console.WriteLine ("=======================================================");
						Console.WriteLine();
						Console.WriteLine ("Run Duration: {0} hours (365 days)", RunDuration);
						Console.WriteLine ("Number of Runs: {0}", NumberOfRuns);
						Console.WriteLine();
						theSimulation.Run();
					}
					catch (Exception ex)
					{
						Console.WriteLine (ex.Message);
					}
				}
				else if (reply == "X")
				{
					Console.WriteLine("*******************************************************");
					Console.WriteLine();
					exit = true;
				}
				else
				{
					Console.WriteLine("                  Invalid Option!                      ");
					Console.WriteLine("*******************************************************");
				}
			} while (! exit);
		}
	}
}
