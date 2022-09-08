  Books API Client App - Ver. 1.0
+---------------------------------+

CONTENTS OF THIS FILE
---------------------

 * Introduction
 * Requirements
 * Installation
 * Configuration
 * Troubleshooting
 * FAQ
 * Maintainers

INTRODUCTION
---------------------
This application displays volume data from the public Google Books API based on a user specified search.
The two available fields to search are Title and Author. At least one must contain search terms to run a search.
The application only returns paid ebooks, and will retrieve no more than 1000 results. The volumes are 
sorted by Average Rating.


REQUIREMENTS
---------------------
Tested on Windows 10 & 11
This is a "Self Contained" app and includes the .NET runtime so no other installation is needed.


INSTALLATION
---------------------
Copy the contents of the Publish folder into a new folder in your Program Files directory, or run the .exe from any
other folder location.
(PUBLISH FOLDER OMITTED FROM GITHUB DUE TO FILE SIZE - Build this project in Visual Studio to run or publish.)


TROUBLESHOOTING
---------------------
The application will display helpful messages if there is an issue.


CONFIGURATION
--------------------
The size and order of the columns in the display table can be adjusted.


FAQ
---------------------
Q. Will the app display duplicate volume entries?
A. No.

Q. What does the app consider a duplicate?
A. Any two volumes that have the same title, authors and publication year are duplicates.

Q. Will the app display any results with no authors or publication year?
A. The app will not display volumes with no authors, but it will display volumes with no publication year.



MAINTAINERS
---------------------
Ver. 1.0 written by Nathan Waye

