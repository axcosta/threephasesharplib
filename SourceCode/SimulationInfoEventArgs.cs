using System;

namespace ThreePhaseSharpLib
{       
    // EventArgs class(es)
    public class SimulationInfoEventArgs : EventArgs
    {
        public readonly uint currentRun;
        public readonly uint time;

        public SimulationInfoEventArgs(uint currentRun, uint time)
        {
            this.currentRun = currentRun;
            this.time = time;
        }
    }    
}
