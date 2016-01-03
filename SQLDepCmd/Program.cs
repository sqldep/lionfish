using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Options;

namespace SQLDepCmd
{
    class Program
    {
        static void Main(string[] args)
        {
            bool show_help = false;
            List<string> names = new List<string>();
            int repeat = 1;
            int verbosity = 0;

            var p = new OptionSet() {
                { "n|name=", "the {NAME} of someone to greet.", v => names.Add (v) },
                { "r|repeat=",  "the number of {TIMES} to repeat the greeting.\n" + "this must be an integer.", (int v) => repeat = v },
                { "v", "increase debug message verbosity", v => { if (v != null) ++verbosity; } },
                { "h|help",  "show this message and exit", v => show_help = v != null },
            };

            List<string> extra;
            try
            {
                extra = p.Parse(args);
            }
            catch (OptionException e)
            {
                Console.Write("greet: ");
                Console.WriteLine(e.Message);
                Console.WriteLine("Try `greet --help' for more information.");
                return;
            }
        }
    }
}
