#region Copyright (C) 2003 - A X Costa
// 
// Filename: Simulation.cs
// Project: ThreePhaseSharpLib
//
// Begin: 14th August 2003
// Last Modification: 17th September 2003
//
// All rights reserved.
//
#endregion

using System;
using System.IO; // log file streams classes
using System.Collections; // collections derived classes (array lists)

namespace ThreePhaseSharpLib
{
	public delegate void BEvent();
	public delegate bool CActivity();

	[Serializable()]
	/// <summary>
	/// This is the Simulation class, core of the Three Phase Library.
	/// </summary>
	public class Simulation
	{
		// EventArgs class(es)
		public class SimulationInfoEventArgs : EventArgs
		{
			public readonly uint currentRun;
			public readonly uint time;

			public SimulationInfoEventArgs (uint currentRun, uint time)
			{
				this.currentRun = currentRun;
				this.time = time;
			}
		}
		// enumeration(s)
		public enum State : byte
		{
			Idle = 0,
			Running = 1,
			Paused = 2,
			Finished = 3
		}
		// constant(s)			 
		public const uint UpperBound32BitUnsignedInteger = 4294967294; //max. 32-bit unsigned integer value - 1
		// field(s)
		private uint duration = 365; // default of ONE year (if minimum unit of time is equal a day)
		private uint numberOfRuns = 1;
		private uint warmUpTime = 0;
		private byte speed = 100;
		private uint delayDuration = 100; //default of 100ms (miliseconds)
		private bool step = false;
		private bool log = false;
		private bool isWarmUpTime = false;
		private uint currentRun = 1;
		private uint time = 0;
		private uint previousTime;
		private State currentState = State.Idle;
		private bool initialised = false; // flag that signals if simulation has been initialised.
		private bool logFileCreated = false; // flag that signals when log file has been created.
		private string logFileName = ""; // current log file name.
		private bool hasSimulationStarted = false; // flag that signals if simulation has started.
		private bool hasRunStarted = false; // flag that signals if a run has started.
		private Calendar calendar = new Calendar();
		private ArrayList bEvents = new ArrayList();
		private ArrayList cActivities = new ArrayList();
		private ArrayList entities = new ArrayList();
		private ArrayList resources = new ArrayList();
		private ArrayList dueNowList = new ArrayList();
		// delegate(s)
		public delegate void CompleteThreePhasesHandler (object simulation, 
			SimulationInfoEventArgs simulationInfo);
		public delegate void StartSimulationHandler (object simulation, 
			SimulationInfoEventArgs simulationInfo);
		public delegate void FinishSimulationHandler (object simulation, 
			SimulationInfoEventArgs simulationInfo);
		public delegate void StartWarmUpTimeHandler (object simulation, 
			SimulationInfoEventArgs simulationInfo);
		public delegate void FinishWarmUpTimeHandler (object simulation, 
			SimulationInfoEventArgs simulationInfo);
		public delegate void StartRunHandler (object simulation, 
			SimulationInfoEventArgs simulationInfo);
		public delegate void FinishRunHandler (object simulation, 
			SimulationInfoEventArgs simulationInfo);
		// event(s)
		/// <summary>
		/// Event that occurs On Completion of Three Phases
		/// </summary>
		public event CompleteThreePhasesHandler OnCompleteThreePhases;
		/// <summary>
		/// Event that occurs On Start of Simulation
		/// </summary>
		public event StartSimulationHandler OnStartSimulation;
		/// <summary>
		/// Event that occurs On Finish of Simulation
		/// </summary>
		public event FinishSimulationHandler OnFinishSimulation;
		/// <summary>
		/// Event that occurs On Start of Warm-Up Time
		/// </summary>
		public event StartWarmUpTimeHandler OnStartWarmUpTime;
		/// <summary>
		/// Event that occurs On Finish of Warm-Up Time
		/// </summary>
		public event FinishWarmUpTimeHandler OnFinishWarmUpTime;
		/// <summary>
		/// Event that occurs On Start of (each) Run
		/// </summary>
		public event StartRunHandler OnStartRun;
		/// <summary>
		/// Event that occurs On Finish of (each) Run
		/// </summary>
		public event FinishRunHandler OnFinishRun;
		// constructor(s)
		public Simulation()
		{
			
		}
		// destructor
		~Simulation()
		{
			if (log) 
			{
				if (logFileCreated)
				{
					Logging ("Finalising Logging.");
				}
			}
		}
		// method(s)
		/// <summary>
		/// A Phase, where is done a time scanning in the calendar to see all due now B events
		/// </summary>
		private void APhase()
		{
			uint minimumTimeCell = duration;
			dueNowList.Clear();
			foreach (CalendarEntry tempCalendarEntry in calendar)
			{
				if (! tempCalendarEntry.Entity.Available) // if the Entity can be scheduled
				{
					if (tempCalendarEntry.TimeCell <= minimumTimeCell) // if time cell is less than minimum
					{
						// if there was another B Event in due list before, these are erased.
						if (tempCalendarEntry.TimeCell < minimumTimeCell)
						{
							dueNowList.Clear();
						}
						// add B Event to Due Now List
						dueNowList.Add (tempCalendarEntry);
						// Minimum Time Cell is EQUAL to this B Event Time Cell.
						minimumTimeCell = tempCalendarEntry.TimeCell;
					}
				}
			}
			// if (simulation) time is equal to (current) minimumTimeCell (and greater than 0),
			// simulation time is not being advanced,
			// and the normal cause for that is there are not B Events or C Activities 
			// (or they are not working properly)
			// if this happens, the library throws a ThreePhaseInfiniteLoopException,
			// and the user will decide how to handle this.
			if ((time > 0) && (time == minimumTimeCell))
			{
				if (log)
				{
					Logging ("EXCEPTION: Simulation Time is not being updated.");
					Logging ("Finalising logging because of a run-time error.");
				}
				throw (new ThreePhaseInfiniteLoopException ("Simulation Time is not being updated."));	
			}
			else 
			{
				if (log)
				{
					Logging ("ST " + minimumTimeCell.ToString() + " - Previous ST was " + time.ToString());
				}
				previousTime = time;
				time = minimumTimeCell;	
			}
			if (log)
			{
				Logging ("ST " + time.ToString() + " - A Phase -> number of events in Calendar: " +
					calendar.Count.ToString());
			}
		}
		/// <summary>
		/// B Phase, where all B event due now are executed
		/// </summary>
		private void BPhase()
		{
			if (log)
			{
				Logging ("ST " + time.ToString() + " - B Phase -> number of events in Due Now List: " +
					dueNowList.Count.ToString());
			}
			foreach (CalendarEntry tempCalendarEntry in dueNowList)
			{
				if (log)
				{
					Logging ("ST " + time.ToString() + " - EXECUTING " + tempCalendarEntry.NextB.Method.Name);
				}
				tempCalendarEntry.Entity.Available = true; //release current entity (so it cannot be scheduled)
				tempCalendarEntry.NextB(); // executes B Event(s) due NOW!
				calendar.Remove (tempCalendarEntry); // remove from the calendar the event that just occurred
				if (log)
				{
					Logging ("ST " + time.ToString() + " - " + tempCalendarEntry.NextB.Method.Name +
						" has been REMOVED from Calendar.");
				}
			}
		}
		/// <summary>
		/// C Phase, where all C activities are tried to execute
		/// </summary>
		private void CPhase()
		{
			bool cStarted;
			do
			{	
				cStarted = false;
				//cycle through each C Activity in the collection and try to execute them.
				foreach (CActivity currentActivity in cActivities)
				{
					
					//if activity had started, it would return true.
					cStarted = currentActivity();
					if (log)
					{
						if (cStarted)
						{
							Logging ("ST " + time.ToString() + " - TRIED " + currentActivity.Method.Name + 
								"... STARTED!");
						}
						else
						{
							Logging ("ST " + time.ToString() + " - TRIED " + currentActivity.Method.Name + 
								"... FAILED!");
						}
					}
				}
			} while (cStarted); // C Phase lasts while there are C Activities to be tried.
		}
		/// <summary>
		/// Run simulation
		/// </summary>
		public void Run()
		{
			if (! hasSimulationStarted)
			{
				// send event on start of simulation (OnStartSimulation)
				if (OnStartSimulation != null)
				{
					OnStartSimulation (this, new SimulationInfoEventArgs (currentRun, time));
				}
				hasSimulationStarted = true;
			}
			if (! initialised) // not initialised yet
			{
				Initialisation(); // call Initialisation method.
			}
			currentState = State.Running;
			if (log) 
			{
				Logging ("Simulation is Running.");
			}
			while (currentState == State.Running) // simulation runs until current state changes.
			{
				if (! hasRunStarted)
				{
					// send event OnStartRun.
					if (OnStartRun != null)
					{
						OnStartRun (this, new SimulationInfoEventArgs (currentRun, time));
					}
					hasRunStarted = true;
				}
				// three-phases
				APhase();
				BPhase();
				CPhase();
				// send event on completion of three phases (OnCompletionThreePhases)
				if (OnCompleteThreePhases != null)
				{
					OnCompleteThreePhases (this, new SimulationInfoEventArgs (currentRun, time));
				}
				if (step) // step mode
				{
					currentState = State.Paused;
					if (log) 
					{
						Logging ("Simulation is Paused (by Step Mode).");
					}
				}
				else // normal mode
				{
					if (time == duration) // run duration elapsed
					{
						hasRunStarted = false; // flag will signal that next run has not started yet
						// send event OnFinishRun.
						if (OnFinishRun != null)
						{
							OnFinishRun (this, new SimulationInfoEventArgs (currentRun, time));
						}
						if (currentRun == numberOfRuns) // number of runs elapsed => finishes simulation
						{
							// send event on completion of simulation (OnFinishedSimulation)
							if (OnFinishSimulation != null)
							{
								OnFinishSimulation (this, new SimulationInfoEventArgs (currentRun, time));
							}
							// change Simulation State to Finished.
							currentState = State.Finished;
							if (log) 
							{
								Logging ("Simulation has Finished.");
								Logging ("Finalising Logging.");
							}
							// log file created flag is set to false,
							// and if simulation is run again, a new log file has to be created.
							logFileCreated = false; 
							// signal to false flag that says if simulation has started.
							hasSimulationStarted = false;
							// initialised is set to false, so values can be reset in the next initialisation.
							initialised = false;
						}
						else // number of runs NOT elapsed
						{
							// reset Calendar
							calendar.Clear();
							// reset Time
							time = 0;
							// advance current run
							currentRun += 1;
							if (warmUpTime > 0) // has warm-up time
							{ 
								isWarmUpTime = true;
								// send an event on start of warm-up time (OnStartWarmUpTime)!
								if (OnStartWarmUpTime != null)
								{
									OnStartWarmUpTime (this, new SimulationInfoEventArgs (currentRun, time));
								}
							}
						}
					}
					else // run duration NOT elapsed
					{
						if (isWarmUpTime) // warm-up mode
						{
							if (time >= warmUpTime) // testing if warm-up time has finished
							{
								isWarmUpTime = false;
								// send an event on warm-up finished (OnFinishedWarmUpTime)!
								if (OnFinishWarmUpTime != null)
								{
									OnFinishWarmUpTime (this, new SimulationInfoEventArgs (currentRun, time));
								}
							}
						}
						Delay(); // delays simulation for a period proportionally inverse to Simulation Speed.
					}
				}
			}
		}
		/// <summary>
		/// Pause simulation
		/// </summary>
		public void Pause()
		{
			// simulation is paused ONLY if current state is running!
			if (currentState == State.Running)
			{
				currentState = State.Paused;
				if (log) 
				{
					Logging ("Simulation is Paused.");
				}
			}
		}
		/// <summary>
		/// Reset simulation 
		/// </summary>
		public void Reset()
		{
			// simulation is reset ONLY if current state is not Idle!
			if (currentState != State.Idle)
			{
				currentState = State.Idle;
				// reinitialise simulation variables
				initialised = false;
				time = 0;
				currentRun = 1;
				if (log) 
				{
					Logging ("Resetting Simulation.");
					Logging ("Finalising Logging.");
				}
				// log file created flag is set to false,
				// and if simulation is run again, a new log file has to be created.
				logFileCreated = false;
			}
		}
		/// <summary>
		/// Schedule a B Event to happen with an Entity at a particular time in future
		/// </summary>
		public void Schedule(ref EntityBase entity, BEvent nextB, uint nextTime)
		{
			uint scheduleTime;
			entity.Available = false; //entity will not be available to be scheduled.
			entity.Utilisation += nextTime; //utilisation statistics is collected
			scheduleTime = time + nextTime;
			//a new entry in the calendar is created.
			CalendarEntry newCalendarEntry = new CalendarEntry (ref entity, nextB, scheduleTime);
			calendar.Add (newCalendarEntry);
			if (log) 
			{
				Logging ("ST " + time.ToString() + " - SCHEDULING Entity [" + entity.Name + "] to execute " +
					nextB.Method.Name + " @ " + scheduleTime.ToString() + " ST.");
			}
		}
		/// <summary>
		/// Delay simulation for a period proportionally inversed to simulation speed
		/// </summary>
		private void Delay()
		{
			int sleepTime;
			//sleepTime (t) is a result of the formula t = ((- d * s)/ 100) + d,
			//where d = delay duration (in miliseconds) and
			// s = speed (from 0 to 100).
			sleepTime = ((-(int)delayDuration * speed) / 100) + (int)delayDuration;
			// simulation process is put to sleep for SleepTime (t) miliseconds
			System.Threading.Thread.Sleep (sleepTime);
		}
		/// <summary>
		/// Check if all steps are done before running a simulation
		/// </summary>
		private void Initialisation()
		{
			if (log) 
			{
				Logging ("Initialising Simulation Run.");
			}	
			// reinitialise simulation variables
			// reset Calendar, Time and CurrentRun
			calendar.Clear();
			time = 0;
			currentRun = 1;
			if (warmUpTime > 0) // has warm-up time
			{
				isWarmUpTime = true;
				// send an event on start of warm-up time (OnStartWarmUpTime)!
				if (OnStartWarmUpTime != null)
				{
					OnStartWarmUpTime (this, new SimulationInfoEventArgs (currentRun, time));
				}
			}
			initialised = true;
		}
		/// <summary>
		/// Add an Entity object to the Simulation collection of Entities
		/// </summary>
		public void AddEntity(EntityBase entity) 
		{
			//simulation configuration can be changed ONLY when current state is IDLE!
			if (currentState == State.Idle)
			{
				if (log) 
				{
					Logging ("A new Entity called " + entity.Name + " was added to simulation collection.");
				}
				entities.Add (entity);	
			}
			else
			{
				if (log) 
				{
					Logging ("EXCEPTION: Configuration Cannot Be Changed until simulation is in IDLE state.");
					Logging ("Finalising logging because of a run-time error.");
				}
				throw (new ConfigurationCannotBeChangedException ("An Entity could not be added because current State IS NOT Idle."));
			}
		}
		/// <summary>
		/// Add a Resource object to the Simulation collection of Resources
		/// </summary>
		public void AddResource(ResourceBase resource) 
		{
			//simulation configuration can be changed ONLY when current state is IDLE!
			if (currentState == State.Idle)
			{
				if (log) 
				{
					Logging ("A new Resource called " + resource.Name + " was added to simulation collection.");
				}
				resources.Add (resource);	
			}
			else
			{
				if (log) 
				{
					Logging ("EXCEPTION: Configuration Cannot Be Changed until simulation is in IDLE state.");
					Logging ("Finalising logging because of a run-time error.");
				}
				throw (new ConfigurationCannotBeChangedException ("A Resource could not be added because current State IS NOT Idle."));
			}
		}
		/// <summary>
		/// Add a B Event to the Simulation collection of B Events
		/// </summary>
		public void AddBEvent(BEvent bEvent) 
		{
			//simulation configuration can be changed ONLY when current state is IDLE!
			if (currentState == State.Idle)
			{
				if (log) 
				{
					Logging ("A new B Event called " + bEvent.Method.Name + " was added to simulation collection.");
				}
				bEvents.Add (bEvent);	
			}
			else
			{
				if (log) 
				{
					Logging ("EXCEPTION: Configuration Cannot Be Changed until simulation is in IDLE state.");
					Logging ("Finalising logging because of a run-time error.");
				}
				throw (new ConfigurationCannotBeChangedException ("A B Event could not be added because current State IS NOT Idle."));
			}
		}
		/// <summary>
		/// Add a C Activity to the Simulation collection of C Activities
		/// </summary>
		public void AddCActivity(CActivity cActivity) 
		{
			//simulation configuration can be changed ONLY when current state is IDLE!
			if (currentState == State.Idle) 
			{
				cActivities.Add (cActivity);
				if (log) 
				{
					Logging ("A new C Activity called " + cActivity.Method.Name + " was added to simulation collection.");
				}
			}
			else
			{
				if (log) 
				{
					Logging ("EXCEPTION: Configuration Cannot Be Changed until simulation is in IDLE state.");
					Logging ("Finalising logging because of a run-time error.");
				}
				throw (new ConfigurationCannotBeChangedException ("A C Activity could not be added because current State IS NOT Idle."));
			}
		}
		/// <summary>
		/// Logging the simulation actions to a text file
		/// </summary>
		private void Logging (string message)
		{
			string logMessage = "";
			try
			{
				logMessage = "Defining FileStream";
				FileStream logFileStream;
				logMessage += " and Defining StreamWriter";
				StreamWriter logStreamWriter;
				// checks if logFileName is null
				logMessage += " and Checking LogFileName Length";
				if (logFileName.Length == 0) 
				{
					logMessage += " and Defining Date and Time Strings";
					string currentDate;
					string currentTime;
					logMessage += " and Defining Current Date";
					currentDate = DateTime.Now.ToString ("ddMMyy"); //date in the format DDMMYY
					
					logMessage += " and Defining Current Time";
					currentTime = DateTime.Now.ToString ("HHmm");  // time in the format HHmm
					logMessage += " and Defining LogFileName";
					logFileName = "LOGTPSL." + currentDate + "." + currentTime + ".TXT";
				}
				if (! logFileCreated) //if a log file has not been created...
				{
					//create a new log file with handle,
					// if file already exists, file will overwritten.
					logMessage += " and Defining LogFileStream - Create";
					logFileStream = new FileStream (logFileName, FileMode.Create, FileAccess.Write, FileShare.
ReadWrite);		
					logFileCreated = true;
				}
				else 
				{
					// append to current log file
					logMessage += " and Defining LogFileStream - Append";
					logFileStream = new FileStream (logFileName, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
				}
				logMessage += " and Defining LogStreamWriter";
				logStreamWriter = new StreamWriter (logFileStream);
				// write message to log file
				logMessage += " and Writing to file";
				logStreamWriter.Write (DateTime.Now.ToString());
				logStreamWriter.Write (" - ");
				logStreamWriter.WriteLine (message);
				// close stream
				logMessage += " and Closing file.";
				logStreamWriter.Close();
			}
			catch (Exception ex)
			{
				throw (new ApplicationException (ex.Message + " " + logMessage));
			}
		}
		// property(ies)
		
		// read-only
		
		/// <summary>
		/// Current Simulation Time (in units of time)
		/// </summary>
		public uint Time
		{
			get
			{
				return time;
			}
		}

		/// <summary>
		/// true if Simulation is in warm-up time
		/// </summary>
		public bool IsWarmUpTime
		{
			get
			{
				return isWarmUpTime;
			}
		}
		/// <summary>
		/// Current Simulation Run
		/// </summary>
		public uint CurrentRun
		{
			get
			{
				return currentRun;
			}
		}
		/// <summary>
		/// Simulation Current State (idle, running, paused or finished)
		/// </summary>
		public State CurrentState {
			get {
				return currentState;
			}
		}
		
		// read/write
		
		/// <summary>
		/// Simulation Speed (0 - 100)
		/// </summary>
		public byte Speed 
		{
			get
			{
				return speed;
			}
			set
			{
				if ((value >= 0) && (value <= 100))
				{
					if (log) {
						Logging ("Change Parameter Speed from (" + speed.ToString() +
							") to (" + value.ToString() + ")");
					}
					speed = value;
				}
				else
				{
					if (log)
					{
						Logging ("EXCEPTION: Speed MUST be in the range [0, 100].");
						Logging ("Finalising logging because of a run-time error.");
					}
					throw (new ValueOutOfRangeException ("Speed MUST be in the range [0, 100]"));
				}
			}
		}
		/// <summary>
		/// Delay Duration (maximum delay time - in miliseconds - of delay after completion of three-phases,
		/// dependable of simulation speed.
		/// </summary>
		public uint DelayDuration
		{
			get
			{
				return delayDuration;
			}
			set
			{
				if ((value >= 0) && (value <= UpperBound32BitUnsignedInteger))
				{
					if (log) 
					{
						Logging ("Change Parameter Delay Duration from (" + delayDuration.ToString() +
							") to (" + value.ToString() + ")");
					}
					delayDuration = value;
				}
				else
				{
					if (log)
					{
						Logging ("EXCEPTION: Delay Duration MUST be in the range [0, " + 
						UpperBound32BitUnsignedInteger.ToString() + "].");
						Logging ("Finalising logging because of a run-time error.");
					}
					throw (new ValueOutOfRangeException ("Delay Duration MUST be in the range [0, " + 
						UpperBound32BitUnsignedInteger.ToString() + "]"));
				}
			}
		}
		/// <summary>
		/// Simulation Duration
		/// </summary>
		public uint Duration
		{
			get
			{
				return duration;
			}
			set
			{
				if (currentState == State.Idle) 
				{
					if ((value >= 0) && (value <= UpperBound32BitUnsignedInteger))
					{
						if (log) 
						{
							Logging ("Change Parameter Duration from (" + duration.ToString() +
								") to (" + value.ToString() + ")");
						}
						duration = value;
					}
					else
					{
						if (log)
						{
							Logging ("EXCEPTION: Duration MUST be in the range [0, " + 
								UpperBound32BitUnsignedInteger.ToString() + "].");
							Logging ("Finalising logging because of a run-time error.");
						}
						throw (new ValueOutOfRangeException ("Duration MUST be in the range [0, " + 
							UpperBound32BitUnsignedInteger.ToString() + "]"));
					}
				}
				else
				{
					if (log)
					{
						Logging ("EXCEPTION: Duration could not be changed because current State IS NOT Idle.");
						Logging ("Finalising logging because of a run-time error.");
					}
					throw (new ConfigurationCannotBeChangedException ("Duration could not be changed because current State IS NOT Idle."));
				}
			}
		}
		/// <summary>
		/// Simulation Number of Runs
		/// </summary>
		public uint NumberOfRuns
		{
			get
			{
				return numberOfRuns;
			}
			set
			{
				if (currentState == State.Idle) 
				{
					if ((value >= 1) && (value <= UpperBound32BitUnsignedInteger))
					{
						if (log) 
						{
							Logging ("Change Parameter Number of Runs from (" + numberOfRuns.ToString() +
								") to (" + value.ToString() + ")");
						}
						numberOfRuns = value;
					}
					else
					{
						if (log)
						{
							Logging ("EXCEPTION: Number of Runs MUST be in the range [0, " + 
								UpperBound32BitUnsignedInteger.ToString() + "].");
							Logging ("Finalising logging because of a run-time error.");
						}
						throw (new ValueOutOfRangeException ("Number of Runs MUST be in the range [1, " + 
							UpperBound32BitUnsignedInteger.ToString() + "]"));
					}
				}
				else
				{
					if (log)
					{
						Logging ("EXCEPTION: Number of Runs could not be changed because current State IS NOT Idle.");
						Logging ("Finalising logging because of a run-time error.");
					}
					throw (new ConfigurationCannotBeChangedException ("Number of Runs could not be changed because current State IS NOT Idle."));
				}
			}
		}
		/// <summary>
		/// Simulation Warm-Up Time
		/// </summary>
		public uint WarmUpTime
		{
			get
			{
				return warmUpTime;
			}
			set
			{
				if (currentState == State.Idle) 
				{
					if ((value >= 0) && (value <= UpperBound32BitUnsignedInteger))
					{
						if (log) 
						{
							Logging ("Change Parameter Warm-Up Time from (" + warmUpTime.ToString() +
								") to (" + value.ToString() + ")");
						}
						warmUpTime = value;
					}
					else
					{
						if (log)
						{
							Logging ("EXCEPTION: Warm-up Time MUST be in the range [0, " + 
								UpperBound32BitUnsignedInteger.ToString() + "].");
							Logging ("Finalising logging because of a run-time error.");
						}
						throw (new ValueOutOfRangeException ("Warm-up Time MUST be in the range [0, " + 
							UpperBound32BitUnsignedInteger.ToString() + "]"));
					}
				}
				else
				{
					if (log)
					{
						Logging ("EXCEPTION: Warm-Up Time could not be changed because current State IS NOT Idle.");
						Logging ("Finalising logging because of a run-time error.");
					}
					throw (new ConfigurationCannotBeChangedException ("Warm-Up Time could not be changed because current State IS NOT Idle."));
				}
			}
		}
		
		/// <summary>
		/// If Simulation is in Step mode
		/// </summary>
		public bool Step
		{
			get
			{
				return step;
			}
			set
			{
				step = value;
				if (log) 
				{
					if (step)
					{
						Logging ("Step Mode is On");
					}
					else
					{
						Logging ("Step Mode is Off");
					}
				}
			}
		}
		/// <summary>
		/// If Simulation is in Log mode
		/// </summary>
		public bool Log
		{
			get
			{
				return log;
			}
			set
			{
				if (logFileCreated)
				{
					if (value)
					{
						if (! log)
						{
							Logging ("Resuming Logging.");
						}
					}
					else
					{
						if (log) 
						{
							Logging ("Stopping Logging.");
						}
					}
				}
				else
				{
					if (value) 
					{
						if (! log)
						{
							Logging ("Initialising Logging.");
						}
					}
				}
				log = value;
			}
		}

		
	}
}