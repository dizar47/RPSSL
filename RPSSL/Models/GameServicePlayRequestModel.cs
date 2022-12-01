using RPSSL.Common.Enums;

namespace RPSSL.Models
{
    public record GameServicePlayRequestModel (ChoiceType Player, string? PlayerName);
}
