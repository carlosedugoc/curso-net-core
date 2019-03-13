using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CursoNetCore.Contexts;
using CursoNetCore.Entities;
using CursoNetCore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
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
        public ActionResult Put(int id, [FromBody] AutorCreacionDTO value)
        {
            var autor = mapper.Map<Autor>(value);
            autor.Id = id;
            context.Entry(autor).State = EntityState.Modified;
            context.SaveChanges();
            return NoContent();
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult> Patch(int id, [FromBody] JsonPatchDocument<AutorCreacionDTO> patchDocument)
        {
            if(patchDocument == null)
            {
                return BadRequest();
            }
            var autorDeLaDB = await context.Autores.FirstOrDefaultAsync(x => x.Id == id);
            if (autorDeLaDB == null)
            {
                return NotFound();
            }
            var autorDTO = mapper.Map<AutorCreacionDTO>(autorDeLaDB);
            patchDocument.ApplyTo(autorDTO, ModelState);
            var isValid = TryValidateModel(autorDeLaDB);
            if (!isValid)
            {
                return BadRequest(ModelState);
            }
            mapper.Map(autorDTO, autorDeLaDB);
            await context.SaveChangesAsync();
            return NoContent();
          }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Autor>> Delete(int id)
        {
            var autorId = await context.Autores.Select(x=> x.Id).FirstOrDefaultAsync(x => x == id);
            if(autorId == default(int))
            {
                return NotFound();
            }
            context.Remove(new Autor { Id = autorId });
            await context.SaveChangesAsync();
            return NoContent();
        }


    }
}