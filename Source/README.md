# Ideageek.FightersArenaApi

## Auth behavior (current)
- Endpoints: `POST /api/auth/signupmobile`, `POST /api/auth/register`, `POST /api/auth/login`
- Successful responses always return HTTP 200. The JWT token is provided in the `message` field; the `value` field is empty. Example success body:
  ```json
  {
    "code": 200,
    "error": false,
    "message": "<jwt-token-here>",
    "value": null,
    "count": 0
  }
  ```
- Error responses also use HTTP 200 with `code` set to the logical status (e.g., 400/401/500) and a descriptive `message`.
- Password policy for signup: minimum 6 characters; numbers are not required.

## Response shape (all APIs)
- HTTP status is always 200.
- Body follows:
  ```json
  {
    "code": <logical status code>,
    "date": "<date string>",
    "error": <true|false>,
    "message": "<description or token>",
    "value": <payload or null>,
    "count": <item count>
  }
  ```
- Validation errors are returned with `code` 400 and a combined error message; `value` is null.

## CORS
- Policy `AllowAll` permits any origin and allows credentials.

## Tests
- Run all tests: `dotnet test Ideageek.FightersArena.sln`
