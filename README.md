# Task Management API

This is a simple .NET Core Web API project that manages tasks. It allows you to create tasks, view task details by ID, and retrieve tasks assigned to a specific user. The API uses JWT-based authentication and supports basic role checks (Admin, User).

## Features
- **POST api/tasks** → Create a new task
- **GET api/tasks/{id}** → Get task details by ID
- **GET api/tasks/user/{userId}** → Get tasks assigned to a specific user

## Requirements

- [.NET 6.0 SDK or higher](https://dotnet.microsoft.com/download)
- Visual Studio or any IDE that supports .NET Core
- SQL Server Express (or use the In-Memory database)

## Setup Instructions

Follow these steps to run the project locally:

### 1. Clone the repository

Clone the repository to your local machine:

```bash
git clone https://github.com/biradarniriksha/TaskManagementAPI.git
```

### 2. Navigate to the project folder

Change into the project directory:

```bash
cd TaskManagementAPI
```

### 3. Install the required dependencies

Run the following command to restore the NuGet packages:

```bash
dotnet restore
```

### 4. Set up the database

This project uses an in-memory database by default for simplicity. If you prefer using SQL Server Express, you can modify the connection string in `appsettings.json` else keep the DefaultConnections as null or empty string.

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=TaskManagementApiDb;Trusted_Connection=True;"
}
```

Run the following command to apply any pending migrations (if using a SQL database):

```bash
dotnet ef database update
```

### 5. Running the Application

To run the application, use the following command:

```bash
dotnet run
```

This will start the API locally. By default, the application will run on `http://localhost:5235` or `https://localhost:7224`.

### 6. Using Swagger UI
Access Swagger UI at http://localhost:5235/swagger

### 6. Authentication

The API uses JWT-based authentication. To test the endpoints, you need to obtain a JWT token.

**Example login endpoint:**
- **POST** `api/auth/login` with the following JSON payload to get a JWT token:

## To login as admin use the below json payload:
```json
{
  "username": "admin",
  "password": "admin123"
}
```
## To login as user use the below json payload:
```json
{
  "username": "user1",
  "password": "user123"
}
```

The response will contain the JWT token.

**Authorization Header:**
For protected endpoints, you need to pass the JWT token in the `Authorization` header like this:

```bash
Authorization: Bearer <your-jwt-token>
```

### 7. Testing the API

You can test the API using **Swagger UI**.

- **Swagger UI**: Navigate to `http://localhost:5235/swagger` to interact with the API.

### 8. Endpoints

- **POST api/tasks** 
    - Create a new task (Admin only).
    - Body:
    ```json 
    {
      "title": "string",
      "description": "string",
      "userId": 0
    }
    ```

- **GET api/tasks/{id}**
    - Get task details by ID(Admin and User).
    - Example: `api/tasks/1`

- **GET api/tasks/user/{userId}**
    - Get tasks assigned to a specific user(Admin and User).
    - Example: `api/tasks/user/1`

### 9. Additional Notes

- This project uses **Entity Framework Core** for database interactions.
- Role management is done using [Authorize(Roles = "RoleName")] attributes.
- JWT authentication is set up with basic middleware in `Startup.cs`.
- Seed data is included for initial testing purposes (tasks and users).
- Serilog is used for logging errors.
- Global Exception Handling ensures better error response and avoids crashes

# Task Management API - Test Suite

## Test Overview

This test suite validates the core functionality of the Task Management API using xUnit and Moq with an in-memory database.

##  Test Cases

### Task Controller Tests

| Test Method | Description | Assertions |
|-------------|-------------|------------|
| `GetTaskById_ReturnsTask_WhenExists` | Verifies successful task retrieval | Returns 200 OK with correct task |
| `GetTaskById_ReturnsNotFound_WhenNotExists` | Tests missing task handling | Returns 404 Not Found |
| `CreateTask_ReturnsCreatedResult_WhenValid` | Validates task creation | Returns 201 Created, verifies DB count |
| `GetTasksByUserId_ReturnsFilteredResults` | Checks user-specific task filtering | Returns correct task count for user |
| `CreateTask_ReturnsBadRequest_WhenInvalid` | Tests validation for empty title | Returns 400 Bad Request |
| `GetTasksByUserId_ReturnsEmpty_WhenNoTasksExist` | Verifies empty result handling | Returns empty list |


## Running Tests
### Prerequisites
.NET 6.0+ SDK

IDE with test runner (VS, VS Code, Rider)

## Test Dependencies
dotnet add TaskManagementAPI.Tests package Microsoft.EntityFrameworkCore.InMemory --version 6.0.0
dotnet add TaskManagementAPI.Tests package Moq --version 4.16.1
dotnet add TaskManagementAPI.Tests package xunit --version 2.4.1
dotnet add TaskManagementAPI.Tests package xunit.runner.visualstudio --version 2.4.1
dotnet add TaskManagementAPI.Tests package coverlet.collector --version 3.2.0

Execute Tests
bash
## Run all tests
dotnet test

## Run specific test class
dotnet test --filter "FullyQualifiedName~TaskControllerTests"

## Generate coverage report
dotnet test --collect:"XPlat Code Coverage"


