# invalid-followers-count

## What the user sees

When this error appears, it means the followers count provided is invalid.

Example API response:

```json
{
  "type": "https://github.com/Diogoarnauth/Hubly-Ps/docs/errors/invalid-followers-count",
  "message": "Invalid Followers Count",
  "status": 401
}
```

## What to do

- Enter a valid follower count or leave the field blank if optional.
- Check the allowed format and value range.

## Why this happens

- The follower count is not within the accepted range.
- The value is malformed or unsupported.

