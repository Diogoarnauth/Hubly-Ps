# email-already-exists

## What the user sees

When this error appears, it means the email you are trying to use is already registered in Hubly.

Example API response:

```json
{
  "type": "https://github.com/Diogoarnauth/Hubly-Ps/docs/errors/email-already-exists",
  "message": "There is already an account registered on that email",
  "status": 400
}
```

## What to do

- Check that the email is typed correctly.
- If you already have an account, log in instead of registering again.
- If you forgot your password, use the password recovery option.

## Why this happens

- The email address is already linked to a Hubly account.
- The same email cannot be used for more than one account.
- Even if the previous account is not confirmed yet, the email is still reserved.
