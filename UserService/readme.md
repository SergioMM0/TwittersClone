# UserService README

## Overview
UserService facilitates user management in a social media application context. It provides functionalities for creating users, retrieving user information, and managing user data. The service communicates with other parts of the application through RabbitMQ for certain operations, ensuring a decoupled architecture.

## Key Components
- **UserController**: Exposes HTTP endpoints for user operations such as creating a new user and fetching user details.
- **UserServiceMessageHandler**: Handles incoming RabbitMQ messages that require user management actions.
- **UserManager**: Contains the logic for managing users, including creation and data retrieval.
- **UserRepository**: Responsible for direct database access, utilizing Entity Framework Core for operations on the `User` entity.
- **DatabaseContext**: The EF Core context used for database interactions, central to the repository pattern implemented.
- **User**: Entity representing the user, encapsulating user data within the system.

## Technology Stack
- **ASP.NET Core**: For the RESTful API layer.
- **RabbitMQ**: As the messaging system for async communication.
- **Entity Framework Core**: For ORM capabilities.
- **Docker**: For containerization of the UserService.

## Setup and Running Instructions

### Prerequisites
- Docker and Docker Compose installed.
- A running instance of RabbitMQ, preferably containerized within the same Docker Compose environment.

### Docker Configuration
Include UserService in your `docker-compose.yml` to manage it alongside other services:

```yaml
services:
  user-service:
    build: ./UserService
    ports:
      - "87:80"
    depends_on:
      - rabbitmq
```

### Running the Service
Deploy UserService with the following command, ensuring all dependencies are up and running:

```sh
docker-compose up --build user-service
```

## Usage
Utilize UserService through its provided endpoints for various user-related actions:

- **Create a User**: `POST /user` with a JSON body including `Username` and `Password`.
- **Get User by ID**: `GET /user?id={userId}` to retrieve user details by their unique identifier.
- **Get All Users**: `GET /user/all` to fetch a list of all users.

## Integration Points
- UserService is integral to the system, interfacing with other services for actions requiring user validation or notification, mediated through RabbitMQ messages.

## Docker Setup
The service's deployment is streamlined through Docker, ensuring an isolated and consistent environment. `docker-compose.yml` orchestrates UserService in conjunction with the necessary infrastructure services like RabbitMQ.
