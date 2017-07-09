using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TechTalk.SpecFlow;
using ThreePhaseSharpLib;

namespace ThreePhaseSharpLibTest
{
    [Binding]
    public class SetTheParametersOfASimulationModelSteps
    {
        private Simulation theSimulation = new Simulation();
        
        [Given(@"simulation state is ""(.*)""")]
        public void GivenSimulationStateIs(Simulation.State state)
        {
            Assert.AreEqual(theSimulation.CurrentState, state);
        }

        [Given(@"simulation state is any")]
        public void GivenSimulationStateIsAny()
        {
            Assert.IsTrue(true);
        }


        [When(@"I set that duration should be equal to (.*)")]
        public void WhenISetThatDurationShouldBeEqualTo(uint duration)
        {
            theSimulation.Duration = duration;
        }
        
        [When(@"I set that number of runs should be equal to (.*)")]
        public void WhenISetThatNumberOfRunsShouldBeEqualTo(uint numberOfRuns)
        {
            theSimulation.NumberOfRuns = numberOfRuns;
        }
        
        [When(@"I set that warm up time should be equal to (.*)")]
        public void WhenISetThatWarmUpTimeShouldBeEqualTo(uint warmUpTime)
        {
            theSimulation.WarmUpTime = warmUpTime;
        }
        
        [When(@"I set that speed should be equal to (.*)")]
        public void WhenISetThatSpeedShouldBeEqualTo(byte speed)
        {
            theSimulation.Speed = speed;
        }
        
        [Then(@"the duration should change to (.*)")]
        public void ThenTheDurationShouldChangeTo(uint duration)
        {
            Assert.AreEqual(theSimulation.Duration, duration);
        }
        
        [Then(@"number of runs should change to (.*)")]
        public void ThenNumberOfRunsShouldChangeTo(uint numberOfRuns)
        {
            Assert.AreEqual(theSimulation.NumberOfRuns, numberOfRuns);
        }
        
        [Then(@"warm up time should change to (.*)")]
        public void ThenWarmUpTimeShouldChangeTo(uint warmUpTime)
        {
            Assert.AreEqual(theSimulation.WarmUpTime, warmUpTime);
        }
        
        [Then(@"speed should change to (.*)")]
        public void ThenSpeedShouldChangeTo(byte speed)
        {
            Assert.AreEqual(theSimulation.Speed, speed);
        }
    }
}
