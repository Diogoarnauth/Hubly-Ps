# invalid-color-hex

## What the user sees

When this error appears, it means the color value is not a valid hex code.

Example API response:

```json
{
  "type": "https://github.com/Diogoarnauth/Hubly-Ps/docs/errors/invalid-color-hex",
  "message": "Invalid Color Hex",
  "status": 400
}
```

## What to do

- Enter a valid hex color like #FF5733.
- Check that the value starts with "#" and contains 6 hex digits.

## Why this happens

- The color value is malformed.
- The value does not meet the expected hex code format.
