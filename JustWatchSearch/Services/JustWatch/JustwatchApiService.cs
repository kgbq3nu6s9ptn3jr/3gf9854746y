using GraphQL.Client.Http;
using GraphQL.Client.Serializer.SystemTextJson;
using JustWatchSearch.Models;
using JustWatchSearch.Services.JustWatch.Responses;
using System.Text.Json;
using System.Web;
using static JustWatchSearch.Services.JustWatch.Responses.SearchTitlesResponse;
namespace JustWatchSearch.Services.JustWatch;

public partial class JustwatchApiService : IJustwatchApiService
{
	private readonly GraphQLHttpClient _graphQLClient;
	private readonly ILogger<JustwatchApiService> _logger;
	private readonly ICurrencyConverter _currencyConverter;
	private readonly CorsProxyState _corsState;
	private readonly Random _random = new();

    public JustwatchApiService(ILogger<JustwatchApiService> logger, ICurrencyConverter currencyConverter, CorsProxyState corsState)
	{
		_logger = logger;
		_currencyConverter = currencyConverter;
		_corsState = corsState;
		_corsState.OnChange += HandleCorsChange;
		CreateClient();
	}

    private void CreateClient()
    {
        var corsProxies = new[]
		{
			"https://anywhere.pwisetthon.com/",
			"https://app004.sitetheory.io/",
			"https://bindi-radio.com/",
			"https://brndn.me/",
			"https://carry-cors.ulam.tech/",
			"https://cori-anywhere.glitch.me/",
			"https://cors-anywhere.bronista.ru/",
			"https://cors-anywhere.creativeclaritycreations.com/",
			"https://cors-anywhere.hitech-land.com/",
			"https://cors-anywhere.mooore-test.nl/",
			"https://cors-everywhere-wc8b4.ondigitalocean.app/",
			"https://cors-for-sammy.onrender.com/",
			"https://cors-proxy.sanderron.de/",
			"https://cors.b-cdn.net/",
			"https://cors.beans.ai/",
			"https://cors.connectionhub.cf/",
			"https://cors.high5.ai/",
			"https://cors.ixigo.es/",
			"https://cors.jugorder.de/",
			"https://cors.lanka.info/",
			"https://cors.mcpsystem.com/",
			"https://cors.morningstreams.com/",
			"https://cors.mumzworld.com/",
			"https://cors.nepunep.xyz/",
			"https://cors.netlob.dev/",
			"https://cors.svaren.dev/",
			"https://cors.vlinc.ru/",
			"https://cors.weiqh.net/",
			"https://cors.x-sure.co/",
			"https://cors.x-sure.co/",
			"https://cors.yaamp.ru/",
			"https://corsproxy.cognizer.ai/",
			"https://corsproxy.service.echobox.com/",
			"https://corsproxy.site/",
			"https://fw-cors.spritle.com/",
			"https://hemelmechanica.nl:3000/",
			"https://proxy-cors-v-1-dot-saving-the-amazon-155216.ue.r.appspot.com/",
			"https://proxy.mydementiacompanion.com.au/",
			"https://proxy.pubavenue.com/",
			"https://www.tumblecorsserver.com/",
			"https://agile-ridge-02432.herokuapp.com/",
			"https://c558899ors.herokuapp.com/",
			"https://cors-anywhere-zq.herokuapp.com/",
			"https://cors-milanlaser.herokuapp.com/",
			"https://crs-prxy.herokuapp.com/",
			"https://customcorsanywhere.herokuapp.com/",
			"https://hackingtonsapiproxy.herokuapp.com/",
			"https://hammasoskari-proxy.herokuapp.com/",
			"https://info-getthekt.herokuapp.com/",
			"https://morning-stream-08762.herokuapp.com/",
			"https://mpg-cors-proxy.herokuapp.com/",
			"https://my-tb-cors.herokuapp.com/",
			"https://pure-springs-67256.herokuapp.com/",
			"https://satoshi-cors.herokuapp.com/",
			"https://serene-garden-87090.herokuapp.com/",
			"https://servicetitan-cors.herokuapp.com/",
			"https://sj-cors.herokuapp.com/",
			"https://smc.herokuapp.com/",
			"https://sulky-cors-anywhere.herokuapp.com/",
			"https://tradepro-cors.herokuapp.com/",
			"https://vinz-cors.herokuapp.com/",
			"https://vizsoftproxy.herokuapp.com/",
			"https://warm-caverns-48629-92fab798385f.herokuapp.com/",
			"https://your-cors.herokuapp.com/"
		};

        string baseAddress;
        if (_corsState.UseCorsProxy)
        {
            var corsProxy = corsProxies[_random.Next(corsProxies.Length)];
            baseAddress = $"{corsProxy}https://apis.justwatch.com";
        }
        else
        {
            baseAddress = "https://apis.justwatch.com";
        }

        _graphQLClient?.Dispose();
        _graphQLClient = new GraphQLHttpClient($"{baseAddress}/graphql", new SystemTextJsonSerializer());
        _logger.LogInformation("GraphQL client created with base: {base}", baseAddress);
    }

