using System.Collections.Generic;

namespace MyMusic.ViewModels;

public abstract class GenreDto
{
    public string Name { get; set; }
}

public class ReadGenreDto : GenreDto
{
    public int Id { get; set; }
}

public class CreateGenreDto : GenreDto
{ }
