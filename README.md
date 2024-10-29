# TinyUrls Project

## Overview

The TinyUrls project consists of multiple API implementations and library projects. Each API implementation demonstrates a different architectural approach to building a URL shortening service. The library projects provide shared functionality and utilities used across the APIs.

## API Implementations

### 1. Vertical Slices API

#### Description

The Vertical Slices API follows the vertical slice architecture, where each feature is implemented end-to-end, including its own request, handler, and data access logic. This approach promotes modularity and separation of concerns.

#### Key Components

- **Requests and Handlers**: Each feature has its own request and handler classes.
- **Dependency Injection**: MediatR is used for handling requests and responses.
- **In-Memory Database**: Entity Framework Core with an in-memory database is used for data storage.

#### Example Files

- `Slices.cs`: Contains request and handler implementations.
- `Program.cs`: Configures services and sets up the application.

### 2. Controller-Based API

#### Description

The Controller-Based API follows the traditional ASP.NET Core MVC approach, where controllers handle HTTP requests and delegate business logic to services.

#### Key Components

- **Controllers**: Handle HTTP requests and return responses.
- **Services**: Contain business logic and interact with the database.
- **Dependency Injection**: Services are injected into controllers.

#### Example Files

- `Controllers/TinyUrlController.cs`: Contains controller actions for creating and retrieving short URLs.
- `Program.cs`: Configures services and sets up the application.

### 3. Minimal API

#### Description

The Minimal API approach uses the new minimal hosting model introduced in ASP.NET Core 6.0. It provides a lightweight way to build APIs with minimal boilerplate code. This approach emphasizes functional paradigms, making it easier to define and compose HTTP endpoints in a concise and readable manner.

#### Key Components

- **Endpoints**: Defined directly in the `Program.cs` file using lambda expressions and local functions.
- **Dependency Injection**: Services are configured and injected as needed.
- **In-Memory Database**: Entity Framework Core with an in-memory database is used for data storage.

#### Functional Paradigms

- **Lambda Expressions**: Endpoints are defined using lambda expressions, which promote a functional style of programming.
- **Local Functions**: Helper functions can be defined locally within the `Program.cs` file to encapsulate logic and improve readability.
- **Immutable Data**: Emphasis on using immutable data structures and avoiding side effects.

#### Example Files

- `Program.cs`: Contains endpoint definitions and service configurations.

## Library Projects

### 1. TinyUrls.Persistence

#### Description

The `TinyUrls.Persistence` project contains the data access layer, including the Entity Framework Core DbContext and entity configurations.

#### Key Components

- **DbContext**: Defines the database context and DbSet properties for entities.
- **Entity Configurations**: Configure entity properties and relationships.

### 2. TinyUrls.Prelude.Shortner

#### Description

The `TinyUrls.Prelude.Shortner` project provides the URL shortening logic, including the algorithm for generating short codes.

#### Key Components

- **Shortner**: Contains methods for generating and validating short codes.
- **Configuration**: Defines settings for the URL shortening algorithm.

### 3. TinyUrls.Types

#### Description

The `TinyUrls.Types` project contains shared types and models used across the APIs and library projects.

#### Key Components

- **Entities**: Define the data models for the application.
- **Value Objects**: Represent complex types with behavior.

## Configuration

### appsettings.json

Each API project includes an `appsettings.json` file for configuring application settings, such as logging and the URL shortener configuration.

#### Example Configuration

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ShortnerConfig": {
    "MaxLength": 7,
    "Alphabet": "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"
  }
}