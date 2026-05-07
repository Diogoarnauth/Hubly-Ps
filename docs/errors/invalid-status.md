# invalid-status

## What the user sees

When this error appears, it means the status value provided is not valid.

Example API response:

```json
{
  "type": "https://github.com/Diogoarnauth/Hubly-Ps/docs/errors/invalid-status",
  "message": "Invalid Status",
  "status": 400
}
```

## What to do

- Check that the status selection is valid.
- Choose a supported status option and try again.

## Why this happens

- The submitted status is not recognized by the backend.
- The value may be missing or outside the allowed set.
