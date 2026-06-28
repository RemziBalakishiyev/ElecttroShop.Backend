# Task: Health Check API

## Goal

Add a simple health check endpoint to the ElectroShop backend.

## Scope

Backend only.

## Requirements

* Add GET /api/health endpoint.
* Return HTTP 200 OK.
* Response body should contain:

  * status = ok

Example response:

{
"status": "ok"
}

## Do Not Touch

* Admin Front
* User Front
* Database
* Authentication
* Authorization
* Existing business logic
* Existing API contracts

## Expected Backend Impact

* Controller: create a new HealthController or use existing status/health controller if one already exists.
* Service/Application: not required unless the existing architecture requires it.
* Repository: not required.
* Entity: not required.
* DTO: optional.
* Validator: not required.
* Mapper: not required.
* Database/Migration: not required.

## Acceptance Criteria

* GET /api/health returns HTTP 200 OK.
* Response body contains status = ok.
* Backend builds successfully.
* No database changes are made.
* No frontend files are changed.
* No unrelated refactor is done.
* Existing architecture is not broken.

## Manual Test Steps

1. Run the backend project.
2. Send GET request to /api/health.
3. Confirm HTTP status code is 200.
4. Confirm response body contains:

{
"status": "ok"
}

## Notes

This is a test task to verify that Cursor follows backend rules, works only inside the backend repository, and does not touch unrelated files.
