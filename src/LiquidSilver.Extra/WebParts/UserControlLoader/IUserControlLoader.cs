namespace LiquidSilver.Extra.WebParts
{
	/// <summary>
	/// The contract between <see cref="UserControlLoader"/> and
	/// <see cref="UserControlLoaderToolPart"/>.
	/// </summary>
	public interface IUserControlLoader
	{
		bool IsSiteRelative { get; set; }
		string UserControlUrl { get; set; }
	}
}
