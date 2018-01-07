using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReplEngine
{
    public class EvaluateResult
    {
        public string Input;

        public object Result;
        public bool HasResult;
        public bool InputComplete;
        public bool HasErrors;
        public string[] Errors;
    }
}
