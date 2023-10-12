using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace itLab1
{
    class bdTypeDate : bdType
    {
        public override bool Validation(string value)
        {
            // Attempt to parse the input string as a DateTime
            if (DateTime.TryParse(value, out _))
            {
                return true; // Successfully parsed as a date
            }
            else
            {
                return false; // Failed to parse as a date
            }
        }
    }
}
