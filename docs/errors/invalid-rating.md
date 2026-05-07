# invalid-rating

## What the user sees

When this error appears, it means the rating value provided is invalid.

Example API response:

```json
{
  "type": "https://github.com/Diogoarnauth/Hubly-Ps/docs/errors/invalid-rating",
  "message": "Invalid Rating",
  "status": 400
}
```

## What to do

- Check the rating value and enter a correct one.
- Use a valid rating within the allowed range.

## Why this happens

- The rating is outside the accepted range.
- The rating format is not valid.
