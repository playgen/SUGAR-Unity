using System.Collections.Generic;
using CommandLine;

namespace PlayGen.SUGAR.Unity
{
	public static class CommandLineUtility
	{
		public static Dictionary<string, string> CustomArgs;

		public static CommandLineOptions ParseArgs(string[] args)
		{
			var parser = new Parser();
			var options = new CommandLineOptions();
			if (args != null)
			{
				parser.ParseArguments(args, options);
			}
			var customArgs = options.Custom;
			CustomArgs = new Dictionary<string, string>();
			if (customArgs != null)
			{
				foreach (var arg in customArgs)
				{
					var keyValue = arg.Split('=');
					CustomArgs.Add(keyValue[0], keyValue[1]);
				}
			}
			return options;
		}
	}

	public class CommandLineOptions
	{
		[Option('a', "autologin", DefaultValue = false, Required = true, HelpText = "Sets flag to log in the user automatically.")]
		public bool AutoLogin { get; set; }

		[Option('s', "source", Required = true, HelpText = "Specify an authentication source.")]
		public string AuthenticationSource { get; set; }

		[Option('g', "class", Required = false, HelpText = "Specify the id of the class.")]
		public string ClassId { get; set; }

		[Option('u', "uid", Required = true, HelpText = "Specify the id of the user.")]
		public string UserId { get; set; }

		[Option('p', "pass", Required = false, HelpText = "Specify the password for the user.")]
		public string Password { get; set; }

		[OptionArray('c', "custom", Required = false, HelpText = "Customs args list, dictionary pattern, separated by space. Eg: -c key=value key=value etc.")]
		public string[] Custom { get; set; }
	}
}
