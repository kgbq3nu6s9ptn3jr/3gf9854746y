using JustWatchSearch.Services.JustWatch.Responses;
namespace JustWatchSearch.Models;
public class TitleOfferViewModel
{
	public string Country { get; set; }

	public string? PackageURL => OfferDetails?.StandardWebUrl;

	public string? PackageClearName => OfferDetails?.Package?.ClearName;
	public string? RetailPrice => OfferDetails?.RetailPrice;
	public decimal? RetailPriceValue => OfferDetails?.RetailPriceValue;
	public decimal NormalizedPrice { get; init; }
	public string? PresentationType => OfferDetails?.PresentationType.Replace("_4K", "UHD");
	public string? MonetizationType => OfferDetails?.MonetizationType;
	public string? SubtitleLanguages => 
		OfferDetails?.SubtitleLanguages != null 
			? string.Join(", ", OfferDetails.SubtitleLanguages.OrderBy(lang => lang)) 
			: null;
	public string? AudioLanguages => 
		OfferDetails?.AudioLanguages != null 
			? string.Join(", ", OfferDetails.AudioLanguages.OrderBy(lang => lang)) 
			: null;
	public string? Technology =>
		string.Join("\n",
			(OfferDetails.VideoTechnology ?? Enumerable.Empty<string>())
				.Concat(OfferDetails.AudioTechnology?.Select(a => a.ToString()) ?? Enumerable.Empty<string>())
		);
	public OfferDetails OfferDetails { get; set; }


	public TitleOfferViewModel(OfferDetails offerDetails, string country, decimal usdPrice)
	{
		Country = country;
		OfferDetails = offerDetails;
		NormalizedPrice = usdPrice;
	}
}
