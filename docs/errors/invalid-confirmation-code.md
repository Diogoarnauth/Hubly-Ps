# invalid-confirmation-code

## What the user sees

When this error appears, it means the confirmation code entered is invalid.

Example API response:

```json
{
  "type": "https://github.com/Diogoarnauth/Hubly-Ps/docs/errors/invalid-confirmation-code",
  "message": "The provided confirmation code is invalid",
  "status": 400
}
```

## What to do

- Check the confirmation code and enter it again.
- Request a new code if the current one has expired or is incorrect.

## Why this happens

- The code entered does not match the expected value.
- The code may have expired or been mistyped.

