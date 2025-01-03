using System.Collections.Generic;

namespace Scanner.CompilerPhases
{
    public class SemanticAnalyzer
    {
        public List<string> Analyze(List<string> tokens)
        {
            List<string> errors = new List<string>();

            // Example: Check for undeclared variables or type mismatches
            foreach (var token in tokens)
            {
                if (token == "undeclared_variable")  // Example check
                {
                    errors.Add("Error: Undeclared variable found.");
                }
            }

            return errors;
        }
    }
}
