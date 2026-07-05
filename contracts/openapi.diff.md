# OpenAPI Diff — Application Database Logging

## Date
2026-07-05

## New Endpoints

### GET /api/admin/logs
Admin-only paginated application logs from database.

**Auth:** Bearer JWT, role `Admin`

**Query parameters:**
| Name | Type | Description |
|------|------|-------------|
| page | int | Page number (default 1) |
| pageSize | int | Page size (default 20, max 100) |
| level | string | Log level filter |
| eventType | string | HttpRequest, MediatR, Validation, Exception, Application |
| correlationId | string | Request correlation id |
| userId | uuid | Filter by user |
| search | string | Search in message/exception/path |
| dateFrom | datetime | UTC start |
| dateTo | datetime | UTC end |

**Response:** `PagedResult<AppLogDto>`

## New Models

### AppLogDto
Application log entry with HTTP context, user info, timing, and optional JSON properties.

## Breaking Changes
None.

## Notes
Logging infrastructure change only; existing endpoints unchanged.
