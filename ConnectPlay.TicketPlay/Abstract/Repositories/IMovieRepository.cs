using ConnectPlay.TicketPlay.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConnectPlay.TicketPlay.Abstract.Repositories;

public interface IMovieRepository
{
    public Task<IEnumerable<Movie>> GetCurrentMoviesAsync();
    public Task<IEnumerable<Movie>> GetNewMoviesAsync();
    public Task<IEnumerable<Movie>> SearchForMoviesAsync(string query, MovieFilters? filters);
}