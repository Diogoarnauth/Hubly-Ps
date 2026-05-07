# invalid-participant-role

## What the user sees

When this error appears, it means a participant role is invalid.

Example API response:

```json
{
  "type": "https://github.com/Diogoarnauth/Hubly-Ps/docs/errors/invalid-participant-role",
  "message": "Invalid Participant Role",
  "status": 409
}
```

## What to do

- Choose a valid role for the conversation participant.
- Check the participant settings and try again.

## Why this happens

- The selected participant role is not supported.
- The value may be missing or invalid.
