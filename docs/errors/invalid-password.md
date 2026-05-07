# invalid-password

## What the user sees

When this error appears, it means the password entered does not meet the required validation rules.

Example API response:

```json
{
  "type": "https://github.com/Diogoarnauth/Hubly-Ps/docs/errors/invalid-password",
  "message": "The password provided is invalid",
  "status": 400
}
```

## What to do

- Make sure the password follows the format required by the form.
- Use a password with the required length, characters, and complexity.

## Why this happens

- The password field is empty or too short.
- The password contains invalid characters or does not satisfy policy rules.

