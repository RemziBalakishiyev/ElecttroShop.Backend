# OpenAPI diff — Render production deploy prep

**Date:** 2026-06-28

## Summary
Infrastructure/configuration changes only. No API contract changes.

## Endpoints
- Added minimal `GET /health` at root (not in OpenAPI/Swagger — minimal endpoint in Program.cs)
- Existing `GET /api/health` unchanged

## Models
- No changes

## Breaking changes
- None for API consumers
- **Configuration breaking:** `appsettings.json` no longer contains `ConnectionStrings` or JWT secrets — must use environment variables locally and in production
