using swuApi.Models;
using swuApi.Repositories;
using System;
using System.Collections.Generic;

namespace swuApi.Services
{
    public class CardService : IService<Card>
    {
        private readonly IPackOpeningRepository _cardRepository;

        public CardService(IPackOpeningRepository cardRepository)
        {
            _cardRepository = cardRepository;
        }

        // GET: por filtrado
        public async Task<List<Card>> GetFilteredAsync(string? filterField, string? filterValue, string? sortField, string? sortDirection)
        {
            return await _cardRepository.GetFilteredAsync(filterField, filterValue, sortField, sortDirection);
        }

        public async Task<List<Card>> GetAllAsync()
        {
            return await _cardRepository.GetAllAsync();
        }

        public async Task<Card?> GetByIdAsync(int id)
        {
            // Validación de Id, no puede ser 0 o negativo
            if (id <= 0)
                throw new ArgumentException("El ID debe ser mayor que cero.", nameof(id));

            return await _cardRepository.GetByIdAsync(id);
        }

        public async Task AddAsync(Card card)
        {
            // Validación por nombre, no puede estar vació, tiene que tener un nombre
            if (string.IsNullOrWhiteSpace(card.CardName))
                throw new ArgumentException("El nombre del card no puede estar vacío.", nameof(card.CardName));

            // Validación Aspectos de una una carta
            int aspectValue = (int)card.Aspect;

            // Conteo del número de bits (de unos) activos
            int activeAspects = System.Numerics.BitOperations.PopCount((uint)aspectValue);

            // Si hay más de 2 bits encendidos
            if (activeAspects > 2)
            {
                throw new ArgumentException("Una carta no puede tener más de dos Aspectos", nameof(card.Aspect));
            }
            
            // Validación precio, si es negativo se lanza la excepción
            if (card.Price < 0)
            {
                throw new ArgumentException("El precio de la carta no puede ser negativo.", nameof(card.Price));
            }
            
            // Validación por fecha, si el usuario no introduce una, por defecto la de ese mismo momento
            if (card.DateAcquired == default(DateTime))
            {
                // UTC para mantener consistencia en el servidor
                card.DateAcquired = DateTime.UtcNow;
            }

            // Validación de fecha, si es un a fecha futura, lanzar excepción
            if (card.DateAcquired > DateTime.UtcNow)
                throw new ArgumentException("La fecha de adquisición no puede ser en el futuro.", nameof(card.DateAcquired));
            
            // Aquí podrías añadir lógica adicional antes de guardar, como establecer DateAcquired = DateTime.Now si el cliente no la envía.

            await _cardRepository.AddAsync(card);
        }

        public async Task UpdateAsync(Card card)
        {
            // Validación de Id, no puede ser 0 o negativo
            if (card.Id <= 0)
                throw new ArgumentException("El ID no es válido para actualización.", nameof(card.Id));

            // Validación por nombre, no puede estar vació, tiene que tener un nombre
            if (string.IsNullOrWhiteSpace(card.CardName))
                throw new ArgumentException("El nombre del card no puede estar vacío.", nameof(card.CardName));
            
            // Validación precio, si es negativo se lanza la excepción
            if (card.Price < 0)
                throw new ArgumentException("El precio de la carta no puede ser negativo.", nameof(card.Price));

            // Validación de fecha, si es un a fecha futura, lanzar excepción
            if (card.DateAcquired > DateTime.Now)
                throw new ArgumentException("La fecha de adquisición no puede ser en el futuro.", nameof(card.DateAcquired));

            await _cardRepository.UpdateAsync(card);
        }

        public async Task DeleteAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("El ID no es válido para eliminación.", nameof(id));

            await _cardRepository.DeleteAsync(id);
        }
    }
}