    private void HandleCorsChange()
    {
        CreateClient();
    }

    public void Dispose()
    {
        _graphQLClient?.Dispose();
        _corsState.OnChange -= HandleCorsChange;
    }

	public async Task<SearchTitlesResponse> SearchTitlesAsync(string input, string country, CancellationToken? token)
	{
		try
		{
			var searchResult = await _graphQLClient.SendQueryAsync<SearchTitlesResponse>(JustWatchGraphQLQueries.GetSearchTitlesQuery(input, country), token ?? default);
			return searchResult.Data;
		}
		catch (TaskCanceledException) { throw; }
		catch (Exception ex)
		{
			_logger.LogError("Searching title {input} failed with {ex}", input, ex);
			throw;
		}
	}

	public async Task<GetOffersResponse?> GetTitleOffers(string id, IEnumerable<string> countries, CancellationToken? token)
	{
		_logger.LogInformation("Got Title Offer request {id}", id);
		try
		{
			var searchResult = await _graphQLClient.SendQueryAsync<GetOffersResponse>(JustWatchGraphQLQueries.GetTitleOffersQuery(id, countries), token ?? default);
			return searchResult.Data;
		}
		catch (TaskCanceledException) { throw; }
		catch (Exception ex)
		{
			_logger.LogError(" Get title Offers request {id} failed with {ex}", id, ex);
			return null;
		}
	}



	public async Task<UrlMetadataResponse?> GetUrlMetadataResponseAsync(string path)
	{
		using (var httpClient = new HttpClient())
		{
			string url = $"{_baseAddress}/content/urls?path={HttpUtility.UrlEncode(path)}";
			var response = await httpClient.GetAsync(url);
			var urlMetadataResponse = JsonSerializer.Deserialize<UrlMetadataResponse>(await response.Content.ReadAsStringAsync());
			return urlMetadataResponse;
		}
	}

	public async Task<string[]> GetAvaibleLocales(string path)
	{
		var urlMetadataResponse = await GetUrlMetadataResponseAsync(path);
		return urlMetadataResponse?.HrefLangTags?.Select(tag => tag.Locale)?.ToArray() ?? new string[0];
	}

	public async Task<IEnumerable<TitleOfferViewModel>?> GetAllOffers(string nodeId, string path, CancellationToken? token)
	{
		await _currencyConverter.InitializeAsync();
		var locales = await GetAvaibleLocales(path);
		_logger.LogInformation("Got locale {locales}", locales);

		var countries = locales.Select(o => o.Split("_").Last());
		var titleOffer = await GetTitleOffers(nodeId, countries, token);
		if (titleOffer == null)
			return null;

		return titleOffer.Offers.SelectMany(item => item.Value.Select(o => new TitleOfferViewModel(o, item.Key, _currencyConverter.ConvertToUSD(o.Currency, o.RetailPriceValue ?? 0))));

	}

	public async Task<TitleNode?> GetTitle(string nodeId, CancellationToken? token = null)
	{
		try
		{
			var searchResult = await _graphQLClient.SendQueryAsync<TitleNodeWrapper>(JustWatchGraphQLQueries.GetTitleNode(nodeId), token ?? default);
			return searchResult.Data?.Node;
		}
		catch (TaskCanceledException) { throw; }
		catch (Exception ex)
		{
			_logger.LogError(" Get title Offers request {id} failed with {ex}", nodeId, ex);
			return null;
		}
	}
}
