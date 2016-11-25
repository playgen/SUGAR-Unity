using PlayGen.SUGAR.Client;
using PlayGen.SUGAR.Contracts.Shared;

namespace SUGAR.Unity
{
	internal static class SUGAR
	{
		public static SUGARClient Client;

		public static int GameId { get; set; }

		public static ActorResponse CurrentUser { get; set; }

		public static bool Register(SUGARManager manager)
		{
			return Client == null;
		}
	}
}
