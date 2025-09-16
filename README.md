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
  Method: GET
  Route: /api/patients
  Returns: ActionResult<IEnumerable<PatientDto>>
  Response:
    200 OK with list of all patients as PatientDto objects
    Returns empty list if no patients exist

  GET /api/patients/{id}
  Method: GET
  Route: /api/patients/{id}
  Returns: ActionResult<PatientDto>
  Response:
    200 OK with single PatientDto if patient found
    404 Not Found if patient with specified ID doesn't exist

  POST /api/patients
  Method: POST
  Route: /api/patients
  Body: CreatePatientDto
  Returns: ActionResult<PatientDto>
  Response:
    201 Created with Location header pointing to the new resource
    Publishes event: PatientCreated message via MassTransit
    Returns the created patient data

  PUT /api/patients/{id}
  Method: PUT
  Route: /api/patients/{id}
  Body: UpdatePatientDto
  Returns: IActionResult
  Response:
    204 No Content if update successful
    404 Not Found if patient doesn't exist
    Publishes event: PatientUpdated message via MassTransit

  DELETE /api/patients/{id}
  Method: DELETE
  Route: /api/patients/{id}
  Returns: IActionResult
  Response:
    204 No Content if deletion successful
    404 Not Found if patient doesn't exist
    Publishes event: PatientDeleted message via MassTransit

2. Electronic Health Records Service
This service manages comprehensive medical records for each patient. It keeps track of medical appointments, family histories, past illnesses, allergies, and medications.

API Endpoints
  GET /api/MedicalAppointments
  Method: GET
  Route: /api/MedicalAppointments (inherited from controller route)
  Returns: ActionResult<IEnumerable<MedicalAppointmentDto>>
  Response:
    200 OK with list of all medical appointments enriched with patient data
    Returns empty list if no appointments exist
    Note: Automatically joins with patient data for complete information

  GET /api/MedicalAppointments/{id}
  Method: GET
  Route: /api/MedicalAppointments/{id}
  Returns: ActionResult<MedicalAppointmentDto>
  Response:
    200 OK with single MedicalAppointmentDto if appointment found
    404 Not Found if appointment with specified GUID doesn't exist
    Includes patient data if patient exists, otherwise uses empty values.

  POST /api/MedicalAppointments
  Method: POST
  Route: /api/MedicalAppointments
  Body: CreateMedicalAppointmentDto
  Returns: ActionResult<MedicalAppointmentDto>
  Response:
    201 Created with Location header pointing to the new resource
    Returns the created medical appointment data
    Auto-generates: GUID ID for the new appointment

  PUT /api/MedicalAppointments/{id}
  Method: PUT
  Route: /api/MedicalAppointments/{id}
  Body: UpdateMedicalAppointmentDto
  Returns: IActionResult
  Response:
    204 No Content if update successful
    404 Not Found if appointment doesn't exist

  DELETE /api/MedicalAppointments/{id}
  Method: DELETE
  Route: /api/MedicalAppointments/{id}
  Returns: IActionResult
  Response:
    204 No Content if deletion successful
    404 Not Found if appointment doesn't exist

  GET /api/medicalhistories
  Method: GET
  Route: /api/medicalhistories
  Returns: ActionResult<IEnumerable<MedicalHistoryDto>>
  Response:
    200 OK with list of all medical histories enriched with patient data
    Returns empty list if no medical histories exist
    Automatically joins with patient data for complete information
    Uses dictionary lookup for optimal performance.

  GET /api/medicalhistories/{id}
  Method: GET
  Route: /api/medicalhistories/{id}
  Returns: ActionResult<MedicalHistoryDto>
  Response:
    200 OK with single MedicalHistoryDto if medical history found
    404 Not Found if medical history with specified GUID doesn't exist
    Includes patient data if patient exists, otherwise uses empty values

  POST /api/medicalhistories
  Method: POST
  Route: /api/medicalhistories
  Body: CreateMedicalHistoryDto
  Returns: ActionResult<MedicalHistoryDto>
  Response:
    201 Created with Location header pointing to the new resource
    Returns the created medical history data
    Auto-generates: GUID ID for the new medical history record

  PUT /api/medicalhistories/{id}
  Method: PUT
  Route: /api/medicalhistories/{id}
  Body: UpdateMedicalHistoryDto
  Returns: IActionResult
  Response:
    204 No Content if update successful
    404 Not Found if medical history doesn't exist

  DELETE /api/medicalhistories/{id}
  Method: DELETE
  Route: /api/medicalhistories/{id}
  Returns: IActionResult
  Response:
    204 No Content if deletion successful
    404 Not Found if medical history doesn't exist

