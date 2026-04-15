# API Error Contract

## Purpose

This document defines the standard API response and error contract for all backend endpoints.
Frontend must rely on this contract for consistent parsing, user messaging, and error handling.

## Standard Response Envelope

All APIs should return an envelope compatible with `ApiResponse<T>`.

### Success Response

```json
{
  "success": true,
  "data": {},
  "message": "Optional success message",
  "error": null,
  "timestamp": "2026-04-15T17:30:00Z"
}
```

### Error Response

```json
{
  "success": false,
  "data": null,
  "message": "Human-readable error message",
  "error": {
    "code": "ERROR_CODE"
  },
  "timestamp": "2026-04-15T17:30:00Z"
}
```

## HTTP Status + Error Code Mapping

### Generic platform errors

- `400` -> `BAD_REQUEST`
- `401` -> `UNAUTHORIZED`
- `403` -> `FORBIDDEN`
- `404` -> `RESOURCE_NOT_FOUND`
- `500` -> `INTERNAL_ERROR`

### Domain-level examples already used

- `SLOT_GENERATION_CONFLICT`
- `ROLE_FORBIDDEN`
- `INVALID_TRANSITION`
- `CAPACITY_EXCEEDED`
- `SLOT_UNAVAILABLE`

## Frontend Handling Rules

### Global handling

- Always parse `success` first.
- If `success = false`, show `message` as the primary user-facing text.
- Use `error.code` for programmatic logic and UI branching.

### By status code

- `401 UNAUTHORIZED`
  - Clear auth state.
  - Redirect to login.
  - Show session-expired notice if applicable.

- `403 FORBIDDEN`
  - Show permission error screen or inline "Not allowed" state.
  - Do not retry automatically.

- `404 RESOURCE_NOT_FOUND`
  - Show "Not found" UX and optionally navigate back.

- `400 BAD_REQUEST`
  - For validation/input issues, show inline form errors where possible.
  - For known domain errors (`SLOT_GENERATION_CONFLICT`, etc.), trigger dedicated flows.

- `500 INTERNAL_ERROR`
  - Show generic error toast/dialog.
  - Allow manual retry.

## Required Frontend Parsing Priority

1. `success`
2. `error.code`
3. `message`
4. optional additional keys inside `error`

## Example Error Payloads

### Validation error (400)

```json
{
  "success": false,
  "data": null,
  "message": "Invalid date format. Use yyyy-MM-dd.",
  "error": {
    "code": "BAD_REQUEST"
  },
  "timestamp": "2026-04-15T17:30:00Z"
}
```

### Slot generation conflict (400)

```json
{
  "success": false,
  "data": null,
  "message": "Slot generation cancelled due to conflicting already-generated slots.",
  "error": {
    "code": "SLOT_GENERATION_CONFLICT",
    "conflicts": [
      {
        "templateId": 12,
        "providerId": 34,
        "siteId": 2,
        "date": "2026-04-18",
        "startTime": "09:00",
        "endTime": "09:30",
        "existingSlotId": 9012,
        "existingStatus": "Open"
      }
    ]
  },
  "timestamp": "2026-04-15T17:30:00Z"
}
```

### Forbidden role (403)

```json
{
  "success": false,
  "data": null,
  "message": "You do not have the required permissions to access this resource.",
  "error": {
    "code": "FORBIDDEN"
  },
  "timestamp": "2026-04-15T17:30:00Z"
}
```

## Endpoint Migration Note

Slot generation endpoint has been consolidated to:

- `POST /slots/generate`

Deprecated path family (should not be used by frontend):

- `/availability/slot-generation/*`

## Backend Implementation Rules

- Every controller action should return `ApiResponse<T>` envelope on success and failure.
- `error.code` values should remain stable and backward-compatible.
- Do not return raw exception stacks/messages in non-development environments.
- If extra error metadata is needed (like conflicts), include it under `error` with stable keys.

## Frontend Integration Checklist

- [ ] Centralize API interceptor for envelope parsing.
- [ ] Implement status-code-specific handling (401/403/404/500).
- [ ] Implement domain-code handling for known business errors.
- [ ] Ensure all screens consume `message` and `error.code` consistently.
- [ ] Add telemetry/logging with endpoint + status + `error.code`.

