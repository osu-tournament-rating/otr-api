using API.DTOs;
using API.Services.Interfaces;
using AutoMapper;
using Database.Entities;
using Database.Entities.Interfaces;
using Database.Repositories.Interfaces;

namespace API.Services.Implementations;

public class AdminNoteService(
    IAdminNoteRepository adminNoteRepository,
    IUserRepository userRepository,
    IMapper mapper
) : IAdminNoteService
{
    public async Task<bool> ExistsAsync<TAdminNote>(int id) where TAdminNote : AdminNoteEntityBase =>
        await adminNoteRepository.ExistsAsync<TAdminNote>(id);

    public async Task<bool> ExistsAsync(Type adminNoteType, int id) =>
        await adminNoteRepository.ExistsAsync(adminNoteType, id);

    public async Task<AdminNoteDTO?> CreateAsync<TAdminNote>(
        int referenceId,
        int adminUserId,
        string note
    ) where TAdminNote : AdminNoteEntityBase, new()
    {
        if (!await userRepository.ExistsAsync(adminUserId))
        {
            return null;
        }

        TAdminNote entity = await adminNoteRepository.CreateAsync(new TAdminNote
        {
            ReferenceId = referenceId,
            AdminUserId = adminUserId,
            Note = note
        });
        // Get after creation to load navigations
        await adminNoteRepository.GetAsync<TAdminNote>(entity.Id);

        return mapper.Map<AdminNoteDTO>(entity);
    }

    public async Task<AdminNoteDTO?> CreateAsync(
        Type adminNoteType,
        int referenceId,
        int userId,
        string note
    )
    {
        if (!typeof(AdminNoteEntityBase).IsAssignableFrom(adminNoteType))
        {
            throw new ArgumentException($"Type must implement {nameof(AdminNoteEntityBase)}", nameof(adminNoteType));
        }

        // Verify user exists
        if (!await userRepository.ExistsAsync(userId))
        {
            return null;
        }

        var adminNote = (IAdminNoteEntity?)Activator.CreateInstance(adminNoteType);
        if (adminNote is null)
        {
            return null;
        }

        adminNote.ReferenceId = referenceId;
        adminNote.AdminUserId = userId;
        adminNote.Note = note;

        await adminNoteRepository.CreateAsync(adminNote);
        // Get after creation to load navigations
        await adminNoteRepository.GetAsync(adminNoteType, adminNote.Id);

        return mapper.Map<AdminNoteDTO>(adminNote);
    }

    public async Task<AdminNoteDTO?> GetAsync<TAdminNote>(int id) where TAdminNote : AdminNoteEntityBase =>
        mapper.Map<AdminNoteDTO?>(await adminNoteRepository.GetAsync<TAdminNote>(id));

    public async Task<AdminNoteDTO?> GetAsync(Type adminNoteType, int id) =>
        mapper.Map<AdminNoteDTO?>(await adminNoteRepository.GetAsync(adminNoteType, id));

    public async Task<IEnumerable<AdminNoteDTO>> ListAsync<TAdminNote>(int referenceId)
        where TAdminNote : AdminNoteEntityBase =>
        mapper.Map<IEnumerable<AdminNoteDTO>>(await adminNoteRepository.ListAsync<TAdminNote>(referenceId));

    public async Task<IEnumerable<AdminNoteDTO>> ListAsync(
        Type adminNoteType,
        int referenceId
    ) =>
        mapper.Map<IEnumerable<AdminNoteDTO>>(await adminNoteRepository.ListAsync(adminNoteType, referenceId));

    public async Task<AdminNoteDTO?> UpdateAsync<TAdminNote>(AdminNoteDTO updatedNote) where TAdminNote : AdminNoteEntityBase
    {
        TAdminNote? adminNote = await adminNoteRepository.GetAsync<TAdminNote>(updatedNote.Id);

        if (adminNote is null)
        {
            return null;
        }

        adminNote.Note = updatedNote.Note;
        await adminNoteRepository.UpdateAsync(adminNote);

        return mapper.Map<AdminNoteDTO>(adminNote);
    }

    public async Task<AdminNoteDTO?> UpdateAsync(
        Type adminNoteType,
        AdminNoteDTO updatedNote
    )
    {
        IAdminNoteEntity? adminNote = await adminNoteRepository.GetAsync(adminNoteType, updatedNote.Id);

        if (adminNote is null)
        {
            return null;
        }

        adminNote.Note = updatedNote.Note;
        await adminNoteRepository.UpdateAsync(adminNote);

        return mapper.Map<AdminNoteDTO>(adminNote);
    }

    public async Task<bool> DeleteAsync<TAdminNote>(int id) where TAdminNote : AdminNoteEntityBase
    {
        TAdminNote? adminNote = await adminNoteRepository.GetAsync<TAdminNote>(id);

        if (adminNote is null)
        {
            return false;
        }

        await adminNoteRepository.DeleteAsync(adminNote);
        return true;
    }

    public async Task<bool> DeleteAsync(Type adminNoteType, int id)
    {
        IAdminNoteEntity? adminNote = await adminNoteRepository.GetAsync(adminNoteType, id);

        if (adminNote is null)
        {
            return false;
        }

        await adminNoteRepository.DeleteAsync(adminNote);
        return true;
    }
}
