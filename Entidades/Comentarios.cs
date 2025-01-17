﻿using Microsoft.AspNetCore.Identity;

namespace WebApiAutores.Entidades
{
    public class Comentarios
    {
        public int Id { get; set; } 
        public string Contenido { get; set;}
        public int LibroId { get; set;}
        public Libro Libro { get; set;}
        public string UsuaioId { get; set;}
        public IdentityUser Usuario { get; set;}
    }
}
