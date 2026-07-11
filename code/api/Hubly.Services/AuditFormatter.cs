namespace Hubly.api.Services;

public class AuditFormatter
{
    // Dicionário de templates: A chave é a Action, o valor é a string com placeholders
    private static readonly Dictionary<string, string> ActionTemplates = new()
    {
        { "CreateConversation", "Uma conversa com {UserName} foi criada pelo {UserEmail}." },
        { "SendMessage", "Uma mensagem '{Message}' foi enviada por {UserEmail} na conversa com {ReceiverName}." },
        { "EditMessage", "Uma mensagem foi editada para '{NewMessage}', pelo user {UserEmail} na conversa com {ReceiverName}." },
        { "DeleteMessage", "A mensagem '{Message}' foi removida pelo user {UserEmail} na conversa com {ReceiverName}." },
        { "CreateTag", "A etiqueta '{TagReference}' foi criada pelo user {UserEmail} ." },
        { "UpdateTag", "A etiqueta '{TagReference}' foi atualizada pelo user {UserEmail}." },
        { "DeleteTag", "A etiqueta '{TagReference}' foi removida pelo user {UserEmail}." },
        { "AssignTag", "A etiqueta '{TagReference}' foi atribuída pelo user {UserEmail} à uma conversa com {ReceiverName} ." },
        { "UntagConversation", "A etiqueta foi removida pelo user {UserEmail} da conversa com {ReceiverName} ." },
        {
            "EditCompany",
            "O perfil da empresa '{CompanyName}' foi atualizado pelo user {UserEmail}.\n" +
            "📝 Novos dados salvos:\n" +
            "• Nome: {CompanyName}\n" +
            "• Tamanho: {CompanySize}\n" +
            "• Website: {WebsiteLink}\n" +
            "• Sede: {CountryHeadquarters}\n" +
            "• Setores: {Sectors}\n" +
            "• Descrição: {Description}"
        },
        { "UpdateStatus", "O estado de disponibilidade foi atualizado para '{Status}' pelo user {UserEmail}." },
        { "AddSocialProfile", "Um perfil social chamado {SPName} foi adicionado pelo {UserEmail}." },
        { "RemoveSocialProfile", "Um perfil social chamado {SPName} foi removido pelo {UserEmail}." },
        { 
            "EditCreatorSocialProfile", 
            "O perfil social de criador '{UserName}' foi atualizado pelo user {UserEmail}.\n" +
            "📝 Novos dados guardados:\n" +
            "• Link: {Link}\n" +
            "• Seguidores: {FollowersCount}\n" +
            "• Preço: Mín {PriceMin}€ | Máx {PriceMax}€\n" +
            "• Setores: {Sectors}\n" +
            "• Descrição: {Description}" 
        },    
        { "ViewCreatorProfile", "O perfil do criador '{CreatorName}' foi visualizado pelo user {UserEmail}." },
        { "ViewCreatorSocialProfile", "O perfil social '{SPName}' foi visualizado pelo user {UserEmail}." },
        { "EditCreatorProfile", "O nome artístico do criador foi atualizado para '{ArtisticName}' pelo user {UserEmail}." },
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