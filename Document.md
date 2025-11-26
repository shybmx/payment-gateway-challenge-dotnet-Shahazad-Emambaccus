# Shahzad Emambaccus Checkout code test - Payment Gateway

# Key Design Considerations

## 1. Modular Architecture
- **Rationale**: The project is structured into distinct modules (e.g., `Controllers`, `Services`, `Models`, etc.) to ensure separation of concerns and maintainability.
- **Impact**: This design allows for easier testing, debugging, and future enhancements.

## 2. Dependency Injection
- **Rationale**: Dependency injection is used to manage dependencies, as seen in the `Installers` folder.
- **Impact**: Promotes loose coupling and makes the application more testable.

## 3. Middleware
- **Rationale**: Middleware components, such as `ValidateExpiryMiddleware`, are used to validate the reqeust.
- **Impact**: Ensures a clean and reusable pipeline for request processing.
    
## 4. Configuration Management
- **Rationale**: Configuration files (`appsettings.json`, `appsettings.Development.json`) are used to manage environment-specific settings.
- **Impact**: Simplifies deployment and ensures flexibility in different environments.

## 5. Unit Testing
- **Rationale**: Comprehensive unit tests are implemented for controllers, services, and middleware.
- **Impact**: Ensures code reliability and reduces the likelihood of regressions.

## 6. RESTful API Design
- **Rationale**: The API follows RESTful principles, with clear endpoints and HTTP methods.
- **Impact**: Ensures consistency and ease of use for API consumers.

## 7. Validation
- **Rationale**: Input validation is implemented to ensure data integrity and security.
- **Impact**: Prevents invalid or malicious data from being processed.

## 8. Mocking for Tests
- **Rationale**: Mock objects, such as `MockHttpMessageHandler`, are used to isolate components during testing.
- **Impact**: Enables testing in isolation without relying on external dependencies.

## 9. Missing Elements
- **Provisioning**: The repository lacks provisioning scripts for provisioning cloud resources.
- **CI/CD Pipeline**: There is no defined continuous integration or deployment pipeline to automate testing and deployment.
- **Acceptance Criteria (AC) Testing**: Tests to validate the application end to end is not implemented.
- **Integration Testing**: Tests to ensure that different modules or services work together as expected are not present.
- **Security Scanning on the Container**: The repository does not include tools or processes for scanning the container image for vulnerabilities.

## 10. Assumptions
- Any invalid parameter in the POST request will result in a `400 Bad Request` response, with details about what is incorrect, instead of a `Rejected` status.
- A `Rejected` status will only be returned when the application fails to communicate with the bank.
- The GET endpoint will only return a `200 OK` response if the resource is found or a `404 Not Found` response if the resource does not exist.

