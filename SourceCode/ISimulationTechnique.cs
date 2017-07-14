using System.Collections;
using static ThreePhaseSharpLib.SimulationEventAndActivityManager;

namespace ThreePhaseSharpLib
{
    public interface ISimulationTechnique
    {
        void Initialise();
        void Reset();
        uint Run(uint currentRun, uint duration, uint time, ArrayList currentActivities);
        /// <summary>
        /// Schedule an Event to happen with an Entity at a particular time in future
        /// </summary>
        void Schedule(ref EntityBase entity, Event nextEvent, uint nextTime);
    }
}
