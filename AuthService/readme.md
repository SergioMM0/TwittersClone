# AuthService README

## Overview
AuthService is a microservice within a larger application ecosystem, designed to handle user authentication requests. It operates asynchronously, receiving login requests via RabbitMQ, generating tokens, and responding with these tokens through RabbitMQ messages.

## Key Components
- **MessageHandler**: Listens for `GenerateTokenMsg` messages on RabbitMQ, and triggers token generation.
- **AuthenticationService**: Generates tokens for authenticated users and sends them back via RabbitMQ using `LoginMsg`.
- **Docker Configuration**: Containerized setup defined in `Dockerfile` and orchestrated with `docker-compose.yml`.

## Technology Stack
- ASP.NET Core
- RabbitMQ for messaging
- Docker for containerization

## Running the Service
Ensure Docker and Docker Compose are installed. Run the following command from the root directory where `docker-compose.yml` is located:

```sh
docker-compose up --build
```

This command builds and starts all services, including AuthService, API, and RabbitMQ.

## Interactions
1. **Auth Requests**: AuthController in the API service sends login requests to AuthService via RabbitMQ.
2. **Token Generation**: Upon receiving a request, AuthService generates a token and responds back.
3. **Service Communication**: Uses RabbitMQ for asynchronous message passing, allowing decoupled service interaction.

## Endpoints
- **Login**: POST `http://localhost/api/auth` with a JSON body containing `Username` and `Password`.

## Docker Setup
- AuthService is containerized and defined within `docker-compose.yml` alongside other services like RabbitMQ.
- Exposes its functionality through RabbitMQ messages rather than direct HTTP endpoints for internal service communication.

## Environment
- Configured primarily for development and testing with the potential for production use with further security and scalability considerations.