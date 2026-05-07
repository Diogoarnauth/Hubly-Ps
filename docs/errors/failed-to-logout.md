# failed-to-logout

## What the user sees

When this error appears, it means the application could not complete the logout process.

Example API response:

```json
{
  "type": "https://github.com/Diogoarnauth/Hubly-Ps/docs/errors/failed-to-logout",
  "message": "Failed To Logout",
  "status": 400
}
```

## What to do

- Try logging out again.
- If logout continues to fail, close the browser or app and sign in again later.

## Why this happens

- The logout request could not be processed by the server.
- There may be a session or network issue.
