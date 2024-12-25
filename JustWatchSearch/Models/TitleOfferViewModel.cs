using System.Web;
using JustWatchSearch.Services.JustWatch.Responses;
namespace JustWatchSearch.Models;
public class TitleOfferViewModel
{
	public string Country { get; set; }

	public string? PackageURL => GetCleanPackageUrl(OfferDetails?.StandardWebUrl);

	public string? PackageClearName => OfferDetails?.Package?.ClearName;
	public string? RetailPrice => OfferDetails?.RetailPrice;
	public decimal? RetailPriceValue => OfferDetails?.RetailPriceValue;
	public decimal NormalizedPrice { get; init; }
	public string? PresentationType => FormatPresentationType(OfferDetails?.PresentationType);
	public string? MonetizationType => OfferDetails?.MonetizationType;
	public string? SubtitleLanguages => FormatLanguages(OfferDetails?.SubtitleLanguages);
	public string? AudioLanguages =>  FormatLanguages(OfferDetails?.AudioLanguages);
	public string? Technology => FormatTechnology(OfferDetails?.VideoTechnology, OfferDetails?.AudioTechnology);
	public OfferDetails OfferDetails { get; set; }


	public TitleOfferViewModel(OfferDetails offerDetails, string country, decimal usdPrice)
	{
		Country = country;
		OfferDetails = offerDetails;
		NormalizedPrice = usdPrice;
	}

	private string? GetCleanPackageUrl(string? packageUrl)
	{
		if (string.IsNullOrEmpty(packageUrl))
			return null;
		if (packageUrl.Contains("bn5x.net") && packageUrl.Contains("www.disneyplus.com"))
		{
			Uri uri = new Uri(packageUrl);
			var cleanUrl = HttpUtility.ParseQueryString(uri.Query)["u"];
			return !string.IsNullOrEmpty(cleanUrl) ? cleanUrl : packageUrl;
		}
		string[] domains = {"tv.apple.com", "watch.plex.tv", "play.max.com", "therokuchannel.roku.com"};
		if (domains.Any(domain => packageUrl.StartsWith("https://" + domain)))
		{
			Uri uri = new Uri(packageUrl);
			return uri.GetLeftPart(UriPartial.Path);
		}
		return packageUrl;
	}

	private string? FormatPresentationType(string? presentationType)
	{
		return string.IsNullOrEmpty(presentationType) ? null : presentationType.Replace("_4K", "UHD");
	}

	private string? FormatLanguages(List<string>? languages)
	{
		return languages != null && languages.Any()
			? string.Join(", ", languages.OrderBy(lang => lang))
			: null;
	}

	private string? FormatTechnology(List<string>? videoTechnology, List<string>? audioTechnology)
	{
		var dataViewValue = new Dictionary<string, string>
		{
			{ "DOLBY_VISION", "Dolby Vision" },
			{ "DOLBY_ATMOS", "Dolby Atmos" },
			{ "_5_POINT_1", "5.1ch" }
		};
		var technologyList = new List<string>();
		if (videoTechnology != null)
		{
			foreach (var vt in videoTechnology)
			{
				technologyList.Add(dataViewValue.GetValueOrDefault(vt, vt));
			}
		}
		if (audioTechnology != null)
		{
			foreach (var at in audioTechnology)
			{
				technologyList.Add(dataViewValue.GetValueOrDefault(at, at));
			}
		}
		return string.Join("\n", technologyList);
	}
}
