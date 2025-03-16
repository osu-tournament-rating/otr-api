using API.DTOs;
using API.Services.Interfaces;
using AutoMapper;
using Database.Entities;
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

    public async Task<AdminNoteDTO?> GetAsync<TAdminNote>(int id) where TAdminNote : AdminNoteEntityBase =>
        mapper.Map<AdminNoteDTO?>(await adminNoteRepository.GetAsync<TAdminNote>(id));

    public async Task<IEnumerable<AdminNoteDTO>> ListAsync<TAdminNote>(int referenceId)
        where TAdminNote : AdminNoteEntityBase =>
        mapper.Map<IEnumerable<AdminNoteDTO>>(await adminNoteRepository.ListAsync<TAdminNote>(referenceId));

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
}
