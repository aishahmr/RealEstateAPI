using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using RealEstateAPI.DTOs.UserDTos.UserManagement;
using RealEstateAPI.DTOs.UserDTos.Profile;

namespace RealEstateAPI.Service.IServices
{
    public interface IUserService
    {
        Task<List<UserResponseDTO>> GetAllUsers();
        Task<UserProfileDTO> GetUserDetails(string userId);
        Task<bool> UpdateUserProfile(string userId, UpdateUserProfileDTO dto, ILogger logger);
        Task<bool> DeleteUser(string userId);

        // New for logged-in users
        Task<UserProfileDTO> GetLoggedInUserProfile(string userId);
        Task<string?> UploadProfilePicture(string userId, IFormFile file);

        // Admin-only actions
        Task<bool> ApproveProperty(Guid propertyId);
        Task<bool> BlockUser(string userId);
        Task<bool> UnblockUser(string userId);
    }
}
