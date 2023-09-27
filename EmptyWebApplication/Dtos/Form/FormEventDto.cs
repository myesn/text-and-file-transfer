using System.Collections.Immutable;

namespace EmptyWebApplication
{
    public record class FormEventDto(IEnumerable<MessageDto> Messages);
}
