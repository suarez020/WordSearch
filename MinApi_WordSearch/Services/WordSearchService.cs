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

            foreach (string rawWord in request.Palabras)       /// Itera sobre cada palabra a buscar buscando palabras.count.
            {
                Console.WriteLine($"\n--- Buscando la palabra: '{rawWord}' ---"); 

                string word = rawWord?.Trim() ?? string.Empty;/// Limpiamos
                if (string.IsNullOrEmpty(word))
                {
                    Console.WriteLine("Palabra es nula, vacía o solo espacios. Añadiendo resultado 'no encontrada'.");
                    results.Add(new ResultadoPalabras { Palabra = rawWord ?? "", Encontrada = false });
                    continue;                                 /// Pasa a la siguiente palabra en la lista.
                }

                bool encontrada = false;
                ResultadoPalabras? foundResult = null;
                var objetivo = word.ToUpperInvariant();                  /// Convierte la palabra a mayúsculas para la comparación.

                /// *Bucle principal: recorre cada celda de la matriz    /// Bucle de filas. La búsqueda se detiene si ya se encontró la palabra.
                for (int i = 0; i < filas && !encontrada; i++) 
                {
                    for (int j = 0; j < columnas && !encontrada; j++)    /// Bucle de columnas. También se detiene si se encuentra.
                    {
                        if (DataGridView[i, j] != objetivo[0]) continue; /// Verifica si la letra de la celda actual coincide con la primera letra de la palabra.
                                                                Console.WriteLine($"Encontrada coincidencia para la primera letra '{objetivo[0]}' en [{i},{j}].");

                        foreach (var (pasosVertical, pasosHorizontal, name) in DireccionPalabra)          /// *Itera sobre cada una de las 8 direcciones posibles.
                        {
                            Console.WriteLine($"  Intentando la dirección: {name}");
                            /// La llamada a MatchesDirection es el verdadero trabajo de búsqueda.
                            if (MatchesDirection(DataGridView, i, j, pasosVertical, pasosHorizontal, objetivo))
                            {
                                Console.WriteLine($"  ¡Palabra '{objetivo}' encontrada en la dirección {name}!");
                                encontrada = true;

                                /// Calcula las coordenadas finales de la palabra encontrada
                                var endRow = i + pasosVertical * (objetivo.Length - 1);
                                var endCol = j + pasosHorizontal * (objetivo.Length - 1);
                                foundResult = new ResultadoPalabras
                                {
                                    Palabra = word,
                                    Encontrada = true,
                                    Inicia = new Posicion { Row = i, Col = j },
                                    Finaliza = new Posicion { Row = endRow, Col = endCol },
                                    Direction = name
                                };
                                break; /// Detiene el bucle de direcciones una vez que la palabra ha sido encontrada.
                            }
                        }
                    }
                }
                if (foundResult != null){///Añade el resultado final a la lista de resultados
                    Console.WriteLine($"Resultado: La palabra '{word}' fue encontrada.");
                    results.Add(foundResult);
                }else{
                    Console.WriteLine($"Resultado: La palabra '{word}' NO fue encontrada.");
                    results.Add(new ResultadoPalabras { Palabra = word, Encontrada = false });
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
            int filas    = grid.GetLength(0); // 14
            int columnas = grid.GetLength(1); // 14

            Console.WriteLine($"  Verificando la palabra '{objetivo}' desde la posición [{startFila},{startColumna}].");

            for (int i = 0; i < objetivo.Length; i++)  ///iteramos cada palalabra "canguro"
            {
                // Calcula las coordenadas de la celda actual en la dirección dada.
                // 'pasosVertical' y 'pasosHorizontal' son los "pasos" que toma en cada iteración.
                int row = startFila + pasosVertical * i;
                int col = startColumna + pasosHorizontal * i;

                Console.WriteLine($"Iteración {i}: Verificando celda [{row},{col}] vs. letra '{objetivo[i]}'.");
                
                ///________________________________________________________________________________________________________________________________________
                /// Condición 1: Se asegura de que la celda actual esté dentro de los límites de la cuadrícula.
                /// Si no lo está, la palabra no puede encajar.
                if (row < 0 || row >= filas || col < 0 || col >= columnas)
                {
                    Console.WriteLine("¡Fuera de los límites! Retornando 'false'.");
                    return false;
                }
                ///________________________________________________________________________________________________________________________________________
                /// Condición 2: Compara la letra en la cuadrícula con la letra en la palabra objetivo.
                /// Si las letras no coinciden, la palabra no está en esta dirección.
                if (grid[row, col] != objetivo[i])
                {
                    Console.WriteLine($"No coincide. La celda tiene '{grid[row, col]}', pero se esperaba '{objetivo[i]}'. Retornando 'false'.");
                    return false;
                }
                ///________________________________________________________________________________________________________________________________________
                
                Console.WriteLine($"Coincide. La letra en la celda es '{grid[row, col]}'.");
            
            }
            /// Si el bucle termina sin retornar 'false', significa que todas las letras coincidieron.
            Console.WriteLine($"  ¡Todas las letras coincidieron! Retornando 'true'.");
            return true;
        }

        private static char[ , ] LlenandoMatriz(List<string> FilasDeMatriz)
        {
            Console.WriteLine("Inicio de ParseMatrix. Validando la matriz de entrada.");//validación básica
            if (FilasDeMatriz == null || FilasDeMatriz.Count == 0){
                Console.WriteLine("La matriz de entrada está vacía o es nula.");
                throw new ArgumentException("matriz vacía.");
            }

            List<string> filas = FilasDeMatriz;
            int cantidadFilas = filas.Count;
            Console.WriteLine($"Total de filas a procesar: {cantidadFilas}.");
            string[] firstSplit = filas[0].Split (new[] {',',' ',';'}, StringSplitOptions.RemoveEmptyEntries);
            int cantidadColumnas = firstSplit.Length;///primera fila saber cuántas columnas hay.
            Console.WriteLine($"Se determinó que la matriz tendrá {cantidadColumnas} columnas.");

            char[,] matriz = new char[cantidadFilas, cantidadColumnas];///inicializa ya sabemos la cantidad
            Console.WriteLine("Matriz de caracteres creada. Comenzando a llenar celdas...");

            ///recorrer las filas y llenar la matriz vamos a llenar la matriz de i,j
            for (int i = 0; i < cantidadFilas; i++)//fila
            {
                Console.WriteLine($"Procesando fila {i}...");
                var tokens = filas[i].Split(new[] { ',', ' ', ';' }, StringSplitOptions.RemoveEmptyEntries);

                /// Validación de consistencia: todas las filas deben tener la misma cantidad de elementos
                if (tokens.Length != cantidadColumnas)
                {
                    Console.WriteLine($"Error en la fila {i}. Se encontraron {tokens.Length} elementos, pero se esperaban {cantidadColumnas}.");
                    throw new ArgumentException($"Fila {i} tiene {tokens.Length} elementos; se esperaba {cantidadColumnas}.");
                }

                ///recorrer cada elemento de la fila Col
                for (int j = 0; j < cantidadColumnas; j++)
                {
                    string t = tokens[j].Trim();

                    if (t.Length != 1)
                    {
                        Console.WriteLine($"Error en la celda  [{i},{j}]. El valor '{t}' no es un solo carácter.");
                        throw new ArgumentException($"Elemento [{i},{j}] inválido: '{t}'. Debe ser un único carácter.");
                    }

                    // Asignar el carácter a la matriz, convirtiéndolo a mayúscula
                    matriz[i, j] = char.ToUpperInvariant(t[0]);
                    Console.WriteLine($"Asignando '{char.ToUpperInvariant(t[0])}' a la celda [{i},{j}].");
                }
            }

            Console.WriteLine("Matriz procesada con éxito.");
            return matriz;
        }
    }
    #endregion
}