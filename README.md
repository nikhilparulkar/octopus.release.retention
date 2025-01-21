

 # Release Retention Implementation

 This project provides a solution for "implementing a release retention rule". 

 ## project configuration
 The project configurations are in appsettings.json. Please update the following before running the application 
 - "Keep Count": Number of releases to be retained for each Environment-Project pair
 - file paths for Deployment.json, Release.json, Environment.json, and Project.json need to be provided.

 ## Execution 
 The Project is built in NET 6.0. Please download NET 6.0 SDK to your machine.
  - Clone the project in your local directory. 
  - Open the .sln file in VSCode 2022. Build the Project and Run it.
  - Logs are generated inside bin/release or bin/debug folder with names like "nlog-<date>.log"

 The log file has info and traces that describe: what all releases retained for a project/environment pair. Log the Error in case of exceptions.

 ## Tests
 - Run the test project in VS 2022 Test Explorer. All the tests should PASS.
 - The test project has unit tests for individual services and also, and there are sub-contiguous Tests, which are more like integration tests testing the applications's layers and services.

 ## Assumptions
 - While developing the solution, it is assumed that all deployment data files (deployment.json, release.json, etc) are an equal source of truth. For example, envionment.json mentioned 2 environments, but deployment.json mentions 3 environments. In this case, while working with deployment.json I have gone ahead with the assumption that deployment.json 3 environments are valid ones, and there is no cross-validation with environment.json.
 - Data Validation in deployment data files (deployment.json, release.json, etc) is skipped and assumed that data is provided in a valid format and not missing any values in respective JSON files.

 
 