3. Lab Management Service
This service is dedicated to managing all aspects of lab testing. It tracks available lab tests, patient lab orders, and the corresponding results. It utilizes a CQRS pattern with MediatR for clean separation of concerns.

API Endpoints
  GET /api/tests
  Method: GET
  Route: /api/tests
  Returns: IActionResult
  Response:
    200 OK with list of all lab tests when successful
    Problem Details with error descriptions when failed
    Uses CQRS pattern with Mediator

  GET /api/tests/{id}
  Method: GET
  Route: /api/tests/{id}
  Returns: IActionResult
  Response:
    200 OK with single lab test when found
    Problem Details with error descriptions when failed (including not found)

  POST /api/tests
  Method: POST
  Route: /api/tests
  Body: CreateLabTestRequest
  Returns: IActionResult
  Response:
    200 OK with created lab test when successful
    Problem Details with error descriptions when failed

  PUT /api/tests/{id}
  Method: PUT
  Route: /api/tests/{id}
  Body: UpdateLabTestRequest
  Returns: IActionResult
  Response:
    200 OK with updated lab test when successful
    400 Bad Request if ID mismatch between route and body
    Problem Details with error descriptions when failed

  DELETE /api/tests/{id}
  Method: DELETE
  Route: /api/tests/{id}
  Returns: IActionResult
  Response:
    200 OK with deleted lab test when successful
    Problem Details with error descriptions when failed

  GET /api/results
  Method: GET
  Route: /api/results
  Returns: IActionResult
  Response:
    200 OK with list of all lab results when successful
    Problem Details with error descriptions when failed
    Uses CQRS pattern with Mediator

  GET /api/results/{id}
  Method: GET
  Route: /api/results/{id}
  Returns: IActionResult
  Response:
    200 OK with single lab result when found
    Problem Details with error descriptions when failed (including not found)

  POST /api/results
  Method: POST
  Route: /api/results
  Body: CreateLabResultRequest
  Returns: IActionResult
  Response:
    200 OK with created lab result when successful
    Problem Details with error descriptions when failed

  PUT /api/results/{id}
  Method: PUT
  Route: /api/results/{id}
  Body: UpdateLabResultRequest (note: ID in body, not route)
  Returns: IActionResult
  Response:
    200 OK with updated lab result when successful
    Problem Details with error descriptions when failed

  DELETE /api/results/{id}
  Method: DELETE
  Route: /api/results/{id}
  Returns: IActionResult
  Response:
    200 OK with deleted lab result when successful
    Problem Details with error descriptions when failed

  GET /api/orders
  Method: GET
  Route: /api/orders
  Returns: IActionResult
  Response:
    200 OK with list of all lab orders when successful
    Problem Details with error descriptions when failed
    Uses CQRS pattern with Mediator

  GET /api/orders/{id}
  Method: GET
  Route: /api/orders/{id}
  Returns: IActionResult
  Response:
    200 OK with single lab order when found
    Problem Details with error descriptions when failed (including not found)

  POST /api/orders
  Method: POST
  Route: /api/orders
  Body: CreateLabOrderRequest
  Returns: IActionResult
  Response:
    200 OK with created lab order when successful
    Problem Details with error descriptions when failed

  PUT /api/orders/{id}
  Method: PUT
  Route: /api/orders/{id}
  Body: UpdateLabOrderRequest
  Returns: IActionResult
  Response:
    200 OK with updated lab order when successful
    Problem Details with error descriptions when failed
    Validates: ID in route matches ID in body

  PUT /api/orders/results/{id} (Special Endpoint)
  Method: PUT
  Route: /api/orders/results/{id}
  Body: UpdateLabResultsRequest
  Returns: IActionResult
  Response:
    200 OK when lab result is associated with lab order
    Problem Details with error descriptions when failed
    Validates: ID in route matches ID in body

  DELETE /api/orders/{id}
  Method: DELETE
  Route: /api/orders/{id}
  Returns: IActionResult
  Response:
    200 OK with deleted lab order when successful
    Problem Details with error descriptions when failed

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

RUNNING TESTS

Each service has a dedicated unit test project. To run all tests for the entire solution, you can use the following command from the root directory:
dotnet test
This will automatically discover and run all tests across all test projects within the solution.
