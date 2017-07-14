using System;
using System.Collections;
using System.Diagnostics;
using static ThreePhaseSharpLib.SimulationEventAndActivityManager;

namespace ThreePhaseSharpLib
{
    class ThreePhaseSimulation : ISimulationTechnique
    {

        private Calendar calendar = new Calendar();
        private ArrayList dueNowList = new ArrayList();
        private uint currentTime;

        // delegate(s)
        public delegate void CompleteThreePhasesHandler(object simulation, SimulationInfoEventArgs simulationInfo);

        // events
        /// <summary>
        /// Event that occurs On Completion of Three Phases
        /// </summary>
        public event CompleteThreePhasesHandler OnCompleteThreePhases;

        // Initialize the trace source.
        private static readonly TraceSource trace = new TraceSource("ThreePhaseSharpLib.ThreePhaseSimulation");        
        
        public uint Run(uint currentRun, uint duration, uint time, ArrayList currentActivities)
        {
            // three-phases
            currentTime = APhase(duration, time);
            BPhase(currentTime);
            CPhase(currentTime, currentActivities);
            // send event on completion of three phases (OnCompletionThreePhases)
            OnCompleteThreePhases?.Invoke(this, new SimulationInfoEventArgs(currentRun, time));
            return currentTime;
        }

        /// <summary>
		/// A Phase, where is done a time scanning in the calendar to see all due now B events
		/// </summary>
		private uint APhase(uint duration, uint time)
        {
            uint minimumTimeCell = duration;
            uint previousTime;
            uint nextTime;
            dueNowList.Clear();
            foreach (CalendarEntry tempCalendarEntry in calendar)
            {
                if (!tempCalendarEntry.Entity.Available) // if the Entity can be scheduled
                {
                    if (tempCalendarEntry.TimeCell <= minimumTimeCell) // if time cell is less than minimum
                    {
                        // if there was another B Event in due list before, these are erased.
                        if (tempCalendarEntry.TimeCell < minimumTimeCell)
                        {
                            dueNowList.Clear();
                        }
                        // add B Event to Due Now List
                        dueNowList.Add(tempCalendarEntry);
                        // Minimum Time Cell is EQUAL to this B Event Time Cell.
                        minimumTimeCell = tempCalendarEntry.TimeCell;
                    }
                }
            }
            // If (simulation) time is equal to (current) minimumTimeCell (and greater than 0), simulation time is not being advanced,
            // and the normal cause for that is there are not B Events or C Activities (or they are not working properly).
            // If this happens, the library throws a ThreePhaseInfiniteLoopException,
            // and the simulation modeller (user) will decide how to handle this.
            if ((time > 0) && (time == minimumTimeCell))
            {
                trace.TraceEvent(TraceEventType.Critical, 1, Strings.SIMULATION_EXCEPTION_THREE_PHASE_INFINITE_LOOP);
                throw (new ThreePhaseInfiniteLoopException(Strings.SIMULATION_EXCEPTION_THREE_PHASE_INFINITE_LOOP));
            }
            else
            {
                previousTime = time;
                nextTime = minimumTimeCell;
                trace.TraceInformation(Strings.SIMULATION_TIME_REPORT, nextTime.ToString(), previousTime.ToString());
            }
            trace.TraceInformation(Strings.SIMULATION_A_PHASE_CALENDAR_COUNT, nextTime.ToString(), calendar.Count.ToString());
            return nextTime;
        }
        /// <summary>
        /// B Phase, where all B events due now are executed
        /// </summary>
        private void BPhase(uint time)
        {
            trace.TraceInformation(Strings.SIMULATION_B_PHASE_DUE_NOW_LIST_COUNT, time.ToString(), dueNowList.Count.ToString());
            foreach (CalendarEntry tempCalendarEntry in dueNowList)
            {
                trace.TraceInformation(Strings.SIMULATION_B_PHASE_EXECUTING_B_EVENT, time.ToString(), tempCalendarEntry.NextB.Method.Name);
                tempCalendarEntry.Entity.Available = true; //release current entity (so it cannot be scheduled)
                tempCalendarEntry.NextB(); // executes B Event(s) due NOW!
                calendar.Remove(tempCalendarEntry); // remove from the calendar the event that just occurred
                trace.TraceInformation(Strings.SIMULATION_B_PHASE_REMOVING_B_EVENT, time.ToString(), tempCalendarEntry.NextB.Method.Name);
            }
        }
        /// <summary>
        /// C Phase, where all C activities are tried to being executed
        /// </summary>
        private void CPhase(uint time, ArrayList cActivities)
        {
            bool cStarted;
            do
            {
                cStarted = false;
                //cycle through each C Activity in the collection and try to execute them.
                foreach (Activity currentActivity in cActivities)
                {
                    //if activity had started, it would return true.
                    cStarted = currentActivity();
                    if (cStarted)
                        trace.TraceInformation(Strings.SIMULATION_C_PHASE_STARTED_C_ACTIVITY, time.ToString(), currentActivity.Method.Name);
                    else
                        trace.TraceInformation(Strings.SIMULATION_C_PHASE_FAILED_C_ACTIVITY, time.ToString(), currentActivity.Method.Name);
                }
            } while (cStarted); // C Phase lasts while there are C Activities to be tried.
        }

        public void Schedule(ref EntityBase entity, Event nextEvent, uint nextTime)
        {
            uint scheduleTime;
            uint time = currentTime;
            entity.Available = false; //entity will not be available to be scheduled.
            entity.Utilisation += nextTime; //utilisation statistics is collected
            scheduleTime = time + nextTime;
            //a new entry in the calendar is created.
            CalendarEntry newCalendarEntry = new CalendarEntry(ref entity, nextEvent, scheduleTime);
            calendar.Add(newCalendarEntry);
            trace.TraceInformation(Strings.SIMULATION_SCHEDULE_B_EVENT, time.ToString(), entity.Name, nextEvent.Method.Name, scheduleTime.ToString());
        }

        public void Initialise()
        {
            calendar.Clear();
            currentTime = 0;
        }

        public void Reset()
        {
            this.Initialise();
        }
    }
}