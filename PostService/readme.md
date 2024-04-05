# PostService README

## Overview
PostService is integral to a social media application, enabling users to create, delete, and fetch posts. It serves as the backbone for managing user-generated content. Through RabbitMQ, the service also communicates with the NotificationService to inform about new posts, enhancing user engagement through notifications.

## Key Components
- **PostController**: Offers HTTP endpoints for post operations such as creation, deletion, and retrieval.
- **PostServiceMessageHandler**: Handles RabbitMQ messages pertinent to post events, facilitating interaction with other services.
- **PostManager**: Implements the core logic for post management including notification dispatch upon post creation.
- **PostRepository**: Manages data persistence, leveraging Entity Framework Core for database interactions.
- **DatabaseContext**: The EF Core context responsible for database transactions.
- **Post**: The primary entity representing a post within the application.

## Technology Stack
- **ASP.NET Core**: Framework for building web APIs.
- **RabbitMQ**: Messaging queue for inter-service communication.
- **Entity Framework Core**: ORM for database interactions.
- **Docker**: Utilized for containerization and deployment.

## Setup and Running Instructions

### Prerequisites
- Docker and Docker Compose must be installed.
- Ensure RabbitMQ is running as part of your docker-compose setup.

### Docker Configuration
Integrate the service into your `docker-compose.yml` for seamless orchestration:

```yaml
services:
  post-service:
    build: ./PostService
    ports:
      - "85:80"
    depends_on:
      - rabbitmq
```

### Running the Service
Launch the PostService alongside its dependencies with the following command:

```sh
docker-compose up --build post-service
```

## Usage
Interact with the PostService through its defined endpoints:

- **Create a Post**: `POST /post` with the post details in the body.
- **Delete a Post**: `DELETE /post` with the `Id` of the post to be deleted in the body.
- **Get a Post by ID**: `GET /post?id={id}`.
- **Get All Posts**: `GET /post/all` for a list of all posts.

## Integration with NotificationService
- On creating a new post, PostManager sends a message via RabbitMQ to NotificationService, triggering notifications to followers about the new content.

## Docker Setup
Containerization ensures that the PostService can be deployed consistently across all environments. The Dockerfile specifies the setup, while `docker-compose.yml` orchestrates the service deployment in conjunction with RabbitMQ and other dependent services.
