# self-rating-not-allowed

## What the user sees

When this error appears, it means you tried to rate your own profile.

Example API response:

```json
{
  "type": "https://github.com/Diogoarnauth/Hubly-Ps/docs/errors/self-rating-not-allowed",
  "message": "Self Rating Not Allowed",
  "status": 400
}
```

## What to do

- Choose a different creator to rate.
- Do not rate your own account.

## Why this happens

- The system prevents users from rating themselves.
- Self-rating is not permitted for fairness.
