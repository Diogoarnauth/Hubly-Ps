# invalid-artistic-name

## What the user sees

When this error appears, it means the artistic name entered is not valid.

Example API response:

```json
{
  "type": "https://github.com/Diogoarnauth/Hubly-Ps/docs/errors/invalid-artistic-name",
  "message": "Invalid Artistic Name",
  "status": 400
}
```

## What to do

- Check the artistic name and try again.
- Use letters and allowed characters only.

## Why this happens

- The artistic name contains invalid characters or is empty.
- It does not meet the backend validation rules.
