#region Copyright (C) 2003-17 - A X Costa
// 
// Filename: SimulationController.cs
// Project: ThreePhaseSharpLib
//
// Begin: 9th July 2017
// Last Modification: 9th July 2017
//
// All rights reserved.
//
#endregion

// Define the TRACE directive, which enables trace output to the 
// Trace.Listeners collection. Typically, this directive is defined
// as a compilation argument.
#define TRACE

using System.Diagnostics;

namespace ThreePhaseSharpLib
{
    /// <summary>
	/// This is the Simulation Controller class, core of the Three Phase Simulation Library.
	/// </summary>
    public partial class SimulationController
    {
        // enumeration(s)
        public enum State : byte
        {
            Idle,
            Running,
            Paused,
            Finished
        }

        // field(s)        		

        private SimulationConfigurator configurator;
        private SimulationCurrentInformation currentInformation;
        private ISimulationTechnique technique;
        private SimulationEntityAndResourceManager entityAndResourceManager;
        private SimulationEventAndActivityManager eventAndActivityManager;        

        // read-only
        public SimulationConfigurator Configurator { get => configurator; }
        public SimulationCurrentInformation CurrentInformation { get => currentInformation; }
        public ISimulationTechnique Technique { get => technique; }
        public SimulationEntityAndResourceManager EntityAndResourceManager { get => entityAndResourceManager; }
        public SimulationEventAndActivityManager EventAndActivityManager { get => eventAndActivityManager; }        

        // method(s)

        public SimulationController()
        {
            configurator = new SimulationConfigurator(this);
            currentInformation = new SimulationCurrentInformation(this);            
            entityAndResourceManager = new SimulationEntityAndResourceManager(this);
            eventAndActivityManager = new SimulationEventAndActivityManager(this);            
            technique = new ThreePhaseSimulation();
        }

        // delegate(s)     
        public delegate void StartSimulationHandler(object simulation, SimulationInfoEventArgs simulationInfo);
        public delegate void FinishSimulationHandler(object simulation, SimulationInfoEventArgs simulationInfo);
        public delegate void StartWarmUpTimeHandler(object simulation, SimulationInfoEventArgs simulationInfo);
        public delegate void FinishWarmUpTimeHandler(object simulation, SimulationInfoEventArgs simulationInfo);
        public delegate void StartRunHandler(object simulation, SimulationInfoEventArgs simulationInfo);
        public delegate void FinishRunHandler(object simulation, SimulationInfoEventArgs simulationInfo);

        // event(s)        
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

        // Initialize the trace source.
        private static readonly TraceSource trace = new TraceSource("ThreePhaseSharpLib.SimulationController");

        /// <summary>
        /// Check if all steps are done before running a simulation
        /// </summary>
        private void Initialisation()
        {
            trace.TraceInformation(Strings.SIMULATION_RUN_INITIALIZATION);
            // reinitialise simulation variables            
            technique.Initialise();            
            if (configurator.WarmUpTime > 0) // has warm-up time
            {
                currentInformation.IsWarmUpTime = true;
                // send an event on start of warm-up time (OnStartWarmUpTime)!
                OnStartWarmUpTime?.Invoke(this, new SimulationInfoEventArgs(currentInformation.CurrentRun, currentInformation.Time));
            }
            currentInformation.Initialisation();
        }
        /// <summary>
        /// Reset simulation 
        /// </summary>
        public void Reset()
        {
            // simulation is reset ONLY if current state is not Idle!
            if (currentInformation.CurrentState != State.Idle)
            {
                currentInformation.Reset();
                trace.TraceInformation(Strings.SIMULATION_RESETTING);
            }
        }
        /// <summary>
		/// Run simulation
		/// </summary>
		public void Run()
        {
            if (!currentInformation.HasSimulationStarted)
            {                
                currentInformation.HasSimulationStarted = true;
                trace.TraceInformation(Strings.SIMULATION_RUNNING);
                // send event on start of simulation (OnStartSimulation)
                OnStartSimulation?.Invoke(this, new SimulationInfoEventArgs(currentInformation.CurrentRun, currentInformation.Time));
            }
            if (!currentInformation.Initialised) // not initialised yet
            {
                Initialisation(); // call Initialisation method.
            }
            currentInformation.CurrentState = State.Running;            
            while (currentInformation.CurrentState == State.Running) // simulation runs until current state changes.
            {
                if (!currentInformation.HasRunStarted)
                {
                    // send event OnStartRun
                    currentInformation.HasRunStarted = true;
                    trace.TraceInformation(Strings.SIMULATION_START_RUN, currentInformation.CurrentRun, configurator.NumberOfRuns);
                    OnStartRun?.Invoke(this, new SimulationInfoEventArgs(currentInformation.CurrentRun, currentInformation.Time));                    
                }

                // run simulation technique                                
                currentInformation.Time = technique.Run(currentInformation.CurrentRun, configurator.Duration, 
                    CurrentInformation.Time, eventAndActivityManager.Activities);                

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
                        trace.TraceInformation(Strings.SIMULATION_END_RUN, currentInformation.CurrentRun, configurator.NumberOfRuns);
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
                            // clear calendar
                            technique.Reset();
                            // reset Time
                            currentInformation.Time = 0;
                            // advance current run
                            currentInformation.CurrentRun += 1;
                            if (configurator.WarmUpTime > 0) // has warm-up time
                            {
                                currentInformation.IsWarmUpTime = true;
                                // send an event on start of warm-up time (OnStartWarmUpTime)
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
                                // send an event on warm-up finished (OnFinishedWarmUpTime)
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
            System.Threading.Thread.Sleep(sleepTime);
        }        
    }
}