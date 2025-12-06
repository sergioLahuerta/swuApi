using swuApi.Models;
using swuApi.Repositories;
using System;
using System.Collections.Generic;

namespace swuApi.Services
{
    public class CollectionService : IService<Collection>
    {
        private readonly IRepository<Collection> _collectionRepository;

        public CollectionService(IRepository<Collection> collectionRepository)
        {
            _collectionRepository = collectionRepository;
        }

        // GetFilteredAsync (Implementación del Requisito de Búsqueda)
        public async Task<List<Collection>> GetFilteredAsync(string? filterField, string? filterValue, string? sortField, string? sortDirection)
        {
            return await _collectionRepository.GetFilteredAsync(filterField, filterValue, sortField, sortDirection);
        }

        // GET: GetAllAsync
        public async Task<List<Collection>> GetAllAsync()
        {
            return await _collectionRepository.GetAllAsync();
        }

        // GET: GetByIdAsync
        public async Task<Collection?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("El ID de la colección debe ser mayor que cero.", nameof(id));

            var collection = await _collectionRepository.GetByIdAsync(id);
            return collection;
        }

        // POST: AddAsync
        public async Task AddAsync(Collection collection)
        {
            // Mismas validaxiones que en CardService.cs
            if (string.IsNullOrWhiteSpace(collection.CollectionName))
                throw new ArgumentException("El nombre de la colección no puede estar vacío.", nameof(collection.CollectionName));
            
            if (collection.EstimatedValue < 0)
                throw new ArgumentException("El valor estimado no puede ser negativo.", nameof(collection.EstimatedValue));

            if (collection.CreationDate == default(DateTime))
            {
                collection.CreationDate = DateTime.UtcNow;
            }

            if (collection.CreationDate > DateTime.UtcNow)
                throw new ArgumentException("La fecha de creación no puede ser en el futuro.", nameof(collection.CreationDate));
            
            await _collectionRepository.AddAsync(collection);
        }

        // PUT: UpdateAsync
        public async Task UpdateAsync(Collection collection)
        {
            if (collection.Id <= 0)
                throw new ArgumentException("El ID no es válido para actualización.", nameof(collection.Id));

            // Validación de existencia antes de actualizar
            var existing = await _collectionRepository.GetByIdAsync(collection.Id);
            if (existing == null)
            {
                throw new KeyNotFoundException($"Colección con ID {collection.Id} no encontrada para actualización.");
            }
            
            // Misma validación que en CardService.cs, que el nombre no puede estar vació
            if (string.IsNullOrWhiteSpace(collection.CollectionName))
                throw new ArgumentException("El nombre de la colección no puede estar vacío.", nameof(collection.CollectionName));
            
            if (collection.EstimatedValue < 0)
                throw new ArgumentException("El valor estimado no puede ser negativo.", nameof(collection.EstimatedValue));

            await _collectionRepository.UpdateAsync(collection);
        }

        // DELETE: DeleteAsync
        public async Task DeleteAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("El ID no es válido para eliminación.", nameof(id));

            // Validación de existencia antes de eliminar
            var existing = await _collectionRepository.GetByIdAsync(id);
            if (existing == null)
            {
                throw new KeyNotFoundException($"Colección con ID {id} no encontrada para eliminación.");
            }

            // Futura lógica de negocio adicional, como:
            // if (existing.NumCards > 0) 
            //    throw new InvalidOperationException("No se puede eliminar la colección si tiene cartas.");
            
            await _collectionRepository.DeleteAsync(id);
        }
    }
}