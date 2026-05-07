# invalid-credentials

## What the user sees

When this error appears, it means the email or password entered is incorrect.

Example API response:

```json
{
  "type": "https://github.com/Diogoarnauth/Hubly-Ps/docs/errors/invalid-credentials",
  "message": "Invalid Credentials (email or password)",
  "status": 409
}
```

## What to do

- Check the email and password are entered correctly.
- If you forgot your password, use the password recovery option.

## Why this happens

- The email and password combination does not match any account.
- Either the email is wrong or the password does not belong to that account.
