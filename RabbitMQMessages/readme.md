# RabbitMQMessages

## Overview
The `RabbitMQMessages` project centralizes all message definitions used across various services in a social media application. This project facilitates a decoupled architecture by defining a contract for communication between services through RabbitMQ, a message broker.

## Structure
The project is organized into folders, each corresponding to a specific service within the application ecosystem, such as `AuthService`, `UserService`, `PostService`, `LikeService`, `FollowerService`, and `NotificationService`. Each folder contains message definitions relevant to operations and events associated with its service.

## Message Types
- **Request Messages**: Initiate a specific action or request data. Examples include login requests, post creation requests, or follower addition requests.
- **Response Messages**: Return the result of a request, such as the outcome of a login attempt or the details of a requested post.
- **Event Messages**: Notify other services of changes that may affect them. For example, a new post event message might trigger the NotificationService to alert followers.

## Usage
Services send and receive messages defined in this project to perform operations asynchronously. This includes user authentication, post management, liking content, managing followers, and sending notifications.

## Benefits
- **Decoupling**: Services can operate independently, communicating through well-defined messages rather than direct database access or API calls.
- **Scalability**: The system can scale parts independently, adjusting resources where needed based on message load.
- **Resilience**: The architecture allows for more resilient systems that can handle partial failures gracefully.

## Integration with RabbitMQ
Each message type is designed to be serialized into JSON and transmitted over RabbitMQ, allowing for language-agnostic communication. Services subscribe to specific queues and topics to process incoming messages and publish their own messages to inform other parts of the system.
