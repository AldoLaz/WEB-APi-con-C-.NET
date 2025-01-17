﻿using System.ComponentModel.DataAnnotations;

namespace WebApiAutores.DTOs
{
    public class CreacionLibroDTO
    {
        [StringLength(maximumLength: 250)]
        [Required]
        public string Titulo { get; set; }
        public DateTime FechaPublicacion { get; set; }
        public List<int> AutoresIds { get; set; }
    }
}
