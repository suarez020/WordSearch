namespace MinApi_WordSearch.Models
{
    /// <summary>
    /// Representa la estructura de los datos de entrada para el servicio de búsqueda de palabras.
    /// Contiene la lista de palabras a encontrar y la matriz de la sopa de letras.
    /// </summary>
    public class RequestWordSearch
    {
        /// <summary>
        /// Obtiene o establece la lista de palabras que se deben buscar en la matriz.
        /// Obtiene la matriz que se va a implementar.
        /// ejemplo : 
        ///  {
        ///    "Palabras": [
        ///    "MANATI", "LEON", "PERRO", "LORO", "GATO", "TORO", "CONEJO", "ORUGA",
        ///    "TIBURON", "ELEFANTE", "ALCON", "SERPIENTE", "JAGUAR", "CANGURO",
        ///    "LOBO", "MONO", "NUTRIA", "aRLEYaNIMALOTE"
        ///    ],
        ///  "Matriz": [
        ///    "N,D,E,K,I,C,A,N,G,U,R,O,G,E",
        ///    "S,X,R,Y,K,V,I,I,Q,G,W,Q,O,D",
        ///    "J,A,G,U,A,R,Z,W,B,N,K,O,U,A",
        ///    "M,L,E,L,E,F,A,N,T,E,H,O,G,W",
        ///    "L,O,B,O,N,U,T,R,I,A,O,U,S,U",
        ///    "W,W,O,S,O,G,A,T,O,V,R,T,M,O",
        ///    "H,L,Z,N,C,T,Y,Z,E,O,X,A,U,R",
        ///    "C,E,C,Y,T,I,B,U,R,O,N,S,R,O",
        ///    "C,O,N,E,J,O,Y,U,S,M,R,S,H,T",
        ///    "Y,N,I,F,E,F,P,T,E,Z,O,O,S,F",
        ///    "O,S,S,E,R,P,I,E,N,T,E,F,L,G",
        ///    "P,P,V,D,D,X,U,F,A,L,C,O,N,Y",
        ///    "M,O,N,O,C,U,Q,W,M,A,N,A,T,I",
        ///    "N,N,X,H,E,B,P,M,U,P,E,R,R,O"
        ///  ]
        ///}
        /// </summary>
        public List<string> Palabras { get; set; } = new();
        public List<string> Matriz { get; set; } = new();
    }
}