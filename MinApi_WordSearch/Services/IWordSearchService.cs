using Microsoft.AspNetCore.Cors.Infrastructure;
using MinApi_WordSearch.Models;

namespace MinApi_WordSearch.Services
{
    public interface IWordSearchService
    {
        List<ResultadoPalabras> BuscarPalabras(RequestWordSearch request);
    }
}