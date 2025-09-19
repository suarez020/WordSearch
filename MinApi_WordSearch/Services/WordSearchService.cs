using MinApi_WordSearch.Models;
namespace MinApi_WordSearch.Services
{
    public class WordSearchService : IWordSearchService
    {
        public List<ResultadoPalabras> BuscarPalabras(RequestWordSearch request)
        {
            Console.WriteLine("Iniciando la búsqueda de palabras.");
            char[,] DataGridView = LlenandoMatriz(request.Matriz);   /// Convierte la lista de strings a una matriz de caracteres.
            int filas = DataGridView.GetLength(0);                  /// Obtiene el número de filas (0 es la primera dimensión).
            int columnas = DataGridView.GetLength(1);              /// Obtiene el número de columnas (1 es la segunda dimensión).
            Console.WriteLine($"Matriz de {filas} filas por {columnas} columnas.");
            List<ResultadoPalabras> results = new List<ResultadoPalabras>();

            foreach (string rawWord in request.Palabras)      
            {
                Console.WriteLine($"\n--- Buscando la palabra: '{rawWord}' ---"); 

                string palabra = rawWord?.Trim() ?? string.Empty;
                if (string.IsNullOrEmpty(palabra))
                {
                    Console.WriteLine("Palabra es nula, vacía o solo espacios. Añadiendo resultado 'no encontrada'.");
                    results.Add(new ResultadoPalabras { Palabra = rawWord ?? "", Encontrada = false });
                    continue;                                 /// Pasa a la siguiente palabra en la lista.
                }

                bool encontrada = false;
                ResultadoPalabras? ResultadoEncontrado = null;
                var objetivo = palabra.ToUpperInvariant();               

                
                for (int i = 0; i < filas && !encontrada; i++) 
                {
                    for (int j = 0; j < columnas && !encontrada; j++)    
                    {
                        if (DataGridView[i, j] != objetivo[0]) continue; ///letra de la celda actual coincide con la primera letra de la palabra.
                                                                Console.WriteLine($"Encontrada coincidencia para la primera letra '{objetivo[0]}' en [{i},{j}].");

                        foreach (var (pasosVertical, pasosHorizontal, name) in DireccionPalabra)
                        {
                            Console.WriteLine($"  Intentando la dirección: {name}");
                         
                                    if (MatchesDirection(DataGridView, i, j, pasosVertical, pasosHorizontal, objetivo))
                                    {
                                        Console.WriteLine($"  ¡Palabra '{objetivo}' encontrada en la dirección {name}!");
                                        encontrada = true;
                                        var endRow = i + pasosVertical * (objetivo.Length - 1);
                                        var endCol = j + pasosHorizontal * (objetivo.Length - 1);
                                        ResultadoEncontrado = new ResultadoPalabras
                                        {
                                            Palabra = palabra,
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
                if (ResultadoEncontrado != null){///Añade el resultado final a la lista de resultados
                    Console.WriteLine($"Resultado: La palabra '{palabra}' fue encontrada.");
                    results.Add(ResultadoEncontrado);
                }else{
                    Console.WriteLine($"Resultado: La palabra '{palabra}' NO fue encontrada.");
                    results.Add(new ResultadoPalabras { Palabra = palabra, Encontrada = false });
                }
            }
            Console.WriteLine("\nProceso de búsqueda de todas las palabras finalizado. Retornando resultados.");
            return results;
        }

        #region Metodos Auxiliares:
        private static readonly (int pasosVertical, int pasosHorizontal, string name)[] DireccionPalabra = new[]
        {
            ( 0,  1, "derecha"),
            ( 0, -1, "izquierda"),
            ( 1,  0, "abajo"),
            (-1,  0, "arriba"),
            ( 1,  1, "DiagonalAbajoDerecha"),
            ( 1, -1, "DiagonalAbajoIzquierda"),
            (-1,  1, "DiagonalArribaDerecha"),
            (-1, -1, "DiagonalArribaIzquierda")
        };

        private static bool MatchesDirection(char[,] grid, int startFila, int startColumna, int pasosVertical, int pasosHorizontal, string objetivo)
        {
            int filas    = grid.GetLength(0); 
            int columnas = grid.GetLength(1); 
            Console.WriteLine($"  Verificando la palabra '{objetivo}' desde la posición [{startFila},{startColumna}].");

            for (int i = 0; i < objetivo.Length; i++)
            {
                int row = startFila + pasosVertical * i;      ///fila en matriz
                int col = startColumna + pasosHorizontal * i;///columna en matriz

                Console.WriteLine($"Iteración {i}: Verificando celda [{row},{col}] vs. letra '{objetivo[i]}'.");
                
                
                ///_______________________________límites de la cuadrícula.
                if (row < 0 || row >= filas || col < 0 || col >= columnas)    
                {
                    Console.WriteLine("¡Fuera de los límites! Retornando 'false'.");
                    return false;
                }
                ///_______________________________Compara la letra en la cuadrícula con la letra en la palabra objetivo.
                if (grid[row, col] != objetivo[i])
                {
                    Console.WriteLine($"No coincide. La celda tiene '{grid[row, col]}', pero se esperaba '{objetivo[i]}'. Retornando 'false'.");
                    return false;
                }

               Console.WriteLine($"Coincide. La letra en la celda es '{grid [row, col]}'.");
            
            }
            
            Console.WriteLine($"Todas las letras coincidieron.");
            return true;
        }

        private static char[ , ] LlenandoMatriz(List<string> FilasDeMatriz)
        {
            Console.WriteLine("Inicio de ParseMatrix. Validando la matriz de entrada.");///validación básica
            if (FilasDeMatriz == null || FilasDeMatriz.Count == 0){
                Console.WriteLine("La matriz de entrada está vacía o es nula.");
                throw new ArgumentException("matriz vacía.");
            }

            List<string> filas = FilasDeMatriz;
            int cantidadFilas = filas.Count;
            Console.WriteLine($"Total de filas a procesar: {cantidadFilas}.");
            string[] firstSplit = filas[0].Split (new[] {',',' ',';'}, StringSplitOptions.RemoveEmptyEntries);
            int cantidadColumnas = firstSplit.Length;
            Console.WriteLine($"Se determinó que la matriz tendrá {cantidadColumnas} columnas.");

            char[,] matriz = new char[cantidadFilas, cantidadColumnas];///inicializa ya sabemos la cantidad
            Console.WriteLine("Matriz de caracteres creada. Comenzando a llenar celdas...");

            for (int i = 0; i < cantidadFilas; i++)
            {
                Console.WriteLine($"Procesando fila {i}...");
                var tokens = filas[i].Split(new[] { ',', ' ', ';' }, StringSplitOptions.RemoveEmptyEntries);

                if (tokens.Length != cantidadColumnas)///todas las filas deben tener la misma cantidad de elementos
                {
                    Console.WriteLine($"Error en la fila {i}. Se encontraron {tokens.Length} elementos, pero se esperaban {cantidadColumnas}.");
                      throw new ArgumentException($"Fila {i} tiene {tokens.Length} elementos; se esperaba {cantidadColumnas}.");
                }
                        for (int j = 0; j < cantidadColumnas; j++)
                        {
                            string letra = tokens[j].Trim();

                            if (letra.Length != 1)
                            {
                                Console.WriteLine($"Error en la celda  [{i},{j}]. El valor '{letra}' no es un solo carácter.");
                                throw new ArgumentException($"Elemento [{i},{j}] inválido: '{letra}'. Debe ser un único carácter.");
                            }

                            matriz[i, j] = char.ToUpperInvariant(letra[0]);
                            Console.WriteLine($"Asignando '{char.ToUpperInvariant(letra[0])}' a la celda [{i},{j}].");
                        }
            }

            Console.WriteLine("Matriz procesada con éxito.");
            return matriz;
        }
        #endregion
    }
}