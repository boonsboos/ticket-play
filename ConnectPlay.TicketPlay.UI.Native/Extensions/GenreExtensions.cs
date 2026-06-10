using ConnectPlay.TicketPlay.Contracts.Movie;
using ConnectPlay.TicketPlay.Models;
using ConnectPlay.TicketPlay.UI.Native.Resources;

namespace ConnectPlay.TicketPlay.UI.Native.Extensions;

public static class GenreExtensions
{
    public static string GetTranslatedGenre(this OverviewMovie movie)
        => movie.Genre switch
        {
            "thriller" => AppResources.Genre_Thriller,
            "family" => AppResources.Genre_Family,
            "drama" => AppResources.Genre_Drama,
            "science_fiction" => AppResources.Genre_ScienceFiction,
            _ => movie.Genre
        };

    public static string GetTranslatedGenre(this MovieDetailResponse movie)
    => movie.Genre switch
    {
        "thriller" => AppResources.Genre_Thriller,
        "family" => AppResources.Genre_Family,
        "drama" => AppResources.Genre_Drama,
        "science_fiction" => AppResources.Genre_ScienceFiction,
        _ => movie.Genre
    };


    public static string GetTranslatedGenre(this Movie movie)
    => movie.Genre switch
    {
        "thriller" => AppResources.Genre_Thriller,
        "family" => AppResources.Genre_Family,
        "drama" => AppResources.Genre_Drama,
        "science_fiction" => AppResources.Genre_ScienceFiction,
        _ => movie.Genre
    };
}
