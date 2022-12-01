using RPSSL.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RPSSL.BL.Services.Models
{
    public class GameServiceRoundProcessingResponse
    {
        public enum RoundOutcome {
            win,
            lose,
            tie
        }

        public ChoiceType Computer { get; set; }
        public ChoiceType Player { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public RoundOutcome Results { get; set; }
    }
}
