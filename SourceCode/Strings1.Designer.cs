﻿//------------------------------------------------------------------------------
// <auto-generated>
//     O código foi gerado por uma ferramenta.
//     Versão de Tempo de Execução:4.0.30319.42000
//
//     As alterações ao arquivo poderão causar comportamento incorreto e serão perdidas se
//     o código for gerado novamente.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ThreePhaseSharpLib {
    using System;
    
    
    /// <summary>
    ///   Uma classe de recurso de tipo de alta segurança, para pesquisar cadeias de caracteres localizadas etc.
    /// </summary>
    // Essa classe foi gerada automaticamente pela classe StronglyTypedResourceBuilder
    // através de uma ferramenta como ResGen ou Visual Studio.
    // Para adicionar ou remover um associado, edite o arquivo .ResX e execute ResGen novamente
    // com a opção /str, ou recrie o projeto do VS.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Strings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Strings() {
        }
        
        /// <summary>
        ///   Retorna a instância de ResourceManager armazenada em cache usada por essa classe.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("ThreePhaseSharpLib.Strings", typeof(Strings).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Substitui a propriedade CurrentUICulture do thread atual para todas as
        ///   pesquisas de recursos que usam essa classe de recurso de tipo de alta segurança.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Consulta uma cadeia de caracteres localizada semelhante a ST {0} - A Phase -&gt; number of events in Calendar: {1}..
        /// </summary>
        internal static string SIMULATION_A_PHASE_CALENDAR_COUNT {
            get {
                return ResourceManager.GetString("SIMULATION_A_PHASE_CALENDAR_COUNT", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Consulta uma cadeia de caracteres localizada semelhante a A new {0} called {1} was added to simulation collection..
        /// </summary>
        internal static string SIMULATION_ADD_COMPONENT {
            get {
                return ResourceManager.GetString("SIMULATION_ADD_COMPONENT", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Consulta uma cadeia de caracteres localizada semelhante a ST {0} - B Phase -&gt; number of events in Due Now List: {1}..
        /// </summary>
        internal static string SIMULATION_B_PHASE_DUE_NOW_LIST_COUNT {
            get {
                return ResourceManager.GetString("SIMULATION_B_PHASE_DUE_NOW_LIST_COUNT", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Consulta uma cadeia de caracteres localizada semelhante a ST {0} - EXECUTING {1}..
        /// </summary>
        internal static string SIMULATION_B_PHASE_EXECUTING_B_EVENT {
            get {
                return ResourceManager.GetString("SIMULATION_B_PHASE_EXECUTING_B_EVENT", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Consulta uma cadeia de caracteres localizada semelhante a ST {0} - {1}  has been REMOVED from Calendar..
        /// </summary>
        internal static string SIMULATION_B_PHASE_REMOVING_B_EVENT {
            get {
                return ResourceManager.GetString("SIMULATION_B_PHASE_REMOVING_B_EVENT", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Consulta uma cadeia de caracteres localizada semelhante a ST {0} - TRIED {1}  ... FAILED!.
        /// </summary>
        internal static string SIMULATION_C_PHASE_FAILED_C_ACTIVITY {
            get {
                return ResourceManager.GetString("SIMULATION_C_PHASE_FAILED_C_ACTIVITY", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Consulta uma cadeia de caracteres localizada semelhante a ST {0} - TRIED {1}  ... STARTED!.
        /// </summary>
        internal static string SIMULATION_C_PHASE_STARTED_C_ACTIVITY {
            get {
                return ResourceManager.GetString("SIMULATION_C_PHASE_STARTED_C_ACTIVITY", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Consulta uma cadeia de caracteres localizada semelhante a B Event.
        /// </summary>
        internal static string SIMULATION_COMPONENT_B_EVENT {
            get {
                return ResourceManager.GetString("SIMULATION_COMPONENT_B_EVENT", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Consulta uma cadeia de caracteres localizada semelhante a C Activity.
        /// </summary>
        internal static string SIMULATION_COMPONENT_C_ACTIVITY {
            get {
                return ResourceManager.GetString("SIMULATION_COMPONENT_C_ACTIVITY", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Consulta uma cadeia de caracteres localizada semelhante a Entity.
        /// </summary>
        internal static string SIMULATION_COMPONENT_ENTITY {
            get {
                return ResourceManager.GetString("SIMULATION_COMPONENT_ENTITY", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Consulta uma cadeia de caracteres localizada semelhante a Resource.
        /// </summary>
        internal static string SIMULATION_COMPONENT_RESOURCE {
            get {
                return ResourceManager.GetString("SIMULATION_COMPONENT_RESOURCE", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Consulta uma cadeia de caracteres localizada semelhante a {0} could not be added because current State IS NOT Idle..
        /// </summary>
        internal static string SIMULATION_EXCEPTION_ADD_COMPONENT {
            get {
                return ResourceManager.GetString("SIMULATION_EXCEPTION_ADD_COMPONENT", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Consulta uma cadeia de caracteres localizada semelhante a Configuration Cannot Be Changed until simulation is in IDLE state..
        /// </summary>
        internal static string SIMULATION_EXCEPTION_CONFIGURATION_CHANGE {
            get {
                return ResourceManager.GetString("SIMULATION_EXCEPTION_CONFIGURATION_CHANGE", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Consulta uma cadeia de caracteres localizada semelhante a {0} could not be changed because current simulation state IS NOT idle..
        /// </summary>
        internal static string SIMULATION_EXCEPTION_PARAMETER_CHANGE {
            get {
                return ResourceManager.GetString("SIMULATION_EXCEPTION_PARAMETER_CHANGE", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Consulta uma cadeia de caracteres localizada semelhante a {0} MUST be in the range [{1}, {2}]..
        /// </summary>
        internal static string SIMULATION_EXCEPTION_PARAMETER_RANGE {
            get {
                return ResourceManager.GetString("SIMULATION_EXCEPTION_PARAMETER_RANGE", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Consulta uma cadeia de caracteres localizada semelhante a Simulation Time is not being updated..
        /// </summary>
        internal static string SIMULATION_EXCEPTION_THREE_PHASE_INFINITE_LOOP {
            get {
                return ResourceManager.GetString("SIMULATION_EXCEPTION_THREE_PHASE_INFINITE_LOOP", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Consulta uma cadeia de caracteres localizada semelhante a Ending output to trace listener..
        /// </summary>
        internal static string SIMULATION_FINALISE_TRACER {
            get {
                return ResourceManager.GetString("SIMULATION_FINALISE_TRACER", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Consulta uma cadeia de caracteres localizada semelhante a Simulation has Finished..
        /// </summary>
        internal static string SIMULATION_FINISHED {
            get {
                return ResourceManager.GetString("SIMULATION_FINISHED", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Consulta uma cadeia de caracteres localizada semelhante a Starting output to trace listener..
        /// </summary>
        internal static string SIMULATION_INITIALISE_TRACER {
            get {
                return ResourceManager.GetString("SIMULATION_INITIALISE_TRACER", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Consulta uma cadeia de caracteres localizada semelhante a Changed Simulation Parameter {0} from ({1}) to ({2})..
        /// </summary>
        internal static string SIMULATION_PARAMETER_CHANGE {
            get {
                return ResourceManager.GetString("SIMULATION_PARAMETER_CHANGE", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Consulta uma cadeia de caracteres localizada semelhante a Delay Run Duration.
        /// </summary>
        internal static string SIMULATION_PARAMETER_DELAY_DURATION {
            get {
                return ResourceManager.GetString("SIMULATION_PARAMETER_DELAY_DURATION", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Consulta uma cadeia de caracteres localizada semelhante a Run Duration.
        /// </summary>
        internal static string SIMULATION_PARAMETER_DURATION {
            get {
                return ResourceManager.GetString("SIMULATION_PARAMETER_DURATION", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Consulta uma cadeia de caracteres localizada semelhante a Number of Runs.
        /// </summary>
        internal static string SIMULATION_PARAMETER_NUMBER_OF_RUNS {
            get {
                return ResourceManager.GetString("SIMULATION_PARAMETER_NUMBER_OF_RUNS", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Consulta uma cadeia de caracteres localizada semelhante a Speed.
        /// </summary>
        internal static string SIMULATION_PARAMETER_SPEED {
            get {
                return ResourceManager.GetString("SIMULATION_PARAMETER_SPEED", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Consulta uma cadeia de caracteres localizada semelhante a Warm Up Time.
        /// </summary>
        internal static string SIMULATION_PARAMETER_WARM_UP_TIME {
            get {
                return ResourceManager.GetString("SIMULATION_PARAMETER_WARM_UP_TIME", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Consulta uma cadeia de caracteres localizada semelhante a Simulation is Paused..
        /// </summary>
        internal static string SIMULATION_PAUSED {
            get {
                return ResourceManager.GetString("SIMULATION_PAUSED", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Consulta uma cadeia de caracteres localizada semelhante a Simulation is Paused (by Step Mode)..
        /// </summary>
        internal static string SIMULATION_PAUSED_STEP_MODE {
            get {
                return ResourceManager.GetString("SIMULATION_PAUSED_STEP_MODE", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Consulta uma cadeia de caracteres localizada semelhante a Resetting Simulation..
        /// </summary>
        internal static string SIMULATION_RESETTING {
            get {
                return ResourceManager.GetString("SIMULATION_RESETTING", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Consulta uma cadeia de caracteres localizada semelhante a Initialising Simulation Run..
        /// </summary>
        internal static string SIMULATION_RUN_INITIALIZATION {
            get {
                return ResourceManager.GetString("SIMULATION_RUN_INITIALIZATION", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Consulta uma cadeia de caracteres localizada semelhante a Simulation is Running..
        /// </summary>
        internal static string SIMULATION_RUNNING {
            get {
                return ResourceManager.GetString("SIMULATION_RUNNING", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Consulta uma cadeia de caracteres localizada semelhante a ST {0} - SCHEDULING Entity [{1}] to execute {2}  @{3} ST..
        /// </summary>
        internal static string SIMULATION_SCHEDULE_B_EVENT {
            get {
                return ResourceManager.GetString("SIMULATION_SCHEDULE_B_EVENT", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Consulta uma cadeia de caracteres localizada semelhante a Step Mode is Off..
        /// </summary>
        internal static string SIMULATION_STEP_MODE_OFF {
            get {
                return ResourceManager.GetString("SIMULATION_STEP_MODE_OFF", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Consulta uma cadeia de caracteres localizada semelhante a Step Mode is On..
        /// </summary>
        internal static string SIMULATION_STEP_MODE_ON {
            get {
                return ResourceManager.GetString("SIMULATION_STEP_MODE_ON", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Consulta uma cadeia de caracteres localizada semelhante a ST {0} - Previous ST was {1}..
        /// </summary>
        internal static string SIMULATION_TIME_REPORT {
            get {
                return ResourceManager.GetString("SIMULATION_TIME_REPORT", resourceCulture);
            }
        }
    }
}
