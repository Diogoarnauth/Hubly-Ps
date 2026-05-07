# creator-already-exists

## What the user sees

When this error appears, it means a creator profile already exists for this account.

Example API response:

```json
{
  "type": "https://github.com/Diogoarnauth/Hubly-Ps/docs/errors/creator-already-exists",
  "message": "Creator Already Exists",
  "status": 400
}
```

## What to do

- Verify whether you already created a creator profile.
- Use the existing creator account instead of creating a new one.

## Why this happens

- A creator entity with the same identifier already exists.
- The system prevents duplicate creator registrations.

