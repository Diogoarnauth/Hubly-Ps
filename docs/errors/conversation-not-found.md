# conversation-not-found

## What the user sees

When this error appears, it means the requested conversation could not be found.

Example API response:

```json
{
  "type": "https://github.com/Diogoarnauth/Hubly-Ps/docs/errors/conversation-not-found",
  "message": "Conversation Not Found",
  "status": 404
}
```

## What to do

- Verify the conversation exists.
- Refresh the conversation list or try again.

## Why this happens

- The conversation was deleted or never existed.
- The requested conversation ID is invalid.
