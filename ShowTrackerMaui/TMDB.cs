using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DM.MovieApi;
using DM.MovieApi.ApiResponse;
using DM.MovieApi.MovieDb.Movies;
using DM.MovieApi.MovieDb.TV;
using Newtonsoft.Json;
using Humanizer;
using Humanizer.Localisation;

namespace ShowTrackerMaui
{
    public class TMDB
    {
        const string bearerToken = "Token";
        const string posterStartPath = "https://www.themoviedb.org/t/p/w600_and_h900_bestv2";
        public static DateTime today = DateTime.Today;
        public static DateTime nextEpisodeAirDate = DateTime.Today;
        public static IApiMovieRequest movieApi;
        public static IApiTVShowRequest tvApi;

        public static void RegisterSettings()
        {
            MovieDbFactory.RegisterSettings(bearerToken);
            movieApi = MovieDbFactory.Create<IApiMovieRequest>().Value;
            tvApi = MovieDbFactory.Create<IApiTVShowRequest>().Value;
        }

        public static async Task<EpisodeInfo> GetInfoTwo(string input)
        {
            MovieDbFactory.RegisterSettings(bearerToken);
            movieApi = MovieDbFactory.Create<IApiMovieRequest>().Value;
            tvApi = MovieDbFactory.Create<IApiTVShowRequest>().Value;
            var tvShows = await tvApi.SearchByNameAsync(input); // breakpoint 1 - this breakpoint will be hit

            string showResult = FindShow(tvShows).Result; // breakpoint 2 - this breakpoint will not be hit, and will just crash out
            episode show = JsonConvert.DeserializeObject<episode>(showResult);

            EpisodeInfo episodeInfo = new EpisodeInfo(show.name);
            episodeInfo.ProductionStatus = show.in_production;

            if (show.next_episode_to_air != null)
            {
                DateTime.TryParse(show.next_episode_to_air.air_date, out nextEpisodeAirDate);
                episodeInfo.NextAirDate = nextEpisodeAirDate;
                TimeSpan difference = episodeInfo.NextAirDate - today;
                episodeInfo.DaysToGo = difference.Days;
            }

            episodeInfo.TimeToNextEpisode = TimeToNextEpisode(episodeInfo);

            episodeInfo.PosterPath = posterStartPath + show.poster_path;

            return episodeInfo;
            /*Console.WriteLine($"{episodeInfo.ShowName} is {(inProduction ? "" : "not ")}in production{$"{(inProduction ? $", NETA: {episodeInfo.NextAirDate.ToShortDateString()} ({timeToNextEpisode})" : "")}"} and the poster is at: {episodeInfo.PosterPath}\n\n");*/
        }

        public static EpisodeInfo GetInfo(string input)
        {
            MovieDbFactory.RegisterSettings(bearerToken);
            movieApi = MovieDbFactory.Create<IApiMovieRequest>().Value;
            tvApi = MovieDbFactory.Create<IApiTVShowRequest>().Value;
            var tvShows = SearchTV(input).Result;

            string showResult = FindShow(tvShows).Result;
            episode show = JsonConvert.DeserializeObject<episode>(showResult);

            EpisodeInfo episodeInfo = new EpisodeInfo(show.name);
            episodeInfo.ProductionStatus = show.in_production;

            if (show.next_episode_to_air != null)
            {
                DateTime.TryParse(show.next_episode_to_air.air_date, out nextEpisodeAirDate);
                episodeInfo.NextAirDate = nextEpisodeAirDate;
                TimeSpan difference = episodeInfo.NextAirDate - today;
                episodeInfo.DaysToGo = difference.Days;
            }
            
            episodeInfo.TimeToNextEpisode = TimeToNextEpisode(episodeInfo);

            episodeInfo.PosterPath = posterStartPath + show.poster_path;

            return episodeInfo;
            /*Console.WriteLine($"{episodeInfo.ShowName} is {(inProduction ? "" : "not ")}in production{$"{(inProduction ? $", NETA: {episodeInfo.NextAirDate.ToShortDateString()} ({timeToNextEpisode})" : "")}"} and the poster is at: {episodeInfo.PosterPath}\n\n");*/
        }

        public static string TimeToNextEpisode(EpisodeInfo episodeInfo)
        {
            string timeToNextEpisode;
            if (episodeInfo.DaysToGo >= 0)
            {
                timeToNextEpisode = "next episode airing " + (episodeInfo.DaysToGo == 0 ? "today" : "in " + TimeSpan.FromDays(episodeInfo.DaysToGo).Humanize(maxUnit: TimeUnit.Month, minUnit: TimeUnit.Day, precision: 2));
            }
            else if (episodeInfo.ProductionStatus)
            {
                timeToNextEpisode = "No release date yet";
            }
            else
            {
                timeToNextEpisode = "No more episodes";
            }
            return timeToNextEpisode;
        }

        static async Task<String> FindShow(ApiSearchResponse<TVShowInfo> tvShows)
        {
            var tvShow = await tvApi.FindByIdAsync(tvShows.Results[0].Id);
            return tvShow.Json;
        }


        static async Task<ApiSearchResponse<TVShowInfo>> SearchTV(string searchKey) // this one fails
        {
            Console.WriteLine("this is a test");
            ApiSearchResponse<TVShowInfo> response = await tvApi.SearchByNameAsync(searchKey);
            Console.WriteLine("this is 2 test");
            return response;
        }

        static async Task<ApiSearchResponse<MovieInfo>> SearchMovie(string searchKey)
        {
            ApiSearchResponse<MovieInfo> response = await movieApi.SearchByTitleAsync(searchKey);
            return response;
        }

        public class EpisodeInfo
        {
            public string ShowName { get; set; }
            public bool ProductionStatus { get; set; }
            public DateTime NextAirDate { get; set; }
            public int DaysToGo { get; set; }
            public string TimeToNextEpisode { get; set; }
            public string PosterPath { get; set; }

            public EpisodeInfo(string showName)
            {
                ShowName = showName;
                DaysToGo = -1;
            }
        }
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

    }
    
}
