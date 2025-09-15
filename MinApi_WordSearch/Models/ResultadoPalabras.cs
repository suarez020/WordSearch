namespace MinApi_WordSearch.Models
{
    public class ResultadoPalabras
    {
        public string Palabra { get; set; }
        public bool Encontrada { get; set; }
        public Posicion? Inicia { get; set; }  ///cordenadas. 
        public Posicion? Finaliza { get; set; }///cordenadas. 
        public string? Direction { get; set; } /// direccion de la palabra encontrada. 
    }
}
