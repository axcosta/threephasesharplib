#region Copyright (C) 2003-17 - A X Costa
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

// Define the TRACE directive, which enables trace output to the 
// Trace.Listeners collection. Typically, this directive is defined
// as a compilation argument.
#define TRACE

using System;
using System.Collections; // collections derived classes (array lists)
using System.Diagnostics;

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
			Idle,
			Running,
			Paused,
			Finished
		}
		// constant(s)			 
		public const uint UpperBound32BitUnsignedInteger = 4294967294; //max. 32-bit unsigned integer value - 1
        // field(s)        		

        private SimulationConfigurator configurator;
        private SimulationCurrentInformation currentInformation;   

		private Calendar calendar = new Calendar();
		private ArrayList bEvents = new ArrayList();
		private ArrayList cActivities = new ArrayList();
		private ArrayList entities = new ArrayList();
		private ArrayList resources = new ArrayList();
		private ArrayList dueNowList = new ArrayList();

        // Initialize the trace source.
        private static readonly TraceSource trace = new TraceSource("ThreePhaseSharpLib");        

        // delegate(s)
        public delegate void CompleteThreePhasesHandler (object simulation, SimulationInfoEventArgs simulationInfo);
		public delegate void StartSimulationHandler (object simulation, SimulationInfoEventArgs simulationInfo);
		public delegate void FinishSimulationHandler (object simulation,SimulationInfoEventArgs simulationInfo);
		public delegate void StartWarmUpTimeHandler (object simulation, SimulationInfoEventArgs simulationInfo);
		public delegate void FinishWarmUpTimeHandler (object simulation, SimulationInfoEventArgs simulationInfo);
		public delegate void StartRunHandler (object simulation, SimulationInfoEventArgs simulationInfo);
		public delegate void FinishRunHandler (object simulation, SimulationInfoEventArgs simulationInfo);
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
		
		// method(s)

        public Simulation()
        {
            configurator = new SimulationConfigurator(this);
            currentInformation = new SimulationCurrentInformation(this);
        }

		/// <summary>
		/// A Phase, where is done a time scanning in the calendar to see all due now B events
		/// </summary>
		private void APhase()
		{
			uint minimumTimeCell = configurator.Duration;
            uint previousTime;
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
			if ((currentInformation.Time > 0) && (currentInformation.Time == minimumTimeCell))
			{
                trace.TraceEvent(TraceEventType.Critical, 1, Strings.SIMULATION_EXCEPTION_THREE_PHASE_INFINITE_LOOP);                
                throw (new ThreePhaseInfiniteLoopException (Strings.SIMULATION_EXCEPTION_THREE_PHASE_INFINITE_LOOP));	
			}
			else 
			{                
				previousTime = currentInformation.Time;
                currentInformation.Time = minimumTimeCell;
                trace.TraceInformation(Strings.SIMULATION_TIME_REPORT, currentInformation.Time.ToString(), previousTime.ToString());                
            }
            trace.TraceInformation(Strings.SIMULATION_A_PHASE_CALENDAR_COUNT, currentInformation.Time.ToString(), calendar.Count.ToString());
        }
		/// <summary>
		/// B Phase, where all B event due now are executed
		/// </summary>
		private void BPhase()
		{
            trace.TraceInformation(Strings.SIMULATION_B_PHASE_DUE_NOW_LIST_COUNT, currentInformation.Time.ToString(), dueNowList.Count.ToString());            
			foreach (CalendarEntry tempCalendarEntry in dueNowList)
			{
                trace.TraceInformation(Strings.SIMULATION_B_PHASE_EXECUTING_B_EVENT, currentInformation.Time.ToString(), tempCalendarEntry.NextB.Method.Name);
                tempCalendarEntry.Entity.Available = true; //release current entity (so it cannot be scheduled)
				tempCalendarEntry.NextB(); // executes B Event(s) due NOW!
				calendar.Remove (tempCalendarEntry); // remove from the calendar the event that just occurred
                trace.TraceInformation(Strings.SIMULATION_B_PHASE_REMOVING_B_EVENT, currentInformation.Time.ToString(), tempCalendarEntry.NextB.Method.Name);                
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
                    if (cStarted)
                        trace.TraceInformation(Strings.SIMULATION_C_PHASE_STARTED_C_ACTIVITY, currentInformation.Time.ToString(), currentActivity.Method.Name);                        
					else
                        trace.TraceInformation(Strings.SIMULATION_C_PHASE_FAILED_C_ACTIVITY, currentInformation.Time.ToString(), currentActivity.Method.Name);                    
				}
			} while (cStarted); // C Phase lasts while there are C Activities to be tried.
		}
		/// <summary>
		/// Run simulation
		/// </summary>
		public void Run()
		{
			if (!currentInformation.HasSimulationStarted)
			{
                // send event on start of simulation (OnStartSimulation)
                OnStartSimulation?.Invoke(this, new SimulationInfoEventArgs(currentInformation.CurrentRun, currentInformation.Time));
                currentInformation.HasSimulationStarted = true;
			}
			if (!currentInformation.Initialised) // not initialised yet
			{
				Initialisation(); // call Initialisation method.
			}
            currentInformation.CurrentState = State.Running;
            trace.TraceInformation(Strings.SIMULATION_RUNNING);            
			while (currentInformation.CurrentState == State.Running) // simulation runs until current state changes.
			{
				if (!currentInformation.HasRunStarted)
				{
                    // send event OnStartRun.
                    OnStartRun?.Invoke(this, new SimulationInfoEventArgs(currentInformation.CurrentRun, currentInformation.Time));
                    currentInformation.HasRunStarted = true;
				}
				// three-phases
				APhase();
				BPhase();
				CPhase();
                // send event on completion of three phases (OnCompletionThreePhases)
                OnCompleteThreePhases?.Invoke(this, new SimulationInfoEventArgs(currentInformation.CurrentRun, currentInformation.Time));
                if (configurator.Step) // step mode
				{
                    currentInformation.CurrentState = State.Paused;
                    trace.TraceInformation(Strings.SIMULATION_PAUSED_STEP_MODE);                    
				}
				else // normal mode
				{
					if (currentInformation.Time == configurator.Duration) // run duration elapsed
					{
                        currentInformation.HasRunStarted = false; // flag will signal that next run has not started yet
                                               // send event OnFinishRun.
                        OnFinishRun?.Invoke(this, new SimulationInfoEventArgs(currentInformation.CurrentRun, currentInformation.Time));
                        if (currentInformation.CurrentRun == configurator.NumberOfRuns) // number of runs elapsed => finishes simulation
						{
                            // send event on completion of simulation (OnFinishedSimulation)
                            OnFinishSimulation?.Invoke(this, new SimulationInfoEventArgs(currentInformation.CurrentRun, currentInformation.Time));
                            // change Simulation State to Finished.
                            currentInformation.CurrentState = State.Finished;
                            // signal to false flag that says if simulation has started.
                            currentInformation.HasSimulationStarted = false;
                            // initialised is set to false, so values can be reset in the next initialisation.
                            currentInformation.Initialised = false;
                            trace.TraceInformation(Strings.SIMULATION_FINISHED);
                        }
						else // number of runs NOT elapsed
						{
							// reset Calendar
							calendar.Clear();
                            // reset Time
                            currentInformation.Time = 0;
                            // advance current run
                            currentInformation.CurrentRun += 1;
							if (configurator.WarmUpTime > 0) // has warm-up time
							{
                                currentInformation.IsWarmUpTime = true;
                                // send an event on start of warm-up time (OnStartWarmUpTime)!
                                OnStartWarmUpTime?.Invoke(this, new SimulationInfoEventArgs(currentInformation.CurrentRun, currentInformation.Time));
                            }
						}
					}
					else // run duration NOT elapsed
					{
						if (currentInformation.IsWarmUpTime) // warm-up mode
						{
							if (currentInformation.Time >= configurator.WarmUpTime) // testing if warm-up time has finished
							{
                                currentInformation.IsWarmUpTime = false;
                                // send an event on warm-up finished (OnFinishedWarmUpTime)!
                                OnFinishWarmUpTime?.Invoke(this, new SimulationInfoEventArgs(currentInformation.CurrentRun, currentInformation.Time));
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
			if (currentInformation.CurrentState == State.Running)
			{
                currentInformation.CurrentState = State.Paused;
                trace.TraceInformation(Strings.SIMULATION_PAUSED);
            }
		}
		/// <summary>
		/// Reset simulation 
		/// </summary>
		public void Reset()
		{
			// simulation is reset ONLY if current state is not Idle!
			if (currentInformation.CurrentState != State.Idle)
			{
				currentInformation.CurrentState = State.Idle;
                // reinitialise simulation variables
                currentInformation.Initialised = false;
				currentInformation.Time = 0;
                currentInformation.CurrentRun = 1;
                trace.TraceInformation(Strings.SIMULATION_RESETTING);
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
			scheduleTime = currentInformation.Time + nextTime;
			//a new entry in the calendar is created.
			CalendarEntry newCalendarEntry = new CalendarEntry (ref entity, nextB, scheduleTime);
			calendar.Add (newCalendarEntry);
            trace.TraceInformation(Strings.SIMULATION_SCHEDULE_B_EVENT, currentInformation.Time.ToString(), entity.Name, nextB.Method.Name, scheduleTime.ToString());            
		}
		/// <summary>
		/// Delay simulation for a period proportionally inversed to simulation speed
		/// </summary>
		private void Delay()
		{
			int sleepTime;
            //sleepTime (t) is a result of the formula t = ((- d * s)/ 100) + d,
                        
            uint delayDuration = configurator.DelayDuration; //where d = delay duration (in miliseconds) and
            byte speed = configurator.Speed; // s = speed (from 0 to 100).

            sleepTime = ((-(int)delayDuration * speed) / 100) + (int)delayDuration;
			// simulation process is put to sleep for SleepTime (t) miliseconds
			System.Threading.Thread.Sleep (sleepTime);
		}
		/// <summary>
		/// Check if all steps are done before running a simulation
		/// </summary>
		private void Initialisation()
		{
            trace.TraceInformation(Strings.SIMULATION_RUN_INITIALIZATION);
            // reinitialise simulation variables
            // reset Calendar, Time and CurrentRun
            calendar.Clear();            
            currentInformation.Time = 0;
            currentInformation.CurrentRun = 1;
			if (configurator.WarmUpTime > 0) // has warm-up time
			{
                currentInformation.IsWarmUpTime = true;
                // send an event on start of warm-up time (OnStartWarmUpTime)!
                OnStartWarmUpTime?.Invoke(this, new SimulationInfoEventArgs(currentInformation.CurrentRun, currentInformation.Time));
            }
			currentInformation.Initialised = true;
		}
		/// <summary>
		/// Add an Entity object to the Simulation collection of Entities
		/// </summary>
		public void AddEntity(EntityBase entity) 
		{
			//simulation configuration can be changed ONLY when current state is IDLE!
			if (currentInformation.CurrentState == State.Idle)
			{				
				entities.Add (entity);
                trace.TraceInformation(Strings.SIMULATION_ADD_COMPONENT, Strings.SIMULATION_COMPONENT_ENTITY, entity.Name);
            }
			else
			{
                trace.TraceEvent(TraceEventType.Error, 2, Strings.SIMULATION_EXCEPTION_CONFIGURATION_CHANGE);                
                throw (new ConfigurationCannotBeChangedException(String.Format(Strings.SIMULATION_EXCEPTION_ADD_COMPONENT,
                    Strings.SIMULATION_COMPONENT_ENTITY)));
            }
		}
		/// <summary>
		/// Add a Resource object to the Simulation collection of Resources
		/// </summary>
		public void AddResource(ResourceBase resource) 
		{
			//simulation configuration can be changed ONLY when current state is IDLE!
			if (currentInformation.CurrentState == State.Idle)
			{				
				resources.Add (resource);
                trace.TraceInformation(Strings.SIMULATION_ADD_COMPONENT, Strings.SIMULATION_COMPONENT_RESOURCE, resource.Name);
            }
			else
			{
                trace.TraceEvent(TraceEventType.Error, 2, Strings.SIMULATION_EXCEPTION_CONFIGURATION_CHANGE);
                throw (new ConfigurationCannotBeChangedException(String.Format(Strings.SIMULATION_EXCEPTION_ADD_COMPONENT,
                    Strings.SIMULATION_COMPONENT_RESOURCE)));
            }
		}
		/// <summary>
		/// Add a B Event to the Simulation collection of B Events
		/// </summary>
		public void AddBEvent(BEvent bEvent) 
		{
			//simulation configuration can be changed ONLY when current state is IDLE!
			if (currentInformation.CurrentState == State.Idle)
			{				               
                bEvents.Add (bEvent);
                trace.TraceInformation(Strings.SIMULATION_ADD_COMPONENT, Strings.SIMULATION_COMPONENT_B_EVENT, bEvent.Method.Name);
            }
			else
			{
                trace.TraceEvent(TraceEventType.Error, 2, Strings.SIMULATION_EXCEPTION_CONFIGURATION_CHANGE);
                throw (new ConfigurationCannotBeChangedException(String.Format(Strings.SIMULATION_EXCEPTION_ADD_COMPONENT,
                    Strings.SIMULATION_COMPONENT_B_EVENT)));
            }
		}
		/// <summary>
		/// Add a C Activity to the Simulation collection of C Activities
		/// </summary>
		public void AddCActivity(CActivity cActivity) 
		{
			//simulation configuration can be changed ONLY when current state is IDLE!
			if (currentInformation.CurrentState == State.Idle) 
			{
				cActivities.Add (cActivity);
                trace.TraceInformation(Strings.SIMULATION_ADD_COMPONENT, Strings.SIMULATION_COMPONENT_C_ACTIVITY, cActivity.Method.Name);
            }
			else
			{
                trace.TraceEvent(TraceEventType.Error, 2, Strings.SIMULATION_EXCEPTION_CONFIGURATION_CHANGE);
                throw (new ConfigurationCannotBeChangedException (String.Format(Strings.SIMULATION_EXCEPTION_ADD_COMPONENT, 
                    Strings.SIMULATION_COMPONENT_C_ACTIVITY)));
			}
		}

        // property(ies)

        // read-only
        public SimulationConfigurator Configurator { get => configurator;}
        public SimulationCurrentInformation CurrentInformation { get => currentInformation; }       

    }
}