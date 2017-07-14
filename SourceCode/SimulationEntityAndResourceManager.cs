using System;
using System.Collections;
using System.Diagnostics;
using static ThreePhaseSharpLib.SimulationController;

namespace ThreePhaseSharpLib
{
    public class SimulationEntityAndResourceManager
    {
        private ArrayList entities = new ArrayList();
        private ArrayList resources = new ArrayList();

        // Initialise the trace source.
        private static readonly TraceSource trace = new TraceSource("ThreePhaseSharpLib.SimulationEntityAndResourceManager");

        private SimulationController simulationController;

        public SimulationEntityAndResourceManager(SimulationController simulationController)
        {
            this.simulationController = simulationController;
        }

        /// <summary>
        /// Add an Entity object to the Simulation collection of Entities
        /// </summary>
        public void AddEntity(EntityBase entity)
        {
            //simulation configuration can be changed ONLY when current state is IDLE!
            if (simulationController.CurrentInformation.CurrentState == State.Idle)
            {
                entities.Add(entity);
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
            if (simulationController.CurrentInformation.CurrentState == State.Idle)
            {
                resources.Add(resource);
                trace.TraceInformation(Strings.SIMULATION_ADD_COMPONENT, Strings.SIMULATION_COMPONENT_RESOURCE, resource.Name);
            }
            else
            {
                trace.TraceEvent(TraceEventType.Error, 2, Strings.SIMULATION_EXCEPTION_CONFIGURATION_CHANGE);
                throw (new ConfigurationCannotBeChangedException(String.Format(Strings.SIMULATION_EXCEPTION_ADD_COMPONENT,
                    Strings.SIMULATION_COMPONENT_RESOURCE)));
            }
        }
    }
}
