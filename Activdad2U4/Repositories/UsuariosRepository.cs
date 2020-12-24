using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Activdad2U4.Models;
using Microsoft.EntityFrameworkCore;

namespace Activdad2U4.Repositories
{
    public class UsuariosRepository<T> where T : class
    {
        public roleContext Context { get; set; }
        public UsuariosRepository(roleContext context)
        {
            Context = context;
        }
        public virtual IEnumerable<T> GetAll()
        {
            return Context.Set<T>();
        }
        public T GetById(object id)
        {
            return Context.Find<T>(id);
        }
        public virtual void Insert(T entidad)
        {
            if (Validate(entidad))
            {
                Context.Add(entidad);
                Context.SaveChanges();
            }
        }
        public virtual void Edit(T entidad)
        {
            if (Validate(entidad))
            {
                Context.Update<T>(entidad);
                Context.SaveChanges();
            }
        }
        public virtual void Delete(T entidad)
        {
            Context.Remove<T>(entidad);
            Context.SaveChanges();
        }
        public virtual bool Validate(T entidad)
        {
            return true;
        }
    }
    public class AlumnosRepository : UsuariosRepository<Alumno>
    {
        public AlumnosRepository(roleContext context) : base(context) { }

        public Alumno GetAlumnoByNumeroControl(string noControl)
        {
            return Context.Alumno.FirstOrDefault(x => x.NoControl.ToLower() == noControl.ToLower());
        }

        public override bool Validate(Alumno alumno)
        {
            if (string.IsNullOrEmpty(alumno.NoControl))
            {
                throw new Exception("Escriba el número de control del alumno.");
            }
            if (string.IsNullOrEmpty(alumno.Nombre))
            {
                throw new Exception("Escriba el nombre del alumno.");
            }
            if (alumno.IdMaestro.ToString() == null || alumno.IdMaestro <= 0)
            {
                throw new Exception("Debe asignar el docente al alumno.");
            }
            if (alumno.NoControl == "1234")
            {
                throw new Exception("Número de control inválido.");
            }
            if (alumno.NoControl.Length < 8)
            {
                throw new Exception("Debe contener 8 caracteres para el número de control.");
            }
            if (alumno.NoControl.Length > 8)
            {
                throw new Exception("No debe exceder los 8 dígitos.");
            }
            return true;
        }
    }
    public class DocenteRepository : UsuariosRepository<Maestro>
    {
        public DocenteRepository(roleContext context) : base(context) { }

        public Maestro GetDocenteByClave(int clave)
        {
            return Context.Maestro.FirstOrDefault(x => x.NoControl == clave);
        }
        public Maestro GetAlumnoByDocente(int idmaestro)
        {
            return Context.Maestro.Include(x => x.Alumno).FirstOrDefault(x => x.Id == idmaestro);
        }

        public override bool Validate(Maestro docente)
        {
            if (docente.NoControl <= 0)
            {
                throw new Exception("Escriba el número de control del docente.");
            }
            if (docente.NoControl == 1234)
            {
                throw new Exception("Número de control inválido.");
            }
            if (string.IsNullOrWhiteSpace(docente.Nombre))
            {
                throw new Exception("Escriba el nombre del docente.");
            }
            if (string.IsNullOrWhiteSpace(docente.Contrasena))
            {
                throw new Exception("Escribe la contraseña del docente.");
            }
            if (docente.NoControl.ToString().Length > 4)
            {
                throw new Exception("No debe exceder los 4 dígitos.");
            }
            if (docente.NoControl.ToString().Length < 4)
            {
                throw new Exception("Debe contener 4 caracteres para el número de control.");
            }
            return true;
        }
    }
}
