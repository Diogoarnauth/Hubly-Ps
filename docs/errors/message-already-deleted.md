# message-already-deleted

## What the user sees

When this error appears, it means the message has already been deleted.

Example API response:

```json
{
  "type": "https://github.com/Diogoarnauth/Hubly-Ps/docs/errors/message-already-deleted",
  "message": "Message Already Deleted",
  "status": 404
}
```

## What to do

- Do not retry deleting the message.
- Continue with another conversation action.

## Why this happens

- The message was already removed from the system.
- The request attempted to delete a message that no longer exists.
