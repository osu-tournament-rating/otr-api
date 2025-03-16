using System.Diagnostics.CodeAnalysis;
using API.DTOs;
using API.Services.Interfaces;
using API.Utilities.AdminNotes;
using Database.Entities;

namespace API.Services.Implementations;
/*
 * It is not possible for this class to implement IAdminNoteService directly
 * because of the generic methods. This class should not be interfaced to
 * discourage registering in the service container. Instances should be
 * created and used as necessary
 */

/// <summary>
/// A wrapper for an instance of <see cref="IAdminNoteService"/> that
/// dynamically calls its generic methods
/// </summary>
[RequiresDynamicCode("Dynamically creates and invokes generic methods")]
public class DynamicAdminNoteService
{
    private readonly IAdminNoteService _adminNoteService;
    private readonly AdminNoteRouteTarget _target;

    public DynamicAdminNoteService(
        IAdminNoteService adminNoteService,
        AdminNoteRouteTarget routeTarget
    )
    {
        // Through this check we can ensure that the reflection we do is safe.
        // The generic type constraints on the admin note service require that the
        // type inherit from AdminNoteEntityBase
        if (routeTarget.AdminNoteType.BaseType is null || !(routeTarget.AdminNoteType.BaseType == typeof(AdminNoteEntityBase)))
        {
            throw new ArgumentException(
                $"{nameof(routeTarget.AdminNoteType)} must inherit directly from {nameof(AdminNoteEntityBase)}",
                nameof(routeTarget)
            );
        }

        _adminNoteService = adminNoteService;
        _target = routeTarget;
    }

    /// <summary>
    /// See <see cref="IAdminNoteService.ExistsAsync"/>
    /// </summary>
    public Task<bool> ExistsAsync(int id) =>
        (Task<bool>)DynamicInvokeMethod(nameof(IAdminNoteService.ExistsAsync), id);

    /// <summary>
    /// See <see cref="IAdminNoteService.CreateAsync"/>
    /// </summary>
    public Task<AdminNoteDTO?> CreateAsync(int referenceId, int adminUserId, string note) =>
        (Task<AdminNoteDTO?>)DynamicInvokeMethod(
            nameof(IAdminNoteService.CreateAsync),
            referenceId,
            adminUserId,
            note
        );

    /// <summary>
    /// See <see cref="IAdminNoteService.GetAsync"/>
    /// </summary>
    public Task<AdminNoteDTO?> GetAsync(int id) =>
        (Task<AdminNoteDTO?>)DynamicInvokeMethod(nameof(IAdminNoteService.GetAsync), id);

    /// <summary>
    /// See <see cref="IAdminNoteService.ListAsync"/>
    /// </summary>
    public Task<IEnumerable<AdminNoteDTO>> ListAsync(int referenceId) =>
        (Task<IEnumerable<AdminNoteDTO>>)DynamicInvokeMethod(
            nameof(IAdminNoteService.ListAsync),
            referenceId
        );

    /// <summary>
    /// See <see cref="IAdminNoteService.UpdateAsync"/>
    /// </summary>
    public Task<AdminNoteDTO?> UpdateAsync(AdminNoteDTO updatedNote) =>
        (Task<AdminNoteDTO?>)DynamicInvokeMethod(
            nameof(IAdminNoteService.UpdateAsync),
            updatedNote
        );

    /// <summary>
    /// See <see cref="IAdminNoteService.DeleteAsync"/>
    /// </summary>
    public Task<bool> DeleteAsync(int id) =>
        (Task<bool>)DynamicInvokeMethod(nameof(IAdminNoteService.DeleteAsync), id);

    /// <summary>
    /// Invokes a method interfaced by the <see cref="IAdminNoteService"/>
    /// with the given arguments
    /// </summary>
    /// <param name="methodName">
    /// Name of a method interfaced by the <see cref="IAdminNoteService"/>
    /// </param>
    /// <param name="args">
    /// Arguments to pass to the method
    /// </param>
    /// <returns>The return value of the method that was invoked</returns>
    /// <exception cref="InvalidOperationException">
    /// If the method was not able to be invoked. Typically, this would mean that
    /// the signature for the method has changed, but this class was not updated
    /// </exception>
    private object DynamicInvokeMethod(string methodName, params object[] args) =>
        _adminNoteService
            .GetType()
            .GetMethod(methodName)
            // Pass the given admin note type for the type parameter
            ?.MakeGenericMethod(_target.AdminNoteType)
            .Invoke(_adminNoteService, args) ?? throw new InvalidOperationException($"Dynamic method invocation failed for {nameof(IAdminNoteService)}.{methodName}");
}
