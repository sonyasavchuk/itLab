using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace itLab1
{
    class bdTypeDateInvl : bdType
    {
        public override bool Validation(string value)
        {
            string[] dateParts = value.Split('-'); // Assuming the date interval format is "start_date - end_date"

            if (dateParts.Length != 2)
            {
                return false; // Invalid format (not exactly two parts)
            }

            if (!DateTime.TryParse(dateParts[0], out DateTime startDate) ||
                !DateTime.TryParse(dateParts[1], out DateTime endDate))
            {
                return false; // Unable to parse start or end date
            }

            if (startDate >= endDate)
            {
                return false; // Start date is greater than or equal to end date
            }

            return true; // Valid date interval
        }
    }
}
