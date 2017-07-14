using System;
using System.Diagnostics;
using static ThreePhaseSharpLib.SimulationController;

namespace ThreePhaseSharpLib
{
    public class SimulationConfigurator
    {
        // constant(s)			 
        public const uint UpperBound32BitUnsignedInteger = 4294967294; //max. 32-bit unsigned integer value - 1
        // field(s)
        private uint duration = 365; // default of ONE year (if minimum unit of time is equal a day)
        private uint numberOfRuns = 1;
        private uint warmUpTime = 0;
        private byte speed = 100;
        private uint delayDuration = 100; //default of 100ms (miliseconds)
        private bool step = false;

        // Initialise the trace source.
        private static readonly TraceSource trace = new TraceSource("ThreePhaseSharpLib.SimulationConfigurator");

        // The parent simulation
        private SimulationController simulationController;
        

        public SimulationConfigurator (SimulationController simulationController)
        {
            this.simulationController = simulationController;
        }

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
                    trace.TraceInformation(Strings.SIMULATION_PARAMETER_CHANGE, Strings.SIMULATION_PARAMETER_SPEED,
                            speed.ToString(), value.ToString());
                    speed = value;
                }
                else
                {
                    trace.TraceEvent(TraceEventType.Error, 2, Strings.SIMULATION_EXCEPTION_PARAMETER_RANGE,
                        Strings.SIMULATION_PARAMETER_SPEED, 0, 100);
                    throw (new ValueOutOfRangeException(String.Format(Strings.SIMULATION_EXCEPTION_PARAMETER_RANGE, Strings.SIMULATION_PARAMETER_SPEED,
                        0, 100)));
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
                    trace.TraceInformation(Strings.SIMULATION_PARAMETER_CHANGE, Strings.SIMULATION_PARAMETER_DELAY_DURATION,
                            delayDuration.ToString(), value.ToString());
                    delayDuration = value;
                }
                else
                {
                    trace.TraceEvent(TraceEventType.Error, 2, Strings.SIMULATION_EXCEPTION_PARAMETER_RANGE,
                        Strings.SIMULATION_PARAMETER_DELAY_DURATION, 0, UpperBound32BitUnsignedInteger.ToString());
                    throw (new ValueOutOfRangeException(String.Format(Strings.SIMULATION_EXCEPTION_PARAMETER_RANGE, 0,
                        Strings.SIMULATION_PARAMETER_DELAY_DURATION, UpperBound32BitUnsignedInteger.ToString())));
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
                if (simulationController.CurrentInformation.CurrentState == State.Idle)
                {
                    if ((value >= 0) && (value <= UpperBound32BitUnsignedInteger))
                    {
                        trace.TraceInformation(Strings.SIMULATION_PARAMETER_CHANGE, Strings.SIMULATION_PARAMETER_DURATION,
                            duration.ToString(), value.ToString());
                        duration = value;
                    }
                    else
                    {
                        trace.TraceEvent(TraceEventType.Error, 2, Strings.SIMULATION_EXCEPTION_PARAMETER_RANGE,
                            Strings.SIMULATION_PARAMETER_DURATION, 0, UpperBound32BitUnsignedInteger.ToString());
                        throw (new ValueOutOfRangeException(String.Format(Strings.SIMULATION_EXCEPTION_PARAMETER_RANGE, 0,
                            Strings.SIMULATION_PARAMETER_DURATION, UpperBound32BitUnsignedInteger.ToString())));
                    }
                }
                else
                {
                    trace.TraceEvent(TraceEventType.Error, 2, Strings.SIMULATION_EXCEPTION_PARAMETER_CHANGE, Strings.SIMULATION_PARAMETER_DURATION);
                    throw (new ConfigurationCannotBeChangedException(String.Format(Strings.SIMULATION_EXCEPTION_PARAMETER_CHANGE,
                        Strings.SIMULATION_PARAMETER_DURATION)));
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
                if (simulationController.CurrentInformation.CurrentState == State.Idle)
                {
                    if ((value >= 1) && (value <= UpperBound32BitUnsignedInteger))
                    {
                        trace.TraceInformation(Strings.SIMULATION_PARAMETER_CHANGE, Strings.SIMULATION_PARAMETER_NUMBER_OF_RUNS,
                            numberOfRuns.ToString(), value.ToString());
                        numberOfRuns = value;
                    }
                    else
                    {
                        trace.TraceEvent(TraceEventType.Error, 2, Strings.SIMULATION_EXCEPTION_PARAMETER_RANGE,
                            Strings.SIMULATION_PARAMETER_NUMBER_OF_RUNS, 1, UpperBound32BitUnsignedInteger.ToString());
                        throw (new ValueOutOfRangeException(String.Format(Strings.SIMULATION_EXCEPTION_PARAMETER_RANGE, 1,
                            Strings.SIMULATION_PARAMETER_NUMBER_OF_RUNS, UpperBound32BitUnsignedInteger.ToString())));
                    }
                }
                else
                {
                    trace.TraceEvent(TraceEventType.Error, 2, Strings.SIMULATION_EXCEPTION_PARAMETER_CHANGE, Strings.SIMULATION_PARAMETER_NUMBER_OF_RUNS);
                    throw (new ConfigurationCannotBeChangedException(String.Format(Strings.SIMULATION_EXCEPTION_PARAMETER_CHANGE,
                        Strings.SIMULATION_PARAMETER_NUMBER_OF_RUNS)));
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
                if (simulationController.CurrentInformation.CurrentState == State.Idle)
                {
                    if ((value >= 0) && (value <= UpperBound32BitUnsignedInteger))
                    {
                        trace.TraceInformation(Strings.SIMULATION_PARAMETER_CHANGE, Strings.SIMULATION_PARAMETER_WARM_UP_TIME,
                            warmUpTime.ToString(), value.ToString());
                        warmUpTime = value;
                    }
                    else
                    {
                        trace.TraceEvent(TraceEventType.Error, 2, Strings.SIMULATION_EXCEPTION_PARAMETER_RANGE,
                            Strings.SIMULATION_PARAMETER_WARM_UP_TIME, 0, UpperBound32BitUnsignedInteger.ToString());
                        throw (new ValueOutOfRangeException(String.Format(Strings.SIMULATION_EXCEPTION_PARAMETER_RANGE, 0,
                            Strings.SIMULATION_PARAMETER_WARM_UP_TIME, UpperBound32BitUnsignedInteger.ToString())));
                    }
                }
                else
                {
                    trace.TraceEvent(TraceEventType.Error, 2, Strings.SIMULATION_EXCEPTION_PARAMETER_CHANGE, Strings.SIMULATION_PARAMETER_WARM_UP_TIME);
                    throw (new ConfigurationCannotBeChangedException(String.Format(Strings.SIMULATION_EXCEPTION_PARAMETER_CHANGE,
                        Strings.SIMULATION_PARAMETER_WARM_UP_TIME)));
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

                if (step)
                    trace.TraceInformation(Strings.SIMULATION_STEP_MODE_ON);
                else
                    trace.TraceInformation(Strings.SIMULATION_STEP_MODE_OFF);
            }
        }
    }
}
