@core
Feature: Set the parameters of a simulation model
	As a simulation modeler
	In order to configure a simulation model
	I want to set simulation parameters

Scenario: Set duration of a run
	Given simulation state is "idle"
	When I set that duration should be equal to 8760
	Then the duration should change to 8760

Scenario: Set number of runs
	Given simulation state is "idle"
	When I set that number of runs should be equal to 10
	Then number of runs should change to 10

Scenario: Set warm up time
	Given simulation state is "idle"
	When I set that warm up time should be equal to 2400
	Then warm up time should change to 2400
Scenario: Set speed
	Given simulation state is any
	When I set that speed should be equal to 100
	Then speed should change to 100