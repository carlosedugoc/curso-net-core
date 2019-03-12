using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CursoNetCore.Contexts;
using CursoNetCore.Entities;
using CursoNetCore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CursoNetCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AutoresController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public AutoresController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<AutorDTO>> Get()
        {
            return mapper.Map<List<AutorDTO>>(context.Autores.Include(x=> x.Libros).ToList());
        }

        [HttpGet("{id}", Name = "ObtenerAutor")]
        public ActionResult<AutorDTO> Get(int id)
        {
            var autor = mapper.Map<AutorDTO>(context.Autores.Include(x => x.Libros).FirstOrDefault(x => x.Id == id));

            if (autor == null)
            {
                return NotFound();
            }
            return autor;

        }

        [HttpPost]
        public ActionResult Post([FromBody] AutorCreacionDTO autorCreacion)
        {
            var autor = mapper.Map<Autor>(autorCreacion);
            context.Autores.Add(autor);
            context.SaveChanges();
            return new CreatedAtRouteResult("ObtenerAutor", new { id = autor.Id }, mapper.Map<AutorDTO>(autor)); 
        }

        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] Autor value)
        {
            if (id != value.Id)
            {
                return BadRequest();
            }
            context.Entry(value).State = EntityState.Modified;
            context.SaveChanges();
            return Ok();
        }

        [HttpDelete("{id}")]
        public ActionResult<Autor> Delete(int id)
        {
            var autor = context.Autores.FirstOrDefault(x => x.Id == id);
            context.Autores.Remove(autor);
            context.SaveChanges();
            return autor;
        }


    }
}