# NotificationService README

## Overview
NotificationService is a critical component of a social media application ecosystem, designed to manage and dispatch notifications. It reacts to various events within the system, such as new posts or likes, by sending notifications to relevant users. This service listens for specific events via RabbitMQ messages, processes these events, and then triggers notifications accordingly.

## Key Components
- **MessageHandler**: Listens for messages related to notification events, such as new posts or likes, through RabbitMQ.
- **NotificationManager**: Processes incoming events and manages the creation and dispatching of notifications.
- **Notification**: Entity representing a notification, containing details like the message, recipient, and related entity (e.g., post or like).

## Integrations
- **PostManager & LikeManager**: These managers send messages to the NotificationService via RabbitMQ when a new post is created or a like is added, triggering the notification process.

## Technology Stack
- **ASP.NET Core**: For service implementation.
- **RabbitMQ**: For messaging and event-driven architecture.
- **Entity Framework Core**: ORM for database interactions.
- **Docker**: Used for containerization and deployment.

## Setup and Running Instructions

### Prerequisites
- Docker and Docker Compose are installed.
- RabbitMQ instance running as part of the service orchestration.

### Docker Configuration
NotificationService is defined within `docker-compose.yml` for easy orchestration:

```yaml
services:
  notification-service:
    build: ./NotificationService
    ports:
      - "84:80"
    depends_on:
      - rabbitmq
```

### Running the Service
To start NotificationService along with its dependencies, run:

```sh
docker-compose up --build notification-service
```

## Usage
NotificationService automates the process of sending notifications, and does not expose direct HTTP endpoints for interaction. Instead, it listens for specific events (e.g., post creation, new like) broadcasted over RabbitMQ:

- **New Post Notification**: Triggered by `PostManager` sending a message upon post creation.
- **New Like Notification**: Initiated by `LikeManager` when a like is added to a post.

## Docker Setup
The service is containerized, ensuring consistency across development, testing, and production environments. The Dockerfile specifies the environment setup, and `docker-compose.yml` orchestrates the deployment alongside RabbitMQ and other services.