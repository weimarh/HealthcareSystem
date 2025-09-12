----HEALTHCARE SYSTEM MICROSERVICES----

PROJECT OVERVIEW
This project is a microservices-based healthcare system designed to manage and maintain comprehensive patient records, including their medical history, lab results, and personal information. The system is comprised of three distinct services that communicate asynchronously to ensure a scalable and robust architecture.

The core purpose is to provide a unified record of:
* Patient personal details and emergency contacts.
* Medical history, including appointments, diagnoses, medications, and family history.
* Lab test orders, available tests, and their corresponding results.

ARCHITECTURE
* The system is built on a microservices architecture with a shared communication bus and separate databases.  This design ensures that each service can be developed, deployed, and scaled independently.
* Communication: All inter-service communication is handled via RabbitMQ using MassTransit.
* Databases: Data is persisted in separate databases to maintain service autonomy. The LabManagementService uses a SQL Server database, while the PatientManagementService and ElectronicHealthRecordsService use a MongoDB database.
* Architectural Patterns: The LabManagementService employs a Vertical Slice Architecture with the CQRS (Command Query Responsibility Segregation) pattern, while the PatientManagementService and ElectronicHealthRecordsService use a Layered Architecture.

SERVICE-TO-SERVICE RELATIONSHIPS
While the LabManagementService and ElectronicHealthRecordsService do not communicate directly, they both rely on the PatientManagementService to retrieve core patient information. The PatientManagementService provides a foundational layer of patient data, which the other two services can access as needed.

SERVICES
1. Patient Management Service
This service is the source of truth for all basic patient demographic information. It keeps track of patient names, IDs, addresses, and emergency contacts. It publishes events to the RabbitMQ message bus for every data mutation.

API Endpoints
GET /api/patients
Returns a list of all patients.

GET /api/patients/{id}
Returns a single patient by ID.

POST /api/patients
Creates a new patient. Publishes a PatientCreated event.

PUT /api/patients/{id}
Updates a patient by ID. Publishes a PatientUpdated event.

DELETE /api/patients/{id}
Deletes a patient by ID. Publishes a PatientDeleted event.

2. Electronic Health Records Service
This service manages comprehensive medical records for each patient. It keeps track of medical appointments, family histories, past illnesses, allergies, and medications.

API Endpoints
GET /api/MedicalAppointments
Returns a list of all medical appointments.

GET /api/MedicalAppointments/{id}
Returns a single medical appointment by ID.

POST /api/MedicalAppointments
Creates a new medical appointment.

PUT /api/MedicalAppointments/{id}
Updates a medical appointment by ID.

DELETE /api/MedicalAppointments/{id}
Deletes a medical appointment by ID.

GET /api/medicalhistories
Returns a list of all medical histories.

GET /api/medicalhistories/{id}
Returns a single medical history by ID.

POST /api/medicalhistories
Creates a new medical history record.

PUT /api/medicalhistories/{id}
Updates a medical history by ID.

DELETE /api/medicalhistories/{id}
Deletes a medical history by ID.

3. Lab Management Service
This service is dedicated to managing all aspects of lab testing. It tracks available lab tests, patient lab orders, and the corresponding results. It utilizes a CQRS pattern with MediatR for clean separation of concerns.

API Endpoints
GET /api/tests
Returns a list of all available lab tests.

GET /api/tests/{id}
Returns a single lab test by ID.

POST /api/tests
Creates a new lab test.

PUT /api/tests/{id}
Updates a lab test by ID.

DELETE /api/tests/{id}
Deletes a lab test by ID.

GET /api/results
Returns a list of all lab results.

GET /api/results/{id}
Returns a single lab result by ID.

POST /api/results
Creates a new lab result.

PUT /api/results/{id}
Updates a lab result by ID.

DELETE /api/results/{id}
Deletes a lab result by ID.

GET /api/orders
Returns a list of all lab orders.

GET /api/orders/{id}
Returns a single lab order by ID.

POST /api/orders
Creates a new lab order.

PUT /api/orders/{id}
Updates a lab order by ID.

PUT /api/orders/results/{id}
Associates a lab result with a lab order.

DELETE /api/orders/{id}
Deletes a lab order by ID.

TECHNOLOGY STACK
Backend: .NET 8

Databases: MongoDB and SQL Server

Messaging: RabbitMQ

Libraries: MassTransit (for RabbitMQ messaging) and MediatR (for CQRS).

GETTING STARTED
Prerequisites
.NET 8 SDK

RUNNING THE SOLUTION


To run the services locally, first you will need to run MongoDb at localhost:27017; run SQL Server with this connections tring: "DefaultConnection": "Server=DESKTOP-C0KE7DB;Database=LabManagementDB;Trusted_Connection=True;TrustServerCertificate=True"; and run RabbitMQ al localhost:152672; then you will need to start each one from its respective project directory.

Start the Patient Management Service:
cd PatientManagementService
dotnet run

Start the Electronic Health Records Service:
cd ElectronicHealthRecordsService
dotnet run

Start the Lab Management Service:
cd LabManagementService
dotnet run

Contributing
For more information on the individual projects, please refer to their respective README files located in their project directories.
