using MinApi_WordSearch.Models;

namespace MinApi_WordSearch.Services
{
    public class WordSearchService : IWordSearchService
    {
        private static readonly (int dr, int dc, string name)[] DireccionPalabra = new[]
        {   ///las 8 direcciones.
            ( 0, 1, "derecha"),
            ( 0,-1, "izquierda"),
            ( 1, 0, "abajo"),
            (-1, 0, "arriba"),
            ( 1, 1, "DiagonalAbajoDerecha"),
            ( 1,-1, "DiagonalAbajoIzquierda"),
            (-1, 1, "DiagonalArribaDerecha"),
            (-1,-1, "DiagonalArribaIzquierda")
        };

        public List<ResultadoPalabras> BuscarPalabras(RequestWordSearch request)
        {
            Console.WriteLine("Paso 1: Iniciando la búsqueda de palabras.");
            char[,] DataGridView = ParseMatrix(request.Matriz); /// Convierte la lista de strings a una matriz de caracteres.
            int filas = DataGridView.GetLength(0);             /// Obtiene el número de filas (0 es la primera dimensión).
            int columnas = DataGridView.GetLength(1);         /// Obtiene el número de columnas (1 es la segunda dimensión).
            Console.WriteLine($"Matriz de {filas} filas por {columnas} columnas.");

            List<ResultadoPalabras> results = new List<ResultadoPalabras>();

            Console.WriteLine($"Paso 2: Buscando {request.Palabras.Count} palabras.");
            foreach (var rawWord in request.Palabras)
            {
                Console.WriteLine($"\n--- Buscando la palabra: '{rawWord}' ---");

                string word = rawWord?.Trim() ?? string.Empty; /// Limpia la palabra de espacios extra.
                if (string.IsNullOrEmpty(word))
                {
                    Console.WriteLine("Palabra es nula, vacía o solo espacios. Añadiendo resultado 'no encontrada'.");
                    results.Add(new ResultadoPalabras { Palabra = rawWord ?? "", Encontrada = false });
                    continue;
                }

                bool found = false;
                ResultadoPalabras? foundResult = null;
                var objetivo = word.ToUpperInvariant();

                for (int i = 0; i < filas && !found; i++)
                {
                    for (int j = 0; j < columnas && !found; j++) 
                    {
                        if (DataGridView[i, j] != objetivo[0]) continue;
                        Console.WriteLine($"Encontrada coincidencia para la primera letra '{objetivo[0]}' en [{i},{j}].");

                        foreach (var (dr, dc, name) in DireccionPalabra)
                        {
                            Console.WriteLine($"  Intentando la dirección: {name}");
                            if (MatchesDireccion(DataGridView, i, j, dr, dc, objetivo))
                            {
                                Console.WriteLine($"  ¡Palabra '{objetivo}' encontrada en la dirección {name}!");
                                found = true;

                                var endRow = i + dr * (objetivo.Length - 1);
                                var endCol = j + dc * (objetivo.Length - 1);
                                foundResult = new ResultadoPalabras
                                {
                                    Palabra = word,
                                    Encontrada = true,
                                    Inicia = new Posicion { Row = i, Col = j },
                                    Finaliza = new Posicion { Row = endRow, Col = endCol },
                                    Direction = name
                                };
                                break; 
                            }
                        }
                    }
                }

                if (foundResult != null)
                {
                    Console.WriteLine($"Resultado: La palabra '{word}' fue encontrada.");
                    results.Add(foundResult);
                }
                else
                {
                    Console.WriteLine($"Resultado: La palabra '{word}' NO fue encontrada.");
                    results.Add(new ResultadoPalabras { Palabra = word, Encontrada = false });
                }
            }

            Console.WriteLine("\nProceso de búsqueda de todas las palabras finalizado. Retornando resultados.");
            return results;
        }

        private static bool MatchesDireccion(char[,] grid, int startFila, int startColumna, int dr, int dc, string objetivo)
        {
            int rows = grid.GetLength(0); // 14
            int cols = grid.GetLength(1); // 14

            Console.WriteLine($"Verificando la palabra '{objetivo}' desde la posición [{startFila},{startColumna}].");

            for (int i = 0; i < objetivo.Length; i++)
            {
                int r = startFila + dr * i;
                int c = startColumna + dc * i;

                Console.WriteLine($"Iteración {i}: Verificando celda [{r},{c}] vs. letra '{objetivo[i]}'.");

                if (r < 0 || r >= rows || c < 0 || c >= cols)
                {
                    Console.WriteLine("Fuera de los límites. Retornando 'false'.");
                    return false;
                }

                if (grid[r, c] != objetivo[i])
                {
                    Console.WriteLine($"No coincide. La celda tiene '{grid[r, c]}', pero se esperaba '{objetivo[i]}'. Retornando 'false'.");
                    return false;
                }

                Console.WriteLine($"Coincide. La letra en la celda es '{grid[r, c]}'.");
            }

            Console.WriteLine($"Todas las letras coincidieron. Retornando 'true'.");
            return true;
        }

        private static char[,] ParseMatrix(List<string> FilasDeMatriz)
        {
            Console.WriteLine("Inicio de ParseMatrix. Validando la matriz de entrada.");
            if (FilasDeMatriz == null || FilasDeMatriz.Count == 0)
            {
                Console.WriteLine("La matriz de entrada está vacía o es nula.");
                throw new ArgumentException("matriz vacía.");
            }

            List<string> filas = FilasDeMatriz;
            int cantidadFilas = filas.Count; 
            Console.WriteLine($"Total de filas a procesar: {cantidadFilas}.");

            string[] firstSplit = filas[0].Split(new[] { ',', ' ', ';' }, StringSplitOptions.RemoveEmptyEntries);
            int cantidadColumnas = firstSplit.Length; 
            Console.WriteLine($"Se determinó que la matriz tendrá {cantidadColumnas} columnas.");

            char[,] matriz = new char[cantidadFilas, cantidadColumnas];
            Console.WriteLine("Matriz de caracteres creada. Comenzando a llenar celdas.");

            for (int i = 0; i < cantidadFilas; i++)
            {
                Console.WriteLine($"Procesando fila {i}...");
                var tokens = filas[i].Split(new[] { ',', ' ', ';' }, StringSplitOptions.RemoveEmptyEntries);

                if (tokens.Length != cantidadColumnas)
                {
                    Console.WriteLine($"Error en la fila {i}. Se encontraron {tokens.Length} elementos, pero se esperaban {cantidadColumnas}.");
                    throw new ArgumentException($"Fila {i} tiene {tokens.Length} elementos; se esperaba {cantidadColumnas}.");
                }

                for (int j = 0; j < cantidadColumnas; j++)
                {
                    string t = tokens[j].Trim();

                    if (t.Length != 1)
                    {
                        Console.WriteLine($"Error en la celda  [{i},{j}]. El valor '{t}' no es un solo carácter.");
                        throw new ArgumentException($"Elemento [{i},{j}] inválido: '{t}'. Debe ser un único carácter.");
                    }

                    matriz[i, j] = char.ToUpperInvariant(t[0]);
                    Console.WriteLine($"Asignando '{char.ToUpperInvariant(t[0])}' a la celda [{i},{j}].");
                }
            }

            Console.WriteLine("Matriz procesada con éxito.");
            return matriz;
        }
    }
}
