# invalid-price-range

## What the user sees

When this error appears, it means the price range selected is invalid.

Example API response:

```json
{
  "type": "https://github.com/Diogoarnauth/Hubly-Ps/docs/errors/invalid-price-range",
  "message": "Invalid Price Range",
  "status": 401
}
```

## What to do

- Choose a valid price range from the available options.
- Check the input and try again.

## Why this happens

- The selected price range is not accepted.
- The value may be missing or outside the allowed range.
