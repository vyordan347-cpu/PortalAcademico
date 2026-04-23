using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using PortalAcademico.Data;
using PortalAcademico.Data.Enums;
using PortalAcademico.Models;
using PortalAcademico.Models.ViewModels;

namespace PortalAcademico.Controllers
{
    [Authorize(Roles = "Coordinador")]
    public class CoordinadorController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IDistributedCache _cache;

        public CoordinadorController(ApplicationDbContext context, IDistributedCache cache)
        {
            _context = context;
            _cache = cache;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var cursos = await _context.Cursos
                .OrderBy(c => c.Nombre)
                .ToListAsync();

            return View(cursos);
        }

        [HttpGet]
        public IActionResult CrearCurso()
        {
            return View(new CursoFormViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearCurso(CursoFormViewModel model)
        {
            if (model.HorarioFin <= model.HorarioInicio)
            {
                ModelState.AddModelError(nameof(model.HorarioFin), "El horario fin debe ser mayor al horario inicio.");
            }

            var codigoExiste = await _context.Cursos.AnyAsync(c => c.Codigo == model.Codigo);
            if (codigoExiste)
            {
                ModelState.AddModelError(nameof(model.Codigo), "Ya existe un curso con ese código.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var curso = new Curso
            {
                Codigo = model.Codigo,
                Nombre = model.Nombre,
                Creditos = model.Creditos,
                CupoMaximo = model.CupoMaximo,
                HorarioInicio = model.HorarioInicio,
                HorarioFin = model.HorarioFin,
                Activo = model.Activo
            };

            _context.Cursos.Add(curso);
            await _context.SaveChangesAsync();
            await InvalidarCacheCursosAsync();

            TempData["Success"] = "Curso creado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> EditarCurso(int id)
        {
            var curso = await _context.Cursos.FindAsync(id);
            if (curso == null)
            {
                return NotFound();
            }

            var model = new CursoFormViewModel
            {
                Id = curso.Id,
                Codigo = curso.Codigo,
                Nombre = curso.Nombre,
                Creditos = curso.Creditos,
                CupoMaximo = curso.CupoMaximo,
                HorarioInicio = curso.HorarioInicio,
                HorarioFin = curso.HorarioFin,
                Activo = curso.Activo
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarCurso(CursoFormViewModel model)
        {
            if (model.HorarioFin <= model.HorarioInicio)
            {
                ModelState.AddModelError(nameof(model.HorarioFin), "El horario fin debe ser mayor al horario inicio.");
            }

            var codigoExiste = await _context.Cursos
                .AnyAsync(c => c.Codigo == model.Codigo && c.Id != model.Id);

            if (codigoExiste)
            {
                ModelState.AddModelError(nameof(model.Codigo), "Ya existe un curso con ese código.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var curso = await _context.Cursos.FindAsync(model.Id);
            if (curso == null)
            {
                return NotFound();
            }

            curso.Codigo = model.Codigo;
            curso.Nombre = model.Nombre;
            curso.Creditos = model.Creditos;
            curso.CupoMaximo = model.CupoMaximo;
            curso.HorarioInicio = model.HorarioInicio;
            curso.HorarioFin = model.HorarioFin;
            curso.Activo = model.Activo;

            await _context.SaveChangesAsync();
            await InvalidarCacheCursosAsync();

            TempData["Success"] = "Curso actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DesactivarCurso(int id)
        {
            var curso = await _context.Cursos.FindAsync(id);
            if (curso == null)
            {
                return NotFound();
            }

            curso.Activo = false;
            await _context.SaveChangesAsync();
            await InvalidarCacheCursosAsync();

            TempData["Success"] = "Curso desactivado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> MatriculasPorCurso(int id)
        {
            var curso = await _context.Cursos
                .FirstOrDefaultAsync(c => c.Id == id);

            if (curso == null)
            {
                return NotFound();
            }

            var matriculas = await _context.Matriculas
                .Where(m => m.CursoId == id)
                .OrderByDescending(m => m.FechaRegistro)
                .ToListAsync();

            var model = new CursoMatriculasViewModel
            {
                Curso = curso,
                Matriculas = matriculas
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmarMatricula(int id)
        {
            var matricula = await _context.Matriculas.FindAsync(id);
            if (matricula == null)
            {
                return NotFound();
            }

            matricula.Estado = EstadoMatricula.Confirmada;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Matrícula confirmada.";
            return RedirectToAction(nameof(MatriculasPorCurso), new { id = matricula.CursoId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelarMatricula(int id)
        {
            var matricula = await _context.Matriculas.FindAsync(id);
            if (matricula == null)
            {
                return NotFound();
            }

            matricula.Estado = EstadoMatricula.Cancelada;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Matrícula cancelada.";
            return RedirectToAction(nameof(MatriculasPorCurso), new { id = matricula.CursoId });
        }

        private async Task InvalidarCacheCursosAsync()
        {
            try
            {
                await _cache.RemoveAsync(AppCacheKeys.CursosActivos);
            }
            catch
            {
            }
        }
    }
}