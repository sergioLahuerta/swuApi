using swuApi.Models;
using swuApi.Repositories;
using System;
using System.Collections.Generic;

namespace swuApi.Services
{
    public class ColectionService : IService<Colection>
    {
        private readonly IRepository<Colection> _colectionRepository;

        public ColectionService(IRepository<Colection> colectionRepository)
        {
            _colectionRepository = colectionRepository;
        }

        // GetFilteredAsync (Implementación del Requisito de Búsqueda)
        public async Task<List<Colection>> GetFilteredAsync(string? filterField, string? filterValue, string? sortField, string? sortDirection)
        {
            return await _colectionRepository.GetFilteredAsync(filterField, filterValue, sortField, sortDirection);
        }

        // GET: GetAllAsync
        public async Task<List<Colection>> GetAllAsync()
        {
            return await _colectionRepository.GetAllAsync();
        }

        // GET: GetByIdAsync
        public async Task<Colection?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("El ID de la colección debe ser mayor que cero.", nameof(id));

            var colection = await _colectionRepository.GetByIdAsync(id);
            return colection;
        }

        // POST: AddAsync
        public async Task AddAsync(Colection colection)
        {
            // Mismas validaxiones que en CardService.cs
            if (string.IsNullOrWhiteSpace(colection.CollectionName))
                throw new ArgumentException("El nombre de la colección no puede estar vacío.", nameof(colection.CollectionName));
            
            if (colection.EstimatedValue < 0)
                throw new ArgumentException("El valor estimado no puede ser negativo.", nameof(colection.EstimatedValue));

            if (colection.CreationDate == default(DateTime))
            {
                colection.CreationDate = DateTime.UtcNow;
            }

            if (colection.CreationDate > DateTime.UtcNow)
                throw new ArgumentException("La fecha de creación no puede ser en el futuro.", nameof(colection.CreationDate));
            
            await _colectionRepository.AddAsync(colection);
        }

        // PUT: UpdateAsync
        public async Task UpdateAsync(Colection colection)
        {
            if (colection.Id <= 0)
                throw new ArgumentException("El ID no es válido para actualización.", nameof(colection.Id));

            // Validación de existencia antes de actualizar
            var existing = await _colectionRepository.GetByIdAsync(colection.Id);
            if (existing == null)
            {
                throw new KeyNotFoundException($"Colección con ID {colection.Id} no encontrada para actualización.");
            }
            
            // Misma validación que en CardService.cs, que el nombre no puede estar vació
            if (string.IsNullOrWhiteSpace(colection.CollectionName))
                throw new ArgumentException("El nombre de la colección no puede estar vacío.", nameof(colection.CollectionName));
            
            if (colection.EstimatedValue < 0)
                throw new ArgumentException("El valor estimado no puede ser negativo.", nameof(colection.EstimatedValue));

            await _colectionRepository.UpdateAsync(colection);
        }

        // DELETE: DeleteAsync
        public async Task DeleteAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("El ID no es válido para eliminación.", nameof(id));

            // Validación de existencia antes de eliminar
            var existing = await _colectionRepository.GetByIdAsync(id);
            if (existing == null)
            {
                throw new KeyNotFoundException($"Colección con ID {id} no encontrada para eliminación.");
            }

            // Futura lógica de negocio adicional, como:
            // if (existing.NumCards > 0) 
            //    throw new InvalidOperationException("No se puede eliminar la colección si tiene cartas.");
            
            await _colectionRepository.DeleteAsync(id);
        }
    }
}