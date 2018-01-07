using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.CSharp;

namespace ReplEngine
{
    public class ReplEvaluator
    {
        private CompilerContext compilerContext;
        private Evaluator evaluator;

        public ReplEvaluator()
        {
            compilerContext = new CompilerContext(new CompilerSettings(), new StreamReportPrinter(new StringWriter()));
            evaluator = new Evaluator(compilerContext);
            evaluator.Run("using System;using System.Collections.Generic;using System.Linq;using System.Linq.Expressions;using System.Text;using System.Threading.Tasks;");
        }

        public EvaluateResult Evaluate(string input)
        {
            var errorWriter = new StringWriter();
            compilerContext.Report.SetPrinter(new StreamReportPrinter(errorWriter));
            var res = new EvaluateResult();
            res.Input = input;
            res.HasErrors = false;
            res.HasResult = false;
            res.InputComplete = true;



            object result = null;
            bool resultSet = false;
            

            try
            {
                var command = evaluator.Evaluate(input, out result, out resultSet);
            }
            catch (Exception e)
            {
                res.Errors = new[] { e.ToString() };
                res.HasErrors = true;
            }

            string errorMessage = errorWriter.ToString();
            if (errorMessage.Length > 0)
            {
                var messages = errorMessage.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                var errors = messages.Where(msg => IsMessage(msg, "error")).ToList();


                var allErrors = res.Errors ?? new string[0];
                allErrors = allErrors.Concat(errors).ToArray();
                res.HasErrors = allErrors.Any();
                res.Errors = allErrors;
            }   

            if (resultSet)
            {
                res.HasResult = true;
                res.Result = result;
            }

            return res;
        }

        private static bool IsMessage(string message, string messageType)
        {
            var messageParts = message.Split(':');
            if (messageParts.Length >= 2)
            {
                var type = messageParts[1].Trim();
                return type.StartsWith(messageType, StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }
    }


}
