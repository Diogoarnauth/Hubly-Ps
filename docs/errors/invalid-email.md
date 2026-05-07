# invalid-email

## What the user sees

When this error appears, it means the email address entered is not valid.

Example API response:

```json
{
  "type": "https://github.com/Diogoarnauth/Hubly-Ps/docs/errors/invalid-email",
  "message": "The provided email is invalid",
  "status": 400
}
```

## What to do

- Check that the email is typed correctly.
- Use a valid email format like user@example.com.

## Why this happens

- The email is missing an @ symbol or domain.
- The email format does not pass backend validation.

