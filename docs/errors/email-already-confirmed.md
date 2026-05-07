# email-already-confirmed

## What the user sees

When this error appears, it means the email address has already been confirmed.

Example API response:

```json
{
  "type": "https://github.com/Diogoarnauth/Hubly-Ps/docs/errors/email-already-confirmed",
  "message": "Email already confirmed",
  "status": 400
}
```

## What to do

- Continue using the account without confirming again.
- If you are trying to change your email, use the appropriate update flow.

## Why this happens

- The account email has already been confirmed.
- The system does not allow confirming the same email twice.

