# FollowerService README

## Overview
FollowerService manages follower relationships within a social application. It allows users to add, remove followers, fetch a list of followers, and toggle notification preferences. Communication with other services is achieved via RabbitMQ, ensuring asynchronous and decoupled interactions.

## Key Components
- **FollowerController**: HTTP endpoints for managing followers.
- **MessageHandler**: Listens and processes RabbitMQ messages for follower operations.
- **FollowingManager**: Business logic for follower management.
- **FollowersRepository**: Data access layer for CRUD operations on followers.
- **DatabaseContext**: Entity Framework Core context for database interactions.
- **Follower**: The entity that represents a follower relationship.

## Technology Stack
- **ASP.NET Core**: For RESTful API implementation.
- **RabbitMQ**: Messaging system for inter-service communication.
- **Entity Framework Core**: ORM for database operations.
- **Docker**: Containerization of the service.

## Setup and Run

### Prerequisites
- Docker and Docker Compose installed.
- RabbitMQ server running (configured in `docker-compose.yml`).

### Docker Configuration
Add the service to your `docker-compose.yml`:

```yaml
services:
  followers-service:
    build: ./FollowersService
    ports:
      - "82:80"
    environment:
      ASPNETCORE_ENVIRONMENT: Production
    depends_on:
      - rabbitmq
```

### Running the Service
To start the FollowerService along with its dependencies:

```sh
docker-compose up --build followers-service
```

## Usage
- **Add a Follower**: `POST /follower` with user and follower IDs in the body.
- **Remove a Follower**: `DELETE /follower?userId={userId}&followerId={followerId}`.
- **Fetch Followers**: `GET /follower?userId={userId}`.
- **Toggle Notifications**: `PUT /follower` with user and follower IDs, and notification setting in the body.

## Docker Setup
The service is containerized for deployment ease and scalability. Check the Dockerfile for build specifics and `docker-compose.yml` for orchestration details.

## Development Environment
While the default environment is set to Production, adjust `ASPNETCORE_ENVIRONMENT` in `docker-compose.yml` for different setups.