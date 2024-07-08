namespace WebApiAutores.Entidades
{
    public class AutorLibro
    {

        public int libroID {  get; set; }
        public int autorID { get; set; }
        public int orden { get; set; }
        public Libro Libro { get; set; }
        public Autor Autor { get; set; }
    }
}
