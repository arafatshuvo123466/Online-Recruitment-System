# Online Recruitment System

A comprehensive C# .NET Windows Forms application for managing job postings, applications, and candidates.

## Features

-   **For Employers:** Post new jobs, view and manage applications.
-   **For Job Seekers:** Search and apply for jobs.
-   **Admin Dashboard:** Manage users, roles, and system data.

## Technology Stack

-   **Frontend:** C#, Windows Forms
-   **Backend:** .NET Framework
-   **Database:** MS SQL Server

## Database Schema

The database backup file (`JobConnect.bak`) is included in this repository. Below is the main schema:

![Database Schema Diagram](screenshots/database_schema.png) <!-- You can add this later -->

### Key Tables:
-   `UsersTable` - Stores login credentials and roles
-   `JobsTable` - Contains all job listings
-   `ApplicationsTable` - Tracks job applications

## Installation and Setup

1.  **Restore the Database:**
    -   Use the provided `JobConnect.bak` file to restore the database in SQL Server Management Studio (SSMS).

2.  **Run the Application:**
    -   Open the `Online Recruitment System.sln` solution file in Visual Studio.
    -   Build the solution to restore NuGet packages.
    -   Run the project.

## Screenshots

### Employer Dashboard
![Employer Dashboard](screenshots/employer_dashboard.png)

### Job Application Form
![Application Form](screenshots/application_form.png)

*(You can add your own screenshots later)*

## Contributing

This project was developed as an assignment for the course "OOP2" at AIUB.