# conversation-already-exists

## What the user sees

When this error appears, it means the conversation already exists.

Example API response:

```json
{
  "type": "https://github.com/Diogoarnauth/Hubly-Ps/docs/errors/conversation-already-exists",
  "message": "Conversation Already Exists",
  "status": 409
}
```

## What to do

- Use the existing conversation instead of creating a duplicate.
- If you expected a new conversation, verify the participants.

## Why this happens

- A conversation with the same participants already exists.
- Duplicate conversations are blocked.
