namespace PIMS.allsoft.Configurations;

public static  class ApplicationBuilderExtentions
{

    public static IApplicationBuilder AddGlobalErrorHandeler(this IApplicationBuilder applicationBuilder)
        =>applicationBuilder.UseMiddleware<GlobalExceptionHandelingMiddleware>();
}
