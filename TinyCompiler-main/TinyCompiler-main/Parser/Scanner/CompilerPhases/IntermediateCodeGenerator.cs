using System.Collections.Generic;

namespace Scanner.CompilerPhases
{
    public class IntermediateCodeGenerator
    {
        public List<string> Generate(List<string> tokens)
        {
            List<string> intermediateCode = new List<string>();

            // Example: Generate intermediate code
            foreach (var token in tokens)
            {
                intermediateCode.Add("GeneratedCodeFor: " + token);
            }

            return intermediateCode;
        }
    }
}
