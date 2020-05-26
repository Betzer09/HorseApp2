Version 1.0

A Web API to support:
-saving, deleting, updating listings of a horse
-searching these listings
-saving, deleting specific sires
-searching these sires
-Finding zip codes within a given radius from a provided zip code to find listings with

This API is intended to be used through Microsoft Azure Portal
Current URL: https://horseappservice.azurewebsites.net
Important Client Routes:
/InsertActiveListing
/UpdateActiveListing
/DeleteActiveListings
/SearchActiveListings
/InsertSire
/DeleteSire
/SearchAllSiresElastically
/ZipCodesInRange


Logging into Azure Portal:
Go to: portal.azure.com

This API also is intended to be used alongside an SQL Database stored on Microsoft Azure Portal
Use SSMS(SQL Server Management Studio) to connect
Current Server Name: horse-server.database.windows.net
/*************************
* Important note: use DevHorseDatabase@..... for development.
* DevHorseDatabase2@.... IS PRODUCTION. DO NOT TOUCH IT DURING DEVELOPMENT.
*************************/
Main Endpoints to use:

-InsertActiveListing
-UpdateActiveListing
-DeleteActiveListing
-SearchActiveListings
-InsertSire
-DeleteSire
-SearchAllSiresElastically

Request and Response Models are contained in the 'Models' folder
Each model has a brief description of what they are for in the comments

txt files contain the various sires that EquineTrader wanted inserted into the sires table

Keystore.cs, which includes sensitive info, is not saved in the repo for security purposes. Contact a member of the dev team for this file.

