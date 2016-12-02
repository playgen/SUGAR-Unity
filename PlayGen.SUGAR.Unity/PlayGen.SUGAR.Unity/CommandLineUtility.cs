using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandLine;

namespace PlayGen.SUGAR.Unity
{
    public static class CommandLineUtility 
    {
        public static CommandLineOptions ParseArgs(string[] args)
        {
            var parser = new Parser();
            var options = new CommandLineOptions();
            parser.ParseArguments(args, options);
            return options;
        }
    }

    public class CommandLineOptions
    {
        [Option('a', "autologin", DefaultValue = false, Required = true, HelpText = "Sets flag to log in the user automatically.")]
        public bool AutoLogin { get; set; }

        [Option('s', "source", Required = true, HelpText = "Specify an authentication source.")]
        public string AuthenticationSource { get; set; }

        [Option('u', "uid", Required = true, HelpText = "Specify the id of the user.")]
        public string UserId { get; set; }
    }
}
