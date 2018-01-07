using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using ReplEngine;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit;
using Environment = System.Environment;

namespace CSRepl
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ReplEvaluator evaluator;
        private readonly string path = @"Resources/C#.xshd";
        private static StringWriter consoleText = new StringWriter();

        public MainWindow()
        {
            InitializeComponent();
            codeArea.Text = "> ";

            Console.SetOut(consoleText);

            evaluator = new ReplEvaluator();
            
            codeArea.IsReadOnly = true;
        }



        private void inputAreaOnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            var key = e.Key;
            if (key == Key.Enter)
            {
                var code = inputArea.Text;
                codeArea.Text += code + "\n";
                inputArea.Text = String.Empty;
                inputArea.IsEnabled = false;
                var output = evaluator.Evaluate(code);
                if (output.HasResult && !output.HasErrors)
                {
                    var console = consoleText.ToString();
                    if (console.Length > 0)
                    {
                        var messages = console.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (var it in messages)
                            codeArea.Text += it + "\n";
                        consoleText = new StringWriter();
                        Console.SetOut(consoleText);
                    }
                    codeArea.Text += ToCodeString(output.Result) + "\n> ";
                }

                if (output.HasErrors)
                    codeArea.Text += ToCodeString(output.Errors) + "\n> ";
                if (!output.HasResult && !output.HasErrors)
                {
                    var console = consoleText.ToString();
                    if (console.Length > 0)
                    {
                        var messages = console.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (var it in messages)
                            codeArea.Text += it + "\n";
                        consoleText = new StringWriter();
                        Console.SetOut(consoleText);
                    }
                    codeArea.Text += "> ";
                }

                codeArea.ScrollToEnd();
                inputArea.IsEnabled = true;
            }

        }

        private string ToCodeString(object o)
        {
            if (o is String)
                return o.ToString();

            var enumerable = o as IEnumerable;
            if (enumerable != null)
            {
                var items = enumerable.Cast<object>().Take(21).ToList();
                var firstItems = items.Take(20).ToList();
                var sb = new StringBuilder();
                sb.Append("{");
                sb.Append(String.Join(", ", firstItems));
                if (items.Count > firstItems.Count)
                    sb.Append("...");
                sb.Append("}");
                return sb.ToString();
            }
            return o.ToString();
        }

        
    }
}
