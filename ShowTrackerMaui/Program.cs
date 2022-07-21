// See https://aka.ms/new-console-template for more information
using DM.MovieApi;
using DM.MovieApi.ApiResponse;
using DM.MovieApi.MovieDb.Movies;
using DM.MovieApi.MovieDb.TV;
using Newtonsoft.Json;
using Humanizer;
using Humanizer.Localisation;

const string bearerToken = "eyJhbGciOiJIUzI1NiJ9.eyJhdWQiOiJmZjkyOTlmOTYxOWVhNDU1MDRhZDkyNjIyZTY0MjkyMSIsInN1YiI6IjYyODIzMGNmMjBlNmE1MWEyNmQ5ZTViMiIsInNjb3BlcyI6WyJhcGlfcmVhZCJdLCJ2ZXJzaW9uIjoxfQ.VuzyTQcnjNSar6gA4qvmDUnJPdhX5_1TGgh29eWGuR8";
const string posterStartPath = "https://www.themoviedb.org/t/p/w600_and_h900_bestv2";

MovieDbFactory.RegisterSettings(bearerToken);
var movieApi = MovieDbFactory.Create<IApiMovieRequest>().Value;
var tvApi = MovieDbFactory.Create<IApiTVShowRequest>().Value;

DateTime today = DateTime.Today;
DateTime nextEpisodeAirDate=DateTime.Today;

string input = "";

do
{
    Console.WriteLine("Enter a TV show: ");
    input = Console.ReadLine();

    var tvShows = searchTV(input).Result;

    /*foreach (TVShowInfo info in tvShows.Results)
    {
        Console.WriteLine($"{info.Id}: {info.Name} ({info.FirstAirDate} )");
    }*/

    var tvShow = await tvApi.FindByIdAsync(tvShows.Results[0].Id);

    string nextEpisodeToAir = tvShow.Json;
    episode nextEpisode = JsonConvert.DeserializeObject<episode>(nextEpisodeToAir);
    bool inProduction = nextEpisode.in_production;
    string timeToNextEpisode;
    if (nextEpisode.next_episode_to_air != null)
    {
        DateTime.TryParse(nextEpisode.next_episode_to_air.air_date, out nextEpisodeAirDate);
        TimeSpan difference = nextEpisodeAirDate - today;
        int daysToGo = difference.Days;
        timeToNextEpisode = daysToGo == 0 ? "today" : "in " + TimeSpan.FromDays(daysToGo).Humanize(maxUnit: TimeUnit.Month, minUnit: TimeUnit.Day, precision: 2);
        timeToNextEpisode = "next episode airing " + timeToNextEpisode;
    }
    else if (inProduction)
    {
        timeToNextEpisode = "No release date yet";
    }
    else
    {
        timeToNextEpisode = "No more episodes";
    }
    string posterPath = posterStartPath + nextEpisode.poster_path;

    Console.WriteLine($"{tvShow.Item.Name} is {(inProduction ? "" : "not ")}in production{$"{(inProduction ? $", NETA: {nextEpisodeAirDate.ToShortDateString()} ({timeToNextEpisode})" : "")}"} and the poster is at: {posterStartPath}{posterPath}\n\n");
} while (input != "exit");

/*var movies = searchMovie("Star Trek").Result;

foreach (MovieInfo info in movies.Results)
{
    Console.WriteLine($"{info.Title} ({info.ReleaseDate})");
}*/

static async Task<ApiSearchResponse<TVShowInfo>> searchTV(string searchKey)
{
    var tvApi = MovieDbFactory.Create<IApiTVShowRequest>().Value;
    ApiSearchResponse<TVShowInfo> response = await tvApi.SearchByNameAsync(searchKey);
    return response;
}

static async Task<ApiSearchResponse<MovieInfo>> searchMovie(string searchKey)
{
    var movieApi = MovieDbFactory.Create<IApiMovieRequest>().Value;
    ApiSearchResponse<MovieInfo> response = await movieApi.SearchByTitleAsync(searchKey);
    return response;
}
/*public class episode
{
    //public string? next_episode_to_air { get; set; }
    public bool in_production { get; set; }
    public string poster_path { get; set; }
}*/



public class episode
{
    public bool adult { get; set; }
    public string backdrop_path { get; set; }
    public Created_By[] created_by { get; set; }
    public int[] episode_run_time { get; set; }
    public string first_air_date { get; set; }
    public Genre[] genres { get; set; }
    public string homepage { get; set; }
    public int id { get; set; }
    public bool in_production { get; set; }
    public string[] languages { get; set; }
    public string last_air_date { get; set; }
    public Last_Episode_To_Air last_episode_to_air { get; set; }
    public string name { get; set; }
    public Next_Episode_To_Air next_episode_to_air { get; set; }
    public Network[] networks { get; set; }
    public int number_of_episodes { get; set; }
    public int number_of_seasons { get; set; }
    public string[] origin_country { get; set; }
    public string original_language { get; set; }
    public string original_name { get; set; }
    public string overview { get; set; }
    public float popularity { get; set; }
    public string poster_path { get; set; }
    public Production_Companies[] production_companies { get; set; }
    public Production_Countries[] production_countries { get; set; }
    public Season[] seasons { get; set; }
    public Spoken_Languages[] spoken_languages { get; set; }
    public string status { get; set; }
    public string tagline { get; set; }
    public string type { get; set; }
    public float vote_average { get; set; }
    public int vote_count { get; set; }
    public Keywords keywords { get; set; }
}

public class Last_Episode_To_Air
{
    public string air_date { get; set; }
    public int episode_number { get; set; }
    public int id { get; set; }
    public string name { get; set; }
    public string overview { get; set; }
    public string production_code { get; set; }
    public int runtime { get; set; }
    public int season_number { get; set; }
    public string still_path { get; set; }
    public float vote_average { get; set; }
    public int vote_count { get; set; }
}

public class Next_Episode_To_Air
{
    public string air_date { get; set; }
    public int episode_number { get; set; }
    public int id { get; set; }
    public string name { get; set; }
    public string overview { get; set; }
    public string production_code { get; set; }
    public object runtime { get; set; }
    public int season_number { get; set; }
    public object still_path { get; set; }
    public float vote_average { get; set; }
    public int vote_count { get; set; }
}

public class Keywords
{
    public Result[] results { get; set; }
}

public class Result
{
    public string name { get; set; }
    public int id { get; set; }
}

public class Created_By
{
    public int id { get; set; }
    public string credit_id { get; set; }
    public string name { get; set; }
    public int gender { get; set; }
    public string profile_path { get; set; }
}

public class Genre
{
    public int id { get; set; }
    public string name { get; set; }
}

public class Network
{
    public string name { get; set; }
    public int id { get; set; }
    public string logo_path { get; set; }
    public string origin_country { get; set; }
}

public class Production_Companies
{
    public int id { get; set; }
    public string logo_path { get; set; }
    public string name { get; set; }
    public string origin_country { get; set; }
}

public class Production_Countries
{
    public string iso_3166_1 { get; set; }
    public string name { get; set; }
}

public class Season
{
    public string air_date { get; set; }
    public int episode_count { get; set; }
    public int id { get; set; }
    public string name { get; set; }
    public string overview { get; set; }
    public string poster_path { get; set; }
    public int season_number { get; set; }
}

public class Spoken_Languages
{
    public string english_name { get; set; }
    public string iso_639_1 { get; set; }
    public string name { get; set; }
}
