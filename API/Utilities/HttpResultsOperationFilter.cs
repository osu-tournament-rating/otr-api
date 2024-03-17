/*
 * This code is borrowed from https://github.com/vernou/Vernou.Swashbuckle.HttpResultsAdapter
 * and adapted for use with this project.

Copyright (c) 2023 VERNOU Cédric

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.
 */

using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace API.Utilities;

/// <summary>
/// OperationFilter to generate OAS response to action that return HttpResults type
/// </summary>
/// <remarks>
/// Constructor to inject services
/// </remarks>
/// <param name="mvc">MVC options to define response content types</param>
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class HttpResultsOperationFilter(IOptions<MvcOptions> mvc) : IOperationFilter
{
    private readonly Lazy<string[]> _contentTypes = new(() =>
        {
            var apiResponseTypes = new List<string>();

            var jsonApplicationType = mvc.Value.FormatterMappings.GetMediaTypeMappingForFormat("json");
            if (!string.IsNullOrEmpty(jsonApplicationType))
            {
                apiResponseTypes.Add(jsonApplicationType);
            }
            var xmlApplicationType = mvc.Value.FormatterMappings.GetMediaTypeMappingForFormat("xml");
            if (!string.IsNullOrEmpty(xmlApplicationType))
            {
                apiResponseTypes.Add(xmlApplicationType);
            }

            return [.. apiResponseTypes];
        });

    void IOperationFilter.Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (!IsControllerAction(context))
        {
            return;
        }

        Type actionReturnType = UnwrapTask(context.MethodInfo.ReturnType);
        if (!IsHttpResults(actionReturnType))
        {
            return;
        }

        operation.Responses.Clear();

        // UnauthorizedHttpResult is, for some reason, not able to be cast to IProducesResponseTypeMetadata
        if (actionReturnType.GenericTypeArguments.Contains(typeof(UnauthorizedHttpResult)))
        {
            operation.Responses.Add(
                StatusCodes.Status401Unauthorized.ToString(),
                GenerateOar(typeof(UnauthorizedHttpResult), StatusCodes.Status401Unauthorized, context)
            );
            // Return if this is the only type
            if (actionReturnType == typeof(UnauthorizedHttpResult))
            {
                return;
            }
        }

        if (!typeof(IEndpointMetadataProvider).IsAssignableFrom(actionReturnType))
        {
            return;
        }

        // Get private method for generating metadata
        MethodInfo? populateMetadataMethod = actionReturnType.GetMethod("Microsoft.AspNetCore.Http.Metadata.IEndpointMetadataProvider.PopulateMetadata", BindingFlags.Static | BindingFlags.NonPublic);
        if (populateMetadataMethod == null)
        {
            return;
        }

        // Cast response types to IProducesResponseTypeMetadata
        var endpointBuilder = new MetadataEndpointBuilder();
        populateMetadataMethod.Invoke(null, new object[] { context.MethodInfo, endpointBuilder });
        var responseTypes = endpointBuilder.Metadata.Cast<IProducesResponseTypeMetadata>().ToList();
        if (responseTypes.Count == 0)
        {
            return;
        }

        // Generate Open Api Responses
        foreach (IProducesResponseTypeMetadata? responseType in responseTypes)
        {
            var statusCode = responseType.StatusCode;
            operation.Responses.Add(statusCode.ToString(), GenerateOar(responseType.Type, statusCode, context));
        }
    }

    private OpenApiResponse GenerateOar(Type? responseType, int statusCode, OperationFilterContext context)
    {
        var oar = new OpenApiResponse { Description = ReasonPhrases.GetReasonPhrase(statusCode) };
        if (responseType is null)
        {
            return oar;
        }
        OpenApiSchema schema = context.SchemaGenerator.GenerateSchema(responseType, context.SchemaRepository);
        foreach (var contentType in _contentTypes.Value)
        {
            oar.Content.Add(contentType, new OpenApiMediaType { Schema = schema });
        }

        return oar;
    }

    private static bool IsControllerAction(OperationFilterContext context)
        => context.ApiDescription.ActionDescriptor is ControllerActionDescriptor;

    private static bool IsHttpResults(Type type)
        => type.Namespace == "Microsoft.AspNetCore.Http.HttpResults";

    private static Type UnwrapTask(Type type)
        => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Task<>)
                  ? type.GetGenericArguments()[0]
                  : type;

    private sealed class MetadataEndpointBuilder : EndpointBuilder
    {
        public override Endpoint Build() => throw new NotImplementedException();
    }
}
