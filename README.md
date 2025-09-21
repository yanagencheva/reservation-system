# Reservation System Solution

## Overview
This solution provides a modular reservation system built with **C# 13.0** and **.NET 9**.  
It includes components for managing reservations, handling successful reservation messages, database persistence, and application initialization.  
The architecture leverages **dependency injection**, **Entity Framework Core**, and robust **logging**.

---

## Projects

- **ReservationRestorantTable**  
  Main application entry point. Configures services, logging, and runs the host.

- **Reservation.Persistence**  
  Contains `ReservationDbContext` and data access logic. Handles database migrations and interactions.

- **Reservation.SuccessfulMessages**  
  Provides services for storing and logging successful reservation messages.

- **Reservation.Common**  
  Shared models and enums, such as `ResultMessage` and `ValidationResults`.

- **Reservation.Initializer**  
  Handles application and database initialization.

- **Reservation.Infrastructure**  
  Contains infrastructure services such as `IRabbitMqConsumerService` interface.  
  Provides the contract for consuming and publishing messages to RabbitMQ queues.

- **BackgroundWorkers**  
  Contains background workers that handle RabbitMQ message processing and file monitoring.

---

## Background Workers

### 1. `ProcessFailedMessages`
- **Purpose:**  
  Processes messages from the `FAILED` queue and stores them in persistent storage for later inspection or retry.

- **Workflow:**  
  1. Runs continuously as a background service.  
  2. Consumes messages from the `FAILED` queue using `IRabbitMqConsumerService`.  
  3. Saves each failed message via `IFailedMessagesService`.  
  4. Logs information about processed messages.

---

### 2. `ProcessSuccessfulMessages`
- **Purpose:**  
  Processes messages from the `SUCCESSED` queue, stores them, and optionally publishes a response message to the `RESPONSE` queue.

- **Workflow:**  
  1. Runs continuously as a background service.  
  2. Consumes messages from the `SUCCESSED` queue.  
  3. Saves each message via `ISuccessfulMessagesService`.  
  4. If a result object is returned, serializes it and publishes it to the `RESPONSE` queue.  
  5. Logs information about processed and published messages.

---

### 3. `FileMonitorWorker`
- **Purpose:**  
  Monitors a configured folder for new `.TXT` files, parses them as SWIFT messages, saves them to storage, and moves them to `successful` or `failed` subfolders.

- **Workflow:**  
  1. Runs as a background service and watches the specified folder.  
  2. Detects new `.TXT` files using `FileSystemWatcher`.  
  3. Waits until the file is fully written before processing.  
  4. Reads the file content and parses it as a SWIFT message.  
  5. Saves the parsed message using `ISwiftMessageService`.  
  6. Moves the file to `successful` or `failed` subfolder based on the result.  
  7. Logs all steps, including parsing success/failure and file moves. 
