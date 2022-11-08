# Url Shortener

Sample url shortener service using .NET 6.0 "minimal" web api and Cosmos DB.

Following the project structure outlined here:
https://timdeschryver.dev/blog/maybe-its-time-to-rethink-our-project-structure-with-dot-net-6?#a-domain-driven-api

Also includes the sample "weather" service to provide an additional "module".

## Setup
 - Download, install  and start the Cosmos DB Emulator.
 - Create a new database named "UrlShortner".
 - Create a new container named "Entries" with the partition key "/key".