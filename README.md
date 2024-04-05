# TwittersClone
Compulsory assignment for System Integration
![image](https://github.com/SergioMM0/TwittersClone/assets/90683062/70535833-ab15-4dc3-ae33-630c86ae1f5d)

## Project Overview
This project is a microservices-based social media application designed to demonstrate a decoupled, scalable architecture. Services communicate asynchronously via RabbitMQ, handling distinct aspects of the application such as user authentication, post management, follower relationships, likes, and notifications.

## Services Overview

- **API**: The gateway to the application, exposing HTTP endpoints to the outside world and communicating with other services for data retrieval and action execution.
- **AuthService**: Manages user authentication, issuing tokens upon successful login attempts.
- **FollowersService**: Handles operations related to followers, including adding and removing followers and toggling notification settings.
- **LikeService**: Allows users to like posts, managing like counts and user-post relationships.
- **NotificationService**: Sends notifications to users about various events like new posts from followed users or likes on their posts.
- **PostService**: Central to content creation, enabling users to create, delete, and fetch posts.
- **UserService**: Manages user information, including creation and retrieval of user details.
- **RabbitMQMessages**: Defines the messages used for inter-service communication, encapsulating the data transferred between services.

## Architecture Highlights

- **Microservices Architecture**: Each service runs in its own container, ensuring isolation, scalability, and resilience.
- **Asynchronous Messaging**: Services communicate through RabbitMQ, decoupling them and allowing for scalable, independent processing.
- **Entity Framework Core**: Used for ORM across services interacting with databases.
- **Docker Compose**: Orchestrates the containers, ensuring they are built and started with their dependencies satisfied.

## Running the Project

Ensure Docker and Docker Compose are installed. Navigate to the root directory containing the `docker-compose.yml` file and run:

```sh
docker-compose up --build
```

This command builds and starts all services, including RabbitMQ, setting up the entire application stack.

## Accessing Services

- **RabbitMQ Management Interface**: `http://localhost:15672` to monitor message queues and throughput.
- **API Gateway**: Accessed via `http://localhost`, serving as the entry point for interacting with the application through defined endpoints.

## Development and Production Environments

The `docker-compose.yml` file is pre-configured for production environments with `ASPNETCORE_ENVIRONMENT` set to `Production`. Adjustments can be made for development or staging environments as necessary.

## Volumes

Persistent volumes are used for services with databases (`FollowersService`, `LikeService`, `PostService`, `UserService`), ensuring data persistence across container restarts and rebuilds.

## Dependencies

- **RabbitMQ**: Critical for inter-service communication, must be up and running before other services start. Health checks in `docker-compose.yml` ensure this order is maintained.

## Scalability and Resilience

- Services can be scaled independently based on load.
- RabbitMQ ensures message delivery even if a service is temporarily down, enhancing the system's overall resilience.

## Conclusion

This social media application demonstrates a practical application of microservices architecture principles, asynchronous communication patterns, and containerization for a scalable, maintainable software ecosystem.
