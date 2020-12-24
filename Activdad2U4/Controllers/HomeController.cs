using Activdad2U4.Helpers;
using Activdad2U4.Models;
using Activdad2U4.Models.ViewModels;
using Activdad2U4.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Activdad2U4.Controllers
{
    public class HomeController : Controller
    {
        [Authorize(Roles = "Maestro, Director")]
        public IActionResult Index(int nocontrol)
        {
            return View();
        }
        [AllowAnonymous]
        public IActionResult InicioSesionDirector()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> InicioSesionDirector(Director dir)
        {
            roleContext context = new roleContext();
            UsuariosRepository<Director> directorRepos = new UsuariosRepository<Director>(context);
            var director = context.Director.FirstOrDefault(x => x.NoControl == dir.NoControl);
            try
            {
                if (director != null && director.Contrasena == HashingHelpers.GetHash(dir.Contrasena))
                {
                    List<Claim> info = new List<Claim>();
                    info.Add(new Claim(ClaimTypes.Name, "Usuario" + director.Nombre));
                    info.Add(new Claim(ClaimTypes.Role, "Director"));
                    info.Add(new Claim("NoControl", director.Nombre.ToString()));
                    info.Add(new Claim("Nombre", director.Nombre));

                    var claimsidentity = new ClaimsIdentity(info, CookieAuthenticationDefaults.AuthenticationScheme);
                    var claimsprincipal = new ClaimsPrincipal(claimsidentity);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsprincipal,
                        new AuthenticationProperties { IsPersistent = true });
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "El número de control o la contraseña del director son incorrectas.");
                    return View(dir);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(dir);
            }
        }
        [Authorize(Roles = "Director")]
        public IActionResult RegistrarDocente()
        {
            return View();
        }
        [Authorize(Roles = "Director")]
        [HttpPost]
        public IActionResult RegistrarDocente(Maestro doc)
        {
            roleContext context = new roleContext();
            DocenteRepository repository = new DocenteRepository(context);

            try
            {
                var maestro = repository.GetDocenteByClave(doc.NoControl);
                if (maestro == null)
                {
                    doc.Activo = 1;
                    doc.Contrasena = HashingHelpers.GetHash(doc.Contrasena);
                    repository.Insert(doc);
                    return RedirectToAction("VerDocente");
                }
                else
                {
                    ModelState.AddModelError("", "Clave no está disponible.");
                    return View(doc);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(doc);
            }
        }

        [AllowAnonymous]
        public IActionResult InicioSesionDocente()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> InicioSesionDocente(Maestro doc)
        {
            roleContext context = new roleContext();
            DocenteRepository repository = new DocenteRepository(context);
            var docent = repository.GetDocenteByClave(doc.NoControl);
            try
            {
                if (docent != null && docent.Contrasena == HashingHelpers.GetHash(doc.Contrasena))
                {
                    if (docent.Activo == 1)
                    {
                        List<Claim> info = new List<Claim>();
                        info.Add(new Claim(ClaimTypes.Name, "Usuario" + docent.Nombre));
                        info.Add(new Claim(ClaimTypes.Role, "Maestro"));
                        info.Add(new Claim("NoControl", docent.NoControl.ToString()));
                        info.Add(new Claim("Nombre", docent.Nombre));
                        info.Add(new Claim("Id", docent.Id.ToString()));

                        var claimsidentity = new ClaimsIdentity(info, CookieAuthenticationDefaults.AuthenticationScheme);
                        var claimsprincipal = new ClaimsPrincipal(claimsidentity);
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsprincipal,
                            new AuthenticationProperties { IsPersistent = true });
                        return RedirectToAction("Index", docent.NoControl);
                    }
                    else
                    {
                        ModelState.AddModelError("", "Su cuenta está desactivada.");
                        return View(doc);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "El número de control o la contraseña del docente son incorrectas.");
                    return View(doc);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(doc);
            }
        }

        [AllowAnonymous]
        public async Task<IActionResult> CerrarSesion()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "Director")]
        public IActionResult VerDocente()
        {
            roleContext context = new roleContext();
            DocenteRepository dnteRepos = new DocenteRepository(context);
            var docent = dnteRepos.GetAll();
            return View(docent);
        }

        [Authorize(Roles = "Director")]
        [HttpPost]
        public IActionResult ActivarDocente(Maestro doc)
        {
            roleContext context = new roleContext();
            DocenteRepository docenteRepos = new DocenteRepository(context);
            var docent = docenteRepos.GetById(doc.Id);
            if (docent != null && docent.Activo == 0)
            {
                docent.Activo = 1;
                docenteRepos.Edit(docent);
            }
            else
            {
                docent.Activo = 0;
                docenteRepos.Edit(docent);
            }
            return RedirectToAction("VerDocente");
        }
        [Authorize(Roles = "Director")]
        public IActionResult EditarDocente(int id)
        {
            roleContext context = new roleContext();
            DocenteRepository docenteRepos = new DocenteRepository(context);
            var docent = docenteRepos.GetById(id);
            if (docent != null)
            {
                return View(docent);
            }
            return RedirectToAction("VerDocente");
        }
        [Authorize(Roles = "Director")]
        [HttpPost]
        public IActionResult EditarDocente(Maestro doc)
        {
            roleContext context = new roleContext();
            DocenteRepository deocenteRepos = new DocenteRepository(context);
            var docent = deocenteRepos.GetById(doc.Id);
            try
            {
                if (docent != null)
                {
                    docent.Nombre = doc.Nombre;
                    deocenteRepos.Edit(docent);
                }
                return RedirectToAction("VerDocente");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(docent);
            }
        }
        [Authorize(Roles = "Director")]
        public IActionResult CambiarContrasena(int id)
        {
            roleContext context = new roleContext();
            DocenteRepository repository = new DocenteRepository(context);
            var maestro = repository.GetById(id);
            if (maestro == null)
            {
                return RedirectToAction("VerDocente");
            }
            return View(maestro);
        }
        [Authorize(Roles = "Director")]
        [HttpPost]
        public IActionResult CambiarContrasena(Maestro doc, string nuevaContra, string confirmarContra)
        {
            roleContext context = new roleContext();
            DocenteRepository docenteRepos = new DocenteRepository(context);
            var docent = docenteRepos.GetById(doc.Id);
            try
            {

                if (docent != null)
                {
                    if (nuevaContra != confirmarContra)
                    {
                        ModelState.AddModelError("", "Las contraseñas no coinciden.");
                        return View(docent);
                    }
                    else if (docent.Contrasena == HashingHelpers.GetHash(nuevaContra))
                    {
                        ModelState.AddModelError("", "La contraseña ingresada es antigua, ingrese la nueva.");
                        return View(docent);
                    }
                    else
                    {
                        docent.Contrasena = HashingHelpers.GetHash(nuevaContra);
                        docenteRepos.Edit(docent);
                        return RedirectToAction("VerDocente");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "El docente al que intentó editar no existe.");
                    return View(docent);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(docent);
            }
        }
        [Authorize(Roles = "Maestro, Director")]
        public IActionResult VerAlumno(int id)
        {
            roleContext context = new roleContext();
            DocenteRepository docenteRepos = new DocenteRepository(context);
            var docent = docenteRepos.GetAlumnoByDocente(id);
            if (docent != null)
            {
                if (User.IsInRole("Maestro"))
                {
                    if (User.Claims.FirstOrDefault(x => x.Type == "Id").Value == docent.Id.ToString())
                    {
                        return View(docent);
                    }
                    else
                    {
                        return RedirectToAction("Error");
                    }
                }
                else
                {
                    return View(docent);
                }
            }
            else
            {
                return RedirectToAction("VerAlumno");
            }
        }
        [Authorize(Roles = "Maestro, Director")]
        public IActionResult AgregarAlumno(int id)
        {
            roleContext context = new roleContext();
            DocenteRepository docenteRepos = new DocenteRepository(context);
            AlumnoViewModel vm = new AlumnoViewModel();
            vm.Maestro = docenteRepos.GetById(id);
            if (vm.Maestro != null)
            {
                if (User.IsInRole("Maestro"))
                {
                    if (User.Claims.FirstOrDefault(x => x.Type == "Id").Value == vm.Maestro.Id.ToString())
                    {
                        return View(vm);
                    }
                    else
                    {
                        return RedirectToAction("Error");
                    }
                }
                else
                {
                    return View(vm);
                }
            }
            return View(vm);
        }
        [Authorize(Roles = "Maestro, Director")]
        [HttpPost]
        public IActionResult AgregarAlumno(AlumnoViewModel vm)
        {
            roleContext context = new roleContext();
            DocenteRepository docenteRepos = new DocenteRepository(context);
            AlumnosRepository alumnosRepos = new AlumnosRepository(context);
            try
            {
                if (context.Alumno.Any(x => x.NoControl == vm.Alumno.NoControl))
                {
                    ModelState.AddModelError("", "Número de control ya registrado.");
                    return View(vm);
                }
                else
                {
                    var maestro = docenteRepos.GetDocenteByClave(vm.Maestro.NoControl).Id;
                    vm.Alumno.IdMaestro = maestro;
                    alumnosRepos.Insert(vm.Alumno);
                    return RedirectToAction("VerAlumno", new { id = maestro });
                }

            }
            catch (Exception ex)
            {
                vm.Maestro = docenteRepos.GetById(vm.Maestro.Id);
                vm.Docentes = docenteRepos.GetAll();
                ModelState.AddModelError("", ex.Message);
                return View(vm);
            }
        }
        [Authorize(Roles = "Maestro, Director")]
        public IActionResult EditarAlumno(int id)
        {
            roleContext context = new roleContext();
            DocenteRepository docenteRepos = new DocenteRepository(context);
            AlumnosRepository alumnosRepos = new AlumnosRepository(context);
            AlumnoViewModel vm = new AlumnoViewModel();
            vm.Alumno = alumnosRepos.GetById(id);
            vm.Docentes = docenteRepos.GetAll();
            if (vm.Alumno != null)
            {
                vm.Maestro = docenteRepos.GetById(vm.Alumno.Id);
                if (User.IsInRole("Maestro"))
                {
                    vm.Maestro = docenteRepos.GetById(vm.Alumno.IdMaestro);
                    if (User.Claims.FirstOrDefault(x => x.Type == "NoControl").Value == vm.Maestro.NoControl.ToString())
                    {
                        return View(vm);
                    }
                }
                return View(vm);

            }
            else return RedirectToAction("Index");
        }
        [Authorize(Roles = "Maestro, Director")]
        [HttpPost]  
        public IActionResult EditarAlumno(AlumnoViewModel vm)
        {
            roleContext context = new roleContext();
            DocenteRepository docenteRepos = new DocenteRepository(context);
            AlumnosRepository alumnosRepos = new AlumnosRepository(context);
            try
            {
                var alumno = alumnosRepos.GetById(vm.Alumno.Id);
                if (alumno != null)
                {
                    alumno.Nombre = vm.Alumno.Nombre;
                    alumnosRepos.Edit(alumno);
                    return RedirectToAction("VerAlumno", new { id = alumno.IdMaestro });
                }
                else
                {
                    ModelState.AddModelError("", "El alumno que se intenta editar no existe.");
                    vm.Maestro = docenteRepos.GetById(vm.Alumno.IdMaestro);
                    vm.Docentes = docenteRepos.GetAll();
                    return View(vm);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                vm.Maestro = docenteRepos.GetById(vm.Alumno.IdMaestro);
                vm.Docentes = docenteRepos.GetAll();
                return View(vm);
            }
        }
        [Authorize(Roles = "Docente, Director")]
        [HttpPost]
        public IActionResult EliminarAlumno(Alumno a)
        {
            roleContext context = new roleContext ();
            AlumnosRepository alumnosRepos = new AlumnosRepository(context);
            var alumno = alumnosRepos.GetById(a.Id);
            if (alumno != null)
            {
                alumnosRepos.Delete(alumno);
            }
            else
            {
                ModelState.AddModelError("", "El alumno que se intenta eliminar no existe.");
            }
            return RedirectToAction("VerAlumno", new { id = alumno.IdMaestro });
        }
        [AllowAnonymous]
        public IActionResult Error()
        {
            return View();
        }
    }
}
