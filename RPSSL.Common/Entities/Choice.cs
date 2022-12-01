using RPSSL.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPSSL.Common.Entities
{
    public record Choice(int Id, string Name)
    {
        public Choice(ChoiceType choiceType) : this ((int)choiceType, choiceType.ToString()) { }
    }
}
