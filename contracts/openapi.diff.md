# OpenAPI diff — CORS FRONTEND_URLS

**Date:** 2026-06-28

## Summary
Configuration-only change. No API contract changes.

## Configuration
- `FRONTEND_URL` replaced by `FRONTEND_URLS` (comma-separated origins)
- Supports multiple frontends (admin + user) in production CORS

## Breaking changes
- None for API consumers
- **Ops breaking:** Render/backend env must use `FRONTEND_URLS` instead of `FRONTEND_URL`

