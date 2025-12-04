📘 README.md
Final Project – Cloud-Native Book Management API

Author: Katelyn Flemke
Course: Cloud Computing – CS-432-A
Instructor: Professor Rosa

📚 Project Overview

The Book Management API is a cloud-native application designed to demonstrate production-grade architecture within Microsoft Azure. It expands on the earlier midterm RESTful API by introducing persistent storage, secure secret management, automated validation, observability, and cloud governance best practices.

The system is built using ASP.NET Core, Entity Framework Core, Azure SQL Database, Azure Key Vault, Azure Logic Apps, and Azure Application Insights. The API supports full CRUD (Create, Read, Update, Delete) operations for books while enforcing API-key authentication. A new endpoint—PATCH /api/books/validate—applies custom validation logic that marks outdated books as archived and records audit timestamps.

In a production deployment, the API retrieves its authentication key from Azure Key Vault using a managed identity, and a Logic App securely automates validation on a schedule. Application Insights captures request telemetry, failures, dependency calls, and custom events such as ValidationTriggered. A custom Azure Dashboard aggregates API health metrics, database performance, Key Vault access events, and Logic App run history.

This project demonstrates secure, auditable, and automated cloud-native application design aligned with real-world industry standards.

⚙️ Setup Instructions
1. Prerequisites

.NET 8 SDK

Visual Studio, VS Code, or Rider

Azure subscription (for cloud deployment)

Azure SQL Database (optional)

Azure Key Vault (optional)

Azure Logic Apps (optional)

2. Running Locally (Development Mode)

Clone the project:

git clone <repo-url>
cd FinalProjectAPI


Restore dependencies:

dotnet restore


Build the project:

dotnet build


Run the API:

dotnet run


Open Swagger UI:

https://localhost:<port>/swagger


Add the required API key header to all requests:

x-api-key: dev-local-api-key-123

3. Azure Deployment (Conceptual Steps)

Create Azure SQL Database

Add connection string to appsettings.json.

Create Azure Key Vault

Add secret: ApiKey

Grant API + Logic App managed identities access to secrets.

Deploy API to Azure App Service

Enable system-assigned managed identity

Enable Application Insights monitoring

Configure Key Vault + SQL connection settings

Create a Logic App

Trigger on a schedule

Retrieve ApiKey from Key Vault

Call PATCH /api/books/validate

Optional: send Teams/email notifications

📡 API Reference

All endpoints require the following header:

x-api-key: <your-api-key>

➤ GET /api/books

Returns all books.

Response:

200 OK

List of Book objects

➤ GET /api/books/{id}

Returns a single book by ID.

Responses:

200 OK

404 Not Found

➤ POST /api/books

Creates a new book.

Request Body Example:

{
  "title": "Empire of Storms",
  "author": "Sarah J. Maas",
  "year": 2016
}


Responses:

201 Created

400 Bad Request (invalid or missing fields)

➤ PUT /api/books/{id}

Updates an existing book.

Responses:

204 No Content

400 Bad Request (ID mismatch or invalid input)

➤ DELETE /api/books/{id}

Deletes a book.

Responses:

204 No Content

404 Not Found

⭐ ➤ PATCH /api/books/validate

Runs batch validation on all books.

Validation Rule:

If Year < currentYear – 10, set:

Archived = true
LastValidated = <current timestamp>


Response Example:

{
  "updatedCount": 5,
  "timestamp": "2025-10-31T13:00:00Z"
}


This endpoint is also triggered automatically via Azure Logic Apps.