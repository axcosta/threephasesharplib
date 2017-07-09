namespace ThreePhaseSharpLib
{
    public class SimulationCurrentInformation
    {
        private bool isWarmUpTime = false;
        private uint currentRun = 1;
        private uint time = 0;        
        private Simulation.State currentState = Simulation.State.Idle;
        private bool initialised = false;
        private bool hasSimulationStarted = false;
        private bool hasRunStarted = false; 
        private Simulation simulation;

        public SimulationCurrentInformation(Simulation simulation)
        {
            this.simulation = simulation;
        }

        /// <summary>
        /// Flag that signals if simulation has started
        /// </summary>
        public bool HasSimulationStarted { get => hasSimulationStarted; set => hasSimulationStarted = value; }
        /// <summary>
        /// Flag that signals if a run has started
        /// </summary>
        public bool HasRunStarted { get => hasRunStarted; set => hasRunStarted = value; }
        /// <summary>
        /// Simulation Current State (idle, running, paused or finished)
        /// </summary>
        public Simulation.State CurrentState { get => currentState; set => currentState = value; }
        /// <summary>
        /// true if Simulation is in warm-up time
        /// </summary>
        public bool IsWarmUpTime { get => isWarmUpTime; set => isWarmUpTime = value; }
        /// <summary>
        /// Current Simulation Run
        /// </summary>
        public uint CurrentRun { get => currentRun; set => currentRun = value; }
        /// <summary>
        /// Current Simulation Time (in units of time)
        /// </summary>
        public uint Time { get => time; set => time = value; }
        /// <summary>
        /// Flag that signals if simulation has been initialised.		
        /// </summary>
        public bool Initialised { get => initialised; set => initialised = value; }

        public void Initialisation()
        {
            Time = 0;
            CurrentRun = 1;
            Initialised = true;
        }
    }
}
