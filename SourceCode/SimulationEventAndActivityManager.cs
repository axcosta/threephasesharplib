using System;
using System.Collections;
using System.Diagnostics;
using static ThreePhaseSharpLib.SimulationController;

namespace ThreePhaseSharpLib
{
    public class SimulationEventAndActivityManager
    {
        public delegate void Event();
        public delegate bool Activity();

        private SimulationController simulationController;

        private ArrayList events = new ArrayList();
        private ArrayList activities = new ArrayList();

        // Initialise the trace source.
        private static readonly TraceSource trace = new TraceSource("ThreePhaseSharpLib.SimulationEventAndActivityManager");

        public ArrayList Activities { get => activities; }
        public ArrayList Events { get => events; }

        public SimulationEventAndActivityManager(SimulationController simulationController)
        {
            this.simulationController = simulationController;
        }

        /// <summary>
        /// Add an Event to the Simulation collection of Events
        /// </summary>
        public void AddEvent(Event anyEvent)
        {
            //simulation configuration can be changed ONLY when current state is IDLE!
            if (simulationController.CurrentInformation.CurrentState == State.Idle)
            {
                Events.Add(anyEvent);
                trace.TraceInformation(Strings.SIMULATION_ADD_COMPONENT, Strings.SIMULATION_COMPONENT_B_EVENT, anyEvent.Method.Name);
            }
            else
            {
                trace.TraceEvent(TraceEventType.Error, 2, Strings.SIMULATION_EXCEPTION_CONFIGURATION_CHANGE);
                throw (new ConfigurationCannotBeChangedException(String.Format(Strings.SIMULATION_EXCEPTION_ADD_COMPONENT,
                    Strings.SIMULATION_COMPONENT_B_EVENT)));
            }
        }
        /// <summary>
        /// Add a Activity to the Simulation collection of Activities
        /// </summary>
        public void AddActivity(Activity activity)
        {
            //simulation configuration can be changed ONLY when current state is IDLE!
            if (simulationController.CurrentInformation.CurrentState == State.Idle)
            {
                activities.Add(activity);
                trace.TraceInformation(Strings.SIMULATION_ADD_COMPONENT, Strings.SIMULATION_COMPONENT_C_ACTIVITY, activity.Method.Name);
            }
            else
            {
                trace.TraceEvent(TraceEventType.Error, 2, Strings.SIMULATION_EXCEPTION_CONFIGURATION_CHANGE);
                throw (new ConfigurationCannotBeChangedException(String.Format(Strings.SIMULATION_EXCEPTION_ADD_COMPONENT,
                    Strings.SIMULATION_COMPONENT_C_ACTIVITY)));
            }
        }
    }
}
