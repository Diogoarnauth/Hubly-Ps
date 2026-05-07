# company-already-exists

## What the user sees

When this error appears, it means a company already exists with the same information.

Example API response:

```json
{
  "type": "https://github.com/Diogoarnauth/Hubly-Ps/docs/errors/company-already-exists",
  "message": "Company Already Exists",
  "status": 400
}
```

## What to do

- Check if the company is already registered.
- Use the existing company record instead of creating a duplicate.

## Why this happens

- The company name or identifier is already in use.
- Duplicate company records are not allowed.
