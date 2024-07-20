using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PM2E2Grupo1.Extensions
{
    public static class InputSanitizer
    {
        public static string SanitizeInput(string input)
        {
            // Remueve caracteres peligrosos
            string sanitizedInput = Regex.Replace(input, @"[^\w\s]", string.Empty);

            return sanitizedInput;
        }
    }
}
