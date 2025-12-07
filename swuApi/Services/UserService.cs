using swuApi.Models;
using swuApi.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace swuApi.Services
{
    public class UserService : IService<User>
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<List<User>> GetAllAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        public async Task<List<User>> GetFilteredAsync(string? filterField, string? filterValue, string? sortField, string? sortDirection)
        {
            return await _userRepository.GetFilteredAsync(filterField, filterValue, sortField, sortDirection);
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            // Validación de Id, mayor que cero
            if (id <= 0)
                throw new ArgumentException("El ID debe ser mayor que cero.", nameof(id));

            return await _userRepository.GetByIdAsync(id);
        }

        public async Task AddAsync(User user)
        {
            // Valiaciones de nombre e email no vacío, contraseña mínimo 6 caracteres y valor de colección no negativo
            if (string.IsNullOrWhiteSpace(user.Username))
                throw new ArgumentException("El nombre de usuario no puede estar vacío.", nameof(user.Username));
            
            if (string.IsNullOrWhiteSpace(user.Email))
                throw new ArgumentException("El email no puede estar vacío.", nameof(user.Email));
                
            if (string.IsNullOrWhiteSpace(user.PasswordHash) || user.PasswordHash.Length < 6)
                throw new ArgumentException("La contraseña debe ser provista y tener al menos 6 caracteres.", nameof(user.PasswordHash));
            
            if (user.TotalCollectionValue < 0)
                throw new ArgumentException("El valor de la colección no puede ser negativo.", nameof(user.TotalCollectionValue));

            // Valido que no se cree un usuario con mismo username o email que otro ya existente
            var existingUser = await _userRepository.GetByUsernameOrEmailAsync(user.Username, user.Email);
            if (existingUser != null)
                throw new InvalidOperationException("El nombre de usuario o email ya están registrados.");
            
            if (user.RegistrationDate == default(DateTime))
            {
                user.RegistrationDate = DateTime.UtcNow;
            }
            
            await _userRepository.AddAsync(user);
        }


        public async Task UpdateAsync(User user)
        {
            // Validaciones de usuario, nombre, que exista el id solicitado y valor de colección             
            if (user.Id <= 0)
                throw new ArgumentException("El ID de usuario no es válido.", nameof(user.Id));
            
            if (string.IsNullOrWhiteSpace(user.Username))
                throw new ArgumentException("El nombre de usuario no puede estar vacío.", nameof(user.Username));

            var existingUser = await _userRepository.GetByIdAsync(user.Id);
            if (existingUser == null)
                throw new KeyNotFoundException($"Usuario con ID {user.Id} no encontrado.");
            
            if (user.TotalCollectionValue < 0)
                throw new ArgumentException("El valor de la colección no puede ser negativo.", nameof(user.TotalCollectionValue));

            
            await _userRepository.UpdateAsync(user);
        }


        public async Task DeleteAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("El ID no es válido para eliminación.", nameof(id));
            
            var existingUser = await _userRepository.GetByIdAsync(id);
            if (existingUser == null)
                throw new KeyNotFoundException($"Usuario con ID {id} no encontrado.");
            
            await _userRepository.DeleteAsync(id);
        }
    }
}