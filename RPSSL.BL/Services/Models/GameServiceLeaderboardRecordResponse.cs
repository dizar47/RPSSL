using RPSSL.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RPSSL.BL.Services.Models
{
    public record GameServiceLeaderboardRecordResponse (string Name, int Score);
}
