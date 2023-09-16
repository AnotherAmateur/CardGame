namespace CardGameProj.SeparateClasses
{
	public static class States
	{
		public static int PlayerId { get; set; }
		public static int GameId { get; set; }
		public static string Login { get; set; }
		public static int MasterId { get; set; }
		public static int AntagonistLeaderCardId { get; set; }
		public static int ProtagonistLeaderCardId { get; set; }

		public readonly static (float, float) InitCardSize = (512, 768);

		public static string Url = "http://bloghda-001-site1.htempurl.com/";
	}
}
