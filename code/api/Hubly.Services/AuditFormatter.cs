namespace Hubly.api.Services;

public class AuditFormatter
{
    // Dicionário de templates: A chave é a Action, o valor é a string com placeholders
    private static readonly Dictionary<string, string> ActionTemplates = new()
    {
        { "CreateConversation", "A conversation with {UserName} was created by {UserEmail}." },
        { "SendMessage", "A message '{Message}' was sent by {UserEmail} in the conversation with {ReceiverName}." },
        { "EditMessage", "A message was edited to '{NewMessage}' by user {UserEmail} in the conversation with {ReceiverName}." },
        { "DeleteMessage", "The message '{Message}' was deleted by user {UserEmail} in the conversation with {ReceiverName}." },
        { "CreateTag", "The tag '{TagReference}' was created by user {UserEmail}." },
        { "UpdateTag", "The tag '{TagReference}' was updated by user {UserEmail}." },
        { "DeleteTag", "The tag '{TagReference}' was deleted by user {UserEmail}." },
        { "AssignTag", "The tag '{TagReference}' was assigned by user {UserEmail} to a conversation with {ReceiverName}." },
        { "UntagConversation", "The tag was removed by user {UserEmail} from the conversation with {ReceiverName}." },
        {
            "EditCompany",
            "The company profile '{CompanyName}' was updated by user {UserEmail}.\n" +
            "📝 New data saved:\n" +
            "• Name: {CompanyName}\n" +
            "• Size: {CompanySize}\n" +
            "• Website: {WebsiteLink}\n" +
            "• Headquarters: {CountryHeadquarters}\n" +
            "• Sectors: {Sectors}\n" +
            "• Description: {Description}"
        },
        { "UpdateStatus", "The availability status was updated to '{Status}' by user {UserEmail}." },
        { "AddSocialProfile", "A social profile named {SPName} was added by {UserEmail}." },
        { "RemoveSocialProfile", "A social profile named {SPName} was removed by {UserEmail}." },
        {
            "EditCreatorSocialProfile",
            "The creator social profile '{UserName}' was updated by user {UserEmail}.\n" +
            "📝 New data saved:\n" +
            "• Link: {Link}\n" +
            "• Followers: {FollowersCount}\n" +
            "• Price: Min {PriceMin}€ | Max {PriceMax}€\n" +
            "• Sectors: {Sectors}\n" +
            "• Description: {Description}"
        },
        { "ViewCreatorProfile", "The creator profile '{CreatorName}' was viewed by user {UserEmail}." },
        { "ViewCreatorSocialProfile", "The social profile '{SPName}' was viewed by user {UserEmail}." },
        { "EditCreatorProfile", "The creator artistic name was updated to '{ArtisticName}' by user {UserEmail}." },
        }; public string Format(string action, object? payload, int? userId, int? coWorkerId)
    {
        string template = ActionTemplates.GetValueOrDefault(action, $"Ação de auditoria '{action}' executada.");

        var values = new Dictionary<string, string>();

        if (payload != null)
        {
            foreach (var prop in payload.GetType().GetProperties())
            {
                values[prop.Name] = prop.GetValue(payload)?.ToString() ?? string.Empty;
            }
        }

        values["UserId"] = userId?.ToString() ?? string.Empty;
        values["CoWorkerId"] = coWorkerId?.ToString() ?? string.Empty;

        return PerformReplacement(template, values);
    }

    private string PerformReplacement(string template, Dictionary<string, string> values)
    {
        string result = template;
        foreach (var (key, value) in values)
        {
            result = result.Replace($"{{{key}}}", value);
        }
        return result;
    }
}