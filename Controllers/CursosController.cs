using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortalAcademico.Data;
using PortalAcademico.Models.ViewModels;

namespace PortalAcademico.Controllers
{
    public class CursosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CursosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Cursos
        [HttpGet]
        public async Task<IActionResult> Index(CatalogoCursosViewModel filtros)
        {
            if (filtros.CreditosMin.HasValue && filtros.CreditosMin.Value < 0)
            {
                ModelState.AddModelError(nameof(filtros.CreditosMin), "Los créditos mínimos no pueden ser negativos.");
            }

            if (filtros.CreditosMax.HasValue && filtros.CreditosMax.Value < 0)
            {
                ModelState.AddModelError(nameof(filtros.CreditosMax), "Los créditos máximos no pueden ser negativos.");
            }

            if (filtros.HorarioInicio.HasValue && filtros.HorarioFin.HasValue &&
                filtros.HorarioFin.Value <= filtros.HorarioInicio.Value)
            {
                ModelState.AddModelError(nameof(filtros.HorarioFin), "El horario fin debe ser mayor al horario inicio.");
            }

            var query = _context.Cursos
                .Where(c => c.Activo)
                .AsQueryable();

            if (ModelState.IsValid)
            {
                if (!string.IsNullOrWhiteSpace(filtros.Nombre))
                {
                    query = query.Where(c => c.Nombre.Contains(filtros.Nombre));
                }

                if (filtros.CreditosMin.HasValue)
                {
                    query = query.Where(c => c.Creditos >= filtros.CreditosMin.Value);
                }

                if (filtros.CreditosMax.HasValue)
                {
                    query = query.Where(c => c.Creditos <= filtros.CreditosMax.Value);
                }

                if (filtros.HorarioInicio.HasValue)
                {
                    query = query.Where(c => c.HorarioInicio >= filtros.HorarioInicio.Value);
                }

                if (filtros.HorarioFin.HasValue)
                {
                    query = query.Where(c => c.HorarioFin <= filtros.HorarioFin.Value);
                }
            }

            filtros.Cursos = await query
                .OrderBy(c => c.Nombre)
                .ToListAsync();

            return View(filtros);
        }

        // GET: /Cursos/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var curso = await _context.Cursos
                .FirstOrDefaultAsync(c => c.Id == id && c.Activo);

            if (curso == null)
            {
                return NotFound();
            }

            return View(curso);
        }
    }
}