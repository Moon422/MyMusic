using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyMusic.Backend.Exceptions;
using MyMusic.Backend.Models;
using MyMusic.ViewModels;

namespace MyMusic.Backend.Services;

public interface IGenreService
{
    Task<ReadGenreDto> GetGenreById(int id);
    Task<List<ReadGenreDto>> GetAllGenre();
    Task<ReadGenreDto> CreateGenre(CreateGenreDto createGenre);
}

public class GenreService : IGenreService
{
    private readonly MusicDB dbcontext;

    public GenreService(MusicDB dbcontext)
    {
        this.dbcontext = dbcontext;
    }

    public async Task<List<ReadGenreDto>> GetAllGenre()
    {
        var genres = await dbcontext.Genres
                        .Select(
                            g => new ReadGenreDto
                            {
                                Id = g.Id,
                                Name = g.Name
                            }
                        ).ToListAsync();

        return genres;
    }

    public async Task<ReadGenreDto> GetGenreById(int id)
    {
        var genre = await dbcontext.Genres.FindAsync(id);

        if (genre is not null)
        {
            return new ReadGenreDto
            {
                Id = genre.Id,
                Name = genre.Name
            };
        }
        else
        {
            throw new GenreNotFoundException();
        }
    }

    public async Task<ReadGenreDto> CreateGenre(CreateGenreDto createGenre)
    {
        Genre genre = new Genre
        {
            Name = createGenre.Name
        };

        await dbcontext.Genres.AddAsync(genre);
        await dbcontext.SaveChangesAsync();

        return new ReadGenreDto
        {
            Id = genre.Id,
            Name = genre.Name
        };
    }
}
