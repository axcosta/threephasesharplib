Feature: Set the parameters of a simulation model
	As a simulation modeler
	In order to configure a simulation model
	I want to set simulation parameters

@core
Scenario: Set duration of a run
	Given I have entered 50 into the calculator
	And I have entered 70 into the calculator
	When I press add
	Then the result should be 120 on the screen
