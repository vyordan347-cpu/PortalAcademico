using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortalAcademico.Data;
using PortalAcademico.Data.Enums;
using PortalAcademico.Models;
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

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Inscribirse(int id)
        {
            var curso = await _context.Cursos
                .FirstOrDefaultAsync(c => c.Id == id && c.Activo);

            if (curso == null)
            {
                TempData["Error"] = "El curso no existe o no está disponible.";
                return RedirectToAction(nameof(Index));
            }

            var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(usuarioId))
            {
                TempData["Error"] = "Debes iniciar sesión para inscribirte.";
                return RedirectToAction(nameof(Details), new { id });
            }

            var yaMatriculado = await _context.Matriculas
                .AnyAsync(m => m.CursoId == id && m.UsuarioId == usuarioId);

            if (yaMatriculado)
            {
                TempData["Error"] = "Ya estás matriculado en este curso.";
                return RedirectToAction(nameof(Details), new { id });
            }

            var cantidadMatriculados = await _context.Matriculas
                .CountAsync(m => m.CursoId == id && m.Estado != EstadoMatricula.Cancelada);

            if (cantidadMatriculados >= curso.CupoMaximo)
            {
                TempData["Error"] = "No hay cupos disponibles para este curso.";
                return RedirectToAction(nameof(Details), new { id });
            }

            var cursosDelUsuario = await _context.Matriculas
                .Where(m => m.UsuarioId == usuarioId && m.Estado != EstadoMatricula.Cancelada)
                .Include(m => m.Curso)
                .ToListAsync();

            var hayCruceHorario = cursosDelUsuario.Any(m =>
                m.Curso != null &&
                curso.HorarioInicio < m.Curso.HorarioFin &&
                curso.HorarioFin > m.Curso.HorarioInicio);

            if (hayCruceHorario)
            {
                TempData["Error"] = "No puedes inscribirte porque el horario se cruza con otro curso ya matriculado.";
                return RedirectToAction(nameof(Details), new { id });
            }

            var matricula = new Matricula
            {
                CursoId = curso.Id,
                UsuarioId = usuarioId,
                FechaRegistro = DateTime.UtcNow,
                Estado = EstadoMatricula.Pendiente
            };

            _context.Matriculas.Add(matricula);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Inscripción registrada correctamente en estado Pendiente.";
            return RedirectToAction(nameof(Details), new { id });
        }
    }
}