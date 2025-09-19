using MinApi_WordSearch.Models;
using MinApi_WordSearch.Services;

namespace MinApi_WordSearch.Endpoints
{
    public static class WordSearchEndpoints
    {
        public static void MapEndpoints(this WebApplication app)
        {
             ///endPoint #1:
            ///
            RouteGroupBuilder routeGroup = app.MapGroup("/api/wordsearch");
            
            routeGroup.MapPost("/find", ProcesarBusquedaPalabras)
                .WithName("FindWords")
                .Produces<List<ResultadoPalabras>>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status500InternalServerError);
        }

        #region
        private static async Task<IResult> ProcesarBusquedaPalabras(RequestWordSearch request, IWordSearchService svc)  
        /// IWordSearchService svc = aquí el método depende de una abstracción(la interfaz), no de una implementación concreta.Eso es inyeccion.
        {
            if (request == null || request.Palabras == null || request.Matriz == null)// Validaciones. 
                return Results.BadRequest("Request inválido. 'palabras' y 'matriz' son obligatorios.");

            try
            {
                List<ResultadoPalabras> result = svc.BuscarPalabras(request);///instancia de servicio inyectado
                return Results.Ok(result);
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return Results.StatusCode(500);
            }
        }
        #endregion
    }

}