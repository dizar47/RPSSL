using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RPSSL.BL.Services.Models
{
    internal class GameServiceRandomGeneratorResponse
    {
        [JsonPropertyName("random_number")]
        public int RecordNumber { get; set; }
    }
}
