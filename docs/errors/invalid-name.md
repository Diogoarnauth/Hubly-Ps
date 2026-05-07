# invalid-name

## What the user sees

When this error appears, it means the name entered is not valid.

Example API response:

```json
{
  "type": "https://github.com/Diogoarnauth/Hubly-Ps/docs/errors/invalid-name",
  "message": "The provided name is invalid",
  "status": 400
}
```

## What to do

- Check the name for invalid characters or blank values.
- Enter a valid display name or full name as required by the form.

## Why this happens

- The name field was empty or contained invalid characters.
- The input did not meet the backend validation rules.
