# LikeService README

## Overview
LikeService is designed to manage likes within a social media application, allowing users to like posts. It handles like operations through RabbitMQ for asynchronous messaging and provides endpoints for managing likes directly.

## Key Components
- **LikeController**: Exposes HTTP endpoints for adding and managing likes on posts.
- **LikeServiceMessageHandler**: Listens for RabbitMQ messages related to like operations and delegates them to the LikeManager for processing.
- **LikeManager**: Implements the logic for adding likes to posts, checking if a post is liked by a user, and counting likes on a post.
- **LikeRepository**: Communicates with the database to perform CRUD operations related to likes.
- **DatabaseContext**: Entity Framework Core context for database interactions, ensuring smooth operations with the Like entity.
- **Like**: Entity that represents a like, containing references to the post and the user who liked the post.

## Technology Stack
- **ASP.NET Core**: For building RESTful APIs.
- **RabbitMQ**: For asynchronous inter-service communication.
- **Entity Framework Core**: As the ORM for interacting with the database.
- **Docker**: For containerizing the service and ensuring consistent, isolated environments.

## Setup and Running Instructions

### Prerequisites
- Docker and Docker Compose must be installed on your machine.
- A RabbitMQ instance running and accessible for messaging.

### Docker Configuration
This service is part of a docker-compose setup, which includes other services like RabbitMQ and API gateways:

```yaml
services:
  like-service:
    build: ./LikeService
    ports:
      - "83:80"
    environment:
      ASPNETCORE_ENVIRONMENT: Production
    depends_on:
      - rabbitmq
```

### Running the Service
Execute the following command in the root directory of your project where `docker-compose.yml` is located:

```sh
docker-compose up --build like-service
```

## Usage

### Adding a Like
- **Endpoint**: `POST /like`
- **Body**: Contains `PostId` and `UserId` indicating which post is being liked by which user.

### Fetching Likes
- The service provides endpoints for fetching likes for a post, checking if a user has liked a post, and counting the total number of likes on a post.

## Docker Setup
The service is fully containerized for easy deployment. The Dockerfile specifies the build and runtime environment, while `docker-compose.yml` orchestrates the service deployment alongside its dependencies.

## Development Environment
While the service defaults to the Production environment when deployed via Docker Compose, it can be adjusted to Development for local testing and debugging purposes by changing the `ASPNETCORE_ENVIRONMENT` variable.
